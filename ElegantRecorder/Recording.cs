using System.Text.Json.Serialization;
using System.IO;
using System.Text.Json;
using System;
using System.Reflection;
using System.Windows;

namespace ElegantRecorder
{
    public class Recording
    {
        public const string DefaultTag = "ElegantRecording";

        public string Tag { get; set; }
        public string PlaybackSpeed { get; set; }
        public bool RestrictToExe { get; set; }
        public string ExePath { get; set; }
        public UIAction[] UIActions { get; set; }

        [JsonIgnore]
        public string Name { get; set; }
        [JsonIgnore]
        public string FilePath { get; set; }

        private ElegantRecorder App;

        public Recording()
        { }

        public Recording(ElegantRecorder app, string name)
        {
            Name = name;
            App = app;
            UIActions = new UIAction[0];
            Tag = DefaultTag;

            PlaybackSpeed = "Normal";
            RestrictToExe = false;
            ExePath = "";

            FilePath = Path.Combine(App.ElegantOptions.DataFolder, Name + ".json");
        }

        public void Load()
        {
            if (FilePath != null && FilePath.Length > 0)
            {
                var rec = JsonSerializer.Deserialize<Recording>(File.ReadAllText(FilePath));

                Type t = typeof(Recording);
                PropertyInfo[] properties = t.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    var val = property.GetValue(rec, null);

                    if (val != null)
                        property.SetValue(this, property.GetValue(rec, null), null);
                }

                App.ElegantOptions.PlaybackSpeed = PlaybackSpeed;
                App.ElegantOptions.RestrictToExe = RestrictToExe;
                App.ElegantOptions.ExePath = ExePath;
            }
        }

        public void Save()
        {
            PlaybackSpeed = App.ElegantOptions.PlaybackSpeed;
            RestrictToExe = App.ElegantOptions.RestrictToExe;
            ExePath = App.ElegantOptions.ExePath;

            try
            {
                using var stream = new StreamWriter(FilePath);

                stream.Write("{");

                stream.Write("\"Tag\":" + JsonSerializer.Serialize(DefaultTag) + ",");
                stream.Write("\"PlaybackSpeed\":" + JsonSerializer.Serialize(PlaybackSpeed) + ",");
                stream.Write("\"RestrictToExe\":" + JsonSerializer.Serialize(RestrictToExe) + ",");
                stream.Write("\"ExePath\":" + JsonSerializer.Serialize(ExePath) + ",");

                stream.Write("\n");

                stream.Write("\"UIActions\": [\n");

                JsonSerializerOptions jsonOptions = new()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                for (int i = 0; i < UIActions.Length; i++)
                {
                    string jsonString = JsonSerializer.Serialize(UIActions[i], jsonOptions);
                    stream.Write(jsonString);
                    stream.Write(i != UIActions.Length - 1 ? ",\n" : "\n");
                }

                stream.Write("]}");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
