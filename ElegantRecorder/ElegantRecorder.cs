using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public List<UIAction> UISteps = new List<UIAction>();

        public string CurrentRecordingName = "";

        private bool recording = false;
        private bool replaying = false;
        private string status;
        private Stopwatch stopwatch = new Stopwatch();

        public int CurrentHotkeyId = 8;
        public Dictionary<int, string> RecHotkeys = new Dictionary<int, string>();

        public ElegantRecorder()
        {
            InitializeComponent();

            ReadOrCreateConfig();
            ReadCurrentRecordings();

            WinAPI = new WinAPI(this);

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

        public void ReadCurrentRecordings()
        {
            dataGridViewRecordings.Rows.Clear();

            foreach (var file in Directory.GetFiles(ElegantOptions.DataFolder))
            {
                using StreamReader stream = new StreamReader(file);
                char[] buffer = new char[32];
                stream.Read(buffer, 0, 32);

                if ((new string(buffer)).Contains(Recording.DefaultTag))
                {
                    dataGridViewRecordings.Rows.Add(Path.GetFileNameWithoutExtension(file), "");
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
                if (ReplayRec.UIActions[currentActionIndex].elapsed != null && ReplayRec.UIActions[currentActionIndex].elapsed != 0)
                {
                    replayTimer.Interval = ElegantOptions.GetPlaybackSpeed(ReplayRec.PlaybackSpeed, ReplayRec.UIActions[currentActionIndex].elapsed);
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
            }
        }

        private void ReplayTimer_Tick(object? sender, EventArgs e)
        {
            AutomationEngine.ReplayAction(ReplayRec.UIActions[currentActionIndex], ref status);
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            Options options = new Options(this);
            options.Show();
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

                    dataGridViewRecordings.Rows.Add(textBoxNewRec.Text, "");
                    textBoxNewRec.Text = "";

                    dataGridViewRecordings.Sort(new RowComparer(this));

                    dataGridViewRecordings.ClearSelection();
                    dataGridViewRecordings.CurrentCell = dataGridViewRecordings.Rows[0].Cells[0];
                    dataGridViewRecordings.Rows[0].Selected = true;
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
                    var currentFile = Path.Combine(ElegantOptions.DataFolder, CurrentRecordingName + ".json");
                    File.Delete(currentFile);

                    ReadCurrentRecordings();
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
            ReadCurrentRecordings();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteRecording();
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options options = new Options(this);
            options.Show();
            options.RenameFocus();
        }

        private void buttonTriggers_Click(object sender, EventArgs e)
        {
            var triggerEditor = new TriggerEditor(this);
            triggerEditor.Show();
        }
    }
}
