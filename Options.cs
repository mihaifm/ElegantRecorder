﻿using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace ElegantRecorder
{
    public partial class Options : Form
    {
        private ElegantRecorder elegantRecorder;

        public Options(ElegantRecorder elegantRecorder)
        {
            InitializeComponent();

            this.elegantRecorder = elegantRecorder;
            var elegantOptions = elegantRecorder.ElegantOptions;

            checkBoxRecMouseMove.Checked = elegantOptions.RecordMouseMove;
            checkBoxRestrictToExe.Checked = elegantOptions.RestrictToExe;
            textBoxExePath.Text = elegantOptions.ExePath;
            textBoxRecordingPath.Text = elegantOptions.RecordingPath;
            comboBoxSpeed.SelectedItem = elegantOptions.PlaybackSpeed;

            textBoxExePath.Enabled = checkBoxRestrictToExe.Checked;
            buttonBrowseExe.Enabled = checkBoxRestrictToExe.Checked;
        }

        public void SaveOptions()
        {
            var elegantOptions = elegantRecorder.ElegantOptions;

            elegantOptions.RecordMouseMove = checkBoxRecMouseMove.Checked;
            elegantOptions.RestrictToExe = checkBoxRestrictToExe.Checked;
            elegantOptions.ExePath = textBoxExePath.Text;
            elegantOptions.RecordingPath = textBoxRecordingPath.Text;
            elegantOptions.PlaybackSpeed = comboBoxSpeed.SelectedItem as string;
            
            File.WriteAllText(elegantRecorder.ConfigFilePath, JsonSerializer.Serialize(elegantOptions));
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
            SaveOptions();
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
    }
}