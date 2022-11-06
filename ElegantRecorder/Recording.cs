using System.Text.Json.Serialization;

namespace ElegantRecorder
{
    public class Recording
    {
        public const string DefaultTag = "ElegantRecording";

        public string Tag { get; set; }
        public UIAction[] UIActions { get; set; }

        [JsonIgnore]
        public string Name { get; set; }
        [JsonIgnore]
        public string Path { get; set; }

        public Recording()
        {
            UIActions = new UIAction[0];
            Tag = DefaultTag;
        }
    }
}
