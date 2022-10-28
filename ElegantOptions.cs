namespace ElegantRecorder
{
    public class ElegantOptions
    {
        public string PlaybackSpeed { get; set; }
        public bool RecordMouseMove { get; set; }
        public bool RestrictToExe { get; set; }
        public string ExePath { get; set; }
        public string RecordingPath { get; set; }

        public ElegantOptions()
        {
            PlaybackSpeed = "Normal";
            RecordMouseMove = false;
            RestrictToExe = false;
            ExePath = "";
            RecordingPath = System.Windows.Forms.Application.StartupPath + "ElegantRecording.json";
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
