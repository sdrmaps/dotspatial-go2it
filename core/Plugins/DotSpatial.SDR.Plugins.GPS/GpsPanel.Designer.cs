namespace DotSpatial.SDR.Plugins.GPS
{
    partial class GpsPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.gpsDetectCancel = new System.Windows.Forms.ToolStripButton();
            this.gpsStartStop = new System.Windows.Forms.ToolStripButton();
            this.gpsPauseResume = new System.Windows.Forms.ToolStripButton();
            this.nmeaInterpreter = new DotSpatial.Positioning.NmeaInterpreter();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblPortNumber = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblBaudRate = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblPosition = new System.Windows.Forms.Label();
            this.lblAltitude = new System.Windows.Forms.Label();
            this.txtPortNumber = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.txtBaudRate = new System.Windows.Forms.TextBox();
            this.txtDate = new System.Windows.Forms.TextBox();
            this.txtPosition = new System.Windows.Forms.TextBox();
            this.txtAltitude = new System.Windows.Forms.TextBox();
            this.txtSpeed = new System.Windows.Forms.TextBox();
            this.txtBearing = new System.Windows.Forms.TextBox();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.lblBearing = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gpsDetectCancel,
            this.gpsStartStop,
            this.gpsPauseResume});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.MinimumSize = new System.Drawing.Size(0, 45);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(739, 45);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // gpsDetectCancel
            // 
            this.gpsDetectCancel.AutoSize = false;
            this.gpsDetectCancel.Image = global::DotSpatial.SDR.Plugins.GPS.Properties.Resources.info_32;
            this.gpsDetectCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.gpsDetectCancel.Name = "gpsDetectCancel";
            this.gpsDetectCancel.Size = new System.Drawing.Size(45, 45);
            this.gpsDetectCancel.Text = "Detect";
            this.gpsDetectCancel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.gpsDetectCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.gpsDetectCancel.Click += new System.EventHandler(this.gpsDetectCancel_Click);
            // 
            // gpsStartStop
            // 
            this.gpsStartStop.AutoSize = false;
            this.gpsStartStop.Image = global::DotSpatial.SDR.Plugins.GPS.Properties.Resources.info_32;
            this.gpsStartStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.gpsStartStop.Name = "gpsStartStop";
            this.gpsStartStop.Size = new System.Drawing.Size(45, 45);
            this.gpsStartStop.Text = "Start";
            this.gpsStartStop.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.gpsStartStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // gpsPauseResume
            // 
            this.gpsPauseResume.AutoSize = false;
            this.gpsPauseResume.Image = global::DotSpatial.SDR.Plugins.GPS.Properties.Resources.info_32;
            this.gpsPauseResume.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.gpsPauseResume.Name = "gpsPauseResume";
            this.gpsPauseResume.Size = new System.Drawing.Size(45, 45);
            this.gpsPauseResume.Text = "Pause";
            this.gpsPauseResume.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.gpsPauseResume.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31.48615F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 68.51385F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 138F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 154F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 122F));
            this.tableLayoutPanel1.Controls.Add(this.lblPortNumber, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblName, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblStatus, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblBaudRate, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblDate, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblPosition, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblAltitude, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtPortNumber, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtName, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtStatus, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtBaudRate, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtDate, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtPosition, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtAltitude, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtSpeed, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtBearing, 4, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblSpeed, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblBearing, 4, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 45);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42.30769F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 57.69231F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 124F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(739, 222);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // lblPortNumber
            // 
            this.lblPortNumber.AutoSize = true;
            this.lblPortNumber.Location = new System.Drawing.Point(3, 0);
            this.lblPortNumber.Name = "lblPortNumber";
            this.lblPortNumber.Size = new System.Drawing.Size(66, 13);
            this.lblPortNumber.TabIndex = 0;
            this.lblPortNumber.Text = "Port Number";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(105, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Name";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(327, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(37, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Status";
            // 
            // lblBaudRate
            // 
            this.lblBaudRate.AutoSize = true;
            this.lblBaudRate.Location = new System.Drawing.Point(465, 0);
            this.lblBaudRate.Name = "lblBaudRate";
            this.lblBaudRate.Size = new System.Drawing.Size(58, 13);
            this.lblBaudRate.TabIndex = 3;
            this.lblBaudRate.Text = "Baud Rate";
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(3, 52);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(30, 13);
            this.lblDate.TabIndex = 4;
            this.lblDate.Text = "Date";
            // 
            // lblPosition
            // 
            this.lblPosition.AutoSize = true;
            this.lblPosition.Location = new System.Drawing.Point(105, 52);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(44, 13);
            this.lblPosition.TabIndex = 5;
            this.lblPosition.Text = "Position";
            // 
            // lblAltitude
            // 
            this.lblAltitude.AutoSize = true;
            this.lblAltitude.Location = new System.Drawing.Point(327, 52);
            this.lblAltitude.Name = "lblAltitude";
            this.lblAltitude.Size = new System.Drawing.Size(42, 13);
            this.lblAltitude.TabIndex = 6;
            this.lblAltitude.Text = "Altitude";
            // 
            // txtPortNumber
            // 
            this.txtPortNumber.Location = new System.Drawing.Point(3, 25);
            this.txtPortNumber.Name = "txtPortNumber";
            this.txtPortNumber.ReadOnly = true;
            this.txtPortNumber.Size = new System.Drawing.Size(96, 20);
            this.txtPortNumber.TabIndex = 9;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(105, 25);
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(100, 20);
            this.txtName.TabIndex = 10;
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(327, 25);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(100, 20);
            this.txtStatus.TabIndex = 11;
            // 
            // txtBaudRate
            // 
            this.txtBaudRate.Location = new System.Drawing.Point(465, 25);
            this.txtBaudRate.Name = "txtBaudRate";
            this.txtBaudRate.ReadOnly = true;
            this.txtBaudRate.Size = new System.Drawing.Size(100, 20);
            this.txtBaudRate.TabIndex = 12;
            // 
            // txtDate
            // 
            this.txtDate.Location = new System.Drawing.Point(3, 74);
            this.txtDate.Name = "txtDate";
            this.txtDate.ReadOnly = true;
            this.txtDate.Size = new System.Drawing.Size(96, 20);
            this.txtDate.TabIndex = 13;
            // 
            // txtPosition
            // 
            this.txtPosition.Location = new System.Drawing.Point(105, 74);
            this.txtPosition.Name = "txtPosition";
            this.txtPosition.ReadOnly = true;
            this.txtPosition.Size = new System.Drawing.Size(100, 20);
            this.txtPosition.TabIndex = 14;
            // 
            // txtAltitude
            // 
            this.txtAltitude.Location = new System.Drawing.Point(327, 74);
            this.txtAltitude.Name = "txtAltitude";
            this.txtAltitude.ReadOnly = true;
            this.txtAltitude.Size = new System.Drawing.Size(100, 20);
            this.txtAltitude.TabIndex = 15;
            // 
            // txtSpeed
            // 
            this.txtSpeed.Location = new System.Drawing.Point(465, 74);
            this.txtSpeed.Name = "txtSpeed";
            this.txtSpeed.ReadOnly = true;
            this.txtSpeed.Size = new System.Drawing.Size(100, 20);
            this.txtSpeed.TabIndex = 16;
            // 
            // txtBearing
            // 
            this.txtBearing.Location = new System.Drawing.Point(619, 74);
            this.txtBearing.Name = "txtBearing";
            this.txtBearing.ReadOnly = true;
            this.txtBearing.Size = new System.Drawing.Size(100, 20);
            this.txtBearing.TabIndex = 17;
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(465, 52);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(38, 13);
            this.lblSpeed.TabIndex = 8;
            this.lblSpeed.Text = "Speed";
            // 
            // lblBearing
            // 
            this.lblBearing.AutoSize = true;
            this.lblBearing.Location = new System.Drawing.Point(619, 52);
            this.lblBearing.Name = "lblBearing";
            this.lblBearing.Size = new System.Drawing.Size(43, 13);
            this.lblBearing.TabIndex = 7;
            this.lblBearing.Text = "Bearing";
            // 
            // GpsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "GpsPanel";
            this.Size = new System.Drawing.Size(739, 267);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton gpsDetectCancel;
        private System.Windows.Forms.ToolStripButton gpsStartStop;
        private System.Windows.Forms.ToolStripButton gpsPauseResume;
        private Positioning.NmeaInterpreter nmeaInterpreter;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblPortNumber;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblBaudRate;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblPosition;
        private System.Windows.Forms.Label lblAltitude;
        private System.Windows.Forms.Label lblBearing;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.TextBox txtPortNumber;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.TextBox txtBaudRate;
        private System.Windows.Forms.TextBox txtDate;
        private System.Windows.Forms.TextBox txtPosition;
        private System.Windows.Forms.TextBox txtAltitude;
        private System.Windows.Forms.TextBox txtSpeed;
        private System.Windows.Forms.TextBox txtBearing;
    }
}
