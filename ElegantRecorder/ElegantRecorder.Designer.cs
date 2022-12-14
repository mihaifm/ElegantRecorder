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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ElegantRecorder));
            this.labelStatus = new System.Windows.Forms.Label();
            this.buttonPin = new System.Windows.Forms.Button();
            this.buttonReplay = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonRecord = new System.Windows.Forms.Button();
            this.buttonPause = new System.Windows.Forms.Button();
            this.buttonSettings = new System.Windows.Forms.Button();
            this.toolTipRec = new System.Windows.Forms.ToolTip(this.components);
            this.buttonTriggers = new System.Windows.Forms.Button();
            this.buttonExpand = new System.Windows.Forms.Button();
            this.dataGridViewRecordings = new System.Windows.Forms.DataGridView();
            this.textBoxNewRec = new System.Windows.Forms.TextBox();
            this.buttonAddRec = new System.Windows.Forms.Button();
            this.contextMenuStripRClick = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Recordings = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Hotkey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Indicator = new System.Windows.Forms.DataGridViewImageColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRecordings)).BeginInit();
            this.contextMenuStripRClick.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelStatus
            // 
            this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(12, 287);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(39, 15);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "Status";
            // 
            // buttonPin
            // 
            this.buttonPin.Image = global::ElegantRecorder.Properties.Resources.geo;
            this.buttonPin.Location = new System.Drawing.Point(375, 12);
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
            // buttonTriggers
            // 
            this.buttonTriggers.Image = global::ElegantRecorder.Properties.Resources.stopwatch;
            this.buttonTriggers.Location = new System.Drawing.Point(320, 12);
            this.buttonTriggers.Name = "buttonTriggers";
            this.buttonTriggers.Size = new System.Drawing.Size(49, 48);
            this.buttonTriggers.TabIndex = 22;
            this.toolTipRec.SetToolTip(this.buttonTriggers, "Triggers");
            this.buttonTriggers.UseVisualStyleBackColor = true;
            this.buttonTriggers.Click += new System.EventHandler(this.buttonTriggers_Click);
            // 
            // buttonExpand
            // 
            this.buttonExpand.FlatAppearance.BorderSize = 0;
            this.buttonExpand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExpand.Image = global::ElegantRecorder.Properties.Resources.double_up;
            this.buttonExpand.Location = new System.Drawing.Point(429, 12);
            this.buttonExpand.Name = "buttonExpand";
            this.buttonExpand.Size = new System.Drawing.Size(49, 48);
            this.buttonExpand.TabIndex = 17;
            this.buttonExpand.UseVisualStyleBackColor = true;
            this.buttonExpand.Click += new System.EventHandler(this.buttonExpand_Click);
            // 
            // dataGridViewRecordings
            // 
            this.dataGridViewRecordings.AllowUserToAddRows = false;
            this.dataGridViewRecordings.AllowUserToDeleteRows = false;
            this.dataGridViewRecordings.AllowUserToResizeColumns = false;
            this.dataGridViewRecordings.AllowUserToResizeRows = false;
            this.dataGridViewRecordings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewRecordings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewRecordings.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewRecordings.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridViewRecordings.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewRecordings.ColumnHeadersVisible = false;
            this.dataGridViewRecordings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Recordings,
            this.Hotkey,
            this.Indicator});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.ControlDark;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewRecordings.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewRecordings.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewRecordings.Location = new System.Drawing.Point(12, 120);
            this.dataGridViewRecordings.MultiSelect = false;
            this.dataGridViewRecordings.Name = "dataGridViewRecordings";
            this.dataGridViewRecordings.RowHeadersVisible = false;
            this.dataGridViewRecordings.RowTemplate.Height = 25;
            this.dataGridViewRecordings.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRecordings.Size = new System.Drawing.Size(469, 162);
            this.dataGridViewRecordings.TabIndex = 18;
            this.dataGridViewRecordings.SelectionChanged += new System.EventHandler(this.dataGridViewRecordings_SelectionChanged);
            this.dataGridViewRecordings.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewRecordings_KeyDown);
            this.dataGridViewRecordings.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridViewRecordings_MouseDown);
            // 
            // textBoxNewRec
            // 
            this.textBoxNewRec.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxNewRec.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBoxNewRec.Location = new System.Drawing.Point(12, 87);
            this.textBoxNewRec.Name = "textBoxNewRec";
            this.textBoxNewRec.PlaceholderText = " New...";
            this.textBoxNewRec.Size = new System.Drawing.Size(411, 29);
            this.textBoxNewRec.TabIndex = 19;
            this.textBoxNewRec.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxNewRec_KeyDown);
            // 
            // buttonAddRec
            // 
            this.buttonAddRec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddRec.Image = global::ElegantRecorder.Properties.Resources.plus_circle;
            this.buttonAddRec.Location = new System.Drawing.Point(429, 87);
            this.buttonAddRec.Name = "buttonAddRec";
            this.buttonAddRec.Size = new System.Drawing.Size(52, 29);
            this.buttonAddRec.TabIndex = 21;
            this.buttonAddRec.UseVisualStyleBackColor = true;
            this.buttonAddRec.Click += new System.EventHandler(this.buttonAddRec_Click);
            // 
            // contextMenuStripRClick
            // 
            this.contextMenuStripRClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextMenuStripRClick.Name = "contextMenuStripRClick";
            this.contextMenuStripRClick.Size = new System.Drawing.Size(118, 70);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // Recordings
            // 
            this.Recordings.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Recordings.DefaultCellStyle = dataGridViewCellStyle1;
            this.Recordings.FillWeight = 197.7221F;
            this.Recordings.HeaderText = "Recordings";
            this.Recordings.Name = "Recordings";
            // 
            // Hotkey
            // 
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Hotkey.DefaultCellStyle = dataGridViewCellStyle2;
            this.Hotkey.FillWeight = 76.14212F;
            this.Hotkey.HeaderText = "Hotkey";
            this.Hotkey.Name = "Hotkey";
            // 
            // Indicator
            // 
            this.Indicator.FillWeight = 26.13569F;
            this.Indicator.HeaderText = "Indicator";
            this.Indicator.Name = "Indicator";
            this.Indicator.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Indicator.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ElegantRecorder
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(493, 311);
            this.Controls.Add(this.buttonTriggers);
            this.Controls.Add(this.buttonAddRec);
            this.Controls.Add(this.textBoxNewRec);
            this.Controls.Add(this.buttonExpand);
            this.Controls.Add(this.buttonSettings);
            this.Controls.Add(this.buttonPause);
            this.Controls.Add(this.buttonRecord);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonReplay);
            this.Controls.Add(this.buttonPin);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.dataGridViewRecordings);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ElegantRecorder";
            this.Text = "Elegant Recorder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ElegantRecorder_FormClosing);
            this.Resize += new System.EventHandler(this.ElegantRecorder_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRecordings)).EndInit();
            this.contextMenuStripRClick.ResumeLayout(false);
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
        private Button buttonExpand;
        private DataGridView dataGridViewRecordings;
        private TextBox textBoxNewRec;
        private Button buttonAddRec;
        private ContextMenuStrip contextMenuStripRClick;
        private ToolStripMenuItem refreshToolStripMenuItem;
        private ToolStripMenuItem renameToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private Button buttonTriggers;
        private DataGridViewTextBoxColumn Recordings;
        private DataGridViewTextBoxColumn Hotkey;
        private DataGridViewImageColumn Indicator;
    }
}