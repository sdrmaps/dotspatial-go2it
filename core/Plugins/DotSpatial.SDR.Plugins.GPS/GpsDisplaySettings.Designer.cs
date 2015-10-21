namespace DotSpatial.SDR.Plugins.GPS
{
    partial class GpsDisplaySettings
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
            this.gpsSymbolColorSlider = new DotSpatial.Symbology.Forms.RampSlider();
            this.gpsSymbolSize = new System.Windows.Forms.NumericUpDown();
            this.gpsSymbolColor = new System.Windows.Forms.Panel();
            this.gpsSymbolStyle = new System.Windows.Forms.ComboBox();
            this.gpsSymbolGraphic = new System.Windows.Forms.Panel();
            this.gpsIntervalModeCount = new System.Windows.Forms.RadioButton();
            this.gpsIntervalModeTime = new System.Windows.Forms.RadioButton();
            this.numGpsIntervalValue = new System.Windows.Forms.NumericUpDown();
            this.numGpsDisplayCount = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gpsSymbolSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGpsIntervalValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGpsDisplayCount)).BeginInit();
            this.SuspendLayout();
            // 
            // gpsSymbolColorSlider
            // 
            this.gpsSymbolColorSlider.BackColor = System.Drawing.SystemColors.Control;
            this.gpsSymbolColorSlider.ColorButton = null;
            this.gpsSymbolColorSlider.FlipRamp = false;
            this.gpsSymbolColorSlider.FlipText = false;
            this.gpsSymbolColorSlider.InvertRamp = false;
            this.gpsSymbolColorSlider.Location = new System.Drawing.Point(115, 77);
            this.gpsSymbolColorSlider.Maximum = 1D;
            this.gpsSymbolColorSlider.MaximumColor = System.Drawing.Color.Green;
            this.gpsSymbolColorSlider.Minimum = 0D;
            this.gpsSymbolColorSlider.MinimumColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.gpsSymbolColorSlider.Name = "gpsSymbolColorSlider";
            this.gpsSymbolColorSlider.NumberFormat = "#%";
            this.gpsSymbolColorSlider.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.gpsSymbolColorSlider.RampRadius = 10F;
            this.gpsSymbolColorSlider.RampText = null;
            this.gpsSymbolColorSlider.RampTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.gpsSymbolColorSlider.RampTextBehindRamp = false;
            this.gpsSymbolColorSlider.RampTextColor = System.Drawing.Color.Black;
            this.gpsSymbolColorSlider.RampTextFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpsSymbolColorSlider.ShowMaximum = false;
            this.gpsSymbolColorSlider.ShowMinimum = false;
            this.gpsSymbolColorSlider.ShowTicks = true;
            this.gpsSymbolColorSlider.ShowValue = true;
            this.gpsSymbolColorSlider.Size = new System.Drawing.Size(122, 23);
            this.gpsSymbolColorSlider.SliderColor = System.Drawing.Color.Blue;
            this.gpsSymbolColorSlider.SliderRadius = 4F;
            this.gpsSymbolColorSlider.TabIndex = 52;
            this.gpsSymbolColorSlider.Text = "rampSlider1";
            this.gpsSymbolColorSlider.TickColor = System.Drawing.Color.DarkGray;
            this.gpsSymbolColorSlider.TickSpacing = 5F;
            this.gpsSymbolColorSlider.Value = 1D;
            // 
            // gpsSymbolSize
            // 
            this.gpsSymbolSize.Location = new System.Drawing.Point(457, 80);
            this.gpsSymbolSize.Name = "gpsSymbolSize";
            this.gpsSymbolSize.Size = new System.Drawing.Size(72, 20);
            this.gpsSymbolSize.TabIndex = 51;
            // 
            // gpsSymbolColor
            // 
            this.gpsSymbolColor.Location = new System.Drawing.Point(74, 76);
            this.gpsSymbolColor.Name = "gpsSymbolColor";
            this.gpsSymbolColor.Size = new System.Drawing.Size(22, 24);
            this.gpsSymbolColor.TabIndex = 50;
            // 
            // gpsSymbolStyle
            // 
            this.gpsSymbolStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.gpsSymbolStyle.FormattingEnabled = true;
            this.gpsSymbolStyle.Location = new System.Drawing.Point(246, 79);
            this.gpsSymbolStyle.Name = "gpsSymbolStyle";
            this.gpsSymbolStyle.Size = new System.Drawing.Size(205, 21);
            this.gpsSymbolStyle.TabIndex = 49;
            // 
            // gpsSymbolGraphic
            // 
            this.gpsSymbolGraphic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gpsSymbolGraphic.Location = new System.Drawing.Point(15, 55);
            this.gpsSymbolGraphic.Margin = new System.Windows.Forms.Padding(0);
            this.gpsSymbolGraphic.Name = "gpsSymbolGraphic";
            this.gpsSymbolGraphic.Size = new System.Drawing.Size(45, 45);
            this.gpsSymbolGraphic.TabIndex = 48;
            // 
            // gpsIntervalModeCount
            // 
            this.gpsIntervalModeCount.AutoSize = true;
            this.gpsIntervalModeCount.Location = new System.Drawing.Point(246, 31);
            this.gpsIntervalModeCount.Name = "gpsIntervalModeCount";
            this.gpsIntervalModeCount.Size = new System.Drawing.Size(53, 17);
            this.gpsIntervalModeCount.TabIndex = 53;
            this.gpsIntervalModeCount.TabStop = true;
            this.gpsIntervalModeCount.Text = "Count";
            this.gpsIntervalModeCount.UseVisualStyleBackColor = true;
            // 
            // gpsIntervalModeTime
            // 
            this.gpsIntervalModeTime.AutoSize = true;
            this.gpsIntervalModeTime.Location = new System.Drawing.Point(192, 31);
            this.gpsIntervalModeTime.Name = "gpsIntervalModeTime";
            this.gpsIntervalModeTime.Size = new System.Drawing.Size(48, 17);
            this.gpsIntervalModeTime.TabIndex = 54;
            this.gpsIntervalModeTime.TabStop = true;
            this.gpsIntervalModeTime.Text = "Time";
            this.gpsIntervalModeTime.UseVisualStyleBackColor = true;
            // 
            // numGpsIntervalValue
            // 
            this.numGpsIntervalValue.Location = new System.Drawing.Point(410, 15);
            this.numGpsIntervalValue.Name = "numGpsIntervalValue";
            this.numGpsIntervalValue.Size = new System.Drawing.Size(72, 20);
            this.numGpsIntervalValue.TabIndex = 55;
            // 
            // numGpsDisplayCount
            // 
            this.numGpsDisplayCount.Location = new System.Drawing.Point(115, 13);
            this.numGpsDisplayCount.Name = "numGpsDisplayCount";
            this.numGpsDisplayCount.Size = new System.Drawing.Size(56, 20);
            this.numGpsDisplayCount.TabIndex = 56;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 57;
            this.label1.Text = "Point Display Count";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(189, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 58;
            this.label2.Text = "GPS Interval Mode";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(307, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 59;
            this.label3.Text = "GPS Interval Value";
            // 
            // GpsDisplaySettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 124);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numGpsDisplayCount);
            this.Controls.Add(this.numGpsIntervalValue);
            this.Controls.Add(this.gpsIntervalModeTime);
            this.Controls.Add(this.gpsIntervalModeCount);
            this.Controls.Add(this.gpsSymbolColorSlider);
            this.Controls.Add(this.gpsSymbolSize);
            this.Controls.Add(this.gpsSymbolColor);
            this.Controls.Add(this.gpsSymbolStyle);
            this.Controls.Add(this.gpsSymbolGraphic);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimizeBox = false;
            this.Name = "GpsDisplaySettings";
            this.Text = "GpsDisplaySettings";
            ((System.ComponentModel.ISupportInitialize)(this.gpsSymbolSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGpsIntervalValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGpsDisplayCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DotSpatial.Symbology.Forms.RampSlider gpsSymbolColorSlider;
        private System.Windows.Forms.NumericUpDown gpsSymbolSize;
        private System.Windows.Forms.Panel gpsSymbolColor;
        private System.Windows.Forms.ComboBox gpsSymbolStyle;
        private System.Windows.Forms.Panel gpsSymbolGraphic;
        private System.Windows.Forms.RadioButton gpsIntervalModeCount;
        private System.Windows.Forms.RadioButton gpsIntervalModeTime;
        private System.Windows.Forms.NumericUpDown numGpsIntervalValue;
        private System.Windows.Forms.NumericUpDown numGpsDisplayCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}