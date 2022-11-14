using System;
using System.Linq;
using System.Windows.Forms;

namespace ElegantRecorder
{
    public partial class ElegantTriggers : Form
    {
        private readonly ElegantRecorder App;
        private Recording Rec;

        private int hotkeyData = 0;

        public ElegantTriggers(ElegantRecorder elegantRecorder)
        {
            InitializeComponent();

            App = elegantRecorder;
            TopMost = App.TopMost;

            Rec = new Recording(App, App.CurrentRecordingName);
            Rec.Load();

            checkBoxHotkey.Checked = Rec.Triggers.HotkeyEnabled;
            textBoxHotkey.Text = Util.HotkeyToString(Rec.Triggers.Hotkey);
            hotkeyData = Rec.Triggers.Hotkey;
            checkBoxTime.Checked = Rec.Triggers.TimeEnabled;
            dateTimePickerDate.Value = Rec.Triggers.Date;
            dateTimePickerTime.Value = Rec.Triggers.Time;
            checkBoxWindow.Checked = Rec.Triggers.WindowEnabled;
            textBoxWindowName.Text = Rec.Triggers.WindowName;
            checkBoxFile.Checked = Rec.Triggers.FileEnabled;
            textBoxFileName.Text = Rec.Triggers.FilePath;
            checkBoxRecording.Checked = Rec.Triggers.RecordingEnabled;
            textBoxRecordingName.Text = Rec.Triggers.RecordingName;

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
            Rec.Triggers.RecordingEnabled = checkBoxRecording.Checked;
            Rec.Triggers.RecordingName = textBoxRecordingName.Text;

            Rec.Save(false);
            Rec.DisarmTriggers();
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
            buttonBrowseFile.Enabled = checkBoxFile.Checked;
            textBoxRecordingName.Enabled = checkBoxRecording.Checked;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            SaveTriggers();
            App.RefreshCurrentRow(Rec);
            Close();
        }

        private void checkBoxHotkey_CheckedChanged(object sender, EventArgs e)
        {
            EnableControls();
        }

        private void checkBoxTime_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxTime.Checked)
            {
                dateTimePickerDate.Value = DateTime.Now;
                dateTimePickerTime.Value = DateTime.Now;
            }

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

        private void checkBoxRecording_CheckedChanged(object sender, EventArgs e)
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

            textBoxHotkey.Text = Util.HotkeyToString((int) e.KeyData);
            hotkeyData = (int)e.KeyData;

            e.SuppressKeyPress = true;
        }

        private void buttonClearHotkey_Click(object sender, EventArgs e)
        {
            textBoxHotkey.Text = Keys.None.ToString();
            hotkeyData = (int)Keys.None;
        }

        private void buttonBrowseFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "All files (*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBoxFileName.Text = dialog.FileName;
            }
        }
    }
}
