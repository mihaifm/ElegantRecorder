using System;
using System.Linq;
using System.Windows.Forms;

namespace ElegantRecorder
{
    public partial class ElegantOptions : Form
    {
        private ElegantRecorder App;
        private Recording Rec;

        private int recordHotkeyData = 0;
        private int stopHotkeyData = 0;

        public ElegantOptions(ElegantRecorder elegantRecorder)
        {
            InitializeComponent();

            App = elegantRecorder;
            TopMost = App.TopMost;

            Rec = new Recording(App, App.CurrentRecordingName);
            Rec.Load();

            textBoxDataFolder.Text = App.Options.DataFolder;
            checkBoxPromptOverwrite.Checked = App.Options.PromptOverwrite;
            checkBoxRecMouseMove.Checked = App.Options.RecordMouseMove;
            checkBoxMouseMoveRelative.Checked = App.Options.MouseMoveRelative;
            textBoxMouseMoveDelay.Text = App.Options.MouseMoveDelay.ToString();
            checkBoxRecClipboard.Checked = App.Options.RecordClipboard;
            comboBoxAutomationEngine.SelectedItem = App.Options.AutomationEngine;

            textBoxRecordHotkey.Text = string.Join("+", ((Keys)App.Options.RecordHotkey).ToString().Split(", ").Reverse());
            recordHotkeyData = App.Options.RecordHotkey;
            textBoxStopHotkey.Text = string.Join("+", ((Keys)App.Options.StopHotkey).ToString().Split(", ").Reverse());
            stopHotkeyData = App.Options.StopHotkey;

            textBoxExePath.Enabled = checkBoxRestrictToExe.Checked;
            buttonBrowseExe.Enabled = checkBoxRestrictToExe.Checked;

            comboBoxSpeed.SelectedItem = App.Options.PlaybackSpeed;
            checkBoxEncrypted.Checked = App.Options.Encrypted;
            checkBoxRestrictToExe.Checked = App.Options.RestrictToExe;
            textBoxExePath.Text = App.Options.ExePath;

            textBoxCurrRecName.Text = App.Options.CurrRecName;

            EnableControls();
        }

        public void SaveOptions()
        {
            App.Options.DataFolder = textBoxDataFolder.Text;
            App.Options.PromptOverwrite = checkBoxPromptOverwrite.Checked;
            App.Options.RecordMouseMove = checkBoxRecMouseMove.Checked;
            App.Options.MouseMoveRelative = checkBoxMouseMoveRelative.Checked;

            try
            {
                App.Options.MouseMoveDelay = int.Parse(textBoxMouseMoveDelay.Text);
            }
            catch
            {
                App.Options.MouseMoveDelay = 30;
            }

            App.Options.RecordClipboard = checkBoxRecClipboard.Checked;
            App.Options.AutomationEngine = comboBoxAutomationEngine.SelectedItem as string;

            App.Options.RecordHotkey = recordHotkeyData;
            App.Options.StopHotkey = stopHotkeyData;

            App.Options.PlaybackSpeed = comboBoxSpeed.SelectedItem as string;
            App.Options.Encrypted = checkBoxEncrypted.Checked;
            App.Options.RestrictToExe = checkBoxRestrictToExe.Checked;
            App.Options.ExePath = textBoxExePath.Text;

            App.Options.CurrRecName = textBoxCurrRecName.Text;

            App.Options.Save(App.ConfigFilePath);
            Rec.Save(false);

            App.ReadRecordingHeaders();
        }

        private void EnableControls()
        {
            checkBoxMouseMoveRelative.Enabled = checkBoxRecMouseMove.Checked;
            textBoxMouseMoveDelay.Enabled = checkBoxRecMouseMove.Checked;
            labelMinEventDelay.Enabled = checkBoxRecMouseMove.Checked;
            labelMs.Enabled = checkBoxRecMouseMove.Checked;
        }

        private void ChangeAutomationEngine()
        {
            string newAutomationEngine = (string)comboBoxAutomationEngine.SelectedItem;

            if (newAutomationEngine == App.Options.AutomationEngine)
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
            ActiveControl = textBoxCurrRecName;
        }

        private void checkBoxRecMouseMove_CheckedChanged(object sender, EventArgs e)
        {
            EnableControls();
        }

        private void checkBoxEncrypted_CheckedChanged(object sender, EventArgs e)
        {
            if (Rec.UIActions.Length > 0 && Rec.EncryptedActions.Length > 0)
            {
                //should not happen, unless the file is edited by hand
                return;
            }

            if (checkBoxEncrypted.Checked && Rec.UIActions.Length > 0)
            {
                var encPwd = new EncryptionPassword(true, TopMost);
                encPwd.ShowDialog();

                if (encPwd.DialogResult == DialogResult.OK)
                {
                    Rec.Password = encPwd.Password;
                    Rec.Encrypt();
                }
                else
                {
                    checkBoxEncrypted.Checked = false;
                }
            }

            if (checkBoxEncrypted.Checked == false && Rec.EncryptedActions.Length > 0)
            {
                var encPwd = new EncryptionPassword(false, TopMost);
                encPwd.ShowDialog();

                if (encPwd.DialogResult == DialogResult.OK)
                {
                    Rec.Password = encPwd.Password;

                    try
                    {
                        Rec.Decrypt();
                    }
                    catch
                    {
                        checkBoxEncrypted.Checked = true;
                    }
                }
                else
                {
                    checkBoxEncrypted.Checked = true;
                }
            }
        }
    }
}
