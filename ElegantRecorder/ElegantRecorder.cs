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
    public partial class ElegantRecorder : Form
    {
        public string ConfigFileName;
        public string ConfigFilePath;

        public ElegantOptions ElegantOptions;
        public WinAPI WinAPI;
        public AutomationEngine AutomationEngine;
        public TriggerData TriggerData;

        public Dictionary<string, Recording> RecHeaders = new();
        public List<UIAction> UISteps = new();

        public string CurrentRecordingName = "";

        private bool recording = false;
        private bool replaying = false;
        private string status;
        private Stopwatch stopwatch = new Stopwatch();

        public ElegantRecorder()
        {
            InitializeComponent();

            WinAPI = new WinAPI(this);
            TriggerData = new TriggerData(this);

            ReadOrCreateConfig();
            ReadRecordingHeaders();

            AutomationEngine = null;

            if (ElegantOptions.AutomationEngine == "Win32")
            {
                AutomationEngine = new Win32Engine(this);
            }
            else if (ElegantOptions.AutomationEngine == "UI Automation")
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
                    ElegantOptions = JsonSerializer.Deserialize<ElegantOptions>(File.ReadAllText(ConfigFilePath));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to read configuratin file. " + ex.ToString());
                }
            }
            else
            {
                try
                {
                    File.Create(ConfigFilePath).Close();

                    ElegantOptions = new ElegantOptions();
                    File.WriteAllText(ConfigFilePath, JsonSerializer.Serialize(ElegantOptions));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to create configuration file. " + ex.ToString());
                }
            }
        }

        public void ReadRecordingHeaders()
        {
            foreach (var rec in RecHeaders.Values)
                rec.DisarmTriggers();

            dataGridViewRecordings.Rows.Clear();
            RecHeaders.Clear();

            foreach (var file in Directory.GetFiles(ElegantOptions.DataFolder, "*.json"))
            {
                using FileStream stream = new FileStream(file, FileMode.Open);
                byte[] buffer = new byte[32];
                stream.Read(buffer, 0, 32);

                if (Encoding.UTF8.GetString(buffer).Contains(Recording.DefaultTag))
                {
                    stream.Position = 0;

                    var tag = "\"UIActions\":";
                    string header = Recording.ReadUntil(stream, "\"UIActions\":");
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
            dataGridViewRecordings.CurrentCell = dataGridViewRecordings.Rows[0].Cells[0];
            dataGridViewRecordings.Rows[0].Selected = true;
        }

        private const int mouseMoveThreshold = 30;

        public void RecordMouseMove(MouseHookStruct currentMouseHookStruct)
        {
            if (!ElegantOptions.RecordMouseMove)
                return;

            if (stopwatch.IsRunning && stopwatch.ElapsedMilliseconds < mouseMoveThreshold && UISteps.Count > 0)
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

        private void ResetButtons()
        {
            recording = false;
            replaying = false;
            buttonRecord.Image = Resources.record_fill;
            buttonReplay.Image = Resources.play_fill;
            buttonPause.Image = Resources.pause_fill;
        }

        private void buttonRecord_Click(object sender, EventArgs e)
        {
            Record();
        }

        public void Record()
        {
            if (recording || replaying)
                return;

            ResetButtons();
            ClearStatus();

            buttonRecord.Image = Resources.record_edit;
            recording = true;

            if (CurrentRecordingName.Length == 0)
            {
                SetStatus("Add a new recording first");
                ResetButtons();
                return;
            }

            var rec = new Recording(this, CurrentRecordingName);
            rec.Load();

            if (ElegantOptions.RestrictToExe == true && ElegantOptions.ExePath.Length == 0)
            {
                SetStatus("Specify target executable");
                ResetButtons();
                return;
            }

            if (ElegantOptions.RecordClipboard)
            {
                PreRecordClipboard();
            }

            stopwatch.Reset();

            WinAPI.InstallHooks();
        }

        // Use MouseDown istead of Click event to avoid situations where other UI events happen between mouse down and up, due to the automation
        private void buttonStop_MouseDown(object sender, MouseEventArgs e)
        {
            Stop(false);
        }

        public void Stop(bool paused)
        {
            WinAPI.UninstallHooks();

            replayInterrupted = true;

            stopwatch.Reset();

            ClearStatus();

            if (recording)
            {
                AutomationEngine.CompressMoveData();
                AutomationEngine.CleanResidualKeys();

                var rec = new Recording(this, CurrentRecordingName);
                rec.Load();

                if (rec.Encrypted)
                {
                    var encPwd = new EncryptionPassword(true);
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
            }

            ResetButtons();

            if (paused == false)
            {
                UISteps.Clear();
                currentActionIndex = 0;
            }
        }

        private void buttonReplay_Click(object sender, EventArgs e)
        {
            Replay(CurrentRecordingName);
        }

        private int currentActionIndex = -1;
        private Timer replayTimer = null;
        private Recording ReplayRec = null;
        private bool replayInterrupted = false;

        public void Replay(string recording)
        {
            if (replaying)
                return;

            ResetButtons();
            ClearStatus();
            buttonReplay.Image = Resources.play_edit;

            replaying = true;
            replayInterrupted = false;

            if (recording != CurrentRecordingName)
            {
                foreach(DataGridViewRow row in dataGridViewRecordings.Rows)
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

            ReplayRec = new Recording(this, CurrentRecordingName);
            ReplayRec.Load();

            if (ReplayRec.Encrypted)
            {
                var encPwd = new EncryptionPassword(false);
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
                        ResetButtons();
                        return;
                    }
                }
            }

            if (currentActionIndex >= ReplayRec.UIActions.Length - 1)
                currentActionIndex = -1;

            replayTimer = new Timer();
            replayTimer.Tick += ReplayTimer_Tick;
            PlayAction();
        }

        public void PlayAction()
        {
            replayTimer.Stop();

            if (replayInterrupted)
            {
                SetStatus("Replay interrupted");
                return;
            }

            currentActionIndex++;

            if (currentActionIndex <= ReplayRec.UIActions.Length - 1)
            {
                var elapsed = ElegantOptions.GetPlaybackSpeed(ReplayRec.PlaybackSpeed, ReplayRec.UIActions[currentActionIndex].elapsed);

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
                ResetButtons();
                TriggerData.TriggerNewRecording(ReplayRec.Name);
            }
        }

        private void ReplayTimer_Tick(object? sender, EventArgs e)
        {
            AutomationEngine.ReplayAction(ReplayRec.UIActions[currentActionIndex], ref status);
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            Options options = new Options(this);
            options.ShowDialog();
        }

        private void buttonPin_Click(object sender, EventArgs e)
        {
            TopMost = !TopMost;

            buttonPin.Image = TopMost ? Resources.geo_edit : Resources.geo;
        }

        private void buttonPause_MouseDown(object sender, MouseEventArgs e)
        {
            if (!recording && !replaying)
                return;

            Stop(true);

            buttonPause.Image = Resources.pause_edit;
        }

        protected override void WndProc(ref Message m)
        {
            if (ElegantOptions.RecordClipboard)
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

            ElegantOptions.Save(ConfigFilePath);
        }

        private void buttonExpand_Click(object sender, EventArgs e)
        {
            ElegantOptions.ExpandedUI = !ElegantOptions.ExpandedUI;

            ExpandUI();
        }

        private void ExpandUI()
        {
            buttonExpand.Image = ElegantOptions.ExpandedUI ? Resources.double_up : Resources.double_down;

            if (ElegantOptions.ExpandedUI)
            {
                Height = ElegantOptions.FormHeight;
                dataGridViewRecordings.Height = ElegantOptions.DataGridHeight;
            }
            else
            {
                Height = ElegantOptions.DefaultFormHeight;
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
                var file1 = Path.Combine(parent.ElegantOptions.DataFolder, (x as DataGridViewRow).Cells[0].Value + ".json");
                var file2 = Path.Combine(parent.ElegantOptions.DataFolder, (y as DataGridViewRow).Cells[0].Value + ".json");

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
            if (CurrentRecordingName.Length > 0 && ElegantOptions.ExpandedUI)
            {
                var diag = MessageBox.Show("Delete recording " + CurrentRecordingName + " ?", "ElegantRecorder - Confirm Delete", MessageBoxButtons.OKCancel);

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
            if (ElegantOptions.ExpandedUI)
            {
                ElegantOptions.FormHeight = Height;
                ElegantOptions.DataGridHeight = dataGridViewRecordings.Height;
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
            Options options = new Options(this);
            options.ShowDialog();
            options.RenameFocus();
        }

        private void buttonTriggers_Click(object sender, EventArgs e)
        {
            var triggerEditor = new TriggerEditor(this);
            triggerEditor.ShowDialog();
        }
    }
}
