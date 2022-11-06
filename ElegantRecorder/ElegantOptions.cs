namespace ElegantRecorder
{
    public class ElegantOptions
    {
        public string PlaybackSpeed { get; set; }
        public int RecordHotkey { get; set; }
        public int StopHotkey { get; set; }
        public bool RecordMouseMove { get; set; }
        public bool RecordClipboard { get; set; }
        public bool RestrictToExe { get; set; }
        public string ExePath { get; set; }
        public string AutomationEngine { get; set; }
        public string DataFolder { get; set; }

        public ElegantOptions()
        {
            PlaybackSpeed = "Normal";
            RecordHotkey = 0;
            StopHotkey = 0;
            RecordMouseMove = false;
            RecordClipboard = false;
            RestrictToExe = false;
            ExePath = "";
            AutomationEngine = "Win32";
            DataFolder = System.Windows.Forms.Application.StartupPath;
        }

        public double GetPlaybackSpeedDuration(double initialDuration)
        {
            switch(PlaybackSpeed)
            {
                case "Fastest":
                    return 0;
                case "Fast":
                    return 0.5 * initialDuration;
                case "Normal":
                    return initialDuration;
                case "Slow":
                    return 1.5 * initialDuration;
                case "Slowest":
                    return 2.0 * initialDuration;
            }

            return initialDuration;
        }
    }
}
