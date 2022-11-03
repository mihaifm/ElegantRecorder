using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace ElegantRecorder
{
    public partial class Options : Form
    {
        private ElegantRecorder elegantRecorder;

        private int recordHotkeyData = 0;
        private int stopHotkeyData = 0;

        public Options(ElegantRecorder elegantRecorder)
        {
            InitializeComponent();

            this.elegantRecorder = elegantRecorder;
            var elegantOptions = elegantRecorder.ElegantOptions;

            comboBoxSpeed.SelectedItem = elegantOptions.PlaybackSpeed;
            checkBoxRecMouseMove.Checked = elegantOptions.RecordMouseMove;
            checkBoxRecClipboard.Checked = elegantOptions.RecordClipboard;
            checkBoxRestrictToExe.Checked = elegantOptions.RestrictToExe;
            textBoxExePath.Text = elegantOptions.ExePath;
            textBoxRecordingPath.Text = elegantOptions.RecordingPath;
            comboBoxAutomationEngine.SelectedItem = elegantOptions.AutomationEngine;

            textBoxRecordHotkey.Text = string.Join("+", ((Keys) elegantOptions.RecordHotkey).ToString().Split(", ").Reverse());
            recordHotkeyData = elegantOptions.RecordHotkey;
            textBoxStopHotkey.Text = string.Join("+", ((Keys)elegantOptions.StopHotkey).ToString().Split(", ").Reverse());
            stopHotkeyData = elegantOptions.StopHotkey;

            textBoxExePath.Enabled = checkBoxRestrictToExe.Checked;
            buttonBrowseExe.Enabled = checkBoxRestrictToExe.Checked;
        }

        public void SaveOptions()
        {
            var elegantOptions = elegantRecorder.ElegantOptions;

            elegantOptions.PlaybackSpeed = comboBoxSpeed.SelectedItem as string;
            elegantOptions.RecordMouseMove = checkBoxRecMouseMove.Checked;
            elegantOptions.RecordClipboard = checkBoxRecClipboard.Checked;
            elegantOptions.RestrictToExe = checkBoxRestrictToExe.Checked;
            elegantOptions.ExePath = textBoxExePath.Text;
            elegantOptions.RecordingPath = textBoxRecordingPath.Text;
            elegantOptions.AutomationEngine = comboBoxAutomationEngine.SelectedItem as string;

            elegantOptions.RecordHotkey = recordHotkeyData;
            elegantOptions.StopHotkey = stopHotkeyData;

            File.WriteAllText(elegantRecorder.ConfigFilePath, JsonSerializer.Serialize(elegantOptions));
        }

        private void ChangeAutomationEngine()
        {
            string newAutomationEngine = (string)comboBoxAutomationEngine.SelectedItem;

            if (newAutomationEngine == elegantRecorder.ElegantOptions.AutomationEngine)
                return;

            if (newAutomationEngine == "Win32")
            {
                elegantRecorder.AutomationEngine = new Win32Engine(elegantRecorder);
            }
            else if (newAutomationEngine == "UI Automation")
            {
                elegantRecorder.AutomationEngine = new UIAEngine(elegantRecorder);
            }
        }

        private void ResetHotkeys()
        {
            elegantRecorder.WinAPI.UnregisterGlobalHotkeys();
            elegantRecorder.WinAPI.RegisterGlobalHotkeys();
        }

        private void buttonBrowseScript_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBoxRecordingPath.Text = dialog.FileName;
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
    }
}
