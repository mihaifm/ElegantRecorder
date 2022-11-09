using System;
using System.Collections.Generic;
using System.Threading;

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
            uiAction.EventType = "mousemove";
            uiAction.OffsetX = currentMouseHookStruct.pt.x;
            uiAction.OffsetY = currentMouseHookStruct.pt.y;
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

        public abstract bool ReplayClickAction(UIAction action, ref string status);

        public virtual bool ReplayMouseMoveAction(UIAction action, ref string status)
        {
            App.WinAPI.MouseMove((int)action.OffsetX, (int)action.OffsetY, (UIntPtr)action.ExtraInfo);
            return true;
        }

        public virtual bool ReplayMousePathAction(UIAction action, string playbackSpeed, ref string status)
        {
            foreach(var m in action.MoveData)
            {
                App.WinAPI.MouseMove(m.X, m.Y, UIntPtr.Zero);
                Thread.Sleep((int)ElegantOptions.GetPlaybackSpeedDuration(playbackSpeed, m.T));
            }

            return true;
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
