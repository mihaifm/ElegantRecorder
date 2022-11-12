using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ElegantRecorder
{
    public class Win32Engine: AutomationEngine
    {
        public Win32Engine(ElegantRecorder App) : base(App)
        {
        }

        public override bool FillClickAction(ref UIAction uiAction, ref string status, MouseHookStruct currentMouseHookStruct)
        {
            var point = new System.Drawing.Point(currentMouseHookStruct.pt.x, currentMouseHookStruct.pt.y);

            IntPtr topLevelWindow = App.WinAPI.GetTopLevelWindow(point);

            if (topLevelWindow == IntPtr.Zero)
            {
                status = "TopLevelWindow not found";
                return false;
            }

            int pid = App.WinAPI.GetWindowPID(topLevelWindow);

            var process = Process.GetProcessById(pid);
            string processName = process.ProcessName;

            string topLevelWindowName = App.WinAPI.GetWindowName(topLevelWindow);

            /*
            if (topLevelWindowName.Length == 0)
            {
                if (lastNamedWindow != IntPtr.Zero)
                {
                    topLevelWindow = lastNamedWindow;
                    topLevelWindowName = App.WinAPI.GetWindowName(topLevelWindow);
                }
            }
            else
            {
                lastNamedWindow = topLevelWindow;
            }
            */

            if (App.ElegantOptions.RestrictToExe)
            {
                if (Path.GetFileNameWithoutExtension(App.ElegantOptions.ExePath).ToLower() != processName.ToLower())
                    return false;
            }
            else
            {
                //ignore actions on the recorder GUI
                if (processName == Process.GetCurrentProcess().ProcessName)
                    return false;
            }

            RECT boundingRect;

            if (topLevelWindowName.Length != 0)
                boundingRect = App.WinAPI.GetBoundingRect(topLevelWindow);
            else
                boundingRect = new RECT();


            uiAction.EventType = "click";
            uiAction.TopLevelWindow = topLevelWindowName;
            uiAction.OffsetX = point.X - boundingRect.Left;
            uiAction.OffsetY = point.Y - boundingRect.Top;
            uiAction.ExtraInfo = (long)currentMouseHookStruct.dwExtraInfo;
            uiAction.Flags = (int)currentMouseHookStruct.flags;

            return true;
        }

        public override bool ReplayClickAction(UIAction action, ref string status)
        {
            IntPtr topLevelWindow = WinAPI.FindWindow(null, action.TopLevelWindow);

            int pid = App.WinAPI.GetWindowPID(topLevelWindow);

            var process = Process.GetProcessById(pid);
            string processName = process.ProcessName;

            if (App.ElegantOptions.RestrictToExe)
            {
                if (Path.GetFileNameWithoutExtension(App.ElegantOptions.ExePath).ToLower() != processName.ToLower())
                {
                    status = "Failed to find window: " + action.TopLevelWindow;
                    return false;
                }
            }

            RECT windowRect = App.WinAPI.GetBoundingRect(topLevelWindow);

            var x = windowRect.Left + (int) action.OffsetX;
            var y = windowRect.Top + (int) action.OffsetY;

            App.WinAPI.MouseClick(x, y, (uint)action.Flags, (UIntPtr)action.ExtraInfo);

            return true;
        }
    }
}
