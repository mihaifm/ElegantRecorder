using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace ElegantRecorder
{
    public partial class Options : Form
    {
        private ElegantRecorder App;
        private Recording Rec;

        private int recordHotkeyData = 0;
        private int stopHotkeyData = 0;

        public Options(ElegantRecorder elegantRecorder)
        {
            InitializeComponent();

            App = elegantRecorder;
            Rec = new Recording(App, App.CurrentRecordingName);
            Rec.Load();

            checkBoxRecMouseMove.Checked = App.ElegantOptions.RecordMouseMove;
            checkBoxRecClipboard.Checked = App.ElegantOptions.RecordClipboard;
            textBoxDataFolder.Text = App.ElegantOptions.DataFolder;
            comboBoxAutomationEngine.SelectedItem = App.ElegantOptions.AutomationEngine;

            textBoxRecordHotkey.Text = string.Join("+", ((Keys)App.ElegantOptions.RecordHotkey).ToString().Split(", ").Reverse());
            recordHotkeyData = App.ElegantOptions.RecordHotkey;
            textBoxStopHotkey.Text = string.Join("+", ((Keys)App.ElegantOptions.StopHotkey).ToString().Split(", ").Reverse());
            stopHotkeyData = App.ElegantOptions.StopHotkey;

            textBoxExePath.Enabled = checkBoxRestrictToExe.Checked;
            buttonBrowseExe.Enabled = checkBoxRestrictToExe.Checked;

            comboBoxSpeed.SelectedItem = App.ElegantOptions.PlaybackSpeed;
            checkBoxRestrictToExe.Checked = App.ElegantOptions.RestrictToExe;
            textBoxExePath.Text = App.ElegantOptions.ExePath;

            textBoxCurrRecName.Text = App.ElegantOptions.CurrRecName;
        }

        public void SaveOptions()
        {
            App.ElegantOptions.RecordMouseMove = checkBoxRecMouseMove.Checked;
            App.ElegantOptions.RecordClipboard = checkBoxRecClipboard.Checked;
            App.ElegantOptions.DataFolder = textBoxDataFolder.Text;
            App.ElegantOptions.AutomationEngine = comboBoxAutomationEngine.SelectedItem as string;

            App.ElegantOptions.RecordHotkey = recordHotkeyData;
            App.ElegantOptions.StopHotkey = stopHotkeyData;

            App.ElegantOptions.PlaybackSpeed = comboBoxSpeed.SelectedItem as string;
            App.ElegantOptions.RestrictToExe = checkBoxRestrictToExe.Checked;
            App.ElegantOptions.ExePath = textBoxExePath.Text;

            App.ElegantOptions.CurrRecName = textBoxCurrRecName.Text;

            App.ElegantOptions.Save(App.ConfigFilePath);
            Rec.Save();

            App.ReadCurrentRecordings();
        }

        private void ChangeAutomationEngine()
        {
            string newAutomationEngine = (string)comboBoxAutomationEngine.SelectedItem;

            if (newAutomationEngine == App.ElegantOptions.AutomationEngine)
                return;

            if (newAutomationEngine == "Win32")
            {
                App.AutomationEngine = new Win32Engine(App);
            }
            else if (newAutomationEngine == "UI Automation")
            {
                App.AutomationEngine = new UIAEngine(App);
            }
        }

        private void ResetHotkeys()
        {
            App.WinAPI.UnregisterGlobalHotkeys();
            App.WinAPI.RegisterGlobalHotkeys();
        }

        private void buttonBrowseScript_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBoxDataFolder.Text = dialog.SelectedPath;
            }
        }

        private void buttonBrowseExe_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Exe files (*.exe)|*.exe|All files (*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBoxExePath.Text = dialog.FileName;
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            ChangeAutomationEngine();
            SaveOptions();
            ResetHotkeys();
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void checkBoxRestrictToExe_CheckedChanged(object sender, EventArgs e)
        {
            textBoxExePath.Enabled = checkBoxRestrictToExe.Checked;
            buttonBrowseExe.Enabled = checkBoxRestrictToExe.Checked;
        }

        private void textBoxRecordHotkey_Enter(object sender, EventArgs e)
        {
            this.KeyDown += new KeyEventHandler(Options_RecordKeyDown);
        }

        private void textBoxRecordHotkey_Leave(object sender, EventArgs e)
        {
            this.KeyDown -= new KeyEventHandler(Options_RecordKeyDown);
        }

        private void Options_RecordKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Menu)
                return;

            textBoxRecordHotkey.Text = string.Join("+", e.KeyData.ToString().Split(", ").Reverse());
            recordHotkeyData = (int) e.KeyData;

            e.SuppressKeyPress = true;
        }

        private void textBoxStopHotkey_Enter(object sender, EventArgs e)
        {
            this.KeyDown += new KeyEventHandler(Options_StopKeyDown);
        }

        private void textBoxStopHotkey_Leave(object sender, EventArgs e)
        {
            this.KeyDown -= new KeyEventHandler(Options_StopKeyDown);
        }

        private void Options_StopKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Menu)
                return;

            textBoxStopHotkey.Text = string.Join("+", e.KeyData.ToString().Split(", ").Reverse());
            stopHotkeyData = (int)e.KeyData;

            e.SuppressKeyPress = true;
        }

        private void buttonClearRecHotkey_Click(object sender, EventArgs e)
        {
            textBoxRecordHotkey.Text = Keys.None.ToString();
            recordHotkeyData = (int)Keys.None;
        }

        private void buttonClearStopHotkey_Click(object sender, EventArgs e)
        {
            textBoxStopHotkey.Text = Keys.None.ToString();
            stopHotkeyData = (int)Keys.None;
        }
        public void RenameFocus()
        {
            textBoxCurrRecName.Focus();
        }
    }
}
