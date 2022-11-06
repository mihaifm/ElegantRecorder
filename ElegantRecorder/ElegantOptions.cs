﻿using System.IO;
using System.Text.Json;

namespace ElegantRecorder
{
    public class ElegantOptions
    {
        public static int DefaultFormHeight = 127;

        public string PlaybackSpeed { get; set; }
        public int RecordHotkey { get; set; }
        public int StopHotkey { get; set; }
        public bool RecordMouseMove { get; set; }
        public bool RecordClipboard { get; set; }
        public bool RestrictToExe { get; set; }
        public string ExePath { get; set; }
        public string AutomationEngine { get; set; }
        public string DataFolder { get; set; }
        public bool ExpandedUI { get; set; }
        public int FormHeight { get; set; }
        public int DataGridHeight { get; set; }

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
            ExpandedUI = true;
            FormHeight = 350;
            DataGridHeight = 155;
        }

        public void Save(string FilePath)
        {
            File.WriteAllText(FilePath, JsonSerializer.Serialize(this));
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
