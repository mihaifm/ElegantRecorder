using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ElegantRecorder
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left, Top, Right, Bottom;
    }

        [StructLayout(LayoutKind.Sequential)]
    public struct MouseHookStruct
    {
        public POINT pt;
        public int mouseData;
        public int flags;
        public int time;
        public UIntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardHookStruct
    {
        public int VirtualKeyCode;
        public int ScanCode;
        public int Flags;
        public int Time;
        public UIntPtr ExtraInfo;
    }

    public enum GetAncestorFlags
    {
        GetParent = 1,
        GetRoot = 2,
        GetRootOwner = 3
    }

    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8
    }

    public class WinAPI
    {
        private ElegantRecorder App;
        private Rectangle screenBounds;
        private string moduleName = null;

        public WinAPI(ElegantRecorder App)
        {
            this.App = App;
            screenBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

            Process curProcess = Process.GetCurrentProcess();
            ProcessModule curModule = curProcess.MainModule;
            moduleName = curModule.ModuleName;
        }

        private IntPtr mouseHookID = IntPtr.Zero;
        private IntPtr keyboardHookID = IntPtr.Zero;

        public int recordHotkeyId = 1;
        public int stopHotkeyId = 2;

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
        private const int WM_HOTKEY = 0x0312;

        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int MOUSEEVENTF_WHEEL = 0x0800;
        private const int KEYEVENTF_KEYUP = 0x0002;

        private const int WM_CLIPBOARDUPDATE = 0x031D;

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
        public static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void keybd_event(uint bVk, uint bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(POINT point);

        [DllImport("user32.dll")]
        public static extern IntPtr GetAncestor(IntPtr hWnd, GetAncestorFlags gaFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, Keys vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private delegate IntPtr LowLevelHookProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelHookProc mouseDelegate = null;
        private LowLevelHookProc keyboardDelegate = null;

        public MouseHookStruct CurrentMouseHookStruct;
        public MouseHookStruct PrevMouseHookStruct;
        public KeyboardHookStruct KeyboardHookStruct;

        private int wparam;

        public void InstallHooks()
        {
            mouseDelegate = new LowLevelHookProc(MouseHookCallback);

            mouseHookID = SetWindowsHookEx(WH_MOUSE_LL, mouseDelegate, GetModuleHandle(moduleName), 0);

            keyboardDelegate = new LowLevelHookProc(KeyboardHookCallback);

            keyboardHookID = SetWindowsHookEx(WH_KEYBOARD_LL, keyboardDelegate, GetModuleHandle(moduleName), 0);

            AddClipboardFormatListener(App.Handle);
        }

        public void UninstallHooks()
        {
            UnhookWindowsHookEx(mouseHookID);
            UnhookWindowsHookEx(keyboardHookID);

            RemoveClipboardFormatListener(App.Handle);
        }

        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            CurrentMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));

            wparam = wParam.ToInt32();

            if (nCode >= 0)
            {
                if (wParam.ToInt32() == WM_LBUTTONDOWN || wParam.ToInt32() == WM_LBUTTONUP ||
                    wParam.ToInt32() == WM_RBUTTONDOWN || wParam.ToInt32() == WM_RBUTTONUP)
                {
                    if (wparam == WM_LBUTTONDOWN)
                        CurrentMouseHookStruct.flags |= MOUSEEVENTF_LEFTDOWN;
                    if (wparam == WM_LBUTTONUP)
                        CurrentMouseHookStruct.flags |= MOUSEEVENTF_LEFTUP;
                    if (wparam == WM_RBUTTONDOWN)
                        CurrentMouseHookStruct.flags |= MOUSEEVENTF_RIGHTDOWN;
                    if (wparam == WM_RBUTTONUP)
                        CurrentMouseHookStruct.flags |= MOUSEEVENTF_RIGHTUP;

                    App.RecordMouse(CurrentMouseHookStruct, PrevMouseHookStruct);

                    PrevMouseHookStruct = CurrentMouseHookStruct;
                }
                else if (wParam.ToInt32() == WM_MOUSEMOVE)
                {
                    App.RecordMouseMove(CurrentMouseHookStruct);
                }
                else if (wParam.ToInt32() == WM_MOUSEWHEEL)
                {
                    CurrentMouseHookStruct.flags |= MOUSEEVENTF_WHEEL;
                    CurrentMouseHookStruct.mouseData = CurrentMouseHookStruct.mouseData >> 16; //get the high-order word

                    App.RecordMouseWheel(CurrentMouseHookStruct);
                }
            }

            return CallNextHookEx(mouseHookID, nCode, wParam, lParam);
        }

        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            wparam = wParam.ToInt32();

            if (nCode >= 0)
            {
                KeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));

                if (wParam.ToInt32() == WM_KEYDOWN || wParam.ToInt32() == WM_SYSKEYDOWN ||
                    wParam.ToInt32() == WM_KEYUP || wParam.ToInt32() == WM_SYSKEYUP)
                {
                    if (wParam.ToInt32() == WM_KEYUP || wParam.ToInt32() == WM_SYSKEYUP)
                    {
                        KeyboardHookStruct.Flags |= KEYEVENTF_KEYUP;
                    }

                    App.RecordKeypress(KeyboardHookStruct);
                }
            }

            return CallNextHookEx(keyboardHookID, nCode, wParam, lParam);
        }

        public bool MergeMouseEvents(ref int previousFlags)
        {
            if (wparam == WM_LBUTTONUP)
            {
                previousFlags |= MOUSEEVENTF_LEFTUP;
                return true;
            }

            if (wparam == WM_RBUTTONUP)
            {
                previousFlags |= MOUSEEVENTF_RIGHTUP;
                return true;
            }

            return false;
        }

        public void MouseClick(int x, int y, uint flags, UIntPtr extraInfo)
        {
            int dx = x * 65535 / screenBounds.Width + 1;
            int dy = y * 65535 / screenBounds.Height + 1;

            if (flags == MOUSEEVENTF_LEFTUP)
            {
                mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, dx, dy, 0, extraInfo);
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, extraInfo);
            }
            else if (flags == MOUSEEVENTF_RIGHTUP)
            {
                mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, dx, dy, 0, extraInfo);
                mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, extraInfo);
            }
            else
            {
                System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)x, (int)y);
                mouse_event(flags, 0, 0, 0, extraInfo);
            }
        }

        public void MouseMove(int x, int y, UIntPtr extraInfo)
        {
            //todo - refactor repeatable code
            int dx = x * 65535 / screenBounds.Width + 1;
            int dy = y * 65535 / screenBounds.Height + 1;

            mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, dx, dy, 0, extraInfo);
        }

        public void MouseWheel(int mouseData, UIntPtr extraInfo)
        {
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, mouseData, extraInfo);
        }

        public void KeyPress(uint vKeyCode, uint scanCode, uint flags, UIntPtr extraInfo)
        {
            keybd_event(vKeyCode, scanCode, flags, extraInfo);
        }

        public IntPtr GetTopLevelWindow(System.Drawing.Point point)
        {
            IntPtr hWnd = WindowFromPoint(new POINT { x = point.X, y = point.Y });

            if (hWnd != IntPtr.Zero)
            {
                return GetAncestor(hWnd, GetAncestorFlags.GetRoot);
            }

            return IntPtr.Zero;
        }

        public string GetWindowName(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
            {
                int length = GetWindowTextLength(hwnd);
                StringBuilder sb = new StringBuilder(length + 1);
                GetWindowText(hwnd, sb, sb.Capacity);

                return sb.ToString();
            }

            return null;
        }

        public int GetWindowPID(IntPtr hwnd)
        {
            uint pid = 0;
            GetWindowThreadProcessId(hwnd, out pid);
            return (int) pid;
        }

        public System.Windows.Rect GetBoundingRect(IntPtr hwnd)
        {
            RECT rect;
            GetWindowRect(hwnd, out rect);

            return new System.Windows.Rect { X = rect.Left, Y = rect.Top, Width = rect.Right, Height = rect.Bottom };
        }

        public bool ProcessClipboardMessage(int message, ref string clipboardText)
        {
            if (message == WM_CLIPBOARDUPDATE)
            {
                IDataObject iData = Clipboard.GetDataObject();

                if (iData.GetDataPresent(DataFormats.Text))
                {
                    clipboardText = (string)iData.GetData(DataFormats.Text);
                    return true;
                }
            }

            return false;
        }

        public static void GetModifiers(Keys keydata, out Keys key, out KeyModifiers modifers)
        {
            key = keydata;
            modifers = KeyModifiers.None;

            if ((keydata & Keys.Control) == Keys.Control)
            {
                modifers |= KeyModifiers.Control;
                key = keydata ^ Keys.Control;
            }

            if ((keydata & Keys.Shift) == Keys.Shift)
            {
                modifers |= KeyModifiers.Shift;
                key = key ^ Keys.Shift;
            }

            if ((keydata & Keys.Alt) == Keys.Alt)
            {
                modifers |= KeyModifiers.Alt;
                key = key ^ Keys.Alt;
            }

            if (key == Keys.ShiftKey || key == Keys.ControlKey || key == Keys.Menu)
            {
                key = Keys.None;
            }
        }

        public void RegisterGlobalHotkeys()
        {
            if (App.ElegantOptions.RecordHotkey != 0)
            {
                GetModifiers((Keys) App.ElegantOptions.RecordHotkey, out var key, out var modifiers);
                RegisterHotKey(App.Handle, recordHotkeyId, modifiers, key);
            }

            if (App.ElegantOptions.StopHotkey != 0)
            {
                GetModifiers((Keys)App.ElegantOptions.StopHotkey, out var key, out var modifiers);
                RegisterHotKey(App.Handle, stopHotkeyId, modifiers, key);
            }
        }

        public void UnregisterGlobalHotkeys()
        {
            UnregisterHotKey(App.Handle, recordHotkeyId);
            UnregisterHotKey(App.Handle, stopHotkeyId);
        }

        public void UnregisterTriggerHotkey(int hotkeyId)
        {
            UnregisterHotKey(App.Handle, hotkeyId);
        }

        public void RegisterTriggerHotkey(int hotkey)
        {
            if (hotkey != 0)
            {
                GetModifiers((Keys)hotkey, out var key, out var modifiers);
                RegisterHotKey(App.Handle, App.CurrentHotkeyId, modifiers, key);
            }
        }

        public void ProcessHotkeyMessage(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                /*
                var key = (System.Windows.Forms.Keys)(((int)m.LParam >> 16) & 0xFFFF);
                var modifier = (System.Windows.Input.ModifierKeys)((int)m.LParam & 0xFFFF);
                */

                int id = m.WParam.ToInt32();

                if (id == recordHotkeyId)
                {
                    App.Record();
                }
                else if (id == stopHotkeyId)
                {
                    App.Stop(false);
                }

                if (App.RecHotkeys.ContainsKey(id))
                {
                    App.Replay(App.RecHotkeys[id]);
                }
            }
        }

        public bool isKeyUp(int flags)
        {
            return ((flags & KEYEVENTF_KEYUP) == KEYEVENTF_KEYUP);
        }

        public string HotkeyToStr(int hotkey)
        {
            return string.Join("+", ((Keys)hotkey).ToString().Split(", ").Reverse());
        }
    }
}
