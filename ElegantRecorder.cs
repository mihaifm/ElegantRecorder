extern alias wrapper;

using wrapper::System.Windows.Automation;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Text.Json.Serialization;
using ElegantRecorder.Properties;
using System.Threading.Tasks;

namespace ElegantRecorder
{
    public partial class ElegantRecorder : Form
    {
        private IntPtr mouseHookID = IntPtr.Zero;
        private IntPtr keyboardHookID = IntPtr.Zero;

        private const int WH_MOUSE_LL = 14;
        private const int WH_KEYBOARD_LL = 13;

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYUP = 0x105;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_MOUSEWHEEL = 0x020A;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;
        private const int WM_MOUSEMOVE = 0x0200;

        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int MOUSEEVENTF_WHEEL = 0x0800;
        private const int KEYEVENTF_KEYUP = 0x0002;

        private string status;
        private Stopwatch stopwatch = new Stopwatch();

        public string ConfigFileName;
        public string ConfigFilePath;

        public ElegantOptions ElegantOptions;

        private class UIAction
        {
            [JsonPropertyName("event")]
            public string EventType { get; set; }
            [JsonPropertyName("ctrlname")]
            public string ControlName { get; set; }
            [JsonPropertyName("ctrltype")]
            public int? ControlType { get; set; }
            [JsonPropertyName("autoid")]
            public string AutomationId { get; set; }
            [JsonPropertyName("win")]
            public string TopLevelWindow { get; set; }
            [JsonPropertyName("x")]
            public int? OffsetX { get; set; }
            [JsonPropertyName("y")]
            public int? OffsetY { get; set; }
            [JsonPropertyName("lvl")]
            public int? Level { get; set; }
            [JsonPropertyName("idx")]
            public int? ChildIndex { get; set; }
            [JsonPropertyName("vkcode")]
            public int? VKeyCode { get; set; }
            [JsonPropertyName("scancode")]
            public int? ScanCode { get; set; }
            [JsonPropertyName("flags")]
            public int? Flags { get; set; }
            [JsonPropertyName("mdata")]
            public int? MouseData { get; set; }
            [JsonPropertyName("einfo")]
            public int? ExtraInfo { get; set; }
            [JsonPropertyName("t")]
            public double elapsed { get; set; }
        }

        List<UIAction> uiSteps = new List<UIAction>();

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MouseHookStruct
        {
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        private MouseHookStruct currentMouseHookStruct;
        private MouseHookStruct prevMouseHookStruct;

        [StructLayout(LayoutKind.Sequential)]
        private struct KeyboardHookStruct
        {
            public int VirtualKeyCode;
            public int ScanCode;
            public int Flags;
            public int Time;
            public int ExtraInfo;
        }

        KeyboardHookStruct keyboardHookStruct;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelHookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, uint dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void keybd_event(uint bVk, uint bScan, uint dwFlags, uint dwExtraInfo);

        public ElegantRecorder()
        {
            InitializeComponent();

            labelStatus.Text = "";

            ReadOrCreateConfig();
        }

        private void ReadOrCreateConfig()
        {
            ConfigFileName = "ElegantRecorderConfig.json";
            ConfigFilePath = System.Windows.Forms.Application.StartupPath + ConfigFileName;

            if (File.Exists(ConfigFilePath))
            {
                try
                {
                    ElegantOptions = JsonSerializer.Deserialize<ElegantOptions>(File.ReadAllText(ConfigFilePath));
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Failed to read configuratin file. " + ex.ToString());
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
                    System.Windows.Forms.MessageBox.Show("Failed to create configuration file. " + ex.ToString());
                }
            }
        }

        public void InstallHooks()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                mouseHookID = SetWindowsHookEx(WH_MOUSE_LL, MouseHookCallback, GetModuleHandle(curModule.ModuleName), 0);

                keyboardHookID = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookCallback, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public void UninstallHooks()
        {
            UnhookWindowsHookEx(mouseHookID);
            UnhookWindowsHookEx(keyboardHookID);
        }

        private delegate IntPtr LowLevelHookProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            currentMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));

            if (nCode >= 0)
            {
                if (wParam.ToInt32() == WM_LBUTTONDOWN || wParam.ToInt32() == WM_LBUTTONUP ||
                    wParam.ToInt32() == WM_RBUTTONDOWN || wParam.ToInt32() == WM_RBUTTONUP)
                {
                    RecordMouse(wParam.ToInt32());

                    prevMouseHookStruct = currentMouseHookStruct;
                }
                else if (wParam.ToInt32() == WM_MOUSEMOVE)
                {
                    RecordMouseMove(wParam.ToInt32());
                }
                else if (wParam.ToInt32() == WM_MOUSEWHEEL)
                {
                    RecordMouseWheel(wParam.ToInt32());
                }
            }

            return CallNextHookEx(mouseHookID, nCode, wParam, lParam);
        }

        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                keyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));

                if (wParam.ToInt32() == WM_KEYDOWN || wParam.ToInt32() == WM_SYSKEYDOWN ||
                    wParam.ToInt32() == WM_KEYUP || wParam.ToInt32() == WM_SYSKEYUP)
                {
                    RecordKeypress(wParam.ToInt32());
                }
            }

            return CallNextHookEx(keyboardHookID, nCode, wParam, lParam);
        }

        private int GetChildIndex(AutomationElement element)
        {
            TreeWalker walker = TreeWalker.ControlViewWalker;
            int index = 0;

            var node = walker.GetNextSibling(element);

            while (node != null)
            {
                node = walker.GetNextSibling(node);
                index++;
            }

            return index;
        }

        private AutomationElement GetTopLevelWindow(AutomationElement element, out int level)
        {
            TreeWalker walker = TreeWalker.ControlViewWalker;
            AutomationElement elementParent;
            AutomationElement node = element;

            level = 0;

            if (node == AutomationElement.RootElement) return node;

            while (true)
            {
                elementParent = walker.GetParent(node);
                level++;

                if (elementParent == null)
                {
                    return null;
                }

                if (elementParent == AutomationElement.RootElement) break;
                node = elementParent;
            }

            return node;
        }

        private AutomationElement FindWindowByName(string name)
        {
            TreeWalker walker = TreeWalker.ControlViewWalker;

            var node = AutomationElement.RootElement;

            var window = walker.GetFirstChild(node);

            while (window != null)
            {
                if (window.Current.Name == name)
                {
                    if (!ElegantOptions.RestrictToExe)
                        return window;

                    int pid = (int)window.GetCurrentPropertyValue(AutomationElement.ProcessIdProperty);
                    var process = Process.GetProcessById(pid);

                    if (Path.GetFileNameWithoutExtension(ElegantOptions.ExePath).ToLower() == process.ProcessName.ToLower())
                        return window;
                }

                window = walker.GetNextSibling(window);
            }

            return null;
        }

        private AutomationElement FindElementInWindow(string controlName, string automationId, int? controlType, AutomationElement window, int? level, int? childIndex)
        {
            int elCount = 0;
            var foundElements = new List<AutomationElement>();

            void TreeFinder(AutomationElement currentNode, int currLevel)
            {
                if (currLevel == level)
                {
                    var currentName = currentNode.Current.Name;

                    //cached control names that are null may become blank strings after caching
                    if (currentName == null)
                        currentName = "";

                    if (currentName == controlName && 
                        currentNode.Current.AutomationId == automationId &&
                        currentNode.Current.ControlType.Id == controlType )
                    {
                        foundElements.Add(currentNode);
                    }

                    return;
                }

                TreeWalker walker = TreeWalker.ControlViewWalker;

                AutomationElement node = walker.GetFirstChild(currentNode);

                while (node != null)
                {
                    TreeFinder(node, currLevel + 1);
                    node = walker.GetNextSibling(node);
                }
            }

            TreeFinder(window, 1);

            elCount = foundElements.Count;

            if (elCount > 1)
            {
                /*
                foreach(var el in foundElements)
                {
                    int idx = GetChildIndex(el);

                    if (idx == childIndex)
                        return el;
                }
                */

                return null;
            }

            if (elCount == 0)
                return null;

            return foundElements[0];
        }

        AutomationElement FindBoundingWindow(AutomationElement node, Point point)
        {
            TreeWalker walker = TreeWalker.ControlViewWalker;

            var original = node;

            node = walker.GetFirstChild(node);

            while (node != null)
            {
                if (node.Current.ControlType == ControlType.Window || node == AutomationElement.RootElement)
                {
                    AutomationElement win = FindBoundingWindow(node, point);

                    if (win != null)
                    {
                        return win;
                    }
                }

                node = walker.GetNextSibling(node);
            }

            Rect boundingRect = original.Current.BoundingRectangle;

            if (point.X >= boundingRect.X &&
                point.X <= boundingRect.X + boundingRect.Width &&
                point.Y >= boundingRect.Y &&
                point.Y <= boundingRect.Y + boundingRect.Height)
            {
                return original;
            }

            return null;
        }

        private AutomationElement FindElementFromPoint(Point point)
        {
            var node = AutomationElement.RootElement;

            AutomationElement boundingWin = FindBoundingWindow(node, point);

            if (boundingWin == null)
                return null;

            var result = boundingWin;

            TreeWalker walker = TreeWalker.RawViewWalker;
            node = walker.GetFirstChild(result);

            while (node != null)
            {
                Rect boundingRect = node.Current.BoundingRectangle;

                if (point.X >= boundingRect.X &&
                    point.X <= boundingRect.X + boundingRect.Width &&
                    point.Y >= boundingRect.Y &&
                    point.Y <= boundingRect.Y + boundingRect.Height)
                {
                    result = node;
                    var tmp = walker.GetFirstChild(node);
                    node = tmp;
                    continue;
                }

                node = walker.GetNextSibling(node);
            }

            return result;
        }

        private const int mouseMoveThreshold = 30;

        private void RecordMouseMove(int wparam)
        {
            if (!ElegantOptions.RecordMouseMove)
                return;

            if (stopwatch.ElapsedMilliseconds < mouseMoveThreshold && uiSteps.Count > 0)
                return;

            UIAction uiAction = new()
            {
                EventType = "mousemove",
                OffsetX = currentMouseHookStruct.pt.x,
                OffsetY = currentMouseHookStruct.pt.y,
                elapsed = stopwatch.Elapsed.TotalMilliseconds,
                ExtraInfo = (int)currentMouseHookStruct.dwExtraInfo,
                Flags = (int)currentMouseHookStruct.flags
            };

            uiSteps.Add(uiAction);

            stopwatch.Reset();
            stopwatch.Start();
        }

        private void RecordMouseWheel(int wparam)
        {
            UIAction uiAction = new()
            {
                EventType = "mousewheel",
                elapsed = stopwatch.Elapsed.TotalMilliseconds,
                MouseData = currentMouseHookStruct.mouseData,
                ExtraInfo = (int)currentMouseHookStruct.dwExtraInfo,
                Flags = currentMouseHookStruct.flags
            };

            uiAction.Flags |= MOUSEEVENTF_WHEEL;
            uiAction.MouseData = uiAction.MouseData >> 16; //get the high-order word

            uiSteps.Add(uiAction);

            stopwatch.Reset();
            stopwatch.Start();
        }

        private void RecordKeypress(int wparam)
        {
            UIAction uiAction = new()
            {
                EventType = "keypress",
                VKeyCode = keyboardHookStruct.VirtualKeyCode,
                ScanCode = keyboardHookStruct.ScanCode,
                Flags = keyboardHookStruct.Flags,
                ExtraInfo = keyboardHookStruct.ExtraInfo,
                elapsed = stopwatch.Elapsed.TotalMilliseconds
            };

            if (wparam == WM_KEYUP || wparam == WM_SYSKEYUP)
            {
                uiAction.Flags |= KEYEVENTF_KEYUP;
            }

            uiSteps.Add(uiAction);

            stopwatch.Reset();
            stopwatch.Start();

            labelStatus.Text = status;
        }

        private /*async*/ void RecordMouse(int wparam)
        {
            //await System.Threading.Tasks.Task.Run(() => RecordMouseWorker());
            RecordMouseWorker(wparam);

            labelStatus.Text = status;
        }

        private void RecordMouseWorker(int wparam)
        {
            var point = new Point(currentMouseHookStruct.pt.x, currentMouseHookStruct.pt.y);

            //merge the BUTTONUP and BUTTONDOWN events if coordinates are the same
            if (currentMouseHookStruct.pt.x == prevMouseHookStruct.pt.x &&
                currentMouseHookStruct.pt.y == prevMouseHookStruct.pt.y &&
                uiSteps.Count > 0 &&
                uiSteps[uiSteps.Count - 1].EventType == "click")
            {
                if (wparam == WM_LBUTTONUP)
                {
                    uiSteps[uiSteps.Count - 1].Flags |= MOUSEEVENTF_LEFTUP;
                    return;
                }

                if (wparam == WM_RBUTTONUP)
                {
                    uiSteps[uiSteps.Count - 1].Flags |= MOUSEEVENTF_RIGHTUP;
                    return;
                }
            }

            var uiAction = new UIAction();

            try
            {
                CacheRequest cacheRequest = new CacheRequest();
                cacheRequest.Add(AutomationElement.NameProperty);
                cacheRequest.Add(AutomationElement.AutomationIdProperty);
                cacheRequest.Add(AutomationElement.ControlTypeProperty);

                AutomationElement element = null;

                using (cacheRequest.Activate())
                {
                    element = AutomationElement.FromPoint(point);
                }

                string elementName = element.Cached.Name;
                string automationId = element.Cached.AutomationId;
                int elementTypeId = element.Cached.ControlType.Id;

                int level = 0;
                AutomationElement topLevelWindow = GetTopLevelWindow(element, out level);

                if (topLevelWindow == null)
                    throw new Exception("TopLevelWindow not found");

                int pid = (int)topLevelWindow.GetCurrentPropertyValue(AutomationElement.ProcessIdProperty);

                var process = Process.GetProcessById(pid);
                string processName = process.ProcessName;

                status = elementName + " : " + automationId + " [" + processName + "]";

                if (ElegantOptions.RestrictToExe)
                {
                    if (Path.GetFileNameWithoutExtension(ElegantOptions.ExePath).ToLower() != processName.ToLower())
                        return;
                }
                else
                {
                    //ignore actions on the recorder GUI
                    if (processName == Process.GetCurrentProcess().ProcessName)
                        return;
                }

                Rect boundingRect = (Rect)topLevelWindow.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);

                uiAction.EventType = "click";
                uiAction.ControlName = elementName;
                uiAction.AutomationId = automationId;
                uiAction.ControlType = elementTypeId;
                uiAction.TopLevelWindow = topLevelWindow.Current.Name;
                uiAction.OffsetX = (int)(point.X - boundingRect.X);
                uiAction.OffsetY = (int)(point.Y - boundingRect.Y);
                uiAction.Level = level;
                uiAction.elapsed = stopwatch.Elapsed.TotalMilliseconds;
                uiAction.ExtraInfo = (int)currentMouseHookStruct.dwExtraInfo;
                uiAction.Flags = (int)currentMouseHookStruct.flags;

                // performance hit
                //uiAction.ChildIndex = GetChildIndex(element);

                if (wparam == WM_LBUTTONDOWN)
                    uiAction.Flags |= MOUSEEVENTF_LEFTDOWN;
                if (wparam == WM_LBUTTONUP)
                    uiAction.Flags |= MOUSEEVENTF_LEFTUP;
                if (wparam == WM_RBUTTONDOWN)
                    uiAction.Flags |= MOUSEEVENTF_RIGHTDOWN;
                if (wparam == WM_RBUTTONUP)
                    uiAction.Flags |= MOUSEEVENTF_RIGHTUP;

                uiSteps.Add(uiAction);

                stopwatch.Reset();
                stopwatch.Start();
            }
            catch (Exception ex)
            {
                status = "Cannot get automation element: " + ex.ToString();
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

            InstallHooks();
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
            UninstallHooks();

            stopwatch.Reset();

            if (tokenSource != null)
            {
                tokenSource.Cancel();
            }

            if (recording)
            {
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
            var screenBounds = Screen.PrimaryScreen.Bounds;

            UIAction[] steps = JsonSerializer.Deserialize<UIAction[]>(File.ReadAllText(ElegantOptions.RecordingPath));

            if (currentActionIndex >= steps.Length - 1)
                currentActionIndex = 0;

            //var testwatch = new Stopwatch();
            //testwatch.Reset();
            //testwatch.Start();
            //double totalElapsed = 0;

            for (int i = currentActionIndex; i < steps.Length; i++)
            {
                var action = steps[i];

                currentActionIndex = i;

                Thread.Sleep((int)ElegantOptions.GetPlaybackSpeedDuration(action.elapsed));

                //todo delete
                //totalElapsed += action.elapsed;

                //if (i == 100)
                //{
                //    System.Windows.Forms.MessageBox.Show("Total elapsed: " + totalElapsed + " " + "Testwatch " + testwatch.ElapsedMilliseconds);
                //    return;
                //}

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
                    AutomationElement topLevelWindow = FindWindowByName(action.TopLevelWindow);

                    if (topLevelWindow == null)
                    {
                        status = "Failed to find window: " + action.TopLevelWindow;
                        ResetButtons();
                        return;
                    }

                    Rect windowRect = (Rect)topLevelWindow.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);

                    var x = (int)windowRect.X + action.OffsetX;
                    var y = (int)windowRect.Y + action.OffsetY;

                    AutomationElement targetElement = FindElementInWindow(action.ControlName, action.AutomationId, action.ControlType, topLevelWindow, action.Level, action.ChildIndex);

                    if (targetElement == null)
                    {
                        status = "Failed to find element: " + action.ControlName;
                    }
                    else
                    {
                        Rect elementRect = (Rect)targetElement.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);

                        if (elementRect.X > x || x > elementRect.X + elementRect.Width ||
                            elementRect.Y > y || y > elementRect.Y + elementRect.Height)
                        {
                            var point = targetElement.GetClickablePoint();
                            x = (int)point.X;
                            y = (int)point.Y;
                        }
                    }

                    int dx = (int)x * 65535 / screenBounds.Width + 1;
                    int dy = (int)y * 65535 / screenBounds.Height + 1;

                    if (action.Flags == MOUSEEVENTF_LEFTUP)
                    {
                        mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, dx, dy, 0, (uint)action.ExtraInfo);
                        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, (uint)action.ExtraInfo);
                    }
                    else if (action.Flags == MOUSEEVENTF_RIGHTUP)
                    {
                        mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, dx, dy, 0, (uint)action.ExtraInfo);
                        mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, (uint)action.ExtraInfo);
                    }
                    else
                    {
                        Cursor.Position = new System.Drawing.Point((int)x, (int)y);
                        mouse_event((uint)action.Flags, 0, 0, 0, (uint)action.ExtraInfo);
                    }
                }
                else if (action.EventType == "mousemove")
                {
                    //todo - refactor repeatable code
                    int dx = (int)action.OffsetX * 65535 / screenBounds.Width + 1;
                    int dy = (int)action.OffsetY * 65535 / screenBounds.Height + 1;

                    mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, dx, dy, 0, (uint)action.ExtraInfo);
                }
                else if (action.EventType == "mousewheel")
                {
                    mouse_event(MOUSEEVENTF_WHEEL, 0, 0, (int)action.MouseData, (uint)action.ExtraInfo);
                }
                else if (action.EventType == "keypress")
                {
                    keybd_event((uint)action.VKeyCode, (uint)action.ScanCode, (uint)action.Flags, (uint)action.ExtraInfo);
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
    }
}
