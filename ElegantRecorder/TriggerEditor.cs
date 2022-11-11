using System;
using System.Linq;
using System.Windows.Forms;

namespace ElegantRecorder
{
    public partial class TriggerEditor : Form
    {
        private readonly ElegantRecorder App;
        private Recording Rec;

        private int hotkeyData = 0;

        public TriggerEditor(ElegantRecorder elegantRecorder)
        {
            InitializeComponent();

            App = elegantRecorder;
            Rec = new Recording(App, App.CurrentRecordingName);
            Rec.Load();

            checkBoxHotkey.Checked = Rec.Triggers.HotkeyEnabled;
            textBoxHotkey.Text = string.Join("+", ((Keys)Rec.Triggers.Hotkey).ToString().Split(", ").Reverse());
            hotkeyData = Rec.Triggers.Hotkey;
            checkBoxTime.Checked = Rec.Triggers.TimeEnabled;
            dateTimePickerDate.Value = Rec.Triggers.Date;
            dateTimePickerTime.Value = Rec.Triggers.Time;
            checkBoxWindow.Checked = Rec.Triggers.WindowEnabled;
            textBoxWindowName.Text = Rec.Triggers.WindowName;
            checkBoxFile.Checked = Rec.Triggers.FileEnabled;
            textBoxFileName.Text = Rec.Triggers.FilePath;

            EnableControls();
        }

        public void SaveTriggers()
        {
            Rec.Triggers.HotkeyEnabled = checkBoxHotkey.Checked;
            Rec.Triggers.Hotkey = hotkeyData;
            Rec.Triggers.TimeEnabled = checkBoxTime.Checked;
            Rec.Triggers.Date = dateTimePickerDate.Value;
            Rec.Triggers.Time = dateTimePickerTime.Value;
            Rec.Triggers.WindowEnabled = checkBoxWindow.Checked;
            Rec.Triggers.WindowName = textBoxWindowName.Text;
            Rec.Triggers.FileEnabled = checkBoxFile.Checked;
            Rec.Triggers.FilePath = textBoxFileName.Text;

            Rec.Save(false);
            Rec.ArmTriggers();
        }

        

        private void EnableControls()
        {
            textBoxHotkey.Enabled = checkBoxHotkey.Checked;
            buttonClearHotkey.Enabled = checkBoxHotkey.Checked;
            dateTimePickerDate.Enabled = checkBoxTime.Checked;
            dateTimePickerTime.Enabled = checkBoxTime.Checked;
            textBoxWindowName.Enabled = checkBoxWindow.Checked;
            textBoxFileName.Enabled = checkBoxFile.Checked;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            SaveTriggers();
            Close();
        }

        private void checkBoxHotkey_CheckedChanged(object sender, EventArgs e)
        {
            EnableControls();
        }

        private void checkBoxTime_CheckedChanged(object sender, EventArgs e)
        {
            EnableControls();
        }

        private void checkBoxWindow_CheckedChanged(object sender, EventArgs e)
        {
            EnableControls();
        }

        private void checkBoxFile_CheckedChanged(object sender, EventArgs e)
        {
            EnableControls();
        }

        private void textBoxHotkey_Enter(object sender, EventArgs e)
        {
            this.KeyDown += new KeyEventHandler(TriggerEditor_KeyDown);
        }

        private void textBoxHotkey_Leave(object sender, EventArgs e)
        {
            this.KeyDown -= new KeyEventHandler(TriggerEditor_KeyDown);
        }

        private void TriggerEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Menu)
                return;

            textBoxHotkey.Text = string.Join("+", e.KeyData.ToString().Split(", ").Reverse());
            hotkeyData = (int)e.KeyData;

            e.SuppressKeyPress = true;
        }

        private void buttonClearHotkey_Click(object sender, EventArgs e)
        {
            textBoxHotkey.Text = Keys.None.ToString();
            hotkeyData = (int)Keys.None;
        }
    }
}
