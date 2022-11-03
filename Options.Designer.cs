
namespace ElegantRecorder
{
    partial class Options
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options));
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxRecordingPath = new System.Windows.Forms.TextBox();
            this.buttonBrowseScript = new System.Windows.Forms.Button();
            this.checkBoxRestrictToExe = new System.Windows.Forms.CheckBox();
            this.textBoxExePath = new System.Windows.Forms.TextBox();
            this.buttonBrowseExe = new System.Windows.Forms.Button();
            this.groupBoxGeneral = new System.Windows.Forms.GroupBox();
            this.textBoxStopHotkey = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxRecordHotkey = new System.Windows.Forms.TextBox();
            this.checkBoxRecClipboard = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxSpeed = new System.Windows.Forms.ComboBox();
            this.checkBoxRecMouseMove = new System.Windows.Forms.CheckBox();
            this.groupBoxAdvanced = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxAutomationEngine = new System.Windows.Forms.ComboBox();
            this.groupBoxGeneral.SuspendLayout();
            this.groupBoxAdvanced.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 188);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Recording file";
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(426, 302);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(507, 302);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // textBoxRecordingPath
            // 
            this.textBoxRecordingPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRecordingPath.Location = new System.Drawing.Point(112, 184);
            this.textBoxRecordingPath.Name = "textBoxRecordingPath";
            this.textBoxRecordingPath.Size = new System.Drawing.Size(388, 23);
            this.textBoxRecordingPath.TabIndex = 4;
            // 
            // buttonBrowseScript
            // 
            this.buttonBrowseScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseScript.Location = new System.Drawing.Point(506, 184);
            this.buttonBrowseScript.Name = "buttonBrowseScript";
            this.buttonBrowseScript.Size = new System.Drawing.Size(59, 23);
            this.buttonBrowseScript.TabIndex = 5;
            this.buttonBrowseScript.Text = "...";
            this.buttonBrowseScript.UseVisualStyleBackColor = true;
            this.buttonBrowseScript.Click += new System.EventHandler(this.buttonBrowseScript_Click);
            // 
            // checkBoxRestrictToExe
            // 
            this.checkBoxRestrictToExe.AutoSize = true;
            this.checkBoxRestrictToExe.Location = new System.Drawing.Point(6, 153);
            this.checkBoxRestrictToExe.Name = "checkBoxRestrictToExe";
            this.checkBoxRestrictToExe.Size = new System.Drawing.Size(100, 19);
            this.checkBoxRestrictToExe.TabIndex = 6;
            this.checkBoxRestrictToExe.Text = "Restrict to exe";
            this.checkBoxRestrictToExe.UseVisualStyleBackColor = true;
            this.checkBoxRestrictToExe.CheckedChanged += new System.EventHandler(this.checkBoxRestrictToExe_CheckedChanged);
            // 
            // textBoxExePath
            // 
            this.textBoxExePath.Location = new System.Drawing.Point(112, 151);
            this.textBoxExePath.Name = "textBoxExePath";
            this.textBoxExePath.Size = new System.Drawing.Size(388, 23);
            this.textBoxExePath.TabIndex = 7;
            // 
            // buttonBrowseExe
            // 
            this.buttonBrowseExe.Location = new System.Drawing.Point(505, 150);
            this.buttonBrowseExe.Name = "buttonBrowseExe";
            this.buttonBrowseExe.Size = new System.Drawing.Size(59, 23);
            this.buttonBrowseExe.TabIndex = 8;
            this.buttonBrowseExe.Text = "...";
            this.buttonBrowseExe.UseVisualStyleBackColor = true;
            this.buttonBrowseExe.Click += new System.EventHandler(this.buttonBrowseExe_Click);
            // 
            // groupBoxGeneral
            // 
            this.groupBoxGeneral.Controls.Add(this.textBoxStopHotkey);
            this.groupBoxGeneral.Controls.Add(this.label6);
            this.groupBoxGeneral.Controls.Add(this.label5);
            this.groupBoxGeneral.Controls.Add(this.textBoxRecordHotkey);
            this.groupBoxGeneral.Controls.Add(this.checkBoxRecClipboard);
            this.groupBoxGeneral.Controls.Add(this.label2);
            this.groupBoxGeneral.Controls.Add(this.comboBoxSpeed);
            this.groupBoxGeneral.Controls.Add(this.checkBoxRecMouseMove);
            this.groupBoxGeneral.Controls.Add(this.label1);
            this.groupBoxGeneral.Controls.Add(this.buttonBrowseExe);
            this.groupBoxGeneral.Controls.Add(this.textBoxRecordingPath);
            this.groupBoxGeneral.Controls.Add(this.textBoxExePath);
            this.groupBoxGeneral.Controls.Add(this.buttonBrowseScript);
            this.groupBoxGeneral.Controls.Add(this.checkBoxRestrictToExe);
            this.groupBoxGeneral.Location = new System.Drawing.Point(12, 12);
            this.groupBoxGeneral.Name = "groupBoxGeneral";
            this.groupBoxGeneral.Size = new System.Drawing.Size(571, 213);
            this.groupBoxGeneral.TabIndex = 9;
            this.groupBoxGeneral.TabStop = false;
            this.groupBoxGeneral.Text = "General";
            // 
            // textBoxStopHotkey
            // 
            this.textBoxStopHotkey.Location = new System.Drawing.Point(414, 71);
            this.textBoxStopHotkey.Name = "textBoxStopHotkey";
            this.textBoxStopHotkey.Size = new System.Drawing.Size(117, 23);
            this.textBoxStopHotkey.TabIndex = 16;
            this.textBoxStopHotkey.Enter += new System.EventHandler(this.textBoxStopHotkey_Enter);
            this.textBoxStopHotkey.Leave += new System.EventHandler(this.textBoxStopHotkey_Leave);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(274, 74);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(124, 15);
            this.label6.TabIndex = 15;
            this.label6.Text = "Stop recording hotkey";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(124, 15);
            this.label5.TabIndex = 14;
            this.label5.Text = "Start recording hotkey";
            // 
            // textBoxRecordHotkey
            // 
            this.textBoxRecordHotkey.Location = new System.Drawing.Point(136, 71);
            this.textBoxRecordHotkey.Name = "textBoxRecordHotkey";
            this.textBoxRecordHotkey.Size = new System.Drawing.Size(122, 23);
            this.textBoxRecordHotkey.TabIndex = 13;
            this.textBoxRecordHotkey.Enter += new System.EventHandler(this.textBoxRecordHotkey_Enter);
            this.textBoxRecordHotkey.Leave += new System.EventHandler(this.textBoxRecordHotkey_Leave);
            // 
            // checkBoxRecClipboard
            // 
            this.checkBoxRecClipboard.AutoSize = true;
            this.checkBoxRecClipboard.Location = new System.Drawing.Point(152, 111);
            this.checkBoxRecClipboard.Name = "checkBoxRecClipboard";
            this.checkBoxRecClipboard.Size = new System.Drawing.Size(173, 19);
            this.checkBoxRecClipboard.TabIndex = 12;
            this.checkBoxRecClipboard.Text = "Record clipboard (text only)";
            this.checkBoxRecClipboard.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 15);
            this.label2.TabIndex = 11;
            this.label2.Text = "Playback speed";
            // 
            // comboBoxSpeed
            // 
            this.comboBoxSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSpeed.FormattingEnabled = true;
            this.comboBoxSpeed.Items.AddRange(new object[] {
            "Fastest",
            "Fast",
            "Normal",
            "Slow",
            "Slowest"});
            this.comboBoxSpeed.Location = new System.Drawing.Point(100, 27);
            this.comboBoxSpeed.Name = "comboBoxSpeed";
            this.comboBoxSpeed.Size = new System.Drawing.Size(84, 23);
            this.comboBoxSpeed.TabIndex = 10;
            // 
            // checkBoxRecMouseMove
            // 
            this.checkBoxRecMouseMove.AutoSize = true;
            this.checkBoxRecMouseMove.Location = new System.Drawing.Point(6, 111);
            this.checkBoxRecMouseMove.Name = "checkBoxRecMouseMove";
            this.checkBoxRecMouseMove.Size = new System.Drawing.Size(135, 19);
            this.checkBoxRecMouseMove.TabIndex = 9;
            this.checkBoxRecMouseMove.Text = "Record mouse move";
            this.checkBoxRecMouseMove.UseVisualStyleBackColor = true;
            // 
            // groupBoxAdvanced
            // 
            this.groupBoxAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxAdvanced.Controls.Add(this.label4);
            this.groupBoxAdvanced.Controls.Add(this.label3);
            this.groupBoxAdvanced.Controls.Add(this.comboBoxAutomationEngine);
            this.groupBoxAdvanced.Location = new System.Drawing.Point(12, 231);
            this.groupBoxAdvanced.Name = "groupBoxAdvanced";
            this.groupBoxAdvanced.Size = new System.Drawing.Size(570, 65);
            this.groupBoxAdvanced.TabIndex = 10;
            this.groupBoxAdvanced.TabStop = false;
            this.groupBoxAdvanced.Text = "Advanced";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(245, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(315, 33);
            this.label4.TabIndex = 2;
            this.label4.Text = "UI Automation is more accurate but can be slow on large apps. Win32 is faster but" +
    " less accurate.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "Automation engine";
            // 
            // comboBoxAutomationEngine
            // 
            this.comboBoxAutomationEngine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAutomationEngine.FormattingEnabled = true;
            this.comboBoxAutomationEngine.Items.AddRange(new object[] {
            "UI Automation",
            "Win32"});
            this.comboBoxAutomationEngine.Location = new System.Drawing.Point(122, 22);
            this.comboBoxAutomationEngine.Name = "comboBoxAutomationEngine";
            this.comboBoxAutomationEngine.Size = new System.Drawing.Size(107, 23);
            this.comboBoxAutomationEngine.TabIndex = 0;
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 337);
            this.Controls.Add(this.groupBoxAdvanced);
            this.Controls.Add(this.groupBoxGeneral);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "Options";
            this.Text = "Elegant Recorder Options";
            this.groupBoxGeneral.ResumeLayout(false);
            this.groupBoxGeneral.PerformLayout();
            this.groupBoxAdvanced.ResumeLayout(false);
            this.groupBoxAdvanced.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBoxRecordingPath;
        private System.Windows.Forms.Button buttonBrowseScript;
        private System.Windows.Forms.CheckBox checkBoxRestrictToExe;
        private System.Windows.Forms.TextBox textBoxExePath;
        private System.Windows.Forms.Button buttonBrowseExe;
        private System.Windows.Forms.GroupBox groupBoxGeneral;
        private System.Windows.Forms.CheckBox checkBoxRecMouseMove;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxSpeed;
        private System.Windows.Forms.GroupBox groupBoxAdvanced;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxAutomationEngine;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxRecClipboard;
        private System.Windows.Forms.TextBox textBoxRecordHotkey;
        private System.Windows.Forms.TextBox textBoxStopHotkey;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
    }
}