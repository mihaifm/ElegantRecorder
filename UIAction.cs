﻿using System.Text.Json.Serialization;

namespace ElegantRecorder
{
    public class UIAction
    {
        [JsonPropertyName("event")]
        public string EventType { get; set; }
        [JsonPropertyName("ctrlname")]
        public string ControlName { get; set; }
        [JsonPropertyName("ctrltype")]
        public int? ControlType { get; set; }
        [JsonPropertyName("autoid")]
        public string AutomationId { get; set; }
        [JsonPropertyName("win")]
        public string TopLevelWindow { get; set; }
        [JsonPropertyName("x")]
        public int? OffsetX { get; set; }
        [JsonPropertyName("y")]
        public int? OffsetY { get; set; }
        [JsonPropertyName("lvl")]
        public int? Level { get; set; }
        [JsonPropertyName("idx")]
        public int? ChildIndex { get; set; }
        [JsonPropertyName("vkcode")]
        public int? VKeyCode { get; set; }
        [JsonPropertyName("scancode")]
        public int? ScanCode { get; set; }
        [JsonPropertyName("flags")]
        public int? Flags { get; set; }
        [JsonPropertyName("mdata")]
        public int? MouseData { get; set; }
        [JsonPropertyName("einfo")]
        public int? ExtraInfo { get; set; }
        [JsonPropertyName("t")]
        public double elapsed { get; set; }
    }
}
