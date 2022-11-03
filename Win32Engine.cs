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

            Rect boundingRect;

            if (topLevelWindowName.Length != 0)
                boundingRect = App.WinAPI.GetBoundingRect(topLevelWindow);
            else
                boundingRect = new Rect(0, 0, 0, 0);


            uiAction.EventType = "click";
            uiAction.TopLevelWindow = topLevelWindowName;
            uiAction.OffsetX = (int)(point.X - boundingRect.X);
            uiAction.OffsetY = (int)(point.Y - boundingRect.Y);
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

            Rect windowRect = App.WinAPI.GetBoundingRect(topLevelWindow);

            var x = (int)windowRect.X + action.OffsetX;
            var y = (int)windowRect.Y + action.OffsetY;

            App.WinAPI.MouseClick((int)x, (int)y, (uint)action.Flags, (UIntPtr)action.ExtraInfo);

            return true;
        }
    }
}
