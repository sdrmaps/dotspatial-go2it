namespace DotSpatial.SDR.Plugins.Search
{
    partial class SearchPanel
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
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.searchAdds = new System.Windows.Forms.ToolStripButton();
            this.searchName = new System.Windows.Forms.ToolStripButton();
            this.searchPhone = new System.Windows.Forms.ToolStripButton();
            this.searchHydrant = new System.Windows.Forms.ToolStripButton();
            this.searchClear = new System.Windows.Forms.ToolStripButton();
            this.searchRoad = new System.Windows.Forms.ToolStripButton();
            this.searchIntersection = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.searchDGV = new System.Windows.Forms.DataGridView();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchDGV)).BeginInit();
            this.SuspendLayout();
            // 
            // txtSearch
            // 
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Location = new System.Drawing.Point(3, 3);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(630, 20);
            this.txtSearch.TabIndex = 0;
            // 
            // btnSearch
            // 
            this.btnSearch.AutoSize = true;
            this.btnSearch.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSearch.Location = new System.Drawing.Point(639, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchAdds,
            this.searchName,
            this.searchPhone,
            this.searchHydrant,
            this.searchClear,
            this.searchRoad,
            this.searchIntersection});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.MinimumSize = new System.Drawing.Size(0, 45);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(717, 45);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // searchAdds
            // 
            this.searchAdds.Checked = true;
            this.searchAdds.CheckOnClick = true;
            this.searchAdds.CheckState = System.Windows.Forms.CheckState.Checked;
            this.searchAdds.Image = global::DotSpatial.SDR.Plugins.Search.Properties.Resources.info_rhombus_32x32;
            this.searchAdds.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchAdds.Name = "searchAdds";
            this.searchAdds.Size = new System.Drawing.Size(64, 42);
            this.searchAdds.Text = "Addresses";
            this.searchAdds.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.searchAdds.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.searchAdds.Click += new System.EventHandler(this.searchAdds_Click);
            // 
            // searchName
            // 
            this.searchName.CheckOnClick = true;
            this.searchName.Image = global::DotSpatial.SDR.Plugins.Search.Properties.Resources.info_rhombus_32x32;
            this.searchName.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchName.Name = "searchName";
            this.searchName.Size = new System.Drawing.Size(43, 42);
            this.searchName.Text = "Name";
            this.searchName.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.searchName.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.searchName.Click += new System.EventHandler(this.searchName_Click);
            // 
            // searchPhone
            // 
            this.searchPhone.CheckOnClick = true;
            this.searchPhone.Image = global::DotSpatial.SDR.Plugins.Search.Properties.Resources.info_rhombus_32x32;
            this.searchPhone.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchPhone.Name = "searchPhone";
            this.searchPhone.Size = new System.Drawing.Size(45, 42);
            this.searchPhone.Text = "Phone";
            this.searchPhone.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.searchPhone.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.searchPhone.Click += new System.EventHandler(this.searchPhone_Click);
            // 
            // searchHydrant
            // 
            this.searchHydrant.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.searchHydrant.Image = global::DotSpatial.SDR.Plugins.Search.Properties.Resources.info_rhombus_32x32;
            this.searchHydrant.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchHydrant.Name = "searchHydrant";
            this.searchHydrant.Size = new System.Drawing.Size(54, 42);
            this.searchHydrant.Text = "Hydrant";
            this.searchHydrant.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.searchHydrant.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.searchHydrant.Click += new System.EventHandler(this.searchHydrant_Click);
            // 
            // searchClear
            // 
            this.searchClear.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.searchClear.Image = global::DotSpatial.SDR.Plugins.Search.Properties.Resources.info_rhombus_32x32;
            this.searchClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchClear.Name = "searchClear";
            this.searchClear.Size = new System.Drawing.Size(38, 42);
            this.searchClear.Text = "Clear";
            this.searchClear.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.searchClear.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.searchClear.Click += new System.EventHandler(this.searchClear_Click);
            // 
            // searchRoad
            // 
            this.searchRoad.CheckOnClick = true;
            this.searchRoad.Image = global::DotSpatial.SDR.Plugins.Search.Properties.Resources.info_rhombus_32x32;
            this.searchRoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchRoad.Name = "searchRoad";
            this.searchRoad.Size = new System.Drawing.Size(38, 42);
            this.searchRoad.Text = "Road";
            this.searchRoad.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.searchRoad.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.searchRoad.Click += new System.EventHandler(this.searchRoad_Click);
            // 
            // searchIntersection
            // 
            this.searchIntersection.Image = global::DotSpatial.SDR.Plugins.Search.Properties.Resources.info_rhombus_32x32;
            this.searchIntersection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchIntersection.Name = "searchIntersection";
            this.searchIntersection.Size = new System.Drawing.Size(73, 42);
            this.searchIntersection.Text = "Intersection";
            this.searchIntersection.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.searchIntersection.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.searchIntersection.Click += new System.EventHandler(this.searchIntersection_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.btnSearch, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtSearch, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.searchDGV, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 45);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(717, 173);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // searchDGV
            // 
            this.searchDGV.AllowUserToAddRows = false;
            this.searchDGV.AllowUserToDeleteRows = false;
            this.searchDGV.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.searchDGV.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.searchDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableLayoutPanel1.SetColumnSpan(this.searchDGV, 2);
            this.searchDGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchDGV.Location = new System.Drawing.Point(3, 32);
            this.searchDGV.MultiSelect = false;
            this.searchDGV.Name = "searchDGV";
            this.searchDGV.ReadOnly = true;
            this.searchDGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.searchDGV.Size = new System.Drawing.Size(711, 138);
            this.searchDGV.TabIndex = 2;
            // 
            // SearchPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "SearchPanel";
            this.Size = new System.Drawing.Size(717, 218);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchDGV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton searchAdds;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView searchDGV;
        private System.Windows.Forms.ToolStripButton searchName;
        private System.Windows.Forms.ToolStripButton searchPhone;
        private System.Windows.Forms.ToolStripButton searchHydrant;
        private System.Windows.Forms.ToolStripButton searchClear;
        private System.Windows.Forms.ToolStripButton searchRoad;
        private System.Windows.Forms.ToolStripButton searchIntersection;

    }
}
