using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;
using System.Text.Json.Serialization;
using ElegantRecorder.Properties;
using System.Threading.Tasks;

namespace ElegantRecorder
{
    public partial class ElegantRecorder : Form
    {
        private string status;
        private Stopwatch stopwatch = new Stopwatch();

        public string ConfigFileName;
        public string ConfigFilePath;

        public ElegantOptions ElegantOptions;
        public WinAPI WinAPI;
        public AutomationEngine AutomationEngine;

        List<UIAction> uiSteps = new List<UIAction>();

        public ElegantRecorder()
        {
            InitializeComponent();

            labelStatus.Text = "";

            ReadOrCreateConfig();

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

        private const int mouseMoveThreshold = 30;

        public void RecordMouseMove(MouseHookStruct currentMouseHookStruct)
        {
            if (!ElegantOptions.RecordMouseMove)
                return;

            if (stopwatch.ElapsedMilliseconds < mouseMoveThreshold && uiSteps.Count > 0)
                return;

            UIAction uiAction = new UIAction();

            AutomationEngine.FillMouseMoveAction(ref uiAction, ref status, currentMouseHookStruct);

            uiAction.elapsed = stopwatch.Elapsed.TotalMilliseconds;

            uiSteps.Add(uiAction);

            stopwatch.Reset();
            stopwatch.Start();
        }

        public void RecordMouseWheel(MouseHookStruct currentMouseHookStruct)
        {
            UIAction uiAction = new UIAction();

            AutomationEngine.FillMouseWheelAction(ref uiAction, ref status, currentMouseHookStruct);

            uiAction.elapsed = stopwatch.Elapsed.TotalMilliseconds;

            uiSteps.Add(uiAction);

            stopwatch.Reset();
            stopwatch.Start();
        }

        public void RecordKeypress(KeyboardHookStruct keyboardHookStruct)
        {
            UIAction uiAction = new UIAction();

            AutomationEngine.FillKeypressAction(ref uiAction, ref status, keyboardHookStruct);

            uiAction.elapsed = stopwatch.Elapsed.TotalMilliseconds;

            uiSteps.Add(uiAction);

            stopwatch.Reset();
            stopwatch.Start();

            labelStatus.Text = status;
        }

        public /*async*/ void RecordMouse(MouseHookStruct currentMouseHookStruct, MouseHookStruct prevMouseHookStruct)
        {
            //await System.Threading.Tasks.Task.Run(() => RecordMouseWorker());
            RecordMouseWorker(currentMouseHookStruct, prevMouseHookStruct);

            labelStatus.Text = status;
        }

        private void RecordMouseWorker(MouseHookStruct currentMouseHookStruct, MouseHookStruct prevMouseHookStruct)
        {
            //merge the BUTTONUP and BUTTONDOWN events if coordinates are the same
            if (currentMouseHookStruct.pt.x == prevMouseHookStruct.pt.x &&
                currentMouseHookStruct.pt.y == prevMouseHookStruct.pt.y &&
                uiSteps.Count > 0 &&
                uiSteps[uiSteps.Count - 1].EventType == "click")
            {
                int prevFlags = (int) uiSteps[uiSteps.Count - 1].Flags;
                if (WinAPI.MergeMouseEvents(ref prevFlags))
                {
                    uiSteps[uiSteps.Count - 1].Flags = prevFlags;
                    return;
                }
            }

            var uiAction = new UIAction();

            if (AutomationEngine.FillClickAction(ref uiAction, ref status, currentMouseHookStruct))
            {
                uiAction.elapsed = stopwatch.Elapsed.TotalMilliseconds;

                uiSteps.Add(uiAction);

                stopwatch.Reset();
                stopwatch.Start();
            }
        }

        private bool recording = false;
        private bool replaying = false;

        private void ResetButtons()
        {
            recording = false;
            replaying = false;
            buttonRecord.Image = Resources.record_fill;
            buttonReplay.Image = Resources.play_fill;
            buttonPause.Image = Resources.pause_fill;
        }

        private void CompressMoveData()
        {
            List<MoveData> moveData = new List<MoveData>();

            for (int i = uiSteps.Count - 1; i >= 0; i--)
            {
                if (uiSteps[i].EventType == "mousemove")
                {
                    moveData.Insert(0, new MoveData { X = (int) uiSteps[i].OffsetX, Y = (int) uiSteps[i].OffsetY, T = (int) uiSteps[i].elapsed });
                    uiSteps.RemoveAt(i);
                }
                else
                {
                    if (moveData.Count != 0)
                    {
                        var uiAction = new UIAction();
                        AutomationEngine.FillMousePathAction(ref uiAction, ref status, moveData);

                        uiSteps.Insert(i+1, uiAction);

                        moveData.Clear();
                    }
                }
            }

            if (moveData.Count != 0)
            {
                var uiAction = new UIAction();
                AutomationEngine.FillMousePathAction(ref uiAction, ref status, moveData);

                uiSteps.Insert(0, uiAction);

                moveData.Clear();
            }
        }

        private void buttonRecord_Click(object sender, EventArgs e)
        {
            if (recording || replaying)
                return;

            ResetButtons();

            buttonRecord.Image = Resources.record_edit;
            recording = true;

            if (ElegantOptions.RecordingPath.Length == 0)
            {
                labelStatus.Text = "Specify recording file";
                
                return;
            }

            if (ElegantOptions.RestrictToExe == true && ElegantOptions.ExePath.Length == 0)
            {
                labelStatus.Text = "Specify target executable";
                ResetButtons();
                return;
            }

            if (!File.Exists(ElegantOptions.RecordingPath))
            {
                try
                {
                    var stream = File.Create(ElegantOptions.RecordingPath);
                    stream.Close();
                }
                catch
                {
                    labelStatus.Text = "Failed to create script file";
                    ResetButtons();
                    return;
                }
            }

            stopwatch.Reset();

            WinAPI.InstallHooks();
        }

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        CancellationToken token;

        // Use MouseDown istead of Click event to avoid situations where other UI events happen between mouse down and up, due to the automation
        private void buttonStop_MouseDown(object sender, MouseEventArgs e)
        {
            Stop(false);
        }

        private void Stop(bool paused)
        {
            WinAPI.UninstallHooks();

            stopwatch.Reset();

            if (tokenSource != null)
            {
                tokenSource.Cancel();
            }

            if (recording)
            {
                CompressMoveData();

                File.WriteAllText(ElegantOptions.RecordingPath, "[\n");

                JsonSerializerOptions jsonOptions = new()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                for (int i = 0; i < uiSteps.Count; i++)
                {
                    string jsonString = JsonSerializer.Serialize(uiSteps[i], jsonOptions);
                    File.AppendAllText(ElegantOptions.RecordingPath, jsonString);

                    if (i != uiSteps.Count - 1)
                    {
                        File.AppendAllText(ElegantOptions.RecordingPath, ",\n");
                    }
                    else
                    {
                        File.AppendAllText(ElegantOptions.RecordingPath, "\n");
                    }
                }

                File.AppendAllText(ElegantOptions.RecordingPath, "]");
            }

            ResetButtons();

            if (paused == false)
            {
                uiSteps.Clear();
                currentActionIndex = 0;
            }
        }

        private async void buttonReplay_Click(object sender, EventArgs e)
        {
            if (replaying)
                return;

            ResetButtons();

            replaying = true;
            buttonReplay.Image = Resources.play_edit;

            tokenSource.Dispose();
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;

            labelStatus.Text = "";
            status = "";

            var task = Task.Run(ReplayWorker, token);

            try
            {
                await task;
            }
            catch (Exception) { }

            labelStatus.Text = status;
        }

        private int currentActionIndex = 0;

        void ReplayWorker()
        {
            UIAction[] steps = JsonSerializer.Deserialize<UIAction[]>(File.ReadAllText(ElegantOptions.RecordingPath));

            if (currentActionIndex >= steps.Length - 1)
                currentActionIndex = 0;

            for (int i = currentActionIndex; i < steps.Length; i++)
            {
                var action = steps[i];

                currentActionIndex = i;

                if (action.elapsed != null)
                {
                    Thread.Sleep((int)ElegantOptions.GetPlaybackSpeedDuration((double)action.elapsed));
                }

                try
                {
                    token.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    status = "Replay interrupted";
                    return;
                }

                if (action.EventType == "click")
                {
                    AutomationEngine.ReplayClickAction(action, ref status);
                }
                else if (action.EventType == "mousemove")
                {
                    AutomationEngine.ReplayMouseMoveAction(action, ref status);
                }
                else if (action.EventType == "mousewheel")
                {
                    AutomationEngine.ReplayMouseWheelAction(action, ref status);
                }
                else if (action.EventType == "keypress")
                {
                    AutomationEngine.ReplayKeypressAction(action, ref status);
                }
                else if (action.EventType == "clipboard")
                {
                    AutomationEngine.ReplayClipboardAction(action, ref status);
                }
                else if (action.EventType == "mousepath")
                {
                    AutomationEngine.ReplayMousePathAction(action, ref status);
                }
            }

            status = "Replay finished";

            ResetButtons();
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
            if (!ElegantOptions.RecordClipboard)
            {
                base.WndProc(ref m);
                return;
            }

            string clipboardText = "";

            if (WinAPI.ProcessClipboardMessage(m.Msg, ref clipboardText))
            {
                UIAction uiAction = new UIAction();

                AutomationEngine.FillClipboardAction(ref uiAction, ref status, clipboardText);

                uiAction.elapsed = stopwatch.Elapsed.TotalMilliseconds;

                uiSteps.Add(uiAction);

                stopwatch.Reset();
                stopwatch.Start();
            }

            base.WndProc(ref m);
        }
    }
}
