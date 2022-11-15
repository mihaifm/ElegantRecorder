using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;

namespace ElegantRecorder
{
    public abstract class AutomationEngine
    {
        public ElegantRecorder App;

        public AutomationEngine(ElegantRecorder App)
        {
            this.App = App;
        }

        public abstract bool FillClickAction(ref UIAction uiAction, ref string status, MouseHookStruct currentMouseHookStruct);

        public virtual bool FillMouseMoveAction(ref UIAction uiAction, ref string status, MouseHookStruct currentMouseHookStruct)
        {
            var point = new System.Drawing.Point(currentMouseHookStruct.pt.x, currentMouseHookStruct.pt.y);

            IntPtr activeWinHwnd = App.WinAPI.GetActiveWin();

            RECT boundingRect = new RECT();

            if (App.Options.MouseMoveRelative)
            {
                if (activeWinHwnd != IntPtr.Zero)
                    boundingRect = App.WinAPI.GetBoundingRect(activeWinHwnd);
            }

            uiAction.EventType = "mousemove";
            uiAction.OffsetX = point.X - boundingRect.Left;
            uiAction.OffsetY = point.Y - boundingRect.Top;
            uiAction.ExtraInfo = (long)currentMouseHookStruct.dwExtraInfo;
            uiAction.Flags = currentMouseHookStruct.flags;

            return true;
        }

        public virtual bool FillMousePathAction(ref UIAction uiAction, ref string status, List<MoveData> moveData)
        {
            uiAction.EventType = "mousepath";
            uiAction.MoveData = moveData.ToArray();

            return true;
        }

        public virtual bool FillMouseWheelAction(ref UIAction uiAction, ref string status, MouseHookStruct currentMouseHookStruct)
        {
            uiAction.EventType = "mousewheel";
            uiAction.MouseData = currentMouseHookStruct.mouseData;
            uiAction.ExtraInfo = (long)currentMouseHookStruct.dwExtraInfo;
            uiAction.Flags = currentMouseHookStruct.flags;

            return true;
        }

        public virtual bool FillKeypressAction(ref UIAction uiAction, ref string status, KeyboardHookStruct keyboardHookStruct)
        {
            uiAction.EventType = "keypress";
            uiAction.VKeyCode = keyboardHookStruct.VirtualKeyCode;
            uiAction.ScanCode = keyboardHookStruct.ScanCode;
            uiAction.Flags = keyboardHookStruct.Flags;
            uiAction.ExtraInfo = (long)keyboardHookStruct.ExtraInfo;
            return true;
        }

        public virtual bool FillClipboardAction(ref UIAction uiAction, ref string status, string clipboardText)
        {
            uiAction.EventType = "clipboard";
            uiAction.TextData = clipboardText;
            return true;
        }

        public virtual void ReplayAction(UIAction action, ref string status)
        {
            bool result = true;

            if (action.EventType == "click")
            {
                result = ReplayClickAction(action, ref status);
                App.PlayAction(result);
            }
            else if (action.EventType == "mousemove")
            {
                result = ReplayMouseMoveAction(action, ref status);
                App.PlayAction(result);
            }
            else if (action.EventType == "mousewheel")
            {
                result = ReplayMouseWheelAction(action, ref status);
                App.PlayAction(result);
            }
            else if (action.EventType == "keypress")
            {
                result = ReplayKeypressAction(action, ref status);
                App.PlayAction(result);
            }
            else if (action.EventType == "clipboard")
            {
                result = ReplayClipboardAction(action, ref status);
                App.PlayAction(result);
            }
            else if (action.EventType == "mousepath")
            {
                result = ReplayMousePathAction(action, ref status);
            }
        }

        public abstract bool ReplayClickAction(UIAction action, ref string status);

        public virtual bool ReplayMouseMoveAction(UIAction action, ref string status)
        {
            IntPtr activeWinHwnd = App.WinAPI.GetActiveWin();

            RECT boundingRect = new RECT();

            if (App.Options.MouseMoveRelative && activeWinHwnd != IntPtr.Zero)
            {
                boundingRect = App.WinAPI.GetBoundingRect(activeWinHwnd);
            }

            var x = boundingRect.Left + (int) action.OffsetX;
            var y = boundingRect.Top + (int) action.OffsetY;

            App.WinAPI.MouseMove(x, y, (UIntPtr)action.ExtraInfo);
            return true;
        }

        Timer mousePathTimer = null;
        int currentMousePathStep = -1;
        UIAction mousePathAction = null;
        RECT activeWinRect;

        public virtual bool ReplayMousePathAction(UIAction action, ref string status)
        {
            mousePathAction = action;
            currentMousePathStep = -1;
            activeWinRect = new RECT();

            mousePathTimer = new Timer();
            mousePathTimer.Tick += MousePathTimer_Tick;
            PlayMousePathStep();

            return false;
        }

        private void PlayMousePathStep()
        {
            mousePathTimer.Stop();

            if (App.PlayerState == State.Stop || App.PlayerState == State.ReplayPause)
            {
                App.PlayAction(true);
                return;
            }

            currentMousePathStep++;

            if (currentMousePathStep <= mousePathAction.MoveData.Length - 1)
            {
                if (mousePathAction.MoveData[currentMousePathStep].T > 0)
                {
                    mousePathTimer.Interval = Options.GetPlaybackSpeed(App.Options.PlaybackSpeed, mousePathAction.MoveData[currentMousePathStep].T);
                    mousePathTimer.Start();
                }
                else
                {
                    MousePathTimer_Tick(null, new EventArgs());
                }
            }
            else
            {
                App.PlayAction(true);
            }
        }

        private void MousePathTimer_Tick(object? sender, EventArgs e)
        {
            if (App.Options.MouseMoveRelative)
            {
                if (currentMousePathStep == 0)
                {
                    IntPtr activeWinHwnd = App.WinAPI.GetActiveWin();
                    if (activeWinHwnd != IntPtr.Zero)
                    {
                        activeWinRect = App.WinAPI.GetBoundingRect(activeWinHwnd);
                    }
                }
            }

            var m = mousePathAction.MoveData[currentMousePathStep];
            App.WinAPI.MouseMove(activeWinRect.Left + m.X, activeWinRect.Top + m.Y, UIntPtr.Zero);

            PlayMousePathStep();
        }

        public virtual bool ReplayMouseWheelAction(UIAction action, ref string status)
        {
            App.WinAPI.MouseWheel((int)action.MouseData, (UIntPtr)action.ExtraInfo);
            return true;
        }

        public virtual bool ReplayKeypressAction(UIAction action, ref string status)
        {
            App.WinAPI.KeyPress((uint)action.VKeyCode, (uint)action.ScanCode, (uint)action.Flags, (UIntPtr)action.ExtraInfo);
            return true;
        }

        public virtual bool ReplayClipboardAction(UIAction action, ref string status)
        {
            System.Windows.Clipboard.SetText(action.TextData);
            return true;
        }

        public void CleanResidualKeys()
        {
            //prevent keys remaining pressed at the end of the recording

            for (int i = App.UISteps.Count - 1; i >= 0; i--)
            {
                if (App.UISteps[i].EventType == "keypress")
                {
                    if (App.WinAPI.isKeyUp((int)App.UISteps[i].Flags))
                        break;
                    else
                        App.UISteps.RemoveAt(i);
                }
                else
                {
                    if (App.UISteps[i].EventType == "mousepath" || App.UISteps[i].EventType == "mousemove")
                        continue;
                    else
                        break;
                }
            }
        }

        public void CompressMoveData()
        {
            List<MoveData> moveData = new List<MoveData>();
            string status = "";

            for (int i = App.UISteps.Count - 1; i >= 0; i--)
            {
                if (App.UISteps[i].EventType == "mousemove")
                {
                    moveData.Insert(0, new MoveData { X = (int)App.UISteps[i].OffsetX, Y = (int)App.UISteps[i].OffsetY, T = (int)App.UISteps[i].elapsed });
                    App.UISteps.RemoveAt(i);
                }
                else
                {
                    if (moveData.Count != 0)
                    {
                        var uiAction = new UIAction();
                        FillMousePathAction(ref uiAction, ref status, moveData);

                        App.UISteps.Insert(i + 1, uiAction);

                        moveData.Clear();
                    }
                }
            }

            if (moveData.Count != 0)
            {
                var uiAction = new UIAction();
                FillMousePathAction(ref uiAction, ref status, moveData);

                App.UISteps.Insert(0, uiAction);

                moveData.Clear();
            }
        }
    }
}
