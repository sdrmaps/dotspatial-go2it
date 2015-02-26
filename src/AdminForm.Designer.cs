﻿namespace Go2It
{
    partial class AdminForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.adminTab_Control = new System.Windows.Forms.TabControl();
            this.adminTab_LayerManagement = new System.Windows.Forms.TabPage();
            this.adminLayerSplitter = new System.Windows.Forms.SplitContainer();
            this.legendSplitter = new System.Windows.Forms.SplitContainer();
            this.legendButtonTable = new System.Windows.Forms.TableLayoutPanel();
            this.btnRemoveLayer = new System.Windows.Forms.Button();
            this.btnAddLayer = new System.Windows.Forms.Button();
            this.btnAddView = new System.Windows.Forms.Button();
            this.txtViewName = new System.Windows.Forms.TextBox();
            this.btnDeleteView = new System.Windows.Forms.Button();
            this.chkViewLayers = new System.Windows.Forms.CheckedListBox();
            this.cmbActiveMapTab = new System.Windows.Forms.ComboBox();
            this.lblMapBGColor = new System.Windows.Forms.Label();
            this.mapBGColorPanel = new System.Windows.Forms.Panel();
            this.panelRadKeyLocations = new System.Windows.Forms.Panel();
            this.radKeyLocationsPolygons = new System.Windows.Forms.RadioButton();
            this.radKeyLocationsPoints = new System.Windows.Forms.RadioButton();
            this.panelRadAddress = new System.Windows.Forms.Panel();
            this.radAddressPoints = new System.Windows.Forms.RadioButton();
            this.radAddressPolygons = new System.Windows.Forms.RadioButton();
            this.lblKeyLocations = new System.Windows.Forms.Label();
            this.lblHydrants = new System.Windows.Forms.Label();
            this.lblParcels = new System.Windows.Forms.Label();
            this.lblEsn = new System.Windows.Forms.Label();
            this.lblCellSector = new System.Windows.Forms.Label();
            this.cmbHydrantsLayer = new System.Windows.Forms.ComboBox();
            this.lblRoads = new System.Windows.Forms.Label();
            this.lblAddresses = new System.Windows.Forms.Label();
            this.chkKeyLocationsLayers = new System.Windows.Forms.CheckedListBox();
            this.cmbParcelsLayer = new System.Windows.Forms.ComboBox();
            this.cmbESNLayer = new System.Windows.Forms.ComboBox();
            this.cmbCellSectorLayer = new System.Windows.Forms.ComboBox();
            this.lblCityLimits = new System.Windows.Forms.Label();
            this.lblNotes = new System.Windows.Forms.Label();
            this.cmbCityLimitLayer = new System.Windows.Forms.ComboBox();
            this.chkRoadLayers = new System.Windows.Forms.CheckedListBox();
            this.chkAddressLayers = new System.Windows.Forms.CheckedListBox();
            this.cmbNotesLayer = new System.Windows.Forms.ComboBox();
            this.adminTab_ProgramManagement = new System.Windows.Forms.TabPage();
            this.btnSaveHotKeys = new System.Windows.Forms.Button();
            this.dgvHotKeys = new System.Windows.Forms.DataGridView();
            this.btnUsersDelete = new System.Windows.Forms.Button();
            this.btnUsersAddUpdate = new System.Windows.Forms.Button();
            this.lstUsers = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtVerifyPassword = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.adminTab_SearchProperties = new System.Windows.Forms.TabPage();
            this.chkEnableQuesryParserLog = new System.Windows.Forms.CheckBox();
            this.chkPretypes = new System.Windows.Forms.CheckBox();
            this.btnRemoveIndex = new System.Windows.Forms.Button();
            this.lstExistingIndexes = new System.Windows.Forms.ListBox();
            this.chkLayersToIndex = new System.Windows.Forms.CheckedListBox();
            this.btnAddIndex = new System.Windows.Forms.Button();
            this.dgvLayerIndex = new System.Windows.Forms.DataGridView();
            this.btnDeleteIndex = new System.Windows.Forms.Button();
            this.btnCreateIndex = new System.Windows.Forms.Button();
            this.cmbLayerIndex = new System.Windows.Forms.ComboBox();
            this.chkLayerIndex = new System.Windows.Forms.CheckedListBox();
            this.adminTab_SymbologySettings = new System.Windows.Forms.TabPage();
            this.ptSymbolColorSlider = new DotSpatial.Symbology.Forms.RampSlider();
            this.lineSymbolColorSlider = new DotSpatial.Symbology.Forms.RampSlider();
            this.lblHydrantDist = new System.Windows.Forms.Label();
            this.lblZoomFactor = new System.Windows.Forms.Label();
            this.lblBufDist = new System.Windows.Forms.Label();
            this.lblHydrantCount = new System.Windows.Forms.Label();
            this.searchHydrantDistance = new System.Windows.Forms.NumericUpDown();
            this.searchBufferDistance = new System.Windows.Forms.NumericUpDown();
            this.searchZoomFactor = new System.Windows.Forms.NumericUpDown();
            this.searchHydrantCount = new System.Windows.Forms.NumericUpDown();
            this.lineSymbolBorderColor = new System.Windows.Forms.Panel();
            this.lineSymbolCap = new System.Windows.Forms.ComboBox();
            this.lineSymbolSize = new System.Windows.Forms.NumericUpDown();
            this.lineSymbolStyle = new System.Windows.Forms.ComboBox();
            this.lineSymbolColor = new System.Windows.Forms.Panel();
            this.ptSymbolSize = new System.Windows.Forms.NumericUpDown();
            this.ptSymbolColor = new System.Windows.Forms.Panel();
            this.ptSymbolStyle = new System.Windows.Forms.ComboBox();
            this.lineSymbolGraphic = new System.Windows.Forms.Panel();
            this.ptSymbolGraphic = new System.Windows.Forms.Panel();
            this.btnSplitSave = new DotSpatial.SDR.Controls.SplitButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.adminTab_Control.SuspendLayout();
            this.adminTab_LayerManagement.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.adminLayerSplitter)).BeginInit();
            this.adminLayerSplitter.Panel1.SuspendLayout();
            this.adminLayerSplitter.Panel2.SuspendLayout();
            this.adminLayerSplitter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.legendSplitter)).BeginInit();
            this.legendSplitter.Panel2.SuspendLayout();
            this.legendSplitter.SuspendLayout();
            this.legendButtonTable.SuspendLayout();
            this.panelRadKeyLocations.SuspendLayout();
            this.panelRadAddress.SuspendLayout();
            this.adminTab_ProgramManagement.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHotKeys)).BeginInit();
            this.adminTab_SearchProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLayerIndex)).BeginInit();
            this.adminTab_SymbologySettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchHydrantDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchBufferDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchZoomFactor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchHydrantCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lineSymbolSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptSymbolSize)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 92F));
            this.tableLayoutPanel1.Controls.Add(this.adminTab_Control, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSplitSave, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(654, 786);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // adminTab_Control
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.adminTab_Control, 2);
            this.adminTab_Control.Controls.Add(this.adminTab_LayerManagement);
            this.adminTab_Control.Controls.Add(this.adminTab_ProgramManagement);
            this.adminTab_Control.Controls.Add(this.adminTab_SearchProperties);
            this.adminTab_Control.Controls.Add(this.adminTab_SymbologySettings);
            this.adminTab_Control.Dock = System.Windows.Forms.DockStyle.Fill;
            this.adminTab_Control.Location = new System.Drawing.Point(0, 0);
            this.adminTab_Control.Margin = new System.Windows.Forms.Padding(0);
            this.adminTab_Control.Name = "adminTab_Control";
            this.adminTab_Control.Padding = new System.Drawing.Point(0, 0);
            this.adminTab_Control.SelectedIndex = 0;
            this.adminTab_Control.Size = new System.Drawing.Size(654, 751);
            this.adminTab_Control.TabIndex = 8;
            // 
            // adminTab_LayerManagement
            // 
            this.adminTab_LayerManagement.BackColor = System.Drawing.Color.Transparent;
            this.adminTab_LayerManagement.Controls.Add(this.adminLayerSplitter);
            this.adminTab_LayerManagement.Location = new System.Drawing.Point(4, 22);
            this.adminTab_LayerManagement.Margin = new System.Windows.Forms.Padding(0);
            this.adminTab_LayerManagement.Name = "adminTab_LayerManagement";
            this.adminTab_LayerManagement.Size = new System.Drawing.Size(646, 725);
            this.adminTab_LayerManagement.TabIndex = 0;
            this.adminTab_LayerManagement.Text = "Map Configuration";
            // 
            // adminLayerSplitter
            // 
            this.adminLayerSplitter.BackColor = System.Drawing.Color.Transparent;
            this.adminLayerSplitter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.adminLayerSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.adminLayerSplitter.Location = new System.Drawing.Point(0, 0);
            this.adminLayerSplitter.Name = "adminLayerSplitter";
            // 
            // adminLayerSplitter.Panel1
            // 
            this.adminLayerSplitter.Panel1.BackColor = System.Drawing.Color.White;
            this.adminLayerSplitter.Panel1.Controls.Add(this.legendSplitter);
            this.adminLayerSplitter.Panel1MinSize = 120;
            // 
            // adminLayerSplitter.Panel2
            // 
            this.adminLayerSplitter.Panel2.BackColor = System.Drawing.Color.Transparent;
            this.adminLayerSplitter.Panel2.Controls.Add(this.btnAddView);
            this.adminLayerSplitter.Panel2.Controls.Add(this.txtViewName);
            this.adminLayerSplitter.Panel2.Controls.Add(this.btnDeleteView);
            this.adminLayerSplitter.Panel2.Controls.Add(this.chkViewLayers);
            this.adminLayerSplitter.Panel2.Controls.Add(this.cmbActiveMapTab);
            this.adminLayerSplitter.Panel2.Controls.Add(this.lblMapBGColor);
            this.adminLayerSplitter.Panel2.Controls.Add(this.mapBGColorPanel);
            this.adminLayerSplitter.Panel2.Controls.Add(this.panelRadKeyLocations);
            this.adminLayerSplitter.Panel2.Controls.Add(this.panelRadAddress);
            this.adminLayerSplitter.Panel2.Controls.Add(this.lblKeyLocations);
            this.adminLayerSplitter.Panel2.Controls.Add(this.lblHydrants);
            this.adminLayerSplitter.Panel2.Controls.Add(this.lblParcels);
            this.adminLayerSplitter.Panel2.Controls.Add(this.lblEsn);
            this.adminLayerSplitter.Panel2.Controls.Add(this.lblCellSector);
            this.adminLayerSplitter.Panel2.Controls.Add(this.cmbHydrantsLayer);
            this.adminLayerSplitter.Panel2.Controls.Add(this.lblRoads);
            this.adminLayerSplitter.Panel2.Controls.Add(this.lblAddresses);
            this.adminLayerSplitter.Panel2.Controls.Add(this.chkKeyLocationsLayers);
            this.adminLayerSplitter.Panel2.Controls.Add(this.cmbParcelsLayer);
            this.adminLayerSplitter.Panel2.Controls.Add(this.cmbESNLayer);
            this.adminLayerSplitter.Panel2.Controls.Add(this.cmbCellSectorLayer);
            this.adminLayerSplitter.Panel2.Controls.Add(this.lblCityLimits);
            this.adminLayerSplitter.Panel2.Controls.Add(this.lblNotes);
            this.adminLayerSplitter.Panel2.Controls.Add(this.cmbCityLimitLayer);
            this.adminLayerSplitter.Panel2.Controls.Add(this.chkRoadLayers);
            this.adminLayerSplitter.Panel2.Controls.Add(this.chkAddressLayers);
            this.adminLayerSplitter.Panel2.Controls.Add(this.cmbNotesLayer);
            this.adminLayerSplitter.Panel2.Margin = new System.Windows.Forms.Padding(3);
            this.adminLayerSplitter.Panel2.Padding = new System.Windows.Forms.Padding(6);
            this.adminLayerSplitter.Size = new System.Drawing.Size(646, 725);
            this.adminLayerSplitter.SplitterDistance = 208;
            this.adminLayerSplitter.SplitterWidth = 10;
            this.adminLayerSplitter.TabIndex = 8;
            this.adminLayerSplitter.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.adminLayerSplitter_SplitterMoved);
            // 
            // legendSplitter
            // 
            this.legendSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.legendSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.legendSplitter.IsSplitterFixed = true;
            this.legendSplitter.Location = new System.Drawing.Point(0, 0);
            this.legendSplitter.Margin = new System.Windows.Forms.Padding(0);
            this.legendSplitter.MinimumSize = new System.Drawing.Size(55, 0);
            this.legendSplitter.Name = "legendSplitter";
            this.legendSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // legendSplitter.Panel1
            // 
            this.legendSplitter.Panel1.AutoScroll = true;
            // 
            // legendSplitter.Panel2
            // 
            this.legendSplitter.Panel2.BackColor = System.Drawing.Color.Transparent;
            this.legendSplitter.Panel2.Controls.Add(this.legendButtonTable);
            this.legendSplitter.Panel2MinSize = 33;
            this.legendSplitter.Size = new System.Drawing.Size(206, 723);
            this.legendSplitter.SplitterDistance = 689;
            this.legendSplitter.SplitterWidth = 1;
            this.legendSplitter.TabIndex = 0;
            // 
            // legendButtonTable
            // 
            this.legendButtonTable.BackColor = System.Drawing.Color.Transparent;
            this.legendButtonTable.ColumnCount = 2;
            this.legendButtonTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.legendButtonTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.legendButtonTable.Controls.Add(this.btnRemoveLayer, 1, 0);
            this.legendButtonTable.Controls.Add(this.btnAddLayer, 0, 0);
            this.legendButtonTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.legendButtonTable.Location = new System.Drawing.Point(0, 0);
            this.legendButtonTable.Name = "legendButtonTable";
            this.legendButtonTable.RowCount = 1;
            this.legendButtonTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.legendButtonTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.legendButtonTable.Size = new System.Drawing.Size(206, 33);
            this.legendButtonTable.TabIndex = 0;
            // 
            // btnRemoveLayer
            // 
            this.btnRemoveLayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRemoveLayer.Location = new System.Drawing.Point(106, 3);
            this.btnRemoveLayer.MaximumSize = new System.Drawing.Size(0, 27);
            this.btnRemoveLayer.MinimumSize = new System.Drawing.Size(55, 23);
            this.btnRemoveLayer.Name = "btnRemoveLayer";
            this.btnRemoveLayer.Size = new System.Drawing.Size(97, 27);
            this.btnRemoveLayer.TabIndex = 1;
            this.btnRemoveLayer.Text = "Remove";
            this.btnRemoveLayer.UseVisualStyleBackColor = true;
            this.btnRemoveLayer.Click += new System.EventHandler(this.btnRemoveLayer_Click);
            // 
            // btnAddLayer
            // 
            this.btnAddLayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddLayer.Location = new System.Drawing.Point(3, 3);
            this.btnAddLayer.MaximumSize = new System.Drawing.Size(0, 27);
            this.btnAddLayer.MinimumSize = new System.Drawing.Size(55, 23);
            this.btnAddLayer.Name = "btnAddLayer";
            this.btnAddLayer.Size = new System.Drawing.Size(97, 27);
            this.btnAddLayer.TabIndex = 2;
            this.btnAddLayer.Text = "Add";
            this.btnAddLayer.UseVisualStyleBackColor = true;
            this.btnAddLayer.Click += new System.EventHandler(this.btnAddLayer_Click);
            // 
            // btnAddView
            // 
            this.btnAddView.Location = new System.Drawing.Point(32, 9);
            this.btnAddView.Name = "btnAddView";
            this.btnAddView.Size = new System.Drawing.Size(75, 23);
            this.btnAddView.TabIndex = 44;
            this.btnAddView.Text = "Add View";
            this.btnAddView.UseVisualStyleBackColor = true;
            this.btnAddView.Click += new System.EventHandler(this.btnAddView_Click);
            // 
            // txtViewName
            // 
            this.txtViewName.Location = new System.Drawing.Point(113, 9);
            this.txtViewName.Name = "txtViewName";
            this.txtViewName.Size = new System.Drawing.Size(199, 20);
            this.txtViewName.TabIndex = 43;
            // 
            // btnDeleteView
            // 
            this.btnDeleteView.Location = new System.Drawing.Point(228, 35);
            this.btnDeleteView.Name = "btnDeleteView";
            this.btnDeleteView.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteView.TabIndex = 42;
            this.btnDeleteView.Text = "Delete View";
            this.btnDeleteView.UseVisualStyleBackColor = true;
            this.btnDeleteView.Click += new System.EventHandler(this.btnRemoveView_Click);
            // 
            // chkViewLayers
            // 
            this.chkViewLayers.CheckOnClick = true;
            this.chkViewLayers.FormattingEnabled = true;
            this.chkViewLayers.Location = new System.Drawing.Point(43, 63);
            this.chkViewLayers.Name = "chkViewLayers";
            this.chkViewLayers.Size = new System.Drawing.Size(241, 94);
            this.chkViewLayers.TabIndex = 41;
            // 
            // cmbActiveMapTab
            // 
            this.cmbActiveMapTab.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActiveMapTab.FormattingEnabled = true;
            this.cmbActiveMapTab.Location = new System.Drawing.Point(43, 36);
            this.cmbActiveMapTab.Name = "cmbActiveMapTab";
            this.cmbActiveMapTab.Size = new System.Drawing.Size(179, 21);
            this.cmbActiveMapTab.TabIndex = 40;
            // 
            // lblMapBGColor
            // 
            this.lblMapBGColor.AutoSize = true;
            this.lblMapBGColor.Location = new System.Drawing.Point(168, 651);
            this.lblMapBGColor.Name = "lblMapBGColor";
            this.lblMapBGColor.Size = new System.Drawing.Size(116, 13);
            this.lblMapBGColor.TabIndex = 38;
            this.lblMapBGColor.Text = "Map Background Color";
            // 
            // mapBGColorPanel
            // 
            this.mapBGColorPanel.BackColor = System.Drawing.Color.Black;
            this.mapBGColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mapBGColorPanel.ForeColor = System.Drawing.Color.Black;
            this.mapBGColorPanel.Location = new System.Drawing.Point(101, 643);
            this.mapBGColorPanel.Name = "mapBGColorPanel";
            this.mapBGColorPanel.Size = new System.Drawing.Size(48, 26);
            this.mapBGColorPanel.TabIndex = 37;
            this.mapBGColorPanel.Click += new System.EventHandler(this.mapBGColorPanel_Click);
            // 
            // panelRadKeyLocations
            // 
            this.panelRadKeyLocations.Controls.Add(this.radKeyLocationsPolygons);
            this.panelRadKeyLocations.Controls.Add(this.radKeyLocationsPoints);
            this.panelRadKeyLocations.Location = new System.Drawing.Point(21, 560);
            this.panelRadKeyLocations.Name = "panelRadKeyLocations";
            this.panelRadKeyLocations.Size = new System.Drawing.Size(77, 55);
            this.panelRadKeyLocations.TabIndex = 36;
            // 
            // radKeyLocationsPolygons
            // 
            this.radKeyLocationsPolygons.AutoSize = true;
            this.radKeyLocationsPolygons.Location = new System.Drawing.Point(7, 27);
            this.radKeyLocationsPolygons.Name = "radKeyLocationsPolygons";
            this.radKeyLocationsPolygons.Size = new System.Drawing.Size(68, 17);
            this.radKeyLocationsPolygons.TabIndex = 1;
            this.radKeyLocationsPolygons.TabStop = true;
            this.radKeyLocationsPolygons.Text = "Polygons";
            this.radKeyLocationsPolygons.UseVisualStyleBackColor = true;
            this.radKeyLocationsPolygons.CheckedChanged += new System.EventHandler(this.radKeyLocationsPolygons_CheckedChanged);
            // 
            // radKeyLocationsPoints
            // 
            this.radKeyLocationsPoints.AutoSize = true;
            this.radKeyLocationsPoints.Checked = true;
            this.radKeyLocationsPoints.Location = new System.Drawing.Point(7, 9);
            this.radKeyLocationsPoints.Name = "radKeyLocationsPoints";
            this.radKeyLocationsPoints.Size = new System.Drawing.Size(54, 17);
            this.radKeyLocationsPoints.TabIndex = 0;
            this.radKeyLocationsPoints.TabStop = true;
            this.radKeyLocationsPoints.Text = "Points";
            this.radKeyLocationsPoints.UseVisualStyleBackColor = true;
            this.radKeyLocationsPoints.CheckedChanged += new System.EventHandler(this.radKeyLocationsPoints_CheckedChanged);
            // 
            // panelRadAddress
            // 
            this.panelRadAddress.Controls.Add(this.radAddressPoints);
            this.panelRadAddress.Controls.Add(this.radAddressPolygons);
            this.panelRadAddress.Location = new System.Drawing.Point(21, 198);
            this.panelRadAddress.Name = "panelRadAddress";
            this.panelRadAddress.Size = new System.Drawing.Size(77, 50);
            this.panelRadAddress.TabIndex = 35;
            // 
            // radAddressPoints
            // 
            this.radAddressPoints.AutoSize = true;
            this.radAddressPoints.Checked = true;
            this.radAddressPoints.Location = new System.Drawing.Point(6, 6);
            this.radAddressPoints.Name = "radAddressPoints";
            this.radAddressPoints.Size = new System.Drawing.Size(54, 17);
            this.radAddressPoints.TabIndex = 33;
            this.radAddressPoints.TabStop = true;
            this.radAddressPoints.Text = "Points";
            this.radAddressPoints.UseVisualStyleBackColor = true;
            this.radAddressPoints.CheckedChanged += new System.EventHandler(this.radAddressPoints_CheckedChanged);
            // 
            // radAddressPolygons
            // 
            this.radAddressPolygons.AutoSize = true;
            this.radAddressPolygons.Location = new System.Drawing.Point(6, 24);
            this.radAddressPolygons.Name = "radAddressPolygons";
            this.radAddressPolygons.Size = new System.Drawing.Size(68, 17);
            this.radAddressPolygons.TabIndex = 34;
            this.radAddressPolygons.TabStop = true;
            this.radAddressPolygons.Text = "Polygons";
            this.radAddressPolygons.UseVisualStyleBackColor = true;
            this.radAddressPolygons.CheckedChanged += new System.EventHandler(this.radAddressPolygons_CheckedChanged);
            // 
            // lblKeyLocations
            // 
            this.lblKeyLocations.AutoSize = true;
            this.lblKeyLocations.Location = new System.Drawing.Point(18, 544);
            this.lblKeyLocations.Name = "lblKeyLocations";
            this.lblKeyLocations.Size = new System.Drawing.Size(77, 13);
            this.lblKeyLocations.TabIndex = 32;
            this.lblKeyLocations.Text = "Key Locations:";
            // 
            // lblHydrants
            // 
            this.lblHydrants.AutoSize = true;
            this.lblHydrants.Location = new System.Drawing.Point(43, 520);
            this.lblHydrants.Name = "lblHydrants";
            this.lblHydrants.Size = new System.Drawing.Size(52, 13);
            this.lblHydrants.TabIndex = 31;
            this.lblHydrants.Text = "Hydrants:";
            // 
            // lblParcels
            // 
            this.lblParcels.AutoSize = true;
            this.lblParcels.Location = new System.Drawing.Point(51, 493);
            this.lblParcels.Name = "lblParcels";
            this.lblParcels.Size = new System.Drawing.Size(45, 13);
            this.lblParcels.TabIndex = 30;
            this.lblParcels.Text = "Parcels:";
            // 
            // lblEsn
            // 
            this.lblEsn.AutoSize = true;
            this.lblEsn.Location = new System.Drawing.Point(66, 466);
            this.lblEsn.Name = "lblEsn";
            this.lblEsn.Size = new System.Drawing.Size(32, 13);
            this.lblEsn.TabIndex = 29;
            this.lblEsn.Text = "ESN:";
            // 
            // lblCellSector
            // 
            this.lblCellSector.AutoSize = true;
            this.lblCellSector.Location = new System.Drawing.Point(29, 439);
            this.lblCellSector.Name = "lblCellSector";
            this.lblCellSector.Size = new System.Drawing.Size(66, 13);
            this.lblCellSector.TabIndex = 27;
            this.lblCellSector.Text = "Cell Sectors:";
            // 
            // cmbHydrantsLayer
            // 
            this.cmbHydrantsLayer.FormattingEnabled = true;
            this.cmbHydrantsLayer.Location = new System.Drawing.Point(101, 517);
            this.cmbHydrantsLayer.Name = "cmbHydrantsLayer";
            this.cmbHydrantsLayer.Size = new System.Drawing.Size(199, 21);
            this.cmbHydrantsLayer.TabIndex = 26;
            this.cmbHydrantsLayer.SelectedIndexChanged += new System.EventHandler(this.cmbHydrantsLayer_SelectedIndexChanged);
            // 
            // lblRoads
            // 
            this.lblRoads.AutoSize = true;
            this.lblRoads.Location = new System.Drawing.Point(54, 282);
            this.lblRoads.Name = "lblRoads";
            this.lblRoads.Size = new System.Drawing.Size(41, 13);
            this.lblRoads.TabIndex = 24;
            this.lblRoads.Text = "Roads:";
            // 
            // lblAddresses
            // 
            this.lblAddresses.AutoSize = true;
            this.lblAddresses.Location = new System.Drawing.Point(39, 182);
            this.lblAddresses.Name = "lblAddresses";
            this.lblAddresses.Size = new System.Drawing.Size(59, 13);
            this.lblAddresses.TabIndex = 23;
            this.lblAddresses.Text = "Addresses:";
            // 
            // chkKeyLocationsLayers
            // 
            this.chkKeyLocationsLayers.CheckOnClick = true;
            this.chkKeyLocationsLayers.FormattingEnabled = true;
            this.chkKeyLocationsLayers.Location = new System.Drawing.Point(101, 544);
            this.chkKeyLocationsLayers.Name = "chkKeyLocationsLayers";
            this.chkKeyLocationsLayers.Size = new System.Drawing.Size(199, 94);
            this.chkKeyLocationsLayers.TabIndex = 22;
            this.chkKeyLocationsLayers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkKeyLocationsLayers_ItemCheck);
            // 
            // cmbParcelsLayer
            // 
            this.cmbParcelsLayer.FormattingEnabled = true;
            this.cmbParcelsLayer.Location = new System.Drawing.Point(101, 490);
            this.cmbParcelsLayer.Name = "cmbParcelsLayer";
            this.cmbParcelsLayer.Size = new System.Drawing.Size(199, 21);
            this.cmbParcelsLayer.TabIndex = 21;
            this.cmbParcelsLayer.SelectedIndexChanged += new System.EventHandler(this.cmbParcelsLayer_SelectedIndexChanged);
            // 
            // cmbESNLayer
            // 
            this.cmbESNLayer.FormattingEnabled = true;
            this.cmbESNLayer.Location = new System.Drawing.Point(101, 463);
            this.cmbESNLayer.Name = "cmbESNLayer";
            this.cmbESNLayer.Size = new System.Drawing.Size(199, 21);
            this.cmbESNLayer.TabIndex = 20;
            this.cmbESNLayer.SelectedIndexChanged += new System.EventHandler(this.cmbESNLayer_SelectedIndexChanged);
            // 
            // cmbCellSectorLayer
            // 
            this.cmbCellSectorLayer.FormattingEnabled = true;
            this.cmbCellSectorLayer.Location = new System.Drawing.Point(101, 436);
            this.cmbCellSectorLayer.Name = "cmbCellSectorLayer";
            this.cmbCellSectorLayer.Size = new System.Drawing.Size(199, 21);
            this.cmbCellSectorLayer.TabIndex = 18;
            this.cmbCellSectorLayer.SelectedIndexChanged += new System.EventHandler(this.cmbCellSectorLayer_SelectedIndexChanged);
            // 
            // lblCityLimits
            // 
            this.lblCityLimits.AutoSize = true;
            this.lblCityLimits.Location = new System.Drawing.Point(39, 412);
            this.lblCityLimits.Name = "lblCityLimits";
            this.lblCityLimits.Size = new System.Drawing.Size(56, 13);
            this.lblCityLimits.TabIndex = 16;
            this.lblCityLimits.Text = "City Limits:";
            // 
            // lblNotes
            // 
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(57, 385);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(38, 13);
            this.lblNotes.TabIndex = 15;
            this.lblNotes.Text = "Notes:";
            // 
            // cmbCityLimitLayer
            // 
            this.cmbCityLimitLayer.FormattingEnabled = true;
            this.cmbCityLimitLayer.Location = new System.Drawing.Point(101, 409);
            this.cmbCityLimitLayer.Name = "cmbCityLimitLayer";
            this.cmbCityLimitLayer.Size = new System.Drawing.Size(199, 21);
            this.cmbCityLimitLayer.TabIndex = 8;
            this.cmbCityLimitLayer.SelectedIndexChanged += new System.EventHandler(this.cmbCityLimitLayer_SelectedIndexChanged);
            // 
            // chkRoadLayers
            // 
            this.chkRoadLayers.CheckOnClick = true;
            this.chkRoadLayers.FormattingEnabled = true;
            this.chkRoadLayers.Location = new System.Drawing.Point(101, 282);
            this.chkRoadLayers.Name = "chkRoadLayers";
            this.chkRoadLayers.Size = new System.Drawing.Size(199, 94);
            this.chkRoadLayers.TabIndex = 7;
            this.chkRoadLayers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkRoadLayers_ItemCheck);
            // 
            // chkAddressLayers
            // 
            this.chkAddressLayers.CheckOnClick = true;
            this.chkAddressLayers.FormattingEnabled = true;
            this.chkAddressLayers.Location = new System.Drawing.Point(101, 182);
            this.chkAddressLayers.Name = "chkAddressLayers";
            this.chkAddressLayers.Size = new System.Drawing.Size(199, 94);
            this.chkAddressLayers.TabIndex = 6;
            this.chkAddressLayers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkAddressLayers_ItemCheck);
            // 
            // cmbNotesLayer
            // 
            this.cmbNotesLayer.FormattingEnabled = true;
            this.cmbNotesLayer.Location = new System.Drawing.Point(101, 382);
            this.cmbNotesLayer.Name = "cmbNotesLayer";
            this.cmbNotesLayer.Size = new System.Drawing.Size(199, 21);
            this.cmbNotesLayer.TabIndex = 5;
            this.cmbNotesLayer.SelectedIndexChanged += new System.EventHandler(this.cmbNotesLayer_SelectedIndexChanged);
            // 
            // adminTab_ProgramManagement
            // 
            this.adminTab_ProgramManagement.Controls.Add(this.btnSaveHotKeys);
            this.adminTab_ProgramManagement.Controls.Add(this.dgvHotKeys);
            this.adminTab_ProgramManagement.Controls.Add(this.btnUsersDelete);
            this.adminTab_ProgramManagement.Controls.Add(this.btnUsersAddUpdate);
            this.adminTab_ProgramManagement.Controls.Add(this.lstUsers);
            this.adminTab_ProgramManagement.Controls.Add(this.label3);
            this.adminTab_ProgramManagement.Controls.Add(this.label2);
            this.adminTab_ProgramManagement.Controls.Add(this.label1);
            this.adminTab_ProgramManagement.Controls.Add(this.txtUsername);
            this.adminTab_ProgramManagement.Controls.Add(this.txtVerifyPassword);
            this.adminTab_ProgramManagement.Controls.Add(this.txtPassword);
            this.adminTab_ProgramManagement.Location = new System.Drawing.Point(4, 22);
            this.adminTab_ProgramManagement.Name = "adminTab_ProgramManagement";
            this.adminTab_ProgramManagement.Padding = new System.Windows.Forms.Padding(3);
            this.adminTab_ProgramManagement.Size = new System.Drawing.Size(992, 725);
            this.adminTab_ProgramManagement.TabIndex = 3;
            this.adminTab_ProgramManagement.Text = "Program Management";
            this.adminTab_ProgramManagement.UseVisualStyleBackColor = true;
            // 
            // btnSaveHotKeys
            // 
            this.btnSaveHotKeys.Location = new System.Drawing.Point(432, 437);
            this.btnSaveHotKeys.Name = "btnSaveHotKeys";
            this.btnSaveHotKeys.Size = new System.Drawing.Size(75, 23);
            this.btnSaveHotKeys.TabIndex = 10;
            this.btnSaveHotKeys.Text = "Save";
            this.btnSaveHotKeys.UseVisualStyleBackColor = true;
            this.btnSaveHotKeys.Click += new System.EventHandler(this.btnSaveHotKeys_Click);
            // 
            // dgvHotKeys
            // 
            this.dgvHotKeys.AllowUserToAddRows = false;
            this.dgvHotKeys.AllowUserToDeleteRows = false;
            this.dgvHotKeys.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvHotKeys.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvHotKeys.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvHotKeys.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvHotKeys.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvHotKeys.Location = new System.Drawing.Point(24, 218);
            this.dgvHotKeys.MultiSelect = false;
            this.dgvHotKeys.Name = "dgvHotKeys";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvHotKeys.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvHotKeys.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHotKeys.Size = new System.Drawing.Size(483, 213);
            this.dgvHotKeys.TabIndex = 9;
            // 
            // btnUsersDelete
            // 
            this.btnUsersDelete.Location = new System.Drawing.Point(324, 166);
            this.btnUsersDelete.Name = "btnUsersDelete";
            this.btnUsersDelete.Size = new System.Drawing.Size(75, 23);
            this.btnUsersDelete.TabIndex = 8;
            this.btnUsersDelete.Text = "Delete";
            this.btnUsersDelete.UseVisualStyleBackColor = true;
            this.btnUsersDelete.Click += new System.EventHandler(this.btnUsersDelete_Click);
            // 
            // btnUsersAddUpdate
            // 
            this.btnUsersAddUpdate.Location = new System.Drawing.Point(237, 166);
            this.btnUsersAddUpdate.Name = "btnUsersAddUpdate";
            this.btnUsersAddUpdate.Size = new System.Drawing.Size(81, 23);
            this.btnUsersAddUpdate.TabIndex = 7;
            this.btnUsersAddUpdate.Text = "Add/Update";
            this.btnUsersAddUpdate.UseVisualStyleBackColor = true;
            this.btnUsersAddUpdate.Click += new System.EventHandler(this.btnUsersAddUpdate_Click);
            // 
            // lstUsers
            // 
            this.lstUsers.FormattingEnabled = true;
            this.lstUsers.Location = new System.Drawing.Point(24, 29);
            this.lstUsers.Name = "lstUsers";
            this.lstUsers.Size = new System.Drawing.Size(134, 173);
            this.lstUsers.TabIndex = 6;
            this.lstUsers.DoubleClick += new System.EventHandler(this.lstUsers_DoubleClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(189, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Username";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(186, 121);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Verify Password";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(200, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Password";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(250, 42);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(178, 20);
            this.txtUsername.TabIndex = 2;
            // 
            // txtVerifyPassword
            // 
            this.txtVerifyPassword.Location = new System.Drawing.Point(274, 118);
            this.txtVerifyPassword.Name = "txtVerifyPassword";
            this.txtVerifyPassword.PasswordChar = '*';
            this.txtVerifyPassword.Size = new System.Drawing.Size(168, 20);
            this.txtVerifyPassword.TabIndex = 1;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(274, 84);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(168, 20);
            this.txtPassword.TabIndex = 0;
            // 
            // adminTab_SearchProperties
            // 
            this.adminTab_SearchProperties.Controls.Add(this.chkEnableQuesryParserLog);
            this.adminTab_SearchProperties.Controls.Add(this.chkPretypes);
            this.adminTab_SearchProperties.Controls.Add(this.btnRemoveIndex);
            this.adminTab_SearchProperties.Controls.Add(this.lstExistingIndexes);
            this.adminTab_SearchProperties.Controls.Add(this.chkLayersToIndex);
            this.adminTab_SearchProperties.Controls.Add(this.btnAddIndex);
            this.adminTab_SearchProperties.Controls.Add(this.dgvLayerIndex);
            this.adminTab_SearchProperties.Controls.Add(this.btnDeleteIndex);
            this.adminTab_SearchProperties.Controls.Add(this.btnCreateIndex);
            this.adminTab_SearchProperties.Controls.Add(this.cmbLayerIndex);
            this.adminTab_SearchProperties.Controls.Add(this.chkLayerIndex);
            this.adminTab_SearchProperties.Location = new System.Drawing.Point(4, 22);
            this.adminTab_SearchProperties.Name = "adminTab_SearchProperties";
            this.adminTab_SearchProperties.Padding = new System.Windows.Forms.Padding(3);
            this.adminTab_SearchProperties.Size = new System.Drawing.Size(646, 725);
            this.adminTab_SearchProperties.TabIndex = 4;
            this.adminTab_SearchProperties.Text = "Layer Search Properties";
            this.adminTab_SearchProperties.UseVisualStyleBackColor = true;
            // 
            // chkEnableQuesryParserLog
            // 
            this.chkEnableQuesryParserLog.AutoSize = true;
            this.chkEnableQuesryParserLog.Location = new System.Drawing.Point(41, 604);
            this.chkEnableQuesryParserLog.Name = "chkEnableQuesryParserLog";
            this.chkEnableQuesryParserLog.Size = new System.Drawing.Size(164, 17);
            this.chkEnableQuesryParserLog.TabIndex = 12;
            this.chkEnableQuesryParserLog.Text = "Enable Query Parser Logging";
            this.chkEnableQuesryParserLog.UseVisualStyleBackColor = true;
            // 
            // chkPretypes
            // 
            this.chkPretypes.AutoSize = true;
            this.chkPretypes.Location = new System.Drawing.Point(41, 581);
            this.chkPretypes.Name = "chkPretypes";
            this.chkPretypes.Size = new System.Drawing.Size(89, 17);
            this.chkPretypes.TabIndex = 11;
            this.chkPretypes.Text = "Use Pretypes";
            this.chkPretypes.UseVisualStyleBackColor = true;
            // 
            // btnRemoveIndex
            // 
            this.btnRemoveIndex.Location = new System.Drawing.Point(90, 286);
            this.btnRemoveIndex.Name = "btnRemoveIndex";
            this.btnRemoveIndex.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveIndex.TabIndex = 10;
            this.btnRemoveIndex.Text = "Remove";
            this.btnRemoveIndex.UseVisualStyleBackColor = true;
            this.btnRemoveIndex.Click += new System.EventHandler(this.btnRemoveIndex_Click);
            // 
            // lstExistingIndexes
            // 
            this.lstExistingIndexes.FormattingEnabled = true;
            this.lstExistingIndexes.Location = new System.Drawing.Point(202, 460);
            this.lstExistingIndexes.Name = "lstExistingIndexes";
            this.lstExistingIndexes.Size = new System.Drawing.Size(311, 95);
            this.lstExistingIndexes.TabIndex = 9;
            // 
            // chkLayersToIndex
            // 
            this.chkLayersToIndex.FormattingEnabled = true;
            this.chkLayersToIndex.Location = new System.Drawing.Point(10, 315);
            this.chkLayersToIndex.Name = "chkLayersToIndex";
            this.chkLayersToIndex.Size = new System.Drawing.Size(187, 139);
            this.chkLayersToIndex.TabIndex = 8;
            // 
            // btnAddIndex
            // 
            this.btnAddIndex.Location = new System.Drawing.Point(9, 286);
            this.btnAddIndex.Name = "btnAddIndex";
            this.btnAddIndex.Size = new System.Drawing.Size(75, 23);
            this.btnAddIndex.TabIndex = 7;
            this.btnAddIndex.Text = "Add";
            this.btnAddIndex.UseVisualStyleBackColor = true;
            this.btnAddIndex.Click += new System.EventHandler(this.btnAddIndex_Click);
            // 
            // dgvLayerIndex
            // 
            this.dgvLayerIndex.AllowUserToAddRows = false;
            this.dgvLayerIndex.AllowUserToDeleteRows = false;
            this.dgvLayerIndex.AllowUserToResizeRows = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLayerIndex.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvLayerIndex.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLayerIndex.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvLayerIndex.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvLayerIndex.Location = new System.Drawing.Point(202, 39);
            this.dgvLayerIndex.MultiSelect = false;
            this.dgvLayerIndex.Name = "dgvLayerIndex";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLayerIndex.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvLayerIndex.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLayerIndex.Size = new System.Drawing.Size(311, 415);
            this.dgvLayerIndex.TabIndex = 4;
            // 
            // btnDeleteIndex
            // 
            this.btnDeleteIndex.Location = new System.Drawing.Point(442, 561);
            this.btnDeleteIndex.Name = "btnDeleteIndex";
            this.btnDeleteIndex.Size = new System.Drawing.Size(71, 23);
            this.btnDeleteIndex.TabIndex = 3;
            this.btnDeleteIndex.Text = "Delete Index";
            this.btnDeleteIndex.UseVisualStyleBackColor = true;
            this.btnDeleteIndex.Click += new System.EventHandler(this.btnDeleteIndex_Click);
            // 
            // btnCreateIndex
            // 
            this.btnCreateIndex.Location = new System.Drawing.Point(126, 460);
            this.btnCreateIndex.Name = "btnCreateIndex";
            this.btnCreateIndex.Size = new System.Drawing.Size(71, 23);
            this.btnCreateIndex.TabIndex = 2;
            this.btnCreateIndex.Text = "Create Index";
            this.btnCreateIndex.UseVisualStyleBackColor = true;
            this.btnCreateIndex.Click += new System.EventHandler(this.btnCreateIndex_Click);
            // 
            // cmbLayerIndex
            // 
            this.cmbLayerIndex.FormattingEnabled = true;
            this.cmbLayerIndex.Location = new System.Drawing.Point(8, 12);
            this.cmbLayerIndex.Name = "cmbLayerIndex";
            this.cmbLayerIndex.Size = new System.Drawing.Size(344, 21);
            this.cmbLayerIndex.TabIndex = 1;
            this.cmbLayerIndex.SelectedValueChanged += new System.EventHandler(this.cmbLayerIndex_SelectedValueChanged);
            // 
            // chkLayerIndex
            // 
            this.chkLayerIndex.CheckOnClick = true;
            this.chkLayerIndex.FormattingEnabled = true;
            this.chkLayerIndex.Location = new System.Drawing.Point(9, 39);
            this.chkLayerIndex.Name = "chkLayerIndex";
            this.chkLayerIndex.Size = new System.Drawing.Size(188, 199);
            this.chkLayerIndex.TabIndex = 0;
            this.chkLayerIndex.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkLayerIndex_ItemCheck);
            // 
            // adminTab_SymbologySettings
            // 
            this.adminTab_SymbologySettings.Controls.Add(this.ptSymbolColorSlider);
            this.adminTab_SymbologySettings.Controls.Add(this.lineSymbolColorSlider);
            this.adminTab_SymbologySettings.Controls.Add(this.lblHydrantDist);
            this.adminTab_SymbologySettings.Controls.Add(this.lblZoomFactor);
            this.adminTab_SymbologySettings.Controls.Add(this.lblBufDist);
            this.adminTab_SymbologySettings.Controls.Add(this.lblHydrantCount);
            this.adminTab_SymbologySettings.Controls.Add(this.searchHydrantDistance);
            this.adminTab_SymbologySettings.Controls.Add(this.searchBufferDistance);
            this.adminTab_SymbologySettings.Controls.Add(this.searchZoomFactor);
            this.adminTab_SymbologySettings.Controls.Add(this.searchHydrantCount);
            this.adminTab_SymbologySettings.Controls.Add(this.lineSymbolBorderColor);
            this.adminTab_SymbologySettings.Controls.Add(this.lineSymbolCap);
            this.adminTab_SymbologySettings.Controls.Add(this.lineSymbolSize);
            this.adminTab_SymbologySettings.Controls.Add(this.lineSymbolStyle);
            this.adminTab_SymbologySettings.Controls.Add(this.lineSymbolColor);
            this.adminTab_SymbologySettings.Controls.Add(this.ptSymbolSize);
            this.adminTab_SymbologySettings.Controls.Add(this.ptSymbolColor);
            this.adminTab_SymbologySettings.Controls.Add(this.ptSymbolStyle);
            this.adminTab_SymbologySettings.Controls.Add(this.lineSymbolGraphic);
            this.adminTab_SymbologySettings.Controls.Add(this.ptSymbolGraphic);
            this.adminTab_SymbologySettings.Location = new System.Drawing.Point(4, 22);
            this.adminTab_SymbologySettings.Name = "adminTab_SymbologySettings";
            this.adminTab_SymbologySettings.Padding = new System.Windows.Forms.Padding(3);
            this.adminTab_SymbologySettings.Size = new System.Drawing.Size(646, 725);
            this.adminTab_SymbologySettings.TabIndex = 5;
            this.adminTab_SymbologySettings.Text = "Project Settings";
            this.adminTab_SymbologySettings.UseVisualStyleBackColor = true;
            // 
            // ptSymbolColorSlider
            // 
            this.ptSymbolColorSlider.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ptSymbolColorSlider.ColorButton = null;
            this.ptSymbolColorSlider.FlipRamp = false;
            this.ptSymbolColorSlider.FlipText = false;
            this.ptSymbolColorSlider.InvertRamp = false;
            this.ptSymbolColorSlider.Location = new System.Drawing.Point(156, 257);
            this.ptSymbolColorSlider.Maximum = 1D;
            this.ptSymbolColorSlider.MaximumColor = System.Drawing.Color.Green;
            this.ptSymbolColorSlider.Minimum = 0D;
            this.ptSymbolColorSlider.MinimumColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.ptSymbolColorSlider.Name = "ptSymbolColorSlider";
            this.ptSymbolColorSlider.NumberFormat = "#%";
            this.ptSymbolColorSlider.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.ptSymbolColorSlider.RampRadius = 10F;
            this.ptSymbolColorSlider.RampText = null;
            this.ptSymbolColorSlider.RampTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ptSymbolColorSlider.RampTextBehindRamp = false;
            this.ptSymbolColorSlider.RampTextColor = System.Drawing.Color.Black;
            this.ptSymbolColorSlider.RampTextFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ptSymbolColorSlider.ShowMaximum = false;
            this.ptSymbolColorSlider.ShowMinimum = false;
            this.ptSymbolColorSlider.ShowTicks = true;
            this.ptSymbolColorSlider.ShowValue = true;
            this.ptSymbolColorSlider.Size = new System.Drawing.Size(122, 23);
            this.ptSymbolColorSlider.SliderColor = System.Drawing.Color.Blue;
            this.ptSymbolColorSlider.SliderRadius = 4F;
            this.ptSymbolColorSlider.TabIndex = 47;
            this.ptSymbolColorSlider.Text = "rampSlider1";
            this.ptSymbolColorSlider.TickColor = System.Drawing.Color.DarkGray;
            this.ptSymbolColorSlider.TickSpacing = 5F;
            this.ptSymbolColorSlider.Value = 1D;
            // 
            // lineSymbolColorSlider
            // 
            this.lineSymbolColorSlider.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lineSymbolColorSlider.ColorButton = null;
            this.lineSymbolColorSlider.FlipRamp = false;
            this.lineSymbolColorSlider.FlipText = false;
            this.lineSymbolColorSlider.InvertRamp = false;
            this.lineSymbolColorSlider.Location = new System.Drawing.Point(156, 329);
            this.lineSymbolColorSlider.Maximum = 1D;
            this.lineSymbolColorSlider.MaximumColor = System.Drawing.Color.Green;
            this.lineSymbolColorSlider.Minimum = 0D;
            this.lineSymbolColorSlider.MinimumColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lineSymbolColorSlider.Name = "lineSymbolColorSlider";
            this.lineSymbolColorSlider.NumberFormat = "#%";
            this.lineSymbolColorSlider.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.lineSymbolColorSlider.RampRadius = 10F;
            this.lineSymbolColorSlider.RampText = null;
            this.lineSymbolColorSlider.RampTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.lineSymbolColorSlider.RampTextBehindRamp = false;
            this.lineSymbolColorSlider.RampTextColor = System.Drawing.Color.Black;
            this.lineSymbolColorSlider.RampTextFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lineSymbolColorSlider.ShowMaximum = false;
            this.lineSymbolColorSlider.ShowMinimum = false;
            this.lineSymbolColorSlider.ShowTicks = true;
            this.lineSymbolColorSlider.ShowValue = true;
            this.lineSymbolColorSlider.Size = new System.Drawing.Size(143, 23);
            this.lineSymbolColorSlider.SliderColor = System.Drawing.Color.Blue;
            this.lineSymbolColorSlider.SliderRadius = 4F;
            this.lineSymbolColorSlider.TabIndex = 46;
            this.lineSymbolColorSlider.Text = "rampSlider1";
            this.lineSymbolColorSlider.TickColor = System.Drawing.Color.DarkGray;
            this.lineSymbolColorSlider.TickSpacing = 5F;
            this.lineSymbolColorSlider.Value = 1D;
            // 
            // lblHydrantDist
            // 
            this.lblHydrantDist.AutoSize = true;
            this.lblHydrantDist.Location = new System.Drawing.Point(55, 468);
            this.lblHydrantDist.Name = "lblHydrantDist";
            this.lblHydrantDist.Size = new System.Drawing.Size(89, 13);
            this.lblHydrantDist.TabIndex = 44;
            this.lblHydrantDist.Text = "Hydrant Distance";
            // 
            // lblZoomFactor
            // 
            this.lblZoomFactor.AutoSize = true;
            this.lblZoomFactor.Location = new System.Drawing.Point(184, 528);
            this.lblZoomFactor.Name = "lblZoomFactor";
            this.lblZoomFactor.Size = new System.Drawing.Size(67, 13);
            this.lblZoomFactor.TabIndex = 43;
            this.lblZoomFactor.Text = "Zoom Factor";
            // 
            // lblBufDist
            // 
            this.lblBufDist.AutoSize = true;
            this.lblBufDist.Location = new System.Drawing.Point(535, 487);
            this.lblBufDist.Name = "lblBufDist";
            this.lblBufDist.Size = new System.Drawing.Size(117, 13);
            this.lblBufDist.TabIndex = 42;
            this.lblBufDist.Text = "Search Buffer Distance";
            // 
            // lblHydrantCount
            // 
            this.lblHydrantCount.AutoSize = true;
            this.lblHydrantCount.Location = new System.Drawing.Point(292, 444);
            this.lblHydrantCount.Name = "lblHydrantCount";
            this.lblHydrantCount.Size = new System.Drawing.Size(75, 13);
            this.lblHydrantCount.TabIndex = 41;
            this.lblHydrantCount.Text = "Hydrant Count";
            // 
            // searchHydrantDistance
            // 
            this.searchHydrantDistance.Location = new System.Drawing.Point(58, 487);
            this.searchHydrantDistance.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.searchHydrantDistance.Name = "searchHydrantDistance";
            this.searchHydrantDistance.Size = new System.Drawing.Size(78, 20);
            this.searchHydrantDistance.TabIndex = 40;
            // 
            // searchBufferDistance
            // 
            this.searchBufferDistance.Location = new System.Drawing.Point(514, 519);
            this.searchBufferDistance.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.searchBufferDistance.Name = "searchBufferDistance";
            this.searchBufferDistance.Size = new System.Drawing.Size(77, 20);
            this.searchBufferDistance.TabIndex = 39;
            // 
            // searchZoomFactor
            // 
            this.searchZoomFactor.DecimalPlaces = 2;
            this.searchZoomFactor.Increment = new decimal(new int[] {
            25,
            0,
            0,
            131072});
            this.searchZoomFactor.Location = new System.Drawing.Point(185, 544);
            this.searchZoomFactor.Name = "searchZoomFactor";
            this.searchZoomFactor.Size = new System.Drawing.Size(66, 20);
            this.searchZoomFactor.TabIndex = 38;
            // 
            // searchHydrantCount
            // 
            this.searchHydrantCount.Location = new System.Drawing.Point(295, 460);
            this.searchHydrantCount.Name = "searchHydrantCount";
            this.searchHydrantCount.Size = new System.Drawing.Size(66, 20);
            this.searchHydrantCount.TabIndex = 37;
            // 
            // lineSymbolBorderColor
            // 
            this.lineSymbolBorderColor.Location = new System.Drawing.Point(101, 362);
            this.lineSymbolBorderColor.Name = "lineSymbolBorderColor";
            this.lineSymbolBorderColor.Size = new System.Drawing.Size(34, 36);
            this.lineSymbolBorderColor.TabIndex = 36;
            // 
            // lineSymbolCap
            // 
            this.lineSymbolCap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lineSymbolCap.FormattingEnabled = true;
            this.lineSymbolCap.Location = new System.Drawing.Point(156, 362);
            this.lineSymbolCap.Name = "lineSymbolCap";
            this.lineSymbolCap.Size = new System.Drawing.Size(206, 21);
            this.lineSymbolCap.TabIndex = 35;
            // 
            // lineSymbolSize
            // 
            this.lineSymbolSize.Location = new System.Drawing.Point(555, 332);
            this.lineSymbolSize.Name = "lineSymbolSize";
            this.lineSymbolSize.Size = new System.Drawing.Size(78, 20);
            this.lineSymbolSize.TabIndex = 34;
            // 
            // lineSymbolStyle
            // 
            this.lineSymbolStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lineSymbolStyle.FormattingEnabled = true;
            this.lineSymbolStyle.Location = new System.Drawing.Point(305, 331);
            this.lineSymbolStyle.Name = "lineSymbolStyle";
            this.lineSymbolStyle.Size = new System.Drawing.Size(244, 21);
            this.lineSymbolStyle.TabIndex = 33;
            // 
            // lineSymbolColor
            // 
            this.lineSymbolColor.Location = new System.Drawing.Point(101, 316);
            this.lineSymbolColor.Name = "lineSymbolColor";
            this.lineSymbolColor.Size = new System.Drawing.Size(34, 36);
            this.lineSymbolColor.TabIndex = 32;
            // 
            // ptSymbolSize
            // 
            this.ptSymbolSize.Location = new System.Drawing.Point(499, 257);
            this.ptSymbolSize.Name = "ptSymbolSize";
            this.ptSymbolSize.Size = new System.Drawing.Size(72, 20);
            this.ptSymbolSize.TabIndex = 31;
            // 
            // ptSymbolColor
            // 
            this.ptSymbolColor.Location = new System.Drawing.Point(128, 257);
            this.ptSymbolColor.Name = "ptSymbolColor";
            this.ptSymbolColor.Size = new System.Drawing.Size(22, 24);
            this.ptSymbolColor.TabIndex = 30;
            // 
            // ptSymbolStyle
            // 
            this.ptSymbolStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ptSymbolStyle.FormattingEnabled = true;
            this.ptSymbolStyle.Location = new System.Drawing.Point(288, 257);
            this.ptSymbolStyle.Name = "ptSymbolStyle";
            this.ptSymbolStyle.Size = new System.Drawing.Size(205, 21);
            this.ptSymbolStyle.TabIndex = 29;
            // 
            // lineSymbolGraphic
            // 
            this.lineSymbolGraphic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lineSymbolGraphic.Location = new System.Drawing.Point(43, 307);
            this.lineSymbolGraphic.Name = "lineSymbolGraphic";
            this.lineSymbolGraphic.Size = new System.Drawing.Size(45, 45);
            this.lineSymbolGraphic.TabIndex = 28;
            // 
            // ptSymbolGraphic
            // 
            this.ptSymbolGraphic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ptSymbolGraphic.Location = new System.Drawing.Point(43, 237);
            this.ptSymbolGraphic.Margin = new System.Windows.Forms.Padding(0);
            this.ptSymbolGraphic.Name = "ptSymbolGraphic";
            this.ptSymbolGraphic.Size = new System.Drawing.Size(45, 45);
            this.ptSymbolGraphic.TabIndex = 27;
            // 
            // btnSplitSave
            // 
            this.btnSplitSave.AutoSize = true;
            this.btnSplitSave.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnSplitSave.Location = new System.Drawing.Point(565, 754);
            this.btnSplitSave.MaximumSize = new System.Drawing.Size(85, 28);
            this.btnSplitSave.MinimumSize = new System.Drawing.Size(85, 23);
            this.btnSplitSave.Name = "btnSplitSave";
            this.btnSplitSave.Size = new System.Drawing.Size(85, 28);
            this.btnSplitSave.TabIndex = 7;
            this.btnSplitSave.Text = "Save";
            this.btnSplitSave.UseVisualStyleBackColor = true;
            this.btnSplitSave.Click += new System.EventHandler(this.btnSplitSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.Location = new System.Drawing.Point(484, 754);
            this.btnCancel.MaximumSize = new System.Drawing.Size(75, 28);
            this.btnCancel.MinimumSize = new System.Drawing.Size(75, 23);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // AdminForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 786);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "AdminForm";
            this.Text = "Go2It Administration";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.adminTab_Control.ResumeLayout(false);
            this.adminTab_LayerManagement.ResumeLayout(false);
            this.adminLayerSplitter.Panel1.ResumeLayout(false);
            this.adminLayerSplitter.Panel2.ResumeLayout(false);
            this.adminLayerSplitter.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.adminLayerSplitter)).EndInit();
            this.adminLayerSplitter.ResumeLayout(false);
            this.legendSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.legendSplitter)).EndInit();
            this.legendSplitter.ResumeLayout(false);
            this.legendButtonTable.ResumeLayout(false);
            this.panelRadKeyLocations.ResumeLayout(false);
            this.panelRadKeyLocations.PerformLayout();
            this.panelRadAddress.ResumeLayout(false);
            this.panelRadAddress.PerformLayout();
            this.adminTab_ProgramManagement.ResumeLayout(false);
            this.adminTab_ProgramManagement.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHotKeys)).EndInit();
            this.adminTab_SearchProperties.ResumeLayout(false);
            this.adminTab_SearchProperties.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLayerIndex)).EndInit();
            this.adminTab_SymbologySettings.ResumeLayout(false);
            this.adminTab_SymbologySettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchHydrantDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchBufferDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchZoomFactor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchHydrantCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lineSymbolSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptSymbolSize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DotSpatial.SDR.Controls.SplitButton btnSplitSave;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabControl adminTab_Control;
        private System.Windows.Forms.TabPage adminTab_LayerManagement;
        private System.Windows.Forms.SplitContainer adminLayerSplitter;
        private System.Windows.Forms.SplitContainer legendSplitter;
        private System.Windows.Forms.TableLayoutPanel legendButtonTable;
        private System.Windows.Forms.Button btnRemoveLayer;
        private System.Windows.Forms.Button btnAddLayer;
        private System.Windows.Forms.Button btnAddView;
        private System.Windows.Forms.TextBox txtViewName;
        private System.Windows.Forms.Button btnDeleteView;
        private System.Windows.Forms.CheckedListBox chkViewLayers;
        private System.Windows.Forms.ComboBox cmbActiveMapTab;
        private System.Windows.Forms.Label lblMapBGColor;
        private System.Windows.Forms.Panel mapBGColorPanel;
        private System.Windows.Forms.Panel panelRadKeyLocations;
        private System.Windows.Forms.RadioButton radKeyLocationsPolygons;
        private System.Windows.Forms.RadioButton radKeyLocationsPoints;
        private System.Windows.Forms.Panel panelRadAddress;
        private System.Windows.Forms.RadioButton radAddressPoints;
        private System.Windows.Forms.RadioButton radAddressPolygons;
        private System.Windows.Forms.Label lblKeyLocations;
        private System.Windows.Forms.Label lblHydrants;
        private System.Windows.Forms.Label lblParcels;
        private System.Windows.Forms.Label lblEsn;
        private System.Windows.Forms.Label lblCellSector;
        private System.Windows.Forms.ComboBox cmbHydrantsLayer;
        private System.Windows.Forms.Label lblRoads;
        private System.Windows.Forms.Label lblAddresses;
        private System.Windows.Forms.CheckedListBox chkKeyLocationsLayers;
        private System.Windows.Forms.ComboBox cmbParcelsLayer;
        private System.Windows.Forms.ComboBox cmbESNLayer;
        private System.Windows.Forms.ComboBox cmbCellSectorLayer;
        private System.Windows.Forms.Label lblCityLimits;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.ComboBox cmbCityLimitLayer;
        private System.Windows.Forms.CheckedListBox chkRoadLayers;
        private System.Windows.Forms.CheckedListBox chkAddressLayers;
        private System.Windows.Forms.ComboBox cmbNotesLayer;
        private System.Windows.Forms.TabPage adminTab_SearchProperties;
        private System.Windows.Forms.DataGridView dgvLayerIndex;
        private System.Windows.Forms.Button btnCreateIndex;
        private System.Windows.Forms.ComboBox cmbLayerIndex;
        private System.Windows.Forms.CheckedListBox chkLayerIndex;
        private System.Windows.Forms.Button btnAddIndex;
        private System.Windows.Forms.CheckedListBox chkLayersToIndex;
        private System.Windows.Forms.ListBox lstExistingIndexes;
        private System.Windows.Forms.Button btnRemoveIndex;
        private System.Windows.Forms.Button btnDeleteIndex;
        private System.Windows.Forms.TabPage adminTab_ProgramManagement;
        private System.Windows.Forms.DataGridView dgvHotKeys;
        private System.Windows.Forms.Button btnUsersDelete;
        private System.Windows.Forms.Button btnUsersAddUpdate;
        private System.Windows.Forms.ListBox lstUsers;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtVerifyPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnSaveHotKeys;
        private System.Windows.Forms.CheckBox chkPretypes;
        private System.Windows.Forms.CheckBox chkEnableQuesryParserLog;
        private System.Windows.Forms.TabPage adminTab_SymbologySettings;
        private System.Windows.Forms.NumericUpDown searchHydrantDistance;
        private System.Windows.Forms.NumericUpDown searchBufferDistance;
        private System.Windows.Forms.NumericUpDown searchZoomFactor;
        private System.Windows.Forms.NumericUpDown searchHydrantCount;
        private System.Windows.Forms.Panel lineSymbolBorderColor;
        private System.Windows.Forms.ComboBox lineSymbolCap;
        private System.Windows.Forms.NumericUpDown lineSymbolSize;
        private System.Windows.Forms.ComboBox lineSymbolStyle;
        private System.Windows.Forms.Panel lineSymbolColor;
        private System.Windows.Forms.NumericUpDown ptSymbolSize;
        private System.Windows.Forms.Panel ptSymbolColor;
        private System.Windows.Forms.ComboBox ptSymbolStyle;
        private System.Windows.Forms.Panel lineSymbolGraphic;
        private System.Windows.Forms.Panel ptSymbolGraphic;
        private System.Windows.Forms.Label lblHydrantDist;
        private System.Windows.Forms.Label lblZoomFactor;
        private System.Windows.Forms.Label lblBufDist;
        private System.Windows.Forms.Label lblHydrantCount;
        private DotSpatial.Symbology.Forms.RampSlider lineSymbolColorSlider;
        private DotSpatial.Symbology.Forms.RampSlider ptSymbolColorSlider;

    }
}