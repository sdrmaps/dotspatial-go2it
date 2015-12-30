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
            this.lblCommLog = new System.Windows.Forms.Label();
            this.aliDGV = new System.Windows.Forms.DataGridView();
            this.aliTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.cmbAliCommLog = new System.Windows.Forms.ComboBox();
            this.chkNetworkfleetVehicles = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aliDGV)).BeginInit();
            this.aliTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbAliLocate,
            this.tsbAliUpdate});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(790, 48);
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
            this.tsbAliUpdate.Image = global::DotSpatial.SDR.Plugins.ALI.Properties.Resources.recent_32;
            this.tsbAliUpdate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAliUpdate.Name = "tsbAliUpdate";
            this.tsbAliUpdate.Size = new System.Drawing.Size(45, 45);
            this.tsbAliUpdate.Text = "Update";
            this.tsbAliUpdate.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.tsbAliUpdate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbAliUpdate.ToolTipText = "Update Record Display";
            this.tsbAliUpdate.Click += new System.EventHandler(this.tsbAliUpdate_Click);
            // 
            // lblCommLog
            // 
            this.lblCommLog.AutoSize = true;
            this.lblCommLog.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblCommLog.Location = new System.Drawing.Point(595, 0);
            this.lblCommLog.Name = "lblCommLog";
            this.lblCommLog.Size = new System.Drawing.Size(90, 13);
            this.lblCommLog.TabIndex = 2;
            this.lblCommLog.Text = "Active Comm Log";
            // 
            // aliDGV
            // 
            this.aliDGV.AllowUserToAddRows = false;
            this.aliDGV.AllowUserToDeleteRows = false;
            this.aliDGV.AllowUserToOrderColumns = true;
            this.aliDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.aliDGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aliDGV.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.aliDGV.Location = new System.Drawing.Point(3, 3);
            this.aliDGV.MultiSelect = false;
            this.aliDGV.Name = "aliDGV";
            this.aliDGV.ReadOnly = true;
            this.aliTableLayoutPanel.SetRowSpan(this.aliDGV, 2);
            this.aliDGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.aliDGV.Size = new System.Drawing.Size(389, 128);
            this.aliDGV.TabIndex = 0;
            // 
            // aliTableLayoutPanel
            // 
            this.aliTableLayoutPanel.ColumnCount = 3;
            this.aliTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.aliTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.aliTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.aliTableLayoutPanel.Controls.Add(this.aliDGV, 0, 0);
            this.aliTableLayoutPanel.Controls.Add(this.cmbAliCommLog, 2, 1);
            this.aliTableLayoutPanel.Controls.Add(this.chkNetworkfleetVehicles, 1, 1);
            this.aliTableLayoutPanel.Controls.Add(this.lblCommLog, 2, 0);
            this.aliTableLayoutPanel.Controls.Add(this.label1, 1, 0);
            this.aliTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aliTableLayoutPanel.Location = new System.Drawing.Point(0, 48);
            this.aliTableLayoutPanel.Name = "aliTableLayoutPanel";
            this.aliTableLayoutPanel.RowCount = 2;
            this.aliTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.aliTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.aliTableLayoutPanel.Size = new System.Drawing.Size(790, 134);
            this.aliTableLayoutPanel.TabIndex = 1;
            // 
            // cmbAliCommLog
            // 
            this.cmbAliCommLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAliCommLog.FormattingEnabled = true;
            this.cmbAliCommLog.Location = new System.Drawing.Point(595, 16);
            this.cmbAliCommLog.MaxDropDownItems = 35;
            this.cmbAliCommLog.Name = "cmbAliCommLog";
            this.cmbAliCommLog.Size = new System.Drawing.Size(192, 21);
            this.cmbAliCommLog.TabIndex = 5;
            // 
            // chkNetworkfleetVehicles
            // 
            this.chkNetworkfleetVehicles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkNetworkfleetVehicles.FormattingEnabled = true;
            this.chkNetworkfleetVehicles.Location = new System.Drawing.Point(398, 16);
            this.chkNetworkfleetVehicles.Name = "chkNetworkfleetVehicles";
            this.chkNetworkfleetVehicles.Size = new System.Drawing.Size(191, 115);
            this.chkNetworkfleetVehicles.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Location = new System.Drawing.Point(398, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Networkfleet Vehicles";
            // 
            // AliPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.aliTableLayoutPanel);
            this.Controls.Add(this.toolStrip1);
            this.Name = "AliPanel";
            this.Size = new System.Drawing.Size(790, 182);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aliDGV)).EndInit();
            this.aliTableLayoutPanel.ResumeLayout(false);
            this.aliTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbAliLocate;
        private System.Windows.Forms.ToolStripButton tsbAliUpdate;
        private System.Windows.Forms.Label lblCommLog;
        private System.Windows.Forms.DataGridView aliDGV;
        private System.Windows.Forms.TableLayoutPanel aliTableLayoutPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckedListBox chkNetworkfleetVehicles;
        private System.Windows.Forms.ComboBox cmbAliCommLog;
    }
}
