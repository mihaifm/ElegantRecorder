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
    }
}
