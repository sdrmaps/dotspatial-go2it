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
            this.lblPosition = new System.Windows.Forms.Label();
            this.clock1 = new DotSpatial.Positioning.Forms.Clock();
            this.satelliteViewer1 = new DotSpatial.Positioning.Forms.SatelliteViewer();
            this.altimeter1 = new DotSpatial.Positioning.Forms.Altimeter();
            this.speedometer1 = new DotSpatial.Positioning.Forms.Speedometer();
            this.compass1 = new DotSpatial.Positioning.Forms.Compass();
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
            this.toolStrip1.Size = new System.Drawing.Size(594, 48);
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
            this.tableLayoutPanel1.Controls.Add(this.clock1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.satelliteViewer1, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.altimeter1, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.speedometer1, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.compass1, 4, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 48);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(594, 224);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDate.Location = new System.Drawing.Point(3, 52);
            this.lblDate.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(112, 13);
            this.lblDate.TabIndex = 4;
            this.lblDate.Text = "Date";
            // 
            // lblAltitude
            // 
            this.lblAltitude.AutoSize = true;
            this.lblAltitude.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAltitude.Location = new System.Drawing.Point(239, 52);
            this.lblAltitude.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblAltitude.Name = "lblAltitude";
            this.lblAltitude.Size = new System.Drawing.Size(112, 13);
            this.lblAltitude.TabIndex = 6;
            this.lblAltitude.Text = "Altitude";
            // 
            // txtDate
            // 
            this.txtDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDate.Location = new System.Drawing.Point(3, 68);
            this.txtDate.Name = "txtDate";
            this.txtDate.ReadOnly = true;
            this.txtDate.Size = new System.Drawing.Size(112, 20);
            this.txtDate.TabIndex = 13;
            // 
            // txtPosition
            // 
            this.txtPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPosition.Location = new System.Drawing.Point(121, 68);
            this.txtPosition.Name = "txtPosition";
            this.txtPosition.ReadOnly = true;
            this.txtPosition.Size = new System.Drawing.Size(112, 20);
            this.txtPosition.TabIndex = 14;
            // 
            // txtAltitude
            // 
            this.txtAltitude.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAltitude.Location = new System.Drawing.Point(239, 68);
            this.txtAltitude.Name = "txtAltitude";
            this.txtAltitude.ReadOnly = true;
            this.txtAltitude.Size = new System.Drawing.Size(112, 20);
            this.txtAltitude.TabIndex = 15;
            // 
            // txtSpeed
            // 
            this.txtSpeed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSpeed.Location = new System.Drawing.Point(357, 68);
            this.txtSpeed.Name = "txtSpeed";
            this.txtSpeed.ReadOnly = true;
            this.txtSpeed.Size = new System.Drawing.Size(112, 20);
            this.txtSpeed.TabIndex = 16;
            // 
            // txtBearing
            // 
            this.txtBearing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBearing.Location = new System.Drawing.Point(475, 68);
            this.txtBearing.Name = "txtBearing";
            this.txtBearing.ReadOnly = true;
            this.txtBearing.Size = new System.Drawing.Size(116, 20);
            this.txtBearing.TabIndex = 17;
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSpeed.Location = new System.Drawing.Point(357, 52);
            this.lblSpeed.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(112, 13);
            this.lblSpeed.TabIndex = 8;
            this.lblSpeed.Text = "Speed";
            // 
            // lblBearing
            // 
            this.lblBearing.AutoSize = true;
            this.lblBearing.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblBearing.Location = new System.Drawing.Point(475, 52);
            this.lblBearing.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblBearing.Name = "lblBearing";
            this.lblBearing.Size = new System.Drawing.Size(116, 13);
            this.lblBearing.TabIndex = 7;
            this.lblBearing.Text = "Bearing";
            // 
            // chkAllowBluetooth
            // 
            this.chkAllowBluetooth.AutoSize = true;
            this.chkAllowBluetooth.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkAllowBluetooth.Location = new System.Drawing.Point(477, 24);
            this.chkAllowBluetooth.Margin = new System.Windows.Forms.Padding(5, 5, 3, 3);
            this.chkAllowBluetooth.Name = "chkAllowBluetooth";
            this.chkAllowBluetooth.Size = new System.Drawing.Size(114, 17);
            this.chkAllowBluetooth.TabIndex = 19;
            this.chkAllowBluetooth.Text = "Accept Bluetooth";
            this.chkAllowBluetooth.UseVisualStyleBackColor = true;
            this.chkAllowBluetooth.CheckedChanged += new System.EventHandler(this.chkAllowBluetooth_CheckedChanged);
            // 
            // cmbName
            // 
            this.cmbName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbName.FormattingEnabled = true;
            this.cmbName.Location = new System.Drawing.Point(3, 22);
            this.cmbName.Name = "cmbName";
            this.cmbName.Size = new System.Drawing.Size(112, 21);
            this.cmbName.TabIndex = 20;
            this.cmbName.SelectedIndexChanged += new System.EventHandler(this.cmbName_SelectedIndexChanged);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblName.Location = new System.Drawing.Point(3, 6);
            this.lblName.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(112, 13);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Name";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblStatus.Location = new System.Drawing.Point(121, 6);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(112, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Status";
            // 
            // txtStatus
            // 
            this.txtStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtStatus.Location = new System.Drawing.Point(121, 22);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(112, 20);
            this.txtStatus.TabIndex = 11;
            // 
            // chkAllowSerial
            // 
            this.chkAllowSerial.AutoSize = true;
            this.chkAllowSerial.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkAllowSerial.Location = new System.Drawing.Point(359, 24);
            this.chkAllowSerial.Margin = new System.Windows.Forms.Padding(5, 5, 3, 3);
            this.chkAllowSerial.Name = "chkAllowSerial";
            this.chkAllowSerial.Size = new System.Drawing.Size(110, 17);
            this.chkAllowSerial.TabIndex = 18;
            this.chkAllowSerial.Text = "Accept Serial";
            this.chkAllowSerial.UseVisualStyleBackColor = true;
            this.chkAllowSerial.CheckedChanged += new System.EventHandler(this.chkAllowSerial_CheckedChanged);
            // 
            // txtConnType
            // 
            this.txtConnType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConnType.Location = new System.Drawing.Point(239, 22);
            this.txtConnType.Name = "txtConnType";
            this.txtConnType.ReadOnly = true;
            this.txtConnType.Size = new System.Drawing.Size(112, 20);
            this.txtConnType.TabIndex = 21;
            // 
            // lblBaudRate
            // 
            this.lblBaudRate.AutoSize = true;
            this.lblBaudRate.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblBaudRate.Location = new System.Drawing.Point(239, 6);
            this.lblBaudRate.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblBaudRate.Name = "lblBaudRate";
            this.lblBaudRate.Size = new System.Drawing.Size(112, 13);
            this.lblBaudRate.TabIndex = 3;
            this.lblBaudRate.Text = "Connection Type";
            // 
            // lblPosition
            // 
            this.lblPosition.AutoSize = true;
            this.lblPosition.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPosition.Location = new System.Drawing.Point(121, 52);
            this.lblPosition.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(112, 13);
            this.lblPosition.TabIndex = 5;
            this.lblPosition.Text = "Position";
            // 
            // clock1
            // 
            this.clock1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.clock1.CenterR = 0F;
            this.clock1.DisplayMode = DotSpatial.Positioning.Forms.ClockDisplayMode.Manual;
            this.clock1.Height = 102;
            this.clock1.IsPaintingOnSeparateThread = true;
            this.clock1.Location = new System.Drawing.Point(8, 99);
            this.clock1.Margin = new System.Windows.Forms.Padding(8, 8, 8, 3);
            this.clock1.MaximumR = 100F;
            this.clock1.Name = "clock1";
            this.clock1.Origin = new DotSpatial.Positioning.Azimuth(0D);
            this.clock1.Rotation = new DotSpatial.Positioning.Angle(0D);
            this.clock1.Size = new System.Drawing.Size(102, 102);
            this.clock1.TabIndex = 22;
            this.clock1.Text = "clock1";
            this.clock1.UpdateInterval = System.TimeSpan.Parse("00:00:00.1000000");
            this.clock1.Value = new System.DateTime(2015, 9, 24, 13, 7, 42, 243);
            this.clock1.ValueColor = System.Drawing.Color.Black;
            this.clock1.Width = 102;
            // 
            // satelliteViewer1
            // 
            this.satelliteViewer1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.satelliteViewer1.Bearing = new DotSpatial.Positioning.Azimuth(0D);
            this.satelliteViewer1.CenterR = 0F;
            this.satelliteViewer1.DirectionLabelInterval = new DotSpatial.Positioning.Angle(45D);
            this.satelliteViewer1.FixColor = System.Drawing.Color.LightGreen;
            this.satelliteViewer1.Height = 102;
            this.satelliteViewer1.IsPaintingOnSeparateThread = true;
            this.satelliteViewer1.Location = new System.Drawing.Point(126, 99);
            this.satelliteViewer1.MajorTickInterval = new DotSpatial.Positioning.Angle(15D);
            this.satelliteViewer1.Margin = new System.Windows.Forms.Padding(8, 8, 8, 3);
            this.satelliteViewer1.MaximumR = 90F;
            this.satelliteViewer1.MinorTickInterval = new DotSpatial.Positioning.Angle(2D);
            this.satelliteViewer1.Name = "satelliteViewer1";
            this.satelliteViewer1.Size = new System.Drawing.Size(102, 102);
            this.satelliteViewer1.TabIndex = 23;
            this.satelliteViewer1.Text = "satelliteViewer1";
            this.satelliteViewer1.Width = 102;
            // 
            // altimeter1
            // 
            this.altimeter1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.altimeter1.CenterR = 0F;
            this.altimeter1.Height = 102;
            this.altimeter1.IsPaintingOnSeparateThread = true;
            this.altimeter1.Location = new System.Drawing.Point(244, 99);
            this.altimeter1.Margin = new System.Windows.Forms.Padding(8, 8, 8, 3);
            this.altimeter1.MaximumR = 100F;
            this.altimeter1.Name = "altimeter1";
            this.altimeter1.Origin = new DotSpatial.Positioning.Azimuth(0D);
            this.altimeter1.Rotation = new DotSpatial.Positioning.Angle(0D);
            this.altimeter1.Size = new System.Drawing.Size(102, 102);
            this.altimeter1.TabIndex = 24;
            this.altimeter1.Text = "altimeter1";
            this.altimeter1.Width = 102;
            // 
            // speedometer1
            // 
            this.speedometer1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.speedometer1.CenterR = 0F;
            this.speedometer1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.speedometer1.Height = 102;
            this.speedometer1.IsPaintingOnSeparateThread = true;
            this.speedometer1.Location = new System.Drawing.Point(362, 99);
            this.speedometer1.Margin = new System.Windows.Forms.Padding(8, 8, 8, 3);
            this.speedometer1.MaximumAngle = new DotSpatial.Positioning.Angle(320D);
            this.speedometer1.MaximumR = 100F;
            this.speedometer1.MinimumAngle = new DotSpatial.Positioning.Angle(40D);
            this.speedometer1.Name = "speedometer1";
            this.speedometer1.Size = new System.Drawing.Size(102, 102);
            this.speedometer1.TabIndex = 25;
            this.speedometer1.Text = "speedometer1";
            this.speedometer1.Width = 102;
            // 
            // compass1
            // 
            this.compass1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.compass1.AngleLabelInterval = new DotSpatial.Positioning.Angle(30D);
            this.compass1.CenterR = 0F;
            this.compass1.DirectionLabelInterval = new DotSpatial.Positioning.Angle(45D);
            this.compass1.Height = 102;
            this.compass1.IsPaintingOnSeparateThread = true;
            this.compass1.Location = new System.Drawing.Point(480, 99);
            this.compass1.MajorTickInterval = new DotSpatial.Positioning.Angle(15D);
            this.compass1.Margin = new System.Windows.Forms.Padding(8, 8, 8, 3);
            this.compass1.MaximumR = 100F;
            this.compass1.MinorTickInterval = new DotSpatial.Positioning.Angle(2D);
            this.compass1.Name = "compass1";
            this.compass1.Origin = new DotSpatial.Positioning.Azimuth(0D);
            this.compass1.Rotation = new DotSpatial.Positioning.Angle(0D);
            this.compass1.Size = new System.Drawing.Size(106, 102);
            this.compass1.TabIndex = 26;
            this.compass1.Text = "compass1";
            this.compass1.Value = new DotSpatial.Positioning.Azimuth(0D);
            this.compass1.Width = 106;
            // 
            // GpsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "GpsPanel";
            this.Size = new System.Drawing.Size(594, 272);
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
        private Positioning.Forms.Clock clock1;
        private Positioning.Forms.SatelliteViewer satelliteViewer1;
        private Positioning.Forms.Altimeter altimeter1;
        private Positioning.Forms.Speedometer speedometer1;
        private Positioning.Forms.Compass compass1;
    }
}
