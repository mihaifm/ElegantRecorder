namespace ElegantRecorder
{
    partial class ElegantTriggers
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ElegantTriggers));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxHotkey = new System.Windows.Forms.CheckBox();
            this.buttonClearHotkey = new System.Windows.Forms.Button();
            this.textBoxHotkey = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxWindow = new System.Windows.Forms.CheckBox();
            this.textBoxWindowName = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxFile = new System.Windows.Forms.CheckBox();
            this.buttonBrowseFile = new System.Windows.Forms.Button();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.checkBoxTime = new System.Windows.Forms.CheckBox();
            this.dateTimePickerTime = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerDate = new System.Windows.Forms.DateTimePicker();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBoxRecordingName = new System.Windows.Forms.TextBox();
            this.checkBoxRecording = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.checkBoxHotkey);
            this.groupBox1.Controls.Add(this.buttonClearHotkey);
            this.groupBox1.Controls.Add(this.textBoxHotkey);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(482, 66);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // checkBoxHotkey
            // 
            this.checkBoxHotkey.AutoSize = true;
            this.checkBoxHotkey.Location = new System.Drawing.Point(10, 27);
            this.checkBoxHotkey.Name = "checkBoxHotkey";
            this.checkBoxHotkey.Size = new System.Drawing.Size(64, 19);
            this.checkBoxHotkey.TabIndex = 3;
            this.checkBoxHotkey.Text = "Hotkey";
            this.checkBoxHotkey.UseVisualStyleBackColor = true;
            this.checkBoxHotkey.CheckedChanged += new System.EventHandler(this.checkBoxHotkey_CheckedChanged);
            // 
            // buttonClearHotkey
            // 
            this.buttonClearHotkey.Location = new System.Drawing.Point(294, 25);
            this.buttonClearHotkey.Name = "buttonClearHotkey";
            this.buttonClearHotkey.Size = new System.Drawing.Size(42, 23);
            this.buttonClearHotkey.TabIndex = 2;
            this.buttonClearHotkey.Text = "Clear";
            this.buttonClearHotkey.UseVisualStyleBackColor = true;
            this.buttonClearHotkey.Click += new System.EventHandler(this.buttonClearHotkey_Click);
            // 
            // textBoxHotkey
            // 
            this.textBoxHotkey.Location = new System.Drawing.Point(119, 25);
            this.textBoxHotkey.Name = "textBoxHotkey";
            this.textBoxHotkey.Size = new System.Drawing.Size(169, 23);
            this.textBoxHotkey.TabIndex = 0;
            this.textBoxHotkey.Enter += new System.EventHandler(this.textBoxHotkey_Enter);
            this.textBoxHotkey.Leave += new System.EventHandler(this.textBoxHotkey_Leave);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.checkBoxWindow);
            this.groupBox2.Controls.Add(this.textBoxWindowName);
            this.groupBox2.Location = new System.Drawing.Point(12, 154);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(482, 64);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // checkBoxWindow
            // 
            this.checkBoxWindow.AutoSize = true;
            this.checkBoxWindow.Location = new System.Drawing.Point(10, 24);
            this.checkBoxWindow.Name = "checkBoxWindow";
            this.checkBoxWindow.Size = new System.Drawing.Size(103, 19);
            this.checkBoxWindow.TabIndex = 1;
            this.checkBoxWindow.Text = "Window name";
            this.checkBoxWindow.UseVisualStyleBackColor = true;
            this.checkBoxWindow.CheckedChanged += new System.EventHandler(this.checkBoxWindow_CheckedChanged);
            // 
            // textBoxWindowName
            // 
            this.textBoxWindowName.Location = new System.Drawing.Point(119, 22);
            this.textBoxWindowName.Name = "textBoxWindowName";
            this.textBoxWindowName.Size = new System.Drawing.Size(288, 23);
            this.textBoxWindowName.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.checkBoxFile);
            this.groupBox3.Controls.Add(this.buttonBrowseFile);
            this.groupBox3.Controls.Add(this.textBoxFileName);
            this.groupBox3.Location = new System.Drawing.Point(12, 224);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(482, 70);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            // 
            // checkBoxFile
            // 
            this.checkBoxFile.AutoSize = true;
            this.checkBoxFile.Location = new System.Drawing.Point(10, 28);
            this.checkBoxFile.Name = "checkBoxFile";
            this.checkBoxFile.Size = new System.Drawing.Size(77, 19);
            this.checkBoxFile.TabIndex = 3;
            this.checkBoxFile.Text = "File name";
            this.checkBoxFile.UseVisualStyleBackColor = true;
            this.checkBoxFile.CheckedChanged += new System.EventHandler(this.checkBoxFile_CheckedChanged);
            // 
            // buttonBrowseFile
            // 
            this.buttonBrowseFile.Location = new System.Drawing.Point(413, 25);
            this.buttonBrowseFile.Name = "buttonBrowseFile";
            this.buttonBrowseFile.Size = new System.Drawing.Size(59, 23);
            this.buttonBrowseFile.TabIndex = 2;
            this.buttonBrowseFile.Text = "...";
            this.buttonBrowseFile.UseVisualStyleBackColor = true;
            this.buttonBrowseFile.Click += new System.EventHandler(this.buttonBrowseFile_Click);
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Location = new System.Drawing.Point(119, 25);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.Size = new System.Drawing.Size(288, 23);
            this.textBoxFileName.TabIndex = 1;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(338, 374);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(419, 374);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.checkBoxTime);
            this.groupBox4.Controls.Add(this.dateTimePickerTime);
            this.groupBox4.Controls.Add(this.dateTimePickerDate);
            this.groupBox4.Location = new System.Drawing.Point(12, 84);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(482, 64);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            // 
            // checkBoxTime
            // 
            this.checkBoxTime.AutoSize = true;
            this.checkBoxTime.Location = new System.Drawing.Point(10, 24);
            this.checkBoxTime.Name = "checkBoxTime";
            this.checkBoxTime.Size = new System.Drawing.Size(52, 19);
            this.checkBoxTime.TabIndex = 3;
            this.checkBoxTime.Text = "Time";
            this.checkBoxTime.UseVisualStyleBackColor = true;
            this.checkBoxTime.CheckedChanged += new System.EventHandler(this.checkBoxTime_CheckedChanged);
            // 
            // dateTimePickerTime
            // 
            this.dateTimePickerTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePickerTime.Location = new System.Drawing.Point(272, 22);
            this.dateTimePickerTime.Name = "dateTimePickerTime";
            this.dateTimePickerTime.ShowUpDown = true;
            this.dateTimePickerTime.Size = new System.Drawing.Size(106, 23);
            this.dateTimePickerTime.TabIndex = 2;
            // 
            // dateTimePickerDate
            // 
            this.dateTimePickerDate.Location = new System.Drawing.Point(119, 22);
            this.dateTimePickerDate.Name = "dateTimePickerDate";
            this.dateTimePickerDate.Size = new System.Drawing.Size(147, 23);
            this.dateTimePickerDate.TabIndex = 0;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBoxRecordingName);
            this.groupBox5.Controls.Add(this.checkBoxRecording);
            this.groupBox5.Location = new System.Drawing.Point(12, 300);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(482, 68);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            // 
            // textBoxRecordingName
            // 
            this.textBoxRecordingName.Location = new System.Drawing.Point(119, 26);
            this.textBoxRecordingName.Name = "textBoxRecordingName";
            this.textBoxRecordingName.Size = new System.Drawing.Size(288, 23);
            this.textBoxRecordingName.TabIndex = 1;
            // 
            // checkBoxRecording
            // 
            this.checkBoxRecording.AutoSize = true;
            this.checkBoxRecording.Location = new System.Drawing.Point(10, 28);
            this.checkBoxRecording.Name = "checkBoxRecording";
            this.checkBoxRecording.Size = new System.Drawing.Size(80, 19);
            this.checkBoxRecording.TabIndex = 0;
            this.checkBoxRecording.Text = "Recording";
            this.checkBoxRecording.UseVisualStyleBackColor = true;
            this.checkBoxRecording.CheckedChanged += new System.EventHandler(this.checkBoxRecording_CheckedChanged);
            // 
            // TriggerEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(506, 409);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "TriggerEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Elegant Recorder - Triggers";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBoxHotkey;
        private System.Windows.Forms.TextBox textBoxWindowName;
        private System.Windows.Forms.Button buttonBrowseFile;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.Button buttonClearHotkey;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.DateTimePicker dateTimePickerDate;
        private System.Windows.Forms.DateTimePicker dateTimePickerTime;
        private System.Windows.Forms.CheckBox checkBoxHotkey;
        private System.Windows.Forms.CheckBox checkBoxTime;
        private System.Windows.Forms.CheckBox checkBoxWindow;
        private System.Windows.Forms.CheckBox checkBoxFile;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox checkBoxRecording;
        private System.Windows.Forms.TextBox textBoxRecordingName;
    }
}