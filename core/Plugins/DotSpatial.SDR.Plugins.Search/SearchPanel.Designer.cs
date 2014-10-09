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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnSearch = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.searchAdds = new System.Windows.Forms.ToolStripButton();
            this.searchName = new System.Windows.Forms.ToolStripButton();
            this.searchPhone = new System.Windows.Forms.ToolStripButton();
            this.searchHydrant = new System.Windows.Forms.ToolStripButton();
            this.searchClear = new System.Windows.Forms.ToolStripButton();
            this.searchRoad = new System.Windows.Forms.ToolStripButton();
            this.searchIntersection = new System.Windows.Forms.ToolStripButton();
            this.searchKeyLocations = new System.Windows.Forms.ToolStripButton();
            this.searchAll = new System.Windows.Forms.ToolStripButton();
            this.searchLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.searchDGV = new System.Windows.Forms.DataGridView();
            this.searchCity = new System.Windows.Forms.ToolStripButton();
            this.searchEsn = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.searchLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchDGV)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSearch
            // 
            this.btnSearch.AutoSize = true;
            this.btnSearch.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSearch.Location = new System.Drawing.Point(639, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
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
            this.searchIntersection,
            this.searchCity,
            this.searchEsn,
            this.searchKeyLocations,
            this.searchAll});
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
            this.searchAdds.Image = global::DotSpatial.SDR.Plugins.Search.Properties.Resources.info_32;
            this.searchAdds.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchAdds.Name = "searchAdds";
            this.searchAdds.Size = new System.Drawing.Size(53, 42);
            this.searchAdds.Text = "Address";
            this.searchAdds.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.searchAdds.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.searchAdds.Click += new System.EventHandler(this.searchAdds_Click);
            // 
            // searchName
            // 
            this.searchName.CheckOnClick = true;
            this.searchName.Image = global::DotSpatial.SDR.Plugins.Search.Properties.Resources.info_32;
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
            this.searchPhone.Image = global::DotSpatial.SDR.Plugins.Search.Properties.Resources.info_32;
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
            this.searchHydrant.Image = global::DotSpatial.SDR.Plugins.Search.Properties.Resources.info_32;
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
            this.searchClear.Image = global::DotSpatial.SDR.Plugins.Search.Properties.Resources.info_32;
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
            this.searchRoad.Image = global::DotSpatial.SDR.Plugins.Search.Properties.Resources.info_32;
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
            this.searchIntersection.Image = global::DotSpatial.SDR.Plugins.Search.Properties.Resources.info_32;
            this.searchIntersection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchIntersection.Name = "searchIntersection";
            this.searchIntersection.Size = new System.Drawing.Size(73, 42);
            this.searchIntersection.Text = "Intersection";
            this.searchIntersection.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.searchIntersection.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.searchIntersection.Click += new System.EventHandler(this.searchIntersection_Click);
            // 
            // searchKeyLocations
            // 
            this.searchKeyLocations.Image = global::DotSpatial.SDR.Plugins.Search.Properties.Resources.info_16;
            this.searchKeyLocations.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchKeyLocations.Name = "searchKeyLocations";
            this.searchKeyLocations.Size = new System.Drawing.Size(84, 42);
            this.searchKeyLocations.Text = "Key Locations";
            this.searchKeyLocations.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.searchKeyLocations.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.searchKeyLocations.Click += new System.EventHandler(this.searchKeyLocations_Click);
            // 
            // searchAll
            // 
            this.searchAll.Image = global::DotSpatial.SDR.Plugins.Search.Properties.Resources.info_16;
            this.searchAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchAll.Name = "searchAll";
            this.searchAll.Size = new System.Drawing.Size(58, 42);
            this.searchAll.Text = "All Fields";
            this.searchAll.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.searchAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.searchAll.Click += new System.EventHandler(this.searchAll_Click);
            // 
            // searchLayoutPanel
            // 
            this.searchLayoutPanel.ColumnCount = 2;
            this.searchLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.searchLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.searchLayoutPanel.Controls.Add(this.btnSearch, 1, 0);
            this.searchLayoutPanel.Controls.Add(this.searchDGV, 0, 1);
            this.searchLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchLayoutPanel.Location = new System.Drawing.Point(0, 45);
            this.searchLayoutPanel.Name = "searchLayoutPanel";
            this.searchLayoutPanel.RowCount = 2;
            this.searchLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.searchLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.searchLayoutPanel.Size = new System.Drawing.Size(717, 173);
            this.searchLayoutPanel.TabIndex = 3;
            // 
            // searchDGV
            // 
            this.searchDGV.AllowUserToAddRows = false;
            this.searchDGV.AllowUserToDeleteRows = false;
            this.searchDGV.AllowUserToOrderColumns = true;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Gainsboro;
            this.searchDGV.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.searchDGV.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.searchDGV.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.searchDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.searchLayoutPanel.SetColumnSpan(this.searchDGV, 2);
            this.searchDGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchDGV.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.searchDGV.Location = new System.Drawing.Point(3, 37);
            this.searchDGV.MultiSelect = false;
            this.searchDGV.Name = "searchDGV";
            this.searchDGV.ReadOnly = true;
            this.searchDGV.RowHeadersVisible = false;
            this.searchDGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.searchDGV.Size = new System.Drawing.Size(711, 133);
            this.searchDGV.TabIndex = 2;
            this.searchDGV.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.searchDGV_CellDoubleClick);
            // 
            // searchCity
            // 
            this.searchCity.Image = global::DotSpatial.SDR.Plugins.Search.Properties.Resources.info_16;
            this.searchCity.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchCity.Name = "searchCity";
            this.searchCity.Size = new System.Drawing.Size(32, 42);
            this.searchCity.Text = "City";
            this.searchCity.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // searchEsn
            // 
            this.searchEsn.Image = global::DotSpatial.SDR.Plugins.Search.Properties.Resources.info_16;
            this.searchEsn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchEsn.Name = "searchEsn";
            this.searchEsn.Size = new System.Drawing.Size(32, 42);
            this.searchEsn.Text = "ESN";
            this.searchEsn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // SearchPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.searchLayoutPanel);
            this.Controls.Add(this.toolStrip1);
            this.Name = "SearchPanel";
            this.Size = new System.Drawing.Size(717, 218);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.searchLayoutPanel.ResumeLayout(false);
            this.searchLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchDGV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton searchAdds;
        private System.Windows.Forms.TableLayoutPanel searchLayoutPanel;
        private System.Windows.Forms.ToolStripButton searchName;
        private System.Windows.Forms.ToolStripButton searchPhone;
        private System.Windows.Forms.ToolStripButton searchHydrant;
        private System.Windows.Forms.ToolStripButton searchClear;
        private System.Windows.Forms.ToolStripButton searchRoad;
        private System.Windows.Forms.ToolStripButton searchIntersection;
        private System.Windows.Forms.DataGridView searchDGV;
        private System.Windows.Forms.ToolStripButton searchKeyLocations;
        private System.Windows.Forms.ToolStripButton searchAll;
        private System.Windows.Forms.ToolStripButton searchCity;
        private System.Windows.Forms.ToolStripButton searchEsn;

    }
}
