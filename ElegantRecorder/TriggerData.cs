using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System;
using System.Windows;

namespace ElegantRecorder
{
    public class TriggerData
    {
        private readonly ElegantRecorder App;

        public int CurrentHotkeyId = 8;
        public Dictionary<int, string> RecHotkeys = new();

        public Dictionary<string, Timer> RecFileTimers = new();

        public Dictionary<string, Timer> RecTimeTimers = new();

        public Dictionary<string, Timer> RecWindowTimers = new();
        public Dictionary<string, bool> RecWindowStatus = new();

        public Dictionary<string, string> RecRec = new();

        public TriggerData(ElegantRecorder App)
        {
            this.App = App;
        }

        public void CreateFileTimer(Recording rec)
        {
            Timer timer = new Timer();
            timer.Interval = 500;
            timer.Tick += FileTimer_Tick;
            timer.Tag = rec.Name;
            RecFileTimers.Add(rec.Name, timer);
            timer.Start();
        }

        private void FileTimer_Tick(object? sender, EventArgs e)
        {
            var rec = App.RecHeaders[(sender as Timer).Tag as string];

            var lastModified = File.GetLastWriteTime(rec.Triggers.FilePath);
            var now = DateTime.Now;

            if (now >= lastModified && (now - lastModified).TotalMilliseconds <= 500)
            {
                App.SelectRecording(rec.Name);
                App.StateSwitch(State.Replay);
            }
        }

        public void CreateWindowTimer(Recording rec)
        {
            Timer timer = new Timer();
            timer.Interval = 500;
            timer.Tick += WindowTimer_Tick;
            timer.Tag = rec.Name;
            RecWindowTimers.Add(rec.Name, timer);
            RecWindowStatus.Add(rec.Name, false);
            timer.Start();
        }

        private void WindowTimer_Tick(object? sender, EventArgs e)
        {
            var rec = App.RecHeaders[(sender as Timer).Tag as string];

            if (WinAPI.FindWindow(null, rec.Triggers.WindowName) != IntPtr.Zero)
            {
                if (RecWindowStatus[rec.Name] == false)
                {
                    App.SelectRecording(rec.Name);
                    App.StateSwitch(State.Replay);
                    RecWindowStatus[rec.Name] = true;
                }
            }
            else
            {
                RecWindowStatus[rec.Name] = false;
            }
        }

        public void CreateTimeTimers(Recording rec)
        {
            DateTime target = rec.Triggers.Date.Date + rec.Triggers.Time.TimeOfDay;
            var interval = target - DateTime.Now;

            if (interval > TimeSpan.Zero)
            {
                Timer timer = new Timer();
                timer.Interval = (int)interval.TotalMilliseconds;
                timer.Tick += TimeTimer_Tick;
                timer.Tag = rec.Name;
                RecTimeTimers.Add(rec.Name, timer);
                timer.Start();
            }
        }

        private void TimeTimer_Tick(object? sender, EventArgs e)
        {
            var rec = App.RecHeaders[(sender as Timer).Tag as string];
            App.SelectRecording(rec.Name);
            App.StateSwitch(State.Replay);
            (sender as Timer).Stop();
            RecTimeTimers.Remove(rec.Name);
        }

        public void TriggerNewRecording(string RecFinishedName)
        {
            foreach(var recrec in RecRec)
            {
                if (recrec.Value.Trim() == RecFinishedName)
                {
                    App.SelectRecording(recrec.Key);
                    App.StateSwitch(State.Replay);
                }
            }
        }
    }
}
