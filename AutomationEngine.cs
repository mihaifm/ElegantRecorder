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
            uiAction.ExtraInfo = (int)currentMouseHookStruct.dwExtraInfo;
            uiAction.Flags = currentMouseHookStruct.flags;

            return true;
        }

        public virtual bool FillMouseWheelAction(ref UIAction uiAction, ref string status, MouseHookStruct currentMouseHookStruct)
        {
            uiAction.EventType = "mousewheel";
            uiAction.MouseData = currentMouseHookStruct.mouseData;
            uiAction.ExtraInfo = (int)currentMouseHookStruct.dwExtraInfo;
            uiAction.Flags = currentMouseHookStruct.flags;

            return true;
        }

        public virtual bool FillKeypressAction(ref UIAction uiAction, ref string status, KeyboardHookStruct keyboardHookStruct)
        {
            uiAction.EventType = "keypress";
            uiAction.VKeyCode = keyboardHookStruct.VirtualKeyCode;
            uiAction.ScanCode = keyboardHookStruct.ScanCode;
            uiAction.Flags = keyboardHookStruct.Flags;
            uiAction.ExtraInfo = keyboardHookStruct.ExtraInfo;

            return true;
        }

        public abstract bool ReplayClickAction(UIAction action, ref string status);

        public virtual bool ReplayMouseMoveAction(UIAction action, ref string status)
        {
            App.WinAPI.MouseMove((int)action.OffsetX, (int)action.OffsetY, (uint)action.ExtraInfo);
            return true;
        }

        public virtual bool ReplayMouseWheelAction(UIAction action, ref string status)
        {
            App.WinAPI.MouseWheel((int)action.MouseData, (uint)action.ExtraInfo);
            return true;
        }

        public virtual bool ReplayKeypressAction(UIAction action, ref string status)
        {
            App.WinAPI.KeyPress((uint)action.VKeyCode, (uint)action.ScanCode, (uint)action.Flags, (uint)action.ExtraInfo);
            return true;
        }
    }
}
