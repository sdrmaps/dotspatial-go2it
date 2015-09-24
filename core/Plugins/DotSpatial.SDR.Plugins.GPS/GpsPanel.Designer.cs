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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblPosition = new System.Windows.Forms.Label();
            this.lblAltitude = new System.Windows.Forms.Label();
            this.txtDate = new System.Windows.Forms.TextBox();
            this.txtPosition = new System.Windows.Forms.TextBox();
            this.txtAltitude = new System.Windows.Forms.TextBox();
            this.txtSpeed = new System.Windows.Forms.TextBox();
            this.txtBearing = new System.Windows.Forms.TextBox();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.lblBearing = new System.Windows.Forms.Label();
            this.chkAllowBluetooth = new System.Windows.Forms.CheckBox();
            this.cmbName = new System.Windows.Forms.ComboBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.chkAllowSerial = new System.Windows.Forms.CheckBox();
            this.txtConnType = new System.Windows.Forms.TextBox();
            this.lblBaudRate = new System.Windows.Forms.Label();
            this.nmeaInterpreter = new DotSpatial.Positioning.NmeaInterpreter();
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
            this.toolStrip1.MinimumSize = new System.Drawing.Size(0, 48);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(739, 48);
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
            this.gpsStartStop.Click += new System.EventHandler(this.gpsStartStop_Click);
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
            this.gpsPauseResume.ToolTipText = "Pause/Resume GPS Feed";
            this.gpsPauseResume.Click += new System.EventHandler(this.gpsPauseResume_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this.lblDate, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblAltitude, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtDate, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtPosition, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtAltitude, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtSpeed, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtBearing, 4, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblSpeed, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblBearing, 4, 2);
            this.tableLayoutPanel1.Controls.Add(this.chkAllowBluetooth, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.cmbName, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblStatus, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtStatus, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.chkAllowSerial, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtConnType, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblBaudRate, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblPosition, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 48);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(739, 219);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDate.Location = new System.Drawing.Point(3, 65);
            this.lblDate.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(141, 13);
            this.lblDate.TabIndex = 4;
            this.lblDate.Text = "Date";
            // 
            // lblPosition
            // 
            this.lblPosition.AutoSize = true;
            this.lblPosition.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPosition.Location = new System.Drawing.Point(150, 65);
            this.lblPosition.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(141, 13);
            this.lblPosition.TabIndex = 5;
            this.lblPosition.Text = "Position";
            // 
            // lblAltitude
            // 
            this.lblAltitude.AutoSize = true;
            this.lblAltitude.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAltitude.Location = new System.Drawing.Point(297, 65);
            this.lblAltitude.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.lblAltitude.Name = "lblAltitude";
            this.lblAltitude.Size = new System.Drawing.Size(141, 13);
            this.lblAltitude.TabIndex = 6;
            this.lblAltitude.Text = "Altitude";
            // 
            // txtDate
            // 
            this.txtDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDate.Location = new System.Drawing.Point(3, 86);
            this.txtDate.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.txtDate.Name = "txtDate";
            this.txtDate.ReadOnly = true;
            this.txtDate.Size = new System.Drawing.Size(141, 20);
            this.txtDate.TabIndex = 13;
            // 
            // txtPosition
            // 
            this.txtPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPosition.Location = new System.Drawing.Point(150, 86);
            this.txtPosition.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.txtPosition.Name = "txtPosition";
            this.txtPosition.ReadOnly = true;
            this.txtPosition.Size = new System.Drawing.Size(141, 20);
            this.txtPosition.TabIndex = 14;
            // 
            // txtAltitude
            // 
            this.txtAltitude.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAltitude.Location = new System.Drawing.Point(297, 86);
            this.txtAltitude.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.txtAltitude.Name = "txtAltitude";
            this.txtAltitude.ReadOnly = true;
            this.txtAltitude.Size = new System.Drawing.Size(141, 20);
            this.txtAltitude.TabIndex = 15;
            // 
            // txtSpeed
            // 
            this.txtSpeed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSpeed.Location = new System.Drawing.Point(444, 86);
            this.txtSpeed.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.txtSpeed.Name = "txtSpeed";
            this.txtSpeed.ReadOnly = true;
            this.txtSpeed.Size = new System.Drawing.Size(141, 20);
            this.txtSpeed.TabIndex = 16;
            // 
            // txtBearing
            // 
            this.txtBearing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBearing.Location = new System.Drawing.Point(591, 86);
            this.txtBearing.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.txtBearing.Name = "txtBearing";
            this.txtBearing.ReadOnly = true;
            this.txtBearing.Size = new System.Drawing.Size(145, 20);
            this.txtBearing.TabIndex = 17;
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSpeed.Location = new System.Drawing.Point(444, 65);
            this.lblSpeed.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(141, 13);
            this.lblSpeed.TabIndex = 8;
            this.lblSpeed.Text = "Speed";
            // 
            // lblBearing
            // 
            this.lblBearing.AutoSize = true;
            this.lblBearing.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblBearing.Location = new System.Drawing.Point(591, 65);
            this.lblBearing.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.lblBearing.Name = "lblBearing";
            this.lblBearing.Size = new System.Drawing.Size(145, 13);
            this.lblBearing.TabIndex = 7;
            this.lblBearing.Text = "Bearing";
            // 
            // chkAllowBluetooth
            // 
            this.chkAllowBluetooth.AutoSize = true;
            this.chkAllowBluetooth.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkAllowBluetooth.Location = new System.Drawing.Point(591, 33);
            this.chkAllowBluetooth.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.chkAllowBluetooth.Name = "chkAllowBluetooth";
            this.chkAllowBluetooth.Size = new System.Drawing.Size(145, 17);
            this.chkAllowBluetooth.TabIndex = 19;
            this.chkAllowBluetooth.Text = "Accept Bluetooth";
            this.chkAllowBluetooth.UseVisualStyleBackColor = true;
            // 
            // cmbName
            // 
            this.cmbName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbName.FormattingEnabled = true;
            this.cmbName.Location = new System.Drawing.Point(3, 31);
            this.cmbName.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.cmbName.Name = "cmbName";
            this.cmbName.Size = new System.Drawing.Size(141, 21);
            this.cmbName.TabIndex = 20;
            this.cmbName.SelectedIndexChanged += new System.EventHandler(this.cmbName_SelectedIndexChanged);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblName.Location = new System.Drawing.Point(3, 10);
            this.lblName.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(141, 13);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Name";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblStatus.Location = new System.Drawing.Point(150, 10);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(141, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Status";
            // 
            // txtStatus
            // 
            this.txtStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtStatus.Location = new System.Drawing.Point(150, 31);
            this.txtStatus.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(141, 20);
            this.txtStatus.TabIndex = 11;
            // 
            // chkAllowSerial
            // 
            this.chkAllowSerial.AutoSize = true;
            this.chkAllowSerial.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkAllowSerial.Location = new System.Drawing.Point(444, 33);
            this.chkAllowSerial.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.chkAllowSerial.Name = "chkAllowSerial";
            this.chkAllowSerial.Size = new System.Drawing.Size(141, 17);
            this.chkAllowSerial.TabIndex = 18;
            this.chkAllowSerial.Text = "Accept Serial";
            this.chkAllowSerial.UseVisualStyleBackColor = true;
            // 
            // txtConnType
            // 
            this.txtConnType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConnType.Location = new System.Drawing.Point(297, 31);
            this.txtConnType.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.txtConnType.Name = "txtConnType";
            this.txtConnType.ReadOnly = true;
            this.txtConnType.Size = new System.Drawing.Size(141, 20);
            this.txtConnType.TabIndex = 21;
            // 
            // lblBaudRate
            // 
            this.lblBaudRate.AutoSize = true;
            this.lblBaudRate.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblBaudRate.Location = new System.Drawing.Point(297, 10);
            this.lblBaudRate.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.lblBaudRate.Name = "lblBaudRate";
            this.lblBaudRate.Size = new System.Drawing.Size(141, 13);
            this.lblBaudRate.TabIndex = 3;
            this.lblBaudRate.Text = "Connection Type";
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
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblBaudRate;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblPosition;
        private System.Windows.Forms.Label lblAltitude;
        private System.Windows.Forms.Label lblBearing;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.TextBox txtDate;
        private System.Windows.Forms.TextBox txtPosition;
        private System.Windows.Forms.TextBox txtAltitude;
        private System.Windows.Forms.TextBox txtSpeed;
        private System.Windows.Forms.TextBox txtBearing;
        private System.Windows.Forms.CheckBox chkAllowSerial;
        private System.Windows.Forms.CheckBox chkAllowBluetooth;
        private Positioning.NmeaInterpreter nmeaInterpreter;
        private System.Windows.Forms.ComboBox cmbName;
        private System.Windows.Forms.TextBox txtConnType;
    }
}
