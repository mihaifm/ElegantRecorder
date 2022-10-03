
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
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxRecordingPath = new System.Windows.Forms.TextBox();
            this.buttonBrowseScript = new System.Windows.Forms.Button();
            this.checkBoxRestrictToExe = new System.Windows.Forms.CheckBox();
            this.textBoxExePath = new System.Windows.Forms.TextBox();
            this.buttonBrowseExe = new System.Windows.Forms.Button();
            this.groupBoxOptions = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.checkBoxRecMouseMove = new System.Windows.Forms.CheckBox();
            this.groupBoxOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 155);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Recording file";
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(426, 208);
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
            this.buttonCancel.Location = new System.Drawing.Point(507, 208);
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
            this.textBoxRecordingPath.Location = new System.Drawing.Point(112, 151);
            this.textBoxRecordingPath.Name = "textBoxRecordingPath";
            this.textBoxRecordingPath.Size = new System.Drawing.Size(388, 23);
            this.textBoxRecordingPath.TabIndex = 4;
            // 
            // buttonBrowseScript
            // 
            this.buttonBrowseScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseScript.Location = new System.Drawing.Point(506, 151);
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
            this.checkBoxRestrictToExe.Location = new System.Drawing.Point(6, 109);
            this.checkBoxRestrictToExe.Name = "checkBoxRestrictToExe";
            this.checkBoxRestrictToExe.Size = new System.Drawing.Size(100, 19);
            this.checkBoxRestrictToExe.TabIndex = 6;
            this.checkBoxRestrictToExe.Text = "Restrict to exe";
            this.checkBoxRestrictToExe.UseVisualStyleBackColor = true;
            // 
            // textBoxExePath
            // 
            this.textBoxExePath.Location = new System.Drawing.Point(112, 107);
            this.textBoxExePath.Name = "textBoxExePath";
            this.textBoxExePath.Size = new System.Drawing.Size(388, 23);
            this.textBoxExePath.TabIndex = 7;
            // 
            // buttonBrowseExe
            // 
            this.buttonBrowseExe.Location = new System.Drawing.Point(505, 106);
            this.buttonBrowseExe.Name = "buttonBrowseExe";
            this.buttonBrowseExe.Size = new System.Drawing.Size(59, 23);
            this.buttonBrowseExe.TabIndex = 8;
            this.buttonBrowseExe.Text = "...";
            this.buttonBrowseExe.UseVisualStyleBackColor = true;
            this.buttonBrowseExe.Click += new System.EventHandler(this.buttonBrowseExe_Click);
            // 
            // groupBoxOptions
            // 
            this.groupBoxOptions.Controls.Add(this.label2);
            this.groupBoxOptions.Controls.Add(this.comboBox1);
            this.groupBoxOptions.Controls.Add(this.checkBoxRecMouseMove);
            this.groupBoxOptions.Controls.Add(this.label1);
            this.groupBoxOptions.Controls.Add(this.buttonBrowseExe);
            this.groupBoxOptions.Controls.Add(this.textBoxRecordingPath);
            this.groupBoxOptions.Controls.Add(this.textBoxExePath);
            this.groupBoxOptions.Controls.Add(this.buttonBrowseScript);
            this.groupBoxOptions.Controls.Add(this.checkBoxRestrictToExe);
            this.groupBoxOptions.Location = new System.Drawing.Point(12, 12);
            this.groupBoxOptions.Name = "groupBoxOptions";
            this.groupBoxOptions.Size = new System.Drawing.Size(571, 180);
            this.groupBoxOptions.TabIndex = 9;
            this.groupBoxOptions.TabStop = false;
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
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Fastest",
            "Fast",
            "Normal",
            "Slow",
            "Slowest"});
            this.comboBox1.Location = new System.Drawing.Point(100, 27);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(84, 23);
            this.comboBox1.TabIndex = 10;
            // 
            // checkBoxRecMouseMove
            // 
            this.checkBoxRecMouseMove.AutoSize = true;
            this.checkBoxRecMouseMove.Location = new System.Drawing.Point(6, 67);
            this.checkBoxRecMouseMove.Name = "checkBoxRecMouseMove";
            this.checkBoxRecMouseMove.Size = new System.Drawing.Size(135, 19);
            this.checkBoxRecMouseMove.TabIndex = 9;
            this.checkBoxRecMouseMove.Text = "Record mouse move";
            this.checkBoxRecMouseMove.UseVisualStyleBackColor = true;
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 243);
            this.Controls.Add(this.groupBoxOptions);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Name = "Options";
            this.Text = "Elegant Recorder Options";
            this.groupBoxOptions.ResumeLayout(false);
            this.groupBoxOptions.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBoxOptions;
        private System.Windows.Forms.CheckBox checkBoxRecMouseMove;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}