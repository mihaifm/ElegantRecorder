using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using ElegantRecorder.Properties;

namespace ElegantRecorder
{
    public enum State
    {
        Default,
        Record,
        Pause,
        RecordPause,
        ReplayPause,
        Stop,
        Replay
    }

    public partial class ElegantRecorder : Form
    {
        public string ConfigFileName;
        public string ConfigFilePath;

        public Options Options;
        public WinAPI WinAPI;
        public AutomationEngine AutomationEngine;
        public TriggerData TriggerData;

        public Dictionary<string, Recording> RecHeaders = new();
        public List<UIAction> UISteps = new();

        public string CurrentRecordingName = "";

        private string status;
        private Stopwatch stopwatch = new Stopwatch();

        private int currentActionIndex = -1;
        private Timer replayTimer = null;
        private Recording ReplayRec = null;

        public State PlayerState { get; private set; }

        public ElegantRecorder()
        {
            InitializeComponent();

            PlayerState = State.Default;

            WinAPI = new WinAPI(this);
            TriggerData = new TriggerData(this);

            ReadOrCreateConfig();
            ReadRecordingHeaders();

            AutomationEngine = null;

            if (Options.AutomationEngine == "Win32")
            {
                AutomationEngine = new Win32Engine(this);
            }
            else if (Options.AutomationEngine == "UI Automation")
            {
                AutomationEngine = new UIAEngine(this);
            }

            WinAPI.RegisterGlobalHotkeys();

            ExpandUI();
            ClearStatus();

            dataGridViewRecordings.Sort(new RowComparer(this));
        }

        private void SetStatus(string status)
        {
            labelStatus.Text = status;
        }

        private void ClearStatus()
        {
            status = "";
            labelStatus.Text = status;
        }

        private void ReadOrCreateConfig()
        {
            ConfigFileName = "ElegantRecorderConfig.json";
            ConfigFilePath = Application.StartupPath + ConfigFileName;

            if (File.Exists(ConfigFilePath))
            {
                try
                {
                    Options = JsonSerializer.Deserialize<Options>(File.ReadAllText(ConfigFilePath));
                }
                catch
                {
                    ElegantMessage.Info("Failed to read configuratin file");
                }
            }
            else
            {
                try
                {
                    File.Create(ConfigFilePath).Close();

                    Options = new Options();
                    File.WriteAllText(ConfigFilePath, JsonSerializer.Serialize(Options));
                }
                catch
                {
                    ElegantMessage.Info("Failed to create configuration file");
                }
            }
        }

        public void ReadRecordingHeaders()
        {
            foreach (var rec in RecHeaders.Values)
                rec.DisarmTriggers();

            dataGridViewRecordings.Rows.Clear();
            RecHeaders.Clear();

            foreach (var file in Directory.GetFiles(Options.DataFolder, "*.json"))
            {
                using FileStream stream = new FileStream(file, FileMode.Open);
                byte[] buffer = new byte[32];
                stream.Read(buffer, 0, 32);

                if (Encoding.UTF8.GetString(buffer).Contains(global::ElegantRecorder.Recording.DefaultTag))
                {
                    stream.Position = 0;

                    var tag = "\"UIActions\":";
                    string header = Util.ReadUntil(stream, "\"UIActions\":");
                    header += tag + "[]}";

                    string recName = Path.GetFileNameWithoutExtension(file);
                    var rec = new Recording(this, recName);
                    rec.Deserialize(header);
                    RecHeaders.Add(recName, rec);

                    string hotkeyStr = "";
                    if (rec.Triggers.Hotkey != 0)
                    {
                        hotkeyStr = WinAPI.HotkeyToStr(rec.Triggers.Hotkey);
                    }

                    dataGridViewRecordings.Rows.Add(recName, hotkeyStr, rec.Encrypted ? Resources.lock_edit : Resources.empty);

                    rec.ArmTriggers();
                }
            }

            dataGridViewRecordings.Sort(new RowComparer(this));

            dataGridViewRecordings.ClearSelection();
            CurrentRecordingName = "";

            if (dataGridViewRecordings.Rows.Count > 0)
            { 
                dataGridViewRecordings.CurrentCell = dataGridViewRecordings.Rows[0].Cells[0];
                dataGridViewRecordings.Rows[0].Selected = true;
            }
        }

        public State StateSwitch(State transition)
        {
            switch(PlayerState)
            {
                case State.Default:
                    if (transition == State.Record)
                    {
                        PlayerState = transition;
                        Record(false);
                    }
                    else if (transition == State.Replay)
                    {
                        PlayerState = transition;
                        Replay();
                    }
                    break;

                case State.Record:
                    if (transition == State.Default)
                    {
                        PlayerState = transition;
                    }
                    if (transition == State.Pause)
                    {
                        return StateSwitch(State.RecordPause);
                    }
                    else if (transition == State.RecordPause)
                    {
                        PlayerState = transition;
                        StopRecording(true);
                    }
                    else if (transition == State.Stop)
                    {
                        PlayerState = transition;
                        StopRecording(false);
                    }
                    break;

                case State.Pause:
                    PlayerState = State.Default;
                    break;

                case State.RecordPause:
                    if (transition == State.Record)
                    {
                        PlayerState = transition;
                        Record(true);
                    }
                    else if (transition == State.Stop)
                    {
                        PlayerState = transition;
                        StopRecording(false);
                    }
                    break;

                case State.ReplayPause:
                    if (transition == State.Stop)
                    {
                        PlayerState = transition;
                        StopReplay(false);
                    }
                    else if (transition == State.Replay)
                    {
                        PlayerState = transition;
                        Replay();
                    }
                    break;

                case State.Stop:
                    PlayerState = State.Default;
                    return StateSwitch(transition);

                case State.Replay:
                    if (transition == State.Pause)
                    {
                        return StateSwitch(State.ReplayPause);
                    }
                    else if (transition == State.ReplayPause)
                    {
                        PlayerState = transition;
                        StopReplay(true);
                    }
                    else if (transition == State.Stop)
                    {
                        PlayerState = transition;
                        StopReplay(false);
                    }
                    else if (transition == State.Default)
                    {
                        PlayerState = transition;
                    }
                    break;
            }

            SetButtons();
            return PlayerState;
        }

        public void RecordMouseMove(MouseHookStruct currentMouseHookStruct)
        {
            if (!Options.RecordMouseMove)
                return;

            if (stopwatch.IsRunning && stopwatch.ElapsedMilliseconds < Options.MouseMoveDelay && UISteps.Count > 0)
                return;

            UIAction uiAction = new UIAction();

            AutomationEngine.FillMouseMoveAction(ref uiAction, ref status, currentMouseHookStruct);

            uiAction.elapsed = stopwatch.Elapsed.TotalMilliseconds;

            UISteps.Add(uiAction);

            stopwatch.Reset();
            stopwatch.Start();
        }

        public void RecordMouseWheel(MouseHookStruct currentMouseHookStruct)
        {
            UIAction uiAction = new UIAction();

            AutomationEngine.FillMouseWheelAction(ref uiAction, ref status, currentMouseHookStruct);

            uiAction.elapsed = stopwatch.Elapsed.TotalMilliseconds;

            UISteps.Add(uiAction);

            stopwatch.Reset();
            stopwatch.Start();
        }

        public void RecordKeypress(KeyboardHookStruct keyboardHookStruct)
        {
            UIAction uiAction = new UIAction();

            AutomationEngine.FillKeypressAction(ref uiAction, ref status, keyboardHookStruct);

            uiAction.elapsed = stopwatch.Elapsed.TotalMilliseconds;

            UISteps.Add(uiAction);

            stopwatch.Reset();
            stopwatch.Start();

            SetStatus(status);
        }

        private void PreRecordClipboard()
        {
            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                var uiAction = new UIAction();

                var clipboardText = Clipboard.GetText(TextDataFormat.Text);
                AutomationEngine.FillClipboardAction(ref uiAction, ref status, clipboardText);

                UISteps.Add(uiAction);
            }
        }

        public void RecordMouse(MouseHookStruct currentMouseHookStruct, MouseHookStruct prevMouseHookStruct)
        {
            RecordMouseWorker(currentMouseHookStruct, prevMouseHookStruct);

            SetStatus(status);
        }

        private void RecordMouseWorker(MouseHookStruct currentMouseHookStruct, MouseHookStruct prevMouseHookStruct)
        {
            //merge the BUTTONUP and BUTTONDOWN events if coordinates are the same
            if (currentMouseHookStruct.pt.x == prevMouseHookStruct.pt.x &&
                currentMouseHookStruct.pt.y == prevMouseHookStruct.pt.y &&
                UISteps.Count > 0 &&
                UISteps[UISteps.Count - 1].EventType == "click")
            {
                int prevFlags = (int) UISteps[UISteps.Count - 1].Flags;
                if (WinAPI.MergeMouseEvents(ref prevFlags))
                {
                    UISteps[UISteps.Count - 1].Flags = prevFlags;
                    return;
                }
            }

            var uiAction = new UIAction();

            if (AutomationEngine.FillClickAction(ref uiAction, ref status, currentMouseHookStruct))
            {
                uiAction.elapsed = stopwatch.Elapsed.TotalMilliseconds;

                UISteps.Add(uiAction);

                stopwatch.Reset();
                stopwatch.Start();
            }
        }

        private void SetButtons()
        {
            buttonRecord.Image = PlayerState == State.Record ? Resources.record_edit : Resources.record_fill;
            buttonReplay.Image = PlayerState == State.Replay ? Resources.play_edit : Resources.play_fill;

            if (PlayerState == State.RecordPause || PlayerState == State.ReplayPause)
                buttonPause.Image = Resources.pause_edit;
            else
                buttonPause.Image = Resources.pause_fill;

            if (PlayerState == State.Default || PlayerState == State.Stop)
                LockUI(false);
            else
                LockUI(true);
        }

        private void LockUI(bool disabled)
        {
            dataGridViewRecordings.Enabled = !disabled;
            buttonAddRec.Enabled = !disabled;
            textBoxNewRec.Enabled = !disabled;
        }

        public void RefreshCurrentRow(Recording rec)
        {
            if (rec.Triggers.Hotkey != 0)
                dataGridViewRecordings.SelectedRows[0].Cells[1].Value = Util.HotkeyToString(rec.Triggers.Hotkey);
            else
                dataGridViewRecordings.SelectedRows[0].Cells[1].Value = "";

            dataGridViewRecordings.SelectedRows[0].Cells[2].Value = rec.Encrypted ? Resources.lock_edit : Resources.empty;
        }

        private void Record(bool paused)
        {
            ClearStatus();

            if (CurrentRecordingName.Length == 0)
            {
                SetStatus("Add a new recording first");
                StateSwitch(State.Default);
                return;
            }

            var rec = new Recording(this, CurrentRecordingName);
            rec.Load();

            if (rec.RestrictToExe == true && rec.ExePath.Length == 0)
            {
                SetStatus("Specify target executable");
                StateSwitch(State.Default);
                return;
            }

            if (Options.PromptOverwrite && !paused &&
                (rec.UIActions.Length > 0 || rec.EncryptedActions.Length > 0))
            {
                var diag = ElegantMessage.Show("Overwrite recording " + CurrentRecordingName + " ?");

                if (diag != DialogResult.OK)
                {
                    StateSwitch(State.Default);
                    return;
                }
            }

            if (Options.RecordClipboard)
            {
                PreRecordClipboard();
            }

            stopwatch.Reset();

            WinAPI.InstallHooks();
        }

        private void StopReplay(bool paused)
        {
            replayTimer.Stop();

            if (status.Length == 0)
                SetStatus("Replay interrupted");

            if (!paused)
                currentActionIndex = -1;
        }

        private void StopRecording(bool paused)
        {
            WinAPI.UninstallHooks();

            stopwatch.Reset();

            ClearStatus();

            AutomationEngine.CompressMoveData();
            AutomationEngine.CleanResidualKeys();

            var rec = new Recording(this, CurrentRecordingName);
            rec.Load();

            if (rec.Encrypted)
            {
                var encPwd = new EncryptionPassword(true, TopMost);
                encPwd.ShowDialog();

                if (encPwd.DialogResult == DialogResult.OK)
                {
                    rec.Password = encPwd.Password;
                    rec.UIActions = UISteps.ToArray();
                    rec.Save(true);
                }
            }
            else
            {
                try
                {
                    rec.UIActions = UISteps.ToArray();
                    rec.Save(false);
                }
                catch (Exception)
                {
                    SetStatus("Invalid recording file, recording not saved!");
                }
            }

            if (paused == false)
            {
                UISteps.Clear();
                currentActionIndex = -1;
            }
        }

        private void Replay()
        {
            ClearStatus();

            ReplayRec = new Recording(this, CurrentRecordingName);
            ReplayRec.Load();

            if (ReplayRec.Encrypted)
            {
                var encPwd = new EncryptionPassword(false, TopMost);
                encPwd.ShowDialog();

                if (encPwd.DialogResult == DialogResult.OK)
                {
                    ReplayRec.Password = encPwd.Password;

                    try
                    {
                        ReplayRec.Decrypt();
                    }
                    catch 
                    {
                        SetStatus("Failed to decrypt");
                        StateSwitch(State.Stop);
                        return;
                    }
                }
            }

            if (currentActionIndex >= ReplayRec.UIActions.Length - 1)
                currentActionIndex = -1;

            replayTimer = new Timer();
            replayTimer.Tick += ReplayTimer_Tick;
            PlayAction(true);
        }

        public void PlayAction(bool previousResult)
        {
            replayTimer.Stop();

            if (PlayerState == State.Stop)
            {
                SetStatus("Replay interrupted");
                StateSwitch(State.Default);
                return;
            }

            if (PlayerState == State.ReplayPause)
            {
                SetStatus("Replay paused");
                return;
            }

            if (!previousResult)
            {
                SetStatus(status);
                currentActionIndex = -1;
                StateSwitch(State.Stop);
                return;
            }

            currentActionIndex++;

            if (currentActionIndex <= ReplayRec.UIActions.Length - 1)
            {
                var elapsed = Options.GetPlaybackSpeed(ReplayRec.PlaybackSpeed, ReplayRec.UIActions[currentActionIndex].elapsed);

                if (elapsed != 0)
                {
                    replayTimer.Interval = elapsed;
                    replayTimer.Start();
                }
                else
                {
                    ReplayTimer_Tick(this, new EventArgs());
                }
            }
            else
            {
                SetStatus("Replay finished");
                StateSwitch(State.Default);
                TriggerData.TriggerNewRecording(ReplayRec.Name);
            }
        }

        public void SelectRecording(string recording)
        {
            if (PlayerState != State.Default)
                return;

            if (recording != CurrentRecordingName)
            {
                foreach (DataGridViewRow row in dataGridViewRecordings.Rows)
                {
                    if (row.Cells[0].Value.ToString() == recording)
                    {
                        dataGridViewRecordings.ClearSelection();
                        row.Selected = true;
                        CurrentRecordingName = recording;
                        break;
                    }
                }
            }
        }

        private void ReplayTimer_Tick(object? sender, EventArgs e)
        {
            AutomationEngine.ReplayAction(ReplayRec.UIActions[currentActionIndex], ref status);
        }

        // Use MouseDown istead of Click event to avoid situations where other UI events happen between mouse down and up, due to the automation
        private void buttonStop_MouseDown(object sender, MouseEventArgs e)
        {
            StateSwitch(State.Stop);
        }

        private void buttonRecord_Click(object sender, EventArgs e)
        {
            StateSwitch(State.Record);
        }

        private void buttonReplay_Click(object sender, EventArgs e)
        {
            StateSwitch(State.Replay);
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            if (PlayerState != State.Default && PlayerState != State.Stop)
                return;

            ElegantOptions elegantOptions = new ElegantOptions(this);
            elegantOptions.ShowDialog();
        }

        private void buttonPin_Click(object sender, EventArgs e)
        {
            TopMost = !TopMost;

            buttonPin.Image = TopMost ? Resources.geo_edit : Resources.geo;
        }

        private void buttonPause_MouseDown(object sender, MouseEventArgs e)
        {
            StateSwitch(State.Pause);
        }

        protected override void WndProc(ref Message m)
        {
            if (Options.RecordClipboard)
            {
                string clipboardText = "";

                if (WinAPI.ProcessClipboardMessage(m.Msg, ref clipboardText))
                {
                    UIAction uiAction = new UIAction();

                    AutomationEngine.FillClipboardAction(ref uiAction, ref status, clipboardText);

                    uiAction.elapsed = stopwatch.Elapsed.TotalMilliseconds;

                    UISteps.Add(uiAction);

                    stopwatch.Reset();
                    stopwatch.Start();
                }
            }

            base.WndProc(ref m);

            WinAPI.ProcessHotkeyMessage(ref m);
        }

        private void ElegantRecorder_FormClosing(object sender, FormClosingEventArgs e)
        {
            WinAPI.UnregisterGlobalHotkeys();

            Options.Save(ConfigFilePath);
        }

        private void buttonExpand_Click(object sender, EventArgs e)
        {
            Options.ExpandedUI = !Options.ExpandedUI;

            ExpandUI();
        }

        private void ExpandUI()
        {
            buttonExpand.Image = Options.ExpandedUI ? Resources.double_up : Resources.double_down;

            if (Options.ExpandedUI)
            {
                Height = Options.FormHeight;
                dataGridViewRecordings.Height = Options.DataGridHeight;
            }
            else
            {
                Height = Options.DefaultFormHeight;
            }
        }

        private void buttonAddRec_Click(object sender, EventArgs e)
        {
            AddRecording();
        }

        private void AddRecording()
        {
            if (textBoxNewRec.Text.Length > 0)
            {
                ClearStatus();

                Recording rec = new Recording(this, textBoxNewRec.Text.Trim());

                if (File.Exists(rec.FilePath))
                {
                    SetStatus("Recording with the same name already exists");
                    return;
                }

                try
                {
                    File.WriteAllText(rec.FilePath, JsonSerializer.Serialize(rec));

                    dataGridViewRecordings.Rows.Add(textBoxNewRec.Text, "", Resources.empty);
                    textBoxNewRec.Text = "";

                    dataGridViewRecordings.Sort(new RowComparer(this));

                    dataGridViewRecordings.ClearSelection();
                    dataGridViewRecordings.CurrentCell = dataGridViewRecordings.Rows[0].Cells[0];
                    dataGridViewRecordings.Rows[0].Selected = true;

                    RecHeaders.Add(rec.Name, rec);
                }
                catch (Exception)
                {
                    SetStatus("Failed to create recording file");
                }
            }
        }

        private void dataGridViewRecordings_SelectionChanged(object sender, EventArgs e)
        {
            currentActionIndex = -1;

            if (dataGridViewRecordings.SelectedRows.Count > 0)
                CurrentRecordingName = dataGridViewRecordings.SelectedRows[0].Cells[0].Value as string;
            else
                CurrentRecordingName = "";
        }

        private class RowComparer : System.Collections.IComparer
        {
            private ElegantRecorder parent;

            public RowComparer(ElegantRecorder parent)
            {
                this.parent = parent;
            }

            public int Compare(object x, object y)
            {
                var file1 = Path.Combine(parent.Options.DataFolder, (x as DataGridViewRow).Cells[0].Value + ".json");
                var file2 = Path.Combine(parent.Options.DataFolder, (y as DataGridViewRow).Cells[0].Value + ".json");

                return File.GetLastWriteTime(file2).CompareTo(File.GetLastWriteTime(file1));
            }
        }

        private void dataGridViewRecordings_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteRecording();
            }
        }

        private void DeleteRecording()
        {
            if (CurrentRecordingName.Length > 0 && Options.ExpandedUI)
            {
                var diag = ElegantMessage.Show("Delete recording " + CurrentRecordingName + " ?", "Elegant Message - Confirm Delete");

                if (diag == DialogResult.OK)
                {
                    var tempRec = new Recording(this, CurrentRecordingName);
                    tempRec.DisarmTriggers();

                    File.Delete(tempRec.FilePath);

                    ReadRecordingHeaders();
                }
            }
        }

        private void textBoxNewRec_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddRecording();

                e.SuppressKeyPress = true;
            }
        }

        private void ElegantRecorder_Resize(object sender, EventArgs e)
        {
            if (Options.ExpandedUI)
            {
                Options.FormHeight = Height;
                Options.DataGridHeight = dataGridViewRecordings.Height;
            }
        }

        private void dataGridViewRecordings_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hti = dataGridViewRecordings.HitTest(e.X, e.Y);
                dataGridViewRecordings.ClearSelection();

                if (hti.RowIndex < 0)
                    return;

                dataGridViewRecordings.Rows[hti.RowIndex].Selected = true;

                contextMenuStripRClick.Show(this, new System.Drawing.Point(e.X + ((Control)sender).Left, e.Y + ((Control)sender).Top));
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReadRecordingHeaders();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteRecording();
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ElegantOptions options = new ElegantOptions(this);
            options.RenameFocus();
            options.ShowDialog();
        }

        private void buttonTriggers_Click(object sender, EventArgs e)
        {
            if (PlayerState != State.Default && PlayerState != State.Stop)
                return;

            var elegantTriggers = new ElegantTriggers(this);
            elegantTriggers.ShowDialog();
        }
    }
}
