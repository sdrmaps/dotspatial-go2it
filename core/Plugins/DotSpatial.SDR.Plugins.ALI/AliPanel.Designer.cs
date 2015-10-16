namespace DotSpatial.SDR.Plugins.ALI
{
    partial class AliPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AliPanel));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbAliLocate = new System.Windows.Forms.ToolStripButton();
            this.tsbAliUpdate = new System.Windows.Forms.ToolStripButton();
            this.aliTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.aliDGV = new System.Windows.Forms.DataGridView();
            this.cmbAliCommLog = new System.Windows.Forms.ComboBox();
            this.lblCommLog = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.aliTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aliDGV)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbAliLocate,
            this.tsbAliUpdate});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(847, 48);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbAliLocate
            // 
            this.tsbAliLocate.AutoSize = false;
            this.tsbAliLocate.Image = ((System.Drawing.Image)(resources.GetObject("tsbAliLocate.Image")));
            this.tsbAliLocate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAliLocate.Name = "tsbAliLocate";
            this.tsbAliLocate.Size = new System.Drawing.Size(45, 45);
            this.tsbAliLocate.Text = "Locate";
            this.tsbAliLocate.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.tsbAliLocate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbAliLocate.ToolTipText = "Locate Selected Record";
            // 
            // tsbAliUpdate
            // 
            this.tsbAliUpdate.AutoSize = false;
            this.tsbAliUpdate.Image = ((System.Drawing.Image)(resources.GetObject("tsbAliUpdate.Image")));
            this.tsbAliUpdate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAliUpdate.Name = "tsbAliUpdate";
            this.tsbAliUpdate.Size = new System.Drawing.Size(45, 45);
            this.tsbAliUpdate.Text = "Update";
            this.tsbAliUpdate.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.tsbAliUpdate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbAliUpdate.ToolTipText = "Update Record Display";
            // 
            // aliTableLayoutPanel
            // 
            this.aliTableLayoutPanel.ColumnCount = 2;
            this.aliTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75.32468F));
            this.aliTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.67533F));
            this.aliTableLayoutPanel.Controls.Add(this.aliDGV, 0, 0);
            this.aliTableLayoutPanel.Controls.Add(this.cmbAliCommLog, 1, 1);
            this.aliTableLayoutPanel.Controls.Add(this.lblCommLog, 1, 0);
            this.aliTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aliTableLayoutPanel.Location = new System.Drawing.Point(0, 48);
            this.aliTableLayoutPanel.Name = "aliTableLayoutPanel";
            this.aliTableLayoutPanel.RowCount = 2;
            this.aliTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.aliTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.aliTableLayoutPanel.Size = new System.Drawing.Size(847, 134);
            this.aliTableLayoutPanel.TabIndex = 1;
            // 
            // aliDGV
            // 
            this.aliDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.aliDGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aliDGV.Location = new System.Drawing.Point(3, 3);
            this.aliDGV.Name = "aliDGV";
            this.aliTableLayoutPanel.SetRowSpan(this.aliDGV, 2);
            this.aliDGV.Size = new System.Drawing.Size(631, 128);
            this.aliDGV.TabIndex = 0;
            // 
            // cmbAliCommLog
            // 
            this.cmbAliCommLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbAliCommLog.FormattingEnabled = true;
            this.cmbAliCommLog.Location = new System.Drawing.Point(640, 16);
            this.cmbAliCommLog.Name = "cmbAliCommLog";
            this.cmbAliCommLog.Size = new System.Drawing.Size(204, 21);
            this.cmbAliCommLog.TabIndex = 1;
            // 
            // lblCommLog
            // 
            this.lblCommLog.AutoSize = true;
            this.lblCommLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblCommLog.Location = new System.Drawing.Point(640, 0);
            this.lblCommLog.Name = "lblCommLog";
            this.lblCommLog.Size = new System.Drawing.Size(204, 13);
            this.lblCommLog.TabIndex = 2;
            this.lblCommLog.Text = "Active Comm Log";
            // 
            // AliPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.aliTableLayoutPanel);
            this.Controls.Add(this.toolStrip1);
            this.Name = "AliPanel";
            this.Size = new System.Drawing.Size(847, 182);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.aliTableLayoutPanel.ResumeLayout(false);
            this.aliTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aliDGV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbAliLocate;
        private System.Windows.Forms.ToolStripButton tsbAliUpdate;
        private System.Windows.Forms.TableLayoutPanel aliTableLayoutPanel;
        private System.Windows.Forms.DataGridView aliDGV;
        private System.Windows.Forms.ComboBox cmbAliCommLog;
        private System.Windows.Forms.Label lblCommLog;
    }
}
