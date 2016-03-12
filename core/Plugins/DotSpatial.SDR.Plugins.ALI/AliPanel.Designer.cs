﻿namespace DotSpatial.SDR.Plugins.ALI
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
            this.chkFleetList = new System.Windows.Forms.CheckedListBox();
            this.lblFleetList = new System.Windows.Forms.Label();
            this.lblMyUnit = new System.Windows.Forms.Label();
            this.cmbAliMyUnit = new System.Windows.Forms.ComboBox();
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
            this.tsbAliLocate.Click += new System.EventHandler(this.tsbAliLocate_Click);
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
            this.lblCommLog.Size = new System.Drawing.Size(90, 20);
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
            this.aliTableLayoutPanel.SetRowSpan(this.aliDGV, 4);
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
            this.aliTableLayoutPanel.Controls.Add(this.chkFleetList, 1, 1);
            this.aliTableLayoutPanel.Controls.Add(this.lblCommLog, 2, 0);
            this.aliTableLayoutPanel.Controls.Add(this.lblFleetList, 1, 0);
            this.aliTableLayoutPanel.Controls.Add(this.lblMyUnit, 2, 2);
            this.aliTableLayoutPanel.Controls.Add(this.cmbAliMyUnit, 2, 3);
            this.aliTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aliTableLayoutPanel.Location = new System.Drawing.Point(0, 48);
            this.aliTableLayoutPanel.Name = "aliTableLayoutPanel";
            this.aliTableLayoutPanel.RowCount = 4;
            this.aliTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.aliTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.aliTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.aliTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.aliTableLayoutPanel.Size = new System.Drawing.Size(790, 134);
            this.aliTableLayoutPanel.TabIndex = 1;
            // 
            // cmbAliCommLog
            // 
            this.cmbAliCommLog.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbAliCommLog.FormattingEnabled = true;
            this.cmbAliCommLog.Location = new System.Drawing.Point(595, 23);
            this.cmbAliCommLog.MaxDropDownItems = 35;
            this.cmbAliCommLog.Name = "cmbAliCommLog";
            this.cmbAliCommLog.Size = new System.Drawing.Size(192, 21);
            this.cmbAliCommLog.TabIndex = 5;
            // 
            // chkFleetList
            // 
            this.chkFleetList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkFleetList.FormattingEnabled = true;
            this.chkFleetList.Location = new System.Drawing.Point(398, 23);
            this.chkFleetList.Name = "chkFleetList";
            this.aliTableLayoutPanel.SetRowSpan(this.chkFleetList, 3);
            this.chkFleetList.Size = new System.Drawing.Size(191, 108);
            this.chkFleetList.TabIndex = 4;
            this.chkFleetList.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.chkFleetList_ControlAdded);
            // 
            // lblFleetList
            // 
            this.lblFleetList.AutoSize = true;
            this.lblFleetList.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblFleetList.Location = new System.Drawing.Point(398, 0);
            this.lblFleetList.Name = "lblFleetList";
            this.lblFleetList.Size = new System.Drawing.Size(110, 20);
            this.lblFleetList.TabIndex = 3;
            this.lblFleetList.Text = "Networkfleet Vehicles";
            // 
            // lblMyUnit
            // 
            this.lblMyUnit.AutoSize = true;
            this.lblMyUnit.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblMyUnit.Location = new System.Drawing.Point(595, 47);
            this.lblMyUnit.Name = "lblMyUnit";
            this.lblMyUnit.Size = new System.Drawing.Size(43, 20);
            this.lblMyUnit.TabIndex = 6;
            this.lblMyUnit.Text = "My Unit";
            // 
            // cmbAliMyUnit
            // 
            this.cmbAliMyUnit.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbAliMyUnit.FormattingEnabled = true;
            this.cmbAliMyUnit.Location = new System.Drawing.Point(595, 70);
            this.cmbAliMyUnit.Name = "cmbAliMyUnit";
            this.cmbAliMyUnit.Size = new System.Drawing.Size(192, 21);
            this.cmbAliMyUnit.TabIndex = 7;
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
        private System.Windows.Forms.Label lblFleetList;
        private System.Windows.Forms.CheckedListBox chkFleetList;
        private System.Windows.Forms.ComboBox cmbAliCommLog;
        private System.Windows.Forms.Label lblMyUnit;
        private System.Windows.Forms.ComboBox cmbAliMyUnit;
    }
}
