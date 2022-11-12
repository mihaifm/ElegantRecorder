using System.Text.Json.Serialization;
using System.IO;
using System.Text.Json;
using System;
using System.Reflection;
using System.Linq;

namespace ElegantRecorder
{
    public class Recording
    {
        public const string DefaultTag = "ElegantRecording";

        public string Tag { get; set; }
        public string PlaybackSpeed { get; set; }
        public bool Encrypted { get; set; }
        public bool RestrictToExe { get; set; }
        public string ExePath { get; set; }
        public Triggers Triggers { get; set; }
        public UIAction[] UIActions { get; set; }
        public string EncryptedActions { get; set; }

        [JsonIgnore]
        public string Name { get; set; }
        [JsonIgnore]
        public string FilePath { get; set; }
        [JsonIgnore]
        public string Password { get; set; }

        private ElegantRecorder App;

        public Recording()
        { }

        public Recording(ElegantRecorder app, string name)
        {
            Name = name;
            App = app;
            UIActions = new UIAction[0];
            Encrypted = false;
            EncryptedActions = "";
            Tag = DefaultTag;

            PlaybackSpeed = "Normal";
            Encrypted = false;
            RestrictToExe = false;
            ExePath = "";

            if (Name.Length == 0)
                throw new Exception();

            FilePath = Path.Combine(App.ElegantOptions.DataFolder, Name + ".json");

            Triggers = new Triggers();
        }

        private void SyncAppOptions(bool direction)
        {
            if (direction)
            {
                App.ElegantOptions.PlaybackSpeed = PlaybackSpeed;
                App.ElegantOptions.Encrypted = Encrypted;
                App.ElegantOptions.RestrictToExe = RestrictToExe;
                App.ElegantOptions.ExePath = ExePath;
                App.ElegantOptions.CurrRecName = Name;
            }
            else
            {
                PlaybackSpeed = App.ElegantOptions.PlaybackSpeed;
                Encrypted = App.ElegantOptions.Encrypted;
                RestrictToExe = App.ElegantOptions.RestrictToExe;
                ExePath = App.ElegantOptions.ExePath;
                Name = App.ElegantOptions.CurrRecName;

                if (Name.Length > 0)
                    FilePath = Path.Combine(App.ElegantOptions.DataFolder, Name + ".json");
                else
                    FilePath = "";
            }
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

                SyncAppOptions(true);
            }
        }

        public void Deserialize(string input)
        {
            var rec = JsonSerializer.Deserialize<Recording>(input);

            Type t = typeof(Recording);
            PropertyInfo[] properties = t.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                var val = property.GetValue(rec, null);

                if (val != null)
                    property.SetValue(this, property.GetValue(rec, null), null);
            }
        }

        public void Decrypt()
        {
            try
            {
                var plainData = StringCipher.Decrypt(EncryptedActions, Password);

                JsonSerializerOptions jsonOptions = new()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                UIActions = JsonSerializer.Deserialize<UIAction[]>(plainData, jsonOptions);
            }
            catch
            {
                throw;
            }
        }

        public void Save(bool encrypt)
        {
            try
            {
                Rename();
            }
            catch(Exception)
            {
                throw;
            }

            SyncAppOptions(false);

            //if (FilePath.Length == 0)
            //    return;

            try
            {
                JsonSerializerOptions jsonOptions = new()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                using var stream = new StreamWriter(FilePath);

                if (encrypt && Encrypted)
                {
                    var plainData = JsonSerializer.Serialize(UIActions, jsonOptions);
                    EncryptedActions = StringCipher.Encrypt(plainData, Password);
                    UIActions = new UIAction[0];

                    //stream.Write("\"EncryptedActions\":" + "\"" + encData + "\"");
                    stream.Write(JsonSerializer.Serialize(this));
                }
                else
                {
                    stream.Write("{");

                    stream.Write("\"Tag\":" + JsonSerializer.Serialize(DefaultTag) + ",");
                    stream.Write("\"PlaybackSpeed\":" + JsonSerializer.Serialize(PlaybackSpeed) + ",");
                    stream.Write("\"Encrypted\":" + JsonSerializer.Serialize(Encrypted) + ",");
                    stream.Write("\"RestrictToExe\":" + JsonSerializer.Serialize(RestrictToExe) + ",");
                    stream.Write("\"ExePath\":" + JsonSerializer.Serialize(ExePath) + ",\n");
                    stream.Write("\"Triggers\":" + JsonSerializer.Serialize(Triggers) + ",");

                    stream.Write("\n");

                    stream.Write("\"UIActions\": [\n");

                    for (int i = 0; i < UIActions.Length; i++)
                    {
                        string jsonString = JsonSerializer.Serialize(UIActions[i], jsonOptions);
                        stream.Write(jsonString);
                        stream.Write(i != UIActions.Length - 1 ? ",\n" : "\n");
                    }

                    stream.Write("],\n");

                    stream.Write("\"EncryptedActions\":" + JsonSerializer.Serialize(EncryptedActions));

                    stream.Write("}");

                }
            }
            catch (Exception)
            {
                throw;
            }

            if (App.RecHeaders.ContainsKey(Name))
            {
                DisarmTriggers();

                App.RecHeaders[Name] = this;
            }
        }

        public void Rename()
        {
            try
            {
                if (Name != App.ElegantOptions.CurrRecName &&
                    Name != null && Name.Length > 0 &&
                    App.ElegantOptions.CurrRecName != null && App.ElegantOptions.CurrRecName.Length > 0)
                {
                    var path1 = Path.Combine(App.ElegantOptions.DataFolder, Name + ".json");
                    var path2 = Path.Combine(App.ElegantOptions.DataFolder, App.ElegantOptions.CurrRecName + ".json");

                    File.Move(path1, path2);
                }
            }
            catch
            {
                throw;
            }
        }

        public void ArmTriggers()
        {
            if (Triggers.HotkeyEnabled)
            {
                if (Triggers.Hotkey != 0)
                {
                    App.TriggerData.CurrentHotkeyId++;
                    App.WinAPI.RegisterTriggerHotkey(Triggers.Hotkey);
                    App.TriggerData.RecHotkeys.Add(App.TriggerData.CurrentHotkeyId, Name);
                }
            }

            if (Triggers.FileEnabled)
            {
                if (App.TriggerData.RecFileTimers.ContainsKey(Name) == false)
                {
                    if (Triggers.FilePath.Length > 0)
                        App.TriggerData.CreateFileTimer(this);
                }
            }

            if (Triggers.WindowEnabled)
            {
                if (App.TriggerData.RecWindowTimers.ContainsKey(Name) == false)
                {
                    if (Triggers.WindowName.Length > 0)
                        App.TriggerData.CreateWindowTimer(this);
                }
            }

            if (Triggers.TimeEnabled)
            {
                if (App.TriggerData.RecTimeTimers.ContainsKey(Name) == false)
                {
                    App.TriggerData.CreateTimeTimers(this);
                }
            }

            if (Triggers.RecordingEnabled)
            {
                if (App.TriggerData.RecRec.ContainsKey(Name) == false)
                {
                    if (Triggers.RecordingName.Length > 0)
                    {
                        App.TriggerData.RecRec.Add(Name, Triggers.RecordingName);
                    }
                }
            }
        }

        public void DisarmTriggers()
        {
            if (App.TriggerData.RecHotkeys.ContainsValue(Name))
            {
                var hotkeyId = App.TriggerData.RecHotkeys.FirstOrDefault(x => x.Value == Name).Key;
                App.WinAPI.UnregisterTriggerHotkey(hotkeyId);
                App.TriggerData.RecHotkeys.Remove(hotkeyId);
            }

            if (App.TriggerData.RecFileTimers.ContainsKey(Name))
            {
                App.TriggerData.RecFileTimers[Name].Stop();
                App.TriggerData.RecFileTimers.Remove(Name);
            }

            if (App.TriggerData.RecWindowTimers.ContainsKey(Name))
            {
                App.TriggerData.RecWindowTimers[Name].Stop();
                App.TriggerData.RecWindowTimers.Remove(Name);
                App.TriggerData.RecWindowStatus.Remove(Name);
            }

            if (App.TriggerData.RecTimeTimers.ContainsKey(Name))
            {
                App.TriggerData.RecTimeTimers[Name].Stop();
                App.TriggerData.RecTimeTimers.Remove(Name);
            }

            if (App.TriggerData.RecRec.ContainsKey(Name))
            {
                App.TriggerData.RecRec.Remove(Name);
            }
        }

        public static string ReadUntil(FileStream Stream, string UntilText)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            System.Text.StringBuilder returnTextBuilder = new System.Text.StringBuilder();
            string returnText = string.Empty;
            int size = Convert.ToInt32(UntilText.Length / (double)2) - 1;
            byte[] buffer = new byte[size + 1];
            int currentRead = -1;

            while (currentRead != 0)
            {
                string collected = null;
                string chars = null;
                int foundIndex = -1;

                currentRead = Stream.Read(buffer, 0, buffer.Length);
                chars = System.Text.Encoding.Default.GetString(buffer, 0, currentRead);

                builder.Append(chars);
                returnTextBuilder.Append(chars);

                collected = builder.ToString();
                foundIndex = collected.IndexOf(UntilText);

                if (foundIndex >= 0)
                {
                    returnText = returnTextBuilder.ToString();

                    int indexOfSep = returnText.IndexOf(UntilText);
                    int cutLength = returnText.Length - indexOfSep;

                    returnText = returnText.Remove(indexOfSep, cutLength);

                    builder.Remove(0, foundIndex + UntilText.Length);

                    if (cutLength > UntilText.Length)
                        Stream.Position -= cutLength - UntilText.Length;

                    return returnText;
                }
                else if (!collected.Contains(UntilText.First()))
                    builder.Length = 0;
            }

            return string.Empty;
        }
    }
}
