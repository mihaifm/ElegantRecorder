using System;

namespace ElegantRecorder
{
    public class Triggers
    {
        public int Hotkey { get; set; }
        public bool HotkeyEnabled { get; set; }

        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public bool TimeEnabled { get; set; }

        public string WindowName { get; set; }
        public bool WindowEnabled { get; set; }

        public string FilePath { get; set; }
        public bool FileEnabled { get; set; }

        public string RecordingName { get; set; }
        public bool RecordingEnabled { get; set; }

        public Triggers()
        {
            Hotkey = 0;
            HotkeyEnabled = false;
            Date = DateTime.Now;
            Time = DateTime.Now;
            TimeEnabled = false;
            WindowName = "";
            WindowEnabled = false;
            FilePath = "";
            FileEnabled = false;
            RecordingName = "";
            RecordingEnabled = false;
        }
    }
}
