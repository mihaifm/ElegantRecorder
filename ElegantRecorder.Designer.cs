using System.Windows.Forms;

namespace ElegantRecorder
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ElegantRecorder));
            this.labelStatus = new System.Windows.Forms.Label();
            this.buttonPin = new System.Windows.Forms.Button();
            this.buttonReplay = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonRecord = new System.Windows.Forms.Button();
            this.buttonPause = new System.Windows.Forms.Button();
            this.buttonSettings = new System.Windows.Forms.Button();
            this.toolTipRec = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // labelStatus
            // 
            this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(12, 71);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(39, 15);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "Status";
            // 
            // buttonPin
            // 
            this.buttonPin.Image = global::ElegantRecorder.Properties.Resources.geo;
            this.buttonPin.Location = new System.Drawing.Point(320, 12);
            this.buttonPin.Name = "buttonPin";
            this.buttonPin.Size = new System.Drawing.Size(49, 48);
            this.buttonPin.TabIndex = 11;
            this.toolTipRec.SetToolTip(this.buttonPin, "Pin to top");
            this.buttonPin.UseVisualStyleBackColor = true;
            this.buttonPin.Click += new System.EventHandler(this.buttonPin_Click);
            // 
            // buttonReplay
            // 
            this.buttonReplay.Image = global::ElegantRecorder.Properties.Resources.play_fill;
            this.buttonReplay.Location = new System.Drawing.Point(177, 12);
            this.buttonReplay.Name = "buttonReplay";
            this.buttonReplay.Size = new System.Drawing.Size(49, 48);
            this.buttonReplay.TabIndex = 12;
            this.toolTipRec.SetToolTip(this.buttonReplay, "Replay");
            this.buttonReplay.UseVisualStyleBackColor = true;
            this.buttonReplay.Click += new System.EventHandler(this.buttonReplay_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Image = global::ElegantRecorder.Properties.Resources.stop_fill;
            this.buttonStop.Location = new System.Drawing.Point(122, 12);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(49, 48);
            this.buttonStop.TabIndex = 13;
            this.toolTipRec.SetToolTip(this.buttonStop, "Stop");
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonStop_MouseDown);
            // 
            // buttonRecord
            // 
            this.buttonRecord.Image = global::ElegantRecorder.Properties.Resources.record_fill;
            this.buttonRecord.Location = new System.Drawing.Point(12, 12);
            this.buttonRecord.Name = "buttonRecord";
            this.buttonRecord.Size = new System.Drawing.Size(49, 48);
            this.buttonRecord.TabIndex = 14;
            this.toolTipRec.SetToolTip(this.buttonRecord, "Record");
            this.buttonRecord.UseVisualStyleBackColor = true;
            this.buttonRecord.Click += new System.EventHandler(this.buttonRecord_Click);
            // 
            // buttonPause
            // 
            this.buttonPause.Image = global::ElegantRecorder.Properties.Resources.pause_fill;
            this.buttonPause.Location = new System.Drawing.Point(67, 12);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(49, 48);
            this.buttonPause.TabIndex = 15;
            this.toolTipRec.SetToolTip(this.buttonPause, "Pause");
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonPause_MouseDown);
            // 
            // buttonSettings
            // 
            this.buttonSettings.Image = global::ElegantRecorder.Properties.Resources.gear;
            this.buttonSettings.Location = new System.Drawing.Point(265, 12);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Size = new System.Drawing.Size(49, 48);
            this.buttonSettings.TabIndex = 16;
            this.toolTipRec.SetToolTip(this.buttonSettings, "Settings");
            this.buttonSettings.UseVisualStyleBackColor = true;
            this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
            // 
            // ElegantRecorder
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(380, 95);
            this.Controls.Add(this.buttonSettings);
            this.Controls.Add(this.buttonPause);
            this.Controls.Add(this.buttonRecord);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonReplay);
            this.Controls.Add(this.buttonPin);
            this.Controls.Add(this.labelStatus);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ElegantRecorder";
            this.Text = "Elegant Recorder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ElegantRecorder_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Label labelStatus;
        private Button buttonPin;
        private Button buttonReplay;
        private Button buttonStop;
        private Button buttonRecord;
        private Button buttonPause;
        private Button buttonSettings;
        private ToolTip toolTipRec;
    }
}