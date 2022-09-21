using System.Windows.Forms;

namespace UIAuto
{
    partial class ElegantRecorder
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
            this.labelStatus = new System.Windows.Forms.Label();
            this.textBoxScriptName = new System.Windows.Forms.TextBox();
            this.buttonBrowseScript = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonRecord = new System.Windows.Forms.Button();
            this.buttonReplay = new System.Windows.Forms.Button();
            this.textBoxExeName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonBrowseExe = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(12, 145);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(39, 15);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "Status";
            // 
            // textBoxScriptName
            // 
            this.textBoxScriptName.Location = new System.Drawing.Point(78, 6);
            this.textBoxScriptName.Name = "textBoxScriptName";
            this.textBoxScriptName.Size = new System.Drawing.Size(313, 23);
            this.textBoxScriptName.TabIndex = 1;
            this.textBoxScriptName.TextChanged += new System.EventHandler(this.textBoxScriptName_TextChanged);
            // 
            // buttonBrowseScript
            // 
            this.buttonBrowseScript.Location = new System.Drawing.Point(397, 6);
            this.buttonBrowseScript.Name = "buttonBrowseScript";
            this.buttonBrowseScript.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseScript.TabIndex = 2;
            this.buttonBrowseScript.Text = "Browse...";
            this.buttonBrowseScript.UseVisualStyleBackColor = true;
            this.buttonBrowseScript.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Script";
            // 
            // buttonRecord
            // 
            this.buttonRecord.Location = new System.Drawing.Point(78, 82);
            this.buttonRecord.Name = "buttonRecord";
            this.buttonRecord.Size = new System.Drawing.Size(84, 34);
            this.buttonRecord.TabIndex = 4;
            this.buttonRecord.Text = "Record";
            this.buttonRecord.UseVisualStyleBackColor = true;
            this.buttonRecord.Click += new System.EventHandler(this.buttonRecord_Click);
            // 
            // buttonReplay
            // 
            this.buttonReplay.Location = new System.Drawing.Point(306, 82);
            this.buttonReplay.Name = "buttonReplay";
            this.buttonReplay.Size = new System.Drawing.Size(84, 34);
            this.buttonReplay.TabIndex = 5;
            this.buttonReplay.Text = "Replay";
            this.buttonReplay.UseVisualStyleBackColor = true;
            this.buttonReplay.Click += new System.EventHandler(this.buttonReplay_Click);
            // 
            // textBoxExeName
            // 
            this.textBoxExeName.Location = new System.Drawing.Point(78, 35);
            this.textBoxExeName.Name = "textBoxExeName";
            this.textBoxExeName.Size = new System.Drawing.Size(313, 23);
            this.textBoxExeName.TabIndex = 6;
            this.textBoxExeName.TextChanged += new System.EventHandler(this.textBoxExeName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "Target Exe";
            // 
            // buttonBrowseExe
            // 
            this.buttonBrowseExe.Location = new System.Drawing.Point(397, 35);
            this.buttonBrowseExe.Name = "buttonBrowseExe";
            this.buttonBrowseExe.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseExe.TabIndex = 8;
            this.buttonBrowseExe.Text = "Browse...";
            this.buttonBrowseExe.UseVisualStyleBackColor = true;
            this.buttonBrowseExe.Click += new System.EventHandler(this.buttonBrowseExe_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(192, 82);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(84, 34);
            this.buttonStop.TabIndex = 9;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // ElegantRecorder
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(484, 169);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonBrowseExe);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxExeName);
            this.Controls.Add(this.buttonReplay);
            this.Controls.Add(this.buttonRecord);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonBrowseScript);
            this.Controls.Add(this.textBoxScriptName);
            this.Controls.Add(this.labelStatus);
            this.Name = "ElegantRecorder";
            this.Text = "Elegant Recorder";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox textBoxScriptName;
        private Button buttonBrowseScript;
        private Label label2;
        private Button buttonRecord;
        private Button buttonReplay;
        private Label labelStatus;
        private TextBox textBoxExeName;
        private Label label1;
        private Button buttonBrowseExe;
        private Button buttonStop;
    }
}