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

namespace UIAuto
{
    public partial class ElegantRecorder : Form
    {
        private IntPtr mouseHookID = IntPtr.Zero;
        private IntPtr keyboardHookID = IntPtr.Zero;

        private const int WH_MOUSE_LL = 14;
        private const int WH_KEYBOARD_LL = 13;
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYUP = 0x105;
        private const int KEYEVENTF_KEYUP = 0x0002;

        private string scriptName;
        private string exeName;
        private string status;
        private Stopwatch stopwatch = new Stopwatch();
        private struct UIAction
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
            [JsonPropertyName("vkcode")]
            public int? VKeyCode { get; set; }
            [JsonPropertyName("scancode")]
            public int? ScanCode { get; set; }
            [JsonPropertyName("flags")]
            public int? Flags { get; set; }
            [JsonPropertyName("einfo")]
            public int? ExtraInfo { get; set; }
            [JsonPropertyName("t")]
            public double elapsed { get; set; }
        }

        List<UIAction> uiSteps = new List<UIAction>();

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

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
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        MouseHookStruct mouseHookStruct;

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
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void keybd_event(uint bVk, uint bScan, uint dwFlags, uint dwExtraInfo);

        public ElegantRecorder()
        {
            InitializeComponent();
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
            if (nCode >= 0 && MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
            {
                mouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));

                RecordClick();
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
                    int pid = (int)window.GetCurrentPropertyValue(AutomationElement.ProcessIdProperty);
                    var process = Process.GetProcessById(pid);

                    if (Path.GetFileNameWithoutExtension(exeName).ToLower() == process.ProcessName.ToLower())
                        return window;
                }

                window = walker.GetNextSibling(window);
            }

            return null;
        }

        private AutomationElement FindElementInWindow(string controlName, string automationId, int? controlType, AutomationElement window, int? level)
        {
            int elCount = 0;
            var foundElements = new List<AutomationElement>();

            void TreeFinder(AutomationElement currentNode, int currLevel)
            {
                if (currLevel == level)
                {
                    if (currentNode.Current.Name == controlName && 
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

            if (elCount != 1)
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

        private void RecordKeypress(int wparam)
        {
            RecordKeypressWorker(wparam);

            labelStatus.Text = status;
        }

        private void RecordKeypressWorker(int wparam)
        {
            UIAction uiAction = new UIAction();

            uiAction.EventType = "keypress";
            uiAction.VKeyCode = keyboardHookStruct.VirtualKeyCode;
            uiAction.ScanCode = keyboardHookStruct.ScanCode;
            uiAction.Flags = keyboardHookStruct.Flags;
            uiAction.ExtraInfo = keyboardHookStruct.ExtraInfo;
            uiAction.elapsed = stopwatch.Elapsed.TotalMilliseconds;

            if (wparam == WM_KEYUP || wparam == WM_SYSKEYUP)
            {
                uiAction.Flags |= KEYEVENTF_KEYUP;
            }

            uiSteps.Add(uiAction);

            stopwatch.Reset();
            stopwatch.Start();
        }

        private /*async*/ void RecordClick()
        {
            //await System.Threading.Tasks.Task.Run(() => RecordClickWorker());
            RecordClickWorker();

            labelStatus.Text = status;
        }

        private void RecordClickWorker()
        {
            var point = new Point(mouseHookStruct.pt.x, mouseHookStruct.pt.y);

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

                if (Path.GetFileNameWithoutExtension(exeName).ToLower() != processName.ToLower())
                    return;

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

                uiSteps.Add(uiAction);

                stopwatch.Reset();
                stopwatch.Start();
            }
            catch (Exception ex)
            {
                status = "Cannot get automation element: " + ex.ToString();
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBoxScriptName.Text = dialog.FileName;
                scriptName = dialog.FileName;
            }
        }

        private void buttonBrowseExe_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Exe files (*.exe)|*.exe|All files (*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBoxExeName.Text = dialog.FileName;
                exeName = dialog.FileName;
            }
        }

        private void buttonRecord_Click(object sender, EventArgs e)
        {
            if (scriptName == null || scriptName.Length == 0)
            {
                labelStatus.Text = "Specify script name";
                return;
            }

            if (exeName == null || exeName.Length == 0)
            {
                labelStatus.Text = "Specify target executable";
                return;
            }

            if (!File.Exists(scriptName))
            {
                try
                {
                    File.Create(scriptName);
                }
                catch
                {
                    labelStatus.Text = "Failed to create script file";
                    return;
                }
            }

            uiSteps.Clear();
            File.WriteAllText(scriptName, "");
            stopwatch.Reset();

            InstallHooks();

            buttonRecord.Text = "Recording...";
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            UninstallHooks();

            buttonRecord.Text = "Record";
            stopwatch.Reset();

            File.WriteAllText(scriptName, "[\n");

            JsonSerializerOptions jsonOptions = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            for (int i = 0; i < uiSteps.Count; i++)
            {
                string jsonString = JsonSerializer.Serialize(uiSteps[i], jsonOptions);
                File.AppendAllText(scriptName, jsonString);

                if (i != uiSteps.Count - 1)
                {
                    File.AppendAllText(scriptName, ",\n");
                }
                else
                {
                    File.AppendAllText(scriptName, "\n");
                }
            }

            File.AppendAllText(scriptName, "]");
        }

        private void buttonReplay_Click(object sender, EventArgs e)
        {
            labelStatus.Text = "";

            UIAction[] steps = JsonSerializer.Deserialize<UIAction[]>(File.ReadAllText(scriptName));

            foreach (var action in steps)
            {
                Thread.Sleep((int)action.elapsed);

                if (action.EventType == "click")
                {
                    AutomationElement topLevelWindow = FindWindowByName(action.TopLevelWindow);

                    if (topLevelWindow == null)
                    {
                        labelStatus.Text = "Failed to find window: " + action.TopLevelWindow;
                        return;
                    }

                    AutomationElement targetElement = FindElementInWindow(action.ControlName, action.AutomationId, action.ControlType, topLevelWindow, action.Level);

                    if (targetElement == null)
                    {
                        labelStatus.Text = "Failed to find element: " + action.ControlName;
                        return;
                    }

                    Rect windowRect = (Rect)topLevelWindow.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
                    Rect elementRect = (Rect)targetElement.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);

                    var x = (int)windowRect.X + action.OffsetX;
                    var y = (int)windowRect.Y + action.OffsetY;

                    if (elementRect.X > x || x > elementRect.X + elementRect.Width ||
                        elementRect.Y > y || y > elementRect.Y + elementRect.Height)
                    {
                        var point = targetElement.GetClickablePoint();
                        x = (int)point.X;
                        y = (int)point.Y;
                    }

                    Cursor.Position = new System.Drawing.Point((int)x, (int)y);
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
                }
                else if (action.EventType == "keypress")
                {
                    keybd_event((uint)action.VKeyCode, (uint)action.ScanCode, (uint)action.Flags, (uint)action.ExtraInfo);
                }
            }
        }

        private void textBoxScriptName_TextChanged(object sender, EventArgs e)
        {
            scriptName = textBoxScriptName.Text;
        }

        private void textBoxExeName_TextChanged(object sender, EventArgs e)
        {
            exeName = textBoxExeName.Text;
        }
    }
}
