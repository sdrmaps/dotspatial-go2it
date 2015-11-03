namespace DotSpatial.SDR.Plugins.Measure
{
    sealed partial class MeasurePanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbUnits = new System.Windows.Forms.ComboBox();
            this.ttHelp = new System.Windows.Forms.ToolTip(this.components);
            this.lblMeasure = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbDistance = new System.Windows.Forms.ToolStripButton();
            this.tsbArea = new System.Windows.Forms.ToolStripButton();
            this.tsbClear = new System.Windows.Forms.ToolStripButton();
            this.lblPartialValue = new System.Windows.Forms.TextBox();
            this.lblTotalValue = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label4, 3);
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(58, 65);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(251, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Click to add a point. Right click when you are done.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(308, 10);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(224, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Units";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(58, 10);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Current:";
            // 
            // cmbUnits
            // 
            this.cmbUnits.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUnits.DropDownWidth = 100;
            this.cmbUnits.FormattingEnabled = true;
            this.cmbUnits.Location = new System.Drawing.Point(308, 31);
            this.cmbUnits.Margin = new System.Windows.Forms.Padding(3, 8, 50, 3);
            this.cmbUnits.Name = "cmbUnits";
            this.cmbUnits.Size = new System.Drawing.Size(177, 21);
            this.cmbUnits.TabIndex = 15;
            this.ttHelp.SetToolTip(this.cmbUnits, "Selecting a unit will also convert the measurements into the specified unit.");
            this.cmbUnits.SelectedIndexChanged += new System.EventHandler(this.cmbUnits_SelectedIndexChanged);
            // 
            // lblMeasure
            // 
            this.lblMeasure.AutoSize = true;
            this.lblMeasure.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMeasure.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblMeasure.Location = new System.Drawing.Point(3, 33);
            this.lblMeasure.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.lblMeasure.Name = "lblMeasure";
            this.lblMeasure.Size = new System.Drawing.Size(49, 13);
            this.lblMeasure.TabIndex = 11;
            this.lblMeasure.Text = "Distance";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(183, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Total:";
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbDistance,
            this.tsbArea,
            this.tsbClear});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(535, 48);
            this.toolStrip1.TabIndex = 10;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbDistance
            // 
            this.tsbDistance.AutoSize = false;
            this.tsbDistance.CheckOnClick = true;
            this.tsbDistance.Image = global::DotSpatial.SDR.Plugins.Measure.Properties.Resources.line_16;
            this.tsbDistance.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDistance.Name = "tsbDistance";
            this.tsbDistance.Size = new System.Drawing.Size(45, 45);
            this.tsbDistance.Text = "Dist.";
            this.tsbDistance.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.tsbDistance.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbDistance.ToolTipText = "Measure Distance";
            this.tsbDistance.Click += new System.EventHandler(this.DistanceButton_Click);
            // 
            // tsbArea
            // 
            this.tsbArea.AutoSize = false;
            this.tsbArea.CheckOnClick = true;
            this.tsbArea.Image = global::DotSpatial.SDR.Plugins.Measure.Properties.Resources.area_16;
            this.tsbArea.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbArea.Margin = new System.Windows.Forms.Padding(1, 1, 1, 2);
            this.tsbArea.Name = "tsbArea";
            this.tsbArea.Size = new System.Drawing.Size(45, 45);
            this.tsbArea.Text = "Area";
            this.tsbArea.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.tsbArea.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbArea.ToolTipText = "Measure Area";
            this.tsbArea.Click += new System.EventHandler(this.AreaButton_Click);
            // 
            // tsbClear
            // 
            this.tsbClear.AutoSize = false;
            this.tsbClear.Image = global::DotSpatial.SDR.Plugins.Measure.Properties.Resources.clear_16;
            this.tsbClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbClear.Name = "tsbClear";
            this.tsbClear.Size = new System.Drawing.Size(45, 45);
            this.tsbClear.Text = "Clear";
            this.tsbClear.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.tsbClear.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbClear.ToolTipText = "Clears the existing measurements";
            this.tsbClear.Click += new System.EventHandler(this.tsbClear_Click);
            // 
            // lblPartialValue
            // 
            this.lblPartialValue.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPartialValue.Location = new System.Drawing.Point(58, 31);
            this.lblPartialValue.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.lblPartialValue.Name = "lblPartialValue";
            this.lblPartialValue.ReadOnly = true;
            this.lblPartialValue.Size = new System.Drawing.Size(119, 20);
            this.lblPartialValue.TabIndex = 20;
            // 
            // lblTotalValue
            // 
            this.lblTotalValue.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTotalValue.Location = new System.Drawing.Point(183, 31);
            this.lblTotalValue.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.lblTotalValue.Name = "lblTotalValue";
            this.lblTotalValue.ReadOnly = true;
            this.lblTotalValue.Size = new System.Drawing.Size(119, 20);
            this.lblTotalValue.TabIndex = 21;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.lblMeasure, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.cmbUnits, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblPartialValue, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblTotalValue, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label4, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 48);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(535, 88);
            this.tableLayoutPanel1.TabIndex = 22;
            // 
            // MeasurePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MeasurePanel";
            this.Size = new System.Drawing.Size(535, 136);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbUnits;
        private System.Windows.Forms.ToolTip ttHelp;
        private System.Windows.Forms.ToolStripButton tsbArea;
        private System.Windows.Forms.ToolStripButton tsbClear;
        private System.Windows.Forms.ToolStripButton tsbDistance;
        private System.Windows.Forms.Label lblMeasure;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.TextBox lblPartialValue;
        private System.Windows.Forms.TextBox lblTotalValue;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;


    }
}
