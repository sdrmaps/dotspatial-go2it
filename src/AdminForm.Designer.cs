namespace Go2It
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.adminTab_Control = new System.Windows.Forms.TabControl();
            this.adminTab_MapConfiguration = new System.Windows.Forms.TabPage();
            this.adminLayerSplitter = new System.Windows.Forms.SplitContainer();
            this.legendSplitter = new System.Windows.Forms.SplitContainer();
            this.legendButtonTable = new System.Windows.Forms.TableLayoutPanel();
            this.btnRemoveLayer = new System.Windows.Forms.Button();
            this.btnAddLayer = new System.Windows.Forms.Button();
            this.btnAddView = new System.Windows.Forms.Button();
            this.txtViewName = new System.Windows.Forms.TextBox();
            this.btnDeleteView = new System.Windows.Forms.Button();
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
            this.adminTab_ProjectSettings = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.ptSymbolColorSlider = new DotSpatial.Symbology.Forms.RampSlider();
            this.lineSymbolColorSlider = new DotSpatial.Symbology.Forms.RampSlider();
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
            this.adminTab_SearchSettings = new System.Windows.Forms.TabPage();
            this.lblHydrantDist = new System.Windows.Forms.Label();
            this.lblZoomFactor = new System.Windows.Forms.Label();
            this.lblBufDist = new System.Windows.Forms.Label();
            this.lblHydrantCount = new System.Windows.Forms.Label();
            this.searchHydrantDistance = new System.Windows.Forms.NumericUpDown();
            this.searchBufferDistance = new System.Windows.Forms.NumericUpDown();
            this.searchZoomFactor = new System.Windows.Forms.NumericUpDown();
            this.searchHydrantCount = new System.Windows.Forms.NumericUpDown();
            this.chkEnableQueryParserLog = new System.Windows.Forms.CheckBox();
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
            this.adminTab_AliSettings = new System.Windows.Forms.TabPage();
            this.pnlAliEnterpolAvl = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.pnlAliAVLInactiveColor = new System.Windows.Forms.Panel();
            this.lblAliEnterpolAVLMyVehicleColor = new System.Windows.Forms.Label();
            this.pnlAliEnterpolAVLMyVehicleColor = new System.Windows.Forms.Panel();
            this.chkAutoHideInactiveUnits = new System.Windows.Forms.CheckBox();
            this.pnlAliEnterpolAVLPdGraphic = new System.Windows.Forms.Panel();
            this.lblAliEnterpolAVLTableName = new System.Windows.Forms.Label();
            this.pnlAliEnterpolAVLFdGraphic = new System.Windows.Forms.Panel();
            this.txtAliEnterpolAVLTableName = new System.Windows.Forms.TextBox();
            this.pnlAliEnterpolAVLEmsGraphic = new System.Windows.Forms.Panel();
            this.txtAliEnterpolAVLInitialCatalog = new System.Windows.Forms.TextBox();
            this.lblAliEnterpolAVLFontSize = new System.Windows.Forms.Label();
            this.txtAliEnterpolAVLSetLocProc = new System.Windows.Forms.TextBox();
            this.lblAliEnterpolAVLFontName = new System.Windows.Forms.Label();
            this.lblAliEnterpolAVLInitialCatalog = new System.Windows.Forms.Label();
            this.lblAliEnterpolAVLUpdateFreq = new System.Windows.Forms.Label();
            this.numAliEnterpolAVLUpdateFreq = new System.Windows.Forms.NumericUpDown();
            this.txtAliEnterpolAVLFdChars = new System.Windows.Forms.TextBox();
            this.txtAliEnterpolAVLWhoAmIProc = new System.Windows.Forms.TextBox();
            this.txtAliEnterpolAVLPdChars = new System.Windows.Forms.TextBox();
            this.lblAliEnterpolAVLSetLocProc = new System.Windows.Forms.Label();
            this.txtAliEnterpolAVLEmsChars = new System.Windows.Forms.TextBox();
            this.lblAliEnterpolAVLWhoAmIProc = new System.Windows.Forms.Label();
            this.btnAliEnterpolAVLFont = new System.Windows.Forms.Button();
            this.pnlAliEnterpolAVLFdColor = new System.Windows.Forms.Panel();
            this.lblAliEnterpolAVLReadFreq = new System.Windows.Forms.Label();
            this.numAliEnterpolAVLAge1Freq = new System.Windows.Forms.NumericUpDown();
            this.pnlAliEnterpolAVLPdColor = new System.Windows.Forms.Panel();
            this.numAliEnterpolAVLAge2Freq = new System.Windows.Forms.NumericUpDown();
            this.numAliEnterpolAVLReadFreq = new System.Windows.Forms.NumericUpDown();
            this.numAliEnterpolAVLAge3Freq = new System.Windows.Forms.NumericUpDown();
            this.pnlAliEnterpolAVLEmsColor = new System.Windows.Forms.Panel();
            this.lblAliEnterpolAVLAgeFreq = new System.Windows.Forms.Label();
            this.chkNetworkfleet = new System.Windows.Forms.CheckBox();
            this.pnlAliNetworkfleet = new System.Windows.Forms.Panel();
            this.ptAliNetworkfleetColor = new System.Windows.Forms.Panel();
            this.ptAliNetworkfleetSize = new System.Windows.Forms.Label();
            this.ptAliNetworkfleetFont = new System.Windows.Forms.Label();
            this.btnAliNetworkfleetFont = new System.Windows.Forms.Button();
            this.ptAliNetworkfleetChar = new System.Windows.Forms.TextBox();
            this.ptAliNetworkfleetGraphic = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.lblAliNetworkfleetUdpHost = new System.Windows.Forms.Label();
            this.numAliNetworkfleetUdpPort = new System.Windows.Forms.NumericUpDown();
            this.txtAliNetworkfleetUdpHost = new System.Windows.Forms.TextBox();
            this.btnAliValidate = new System.Windows.Forms.Button();
            this.pnlAliEnterpol = new System.Windows.Forms.Panel();
            this.label17 = new System.Windows.Forms.Label();
            this.chkEnterpolAvl = new System.Windows.Forms.CheckBox();
            this.lblAliEnterpolInitialCatalog = new System.Windows.Forms.Label();
            this.lblAliEnterpolTableName = new System.Windows.Forms.Label();
            this.lblAliEnterpolDataSource = new System.Windows.Forms.Label();
            this.txtAliEnterpolInitialCatalog = new System.Windows.Forms.TextBox();
            this.txtAliEnterpolDataSource = new System.Windows.Forms.TextBox();
            this.txtAliEnterpolTableName = new System.Windows.Forms.TextBox();
            this.pnlAliGlobalCad = new System.Windows.Forms.Panel();
            this.btnAliGlobalCadConfigIniBrowse = new System.Windows.Forms.Button();
            this.btnAliGlobalCadArchivePathBrowse = new System.Windows.Forms.Button();
            this.txtAliGlobalCadConfigIni = new System.Windows.Forms.TextBox();
            this.lblAliGlobalCadConfigIni = new System.Windows.Forms.Label();
            this.lblAliGlobalCadArchivePath = new System.Windows.Forms.Label();
            this.txtAliGlobalCadArchivePath = new System.Windows.Forms.TextBox();
            this.lblAliGlobalCadLogPath = new System.Windows.Forms.Label();
            this.btnAliGlobalCadLogPathBrowse = new System.Windows.Forms.Button();
            this.txtAliGlobalCadLogPath = new System.Windows.Forms.TextBox();
            this.pnlAliSdrAliServer = new System.Windows.Forms.Panel();
            this.numAliInterfaceUdpPort = new System.Windows.Forms.NumericUpDown();
            this.lblAliInterfaceUdpPort = new System.Windows.Forms.Label();
            this.lblAliInterfaceUdpHost = new System.Windows.Forms.Label();
            this.lblAliInterfaceDbPath = new System.Windows.Forms.Label();
            this.txtAliInterfaceUdpHost = new System.Windows.Forms.TextBox();
            this.btnAliInterfaceDbPathBrowse = new System.Windows.Forms.Button();
            this.txtAliInterfaceDbPath = new System.Windows.Forms.TextBox();
            this.cmbAliMode = new System.Windows.Forms.ComboBox();
            this.btnSplitSave = new DotSpatial.SDR.Controls.SplitButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.adminTab_Control.SuspendLayout();
            this.adminTab_MapConfiguration.SuspendLayout();
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
            this.adminTab_ProjectSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lineSymbolSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptSymbolSize)).BeginInit();
            this.adminTab_ProgramManagement.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHotKeys)).BeginInit();
            this.adminTab_SearchSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchHydrantDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchBufferDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchZoomFactor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchHydrantCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLayerIndex)).BeginInit();
            this.adminTab_AliSettings.SuspendLayout();
            this.pnlAliEnterpolAvl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLUpdateFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLAge1Freq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLAge2Freq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLReadFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLAge3Freq)).BeginInit();
            this.pnlAliNetworkfleet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAliNetworkfleetUdpPort)).BeginInit();
            this.pnlAliEnterpol.SuspendLayout();
            this.pnlAliGlobalCad.SuspendLayout();
            this.pnlAliSdrAliServer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAliInterfaceUdpPort)).BeginInit();
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(609, 794);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // adminTab_Control
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.adminTab_Control, 2);
            this.adminTab_Control.Controls.Add(this.adminTab_MapConfiguration);
            this.adminTab_Control.Controls.Add(this.adminTab_ProjectSettings);
            this.adminTab_Control.Controls.Add(this.adminTab_ProgramManagement);
            this.adminTab_Control.Controls.Add(this.adminTab_SearchSettings);
            this.adminTab_Control.Controls.Add(this.adminTab_AliSettings);
            this.adminTab_Control.Dock = System.Windows.Forms.DockStyle.Fill;
            this.adminTab_Control.Location = new System.Drawing.Point(0, 0);
            this.adminTab_Control.Margin = new System.Windows.Forms.Padding(0);
            this.adminTab_Control.Name = "adminTab_Control";
            this.adminTab_Control.Padding = new System.Drawing.Point(0, 0);
            this.adminTab_Control.SelectedIndex = 0;
            this.adminTab_Control.Size = new System.Drawing.Size(609, 759);
            this.adminTab_Control.TabIndex = 8;
            // 
            // adminTab_MapConfiguration
            // 
            this.adminTab_MapConfiguration.BackColor = System.Drawing.Color.Transparent;
            this.adminTab_MapConfiguration.Controls.Add(this.adminLayerSplitter);
            this.adminTab_MapConfiguration.Location = new System.Drawing.Point(4, 22);
            this.adminTab_MapConfiguration.Margin = new System.Windows.Forms.Padding(0);
            this.adminTab_MapConfiguration.Name = "adminTab_MapConfiguration";
            this.adminTab_MapConfiguration.Size = new System.Drawing.Size(601, 729);
            this.adminTab_MapConfiguration.TabIndex = 0;
            this.adminTab_MapConfiguration.Text = "Map Configuration";
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
            this.adminLayerSplitter.Size = new System.Drawing.Size(601, 729);
            this.adminLayerSplitter.SplitterDistance = 191;
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
            this.legendSplitter.Size = new System.Drawing.Size(189, 727);
            this.legendSplitter.SplitterDistance = 693;
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
            this.legendButtonTable.Size = new System.Drawing.Size(189, 33);
            this.legendButtonTable.TabIndex = 0;
            // 
            // btnRemoveLayer
            // 
            this.btnRemoveLayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRemoveLayer.Location = new System.Drawing.Point(97, 3);
            this.btnRemoveLayer.MaximumSize = new System.Drawing.Size(0, 27);
            this.btnRemoveLayer.MinimumSize = new System.Drawing.Size(55, 23);
            this.btnRemoveLayer.Name = "btnRemoveLayer";
            this.btnRemoveLayer.Size = new System.Drawing.Size(89, 27);
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
            this.btnAddLayer.Size = new System.Drawing.Size(88, 27);
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
            this.btnDeleteView.Location = new System.Drawing.Point(250, 35);
            this.btnDeleteView.Name = "btnDeleteView";
            this.btnDeleteView.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteView.TabIndex = 42;
            this.btnDeleteView.Text = "Delete View";
            this.btnDeleteView.UseVisualStyleBackColor = true;
            this.btnDeleteView.Click += new System.EventHandler(this.btnRemoveView_Click);
            // 
            // cmbActiveMapTab
            // 
            this.cmbActiveMapTab.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActiveMapTab.FormattingEnabled = true;
            this.cmbActiveMapTab.Location = new System.Drawing.Point(32, 36);
            this.cmbActiveMapTab.Name = "cmbActiveMapTab";
            this.cmbActiveMapTab.Size = new System.Drawing.Size(212, 21);
            this.cmbActiveMapTab.TabIndex = 40;
            // 
            // lblMapBGColor
            // 
            this.lblMapBGColor.AutoSize = true;
            this.lblMapBGColor.Location = new System.Drawing.Point(180, 542);
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
            this.mapBGColorPanel.Location = new System.Drawing.Point(113, 534);
            this.mapBGColorPanel.Name = "mapBGColorPanel";
            this.mapBGColorPanel.Size = new System.Drawing.Size(48, 26);
            this.mapBGColorPanel.TabIndex = 37;
            this.mapBGColorPanel.Click += new System.EventHandler(this.mapBGColorPanel_Click);
            // 
            // panelRadKeyLocations
            // 
            this.panelRadKeyLocations.Controls.Add(this.radKeyLocationsPolygons);
            this.panelRadKeyLocations.Controls.Add(this.radKeyLocationsPoints);
            this.panelRadKeyLocations.Location = new System.Drawing.Point(33, 451);
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
            this.panelRadAddress.Location = new System.Drawing.Point(33, 89);
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
            this.lblKeyLocations.Location = new System.Drawing.Point(30, 435);
            this.lblKeyLocations.Name = "lblKeyLocations";
            this.lblKeyLocations.Size = new System.Drawing.Size(77, 13);
            this.lblKeyLocations.TabIndex = 32;
            this.lblKeyLocations.Text = "Key Locations:";
            // 
            // lblHydrants
            // 
            this.lblHydrants.AutoSize = true;
            this.lblHydrants.Location = new System.Drawing.Point(55, 411);
            this.lblHydrants.Name = "lblHydrants";
            this.lblHydrants.Size = new System.Drawing.Size(52, 13);
            this.lblHydrants.TabIndex = 31;
            this.lblHydrants.Text = "Hydrants:";
            // 
            // lblParcels
            // 
            this.lblParcels.AutoSize = true;
            this.lblParcels.Location = new System.Drawing.Point(63, 384);
            this.lblParcels.Name = "lblParcels";
            this.lblParcels.Size = new System.Drawing.Size(45, 13);
            this.lblParcels.TabIndex = 30;
            this.lblParcels.Text = "Parcels:";
            // 
            // lblEsn
            // 
            this.lblEsn.AutoSize = true;
            this.lblEsn.Location = new System.Drawing.Point(78, 357);
            this.lblEsn.Name = "lblEsn";
            this.lblEsn.Size = new System.Drawing.Size(32, 13);
            this.lblEsn.TabIndex = 29;
            this.lblEsn.Text = "ESN:";
            // 
            // lblCellSector
            // 
            this.lblCellSector.AutoSize = true;
            this.lblCellSector.Location = new System.Drawing.Point(41, 330);
            this.lblCellSector.Name = "lblCellSector";
            this.lblCellSector.Size = new System.Drawing.Size(66, 13);
            this.lblCellSector.TabIndex = 27;
            this.lblCellSector.Text = "Cell Sectors:";
            // 
            // cmbHydrantsLayer
            // 
            this.cmbHydrantsLayer.FormattingEnabled = true;
            this.cmbHydrantsLayer.Location = new System.Drawing.Point(113, 408);
            this.cmbHydrantsLayer.Name = "cmbHydrantsLayer";
            this.cmbHydrantsLayer.Size = new System.Drawing.Size(199, 21);
            this.cmbHydrantsLayer.TabIndex = 26;
            this.cmbHydrantsLayer.SelectedIndexChanged += new System.EventHandler(this.cmbHydrantsLayer_SelectedIndexChanged);
            // 
            // lblRoads
            // 
            this.lblRoads.AutoSize = true;
            this.lblRoads.Location = new System.Drawing.Point(66, 173);
            this.lblRoads.Name = "lblRoads";
            this.lblRoads.Size = new System.Drawing.Size(41, 13);
            this.lblRoads.TabIndex = 24;
            this.lblRoads.Text = "Roads:";
            // 
            // lblAddresses
            // 
            this.lblAddresses.AutoSize = true;
            this.lblAddresses.Location = new System.Drawing.Point(51, 73);
            this.lblAddresses.Name = "lblAddresses";
            this.lblAddresses.Size = new System.Drawing.Size(59, 13);
            this.lblAddresses.TabIndex = 23;
            this.lblAddresses.Text = "Addresses:";
            // 
            // chkKeyLocationsLayers
            // 
            this.chkKeyLocationsLayers.CheckOnClick = true;
            this.chkKeyLocationsLayers.FormattingEnabled = true;
            this.chkKeyLocationsLayers.Location = new System.Drawing.Point(113, 435);
            this.chkKeyLocationsLayers.Name = "chkKeyLocationsLayers";
            this.chkKeyLocationsLayers.Size = new System.Drawing.Size(199, 94);
            this.chkKeyLocationsLayers.TabIndex = 22;
            this.chkKeyLocationsLayers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkKeyLocationsLayers_ItemCheck);
            // 
            // cmbParcelsLayer
            // 
            this.cmbParcelsLayer.FormattingEnabled = true;
            this.cmbParcelsLayer.Location = new System.Drawing.Point(113, 381);
            this.cmbParcelsLayer.Name = "cmbParcelsLayer";
            this.cmbParcelsLayer.Size = new System.Drawing.Size(199, 21);
            this.cmbParcelsLayer.TabIndex = 21;
            this.cmbParcelsLayer.SelectedIndexChanged += new System.EventHandler(this.cmbParcelsLayer_SelectedIndexChanged);
            // 
            // cmbESNLayer
            // 
            this.cmbESNLayer.FormattingEnabled = true;
            this.cmbESNLayer.Location = new System.Drawing.Point(113, 354);
            this.cmbESNLayer.Name = "cmbESNLayer";
            this.cmbESNLayer.Size = new System.Drawing.Size(199, 21);
            this.cmbESNLayer.TabIndex = 20;
            this.cmbESNLayer.SelectedIndexChanged += new System.EventHandler(this.cmbESNLayer_SelectedIndexChanged);
            // 
            // cmbCellSectorLayer
            // 
            this.cmbCellSectorLayer.FormattingEnabled = true;
            this.cmbCellSectorLayer.Location = new System.Drawing.Point(113, 327);
            this.cmbCellSectorLayer.Name = "cmbCellSectorLayer";
            this.cmbCellSectorLayer.Size = new System.Drawing.Size(199, 21);
            this.cmbCellSectorLayer.TabIndex = 18;
            this.cmbCellSectorLayer.SelectedIndexChanged += new System.EventHandler(this.cmbCellSectorLayer_SelectedIndexChanged);
            // 
            // lblCityLimits
            // 
            this.lblCityLimits.AutoSize = true;
            this.lblCityLimits.Location = new System.Drawing.Point(51, 303);
            this.lblCityLimits.Name = "lblCityLimits";
            this.lblCityLimits.Size = new System.Drawing.Size(56, 13);
            this.lblCityLimits.TabIndex = 16;
            this.lblCityLimits.Text = "City Limits:";
            // 
            // lblNotes
            // 
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(69, 276);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(38, 13);
            this.lblNotes.TabIndex = 15;
            this.lblNotes.Text = "Notes:";
            // 
            // cmbCityLimitLayer
            // 
            this.cmbCityLimitLayer.FormattingEnabled = true;
            this.cmbCityLimitLayer.Location = new System.Drawing.Point(113, 300);
            this.cmbCityLimitLayer.Name = "cmbCityLimitLayer";
            this.cmbCityLimitLayer.Size = new System.Drawing.Size(199, 21);
            this.cmbCityLimitLayer.TabIndex = 8;
            this.cmbCityLimitLayer.SelectedIndexChanged += new System.EventHandler(this.cmbCityLimitLayer_SelectedIndexChanged);
            // 
            // chkRoadLayers
            // 
            this.chkRoadLayers.CheckOnClick = true;
            this.chkRoadLayers.FormattingEnabled = true;
            this.chkRoadLayers.Location = new System.Drawing.Point(113, 173);
            this.chkRoadLayers.Name = "chkRoadLayers";
            this.chkRoadLayers.Size = new System.Drawing.Size(199, 94);
            this.chkRoadLayers.TabIndex = 7;
            this.chkRoadLayers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkRoadLayers_ItemCheck);
            // 
            // chkAddressLayers
            // 
            this.chkAddressLayers.CheckOnClick = true;
            this.chkAddressLayers.FormattingEnabled = true;
            this.chkAddressLayers.Location = new System.Drawing.Point(113, 73);
            this.chkAddressLayers.Name = "chkAddressLayers";
            this.chkAddressLayers.Size = new System.Drawing.Size(199, 94);
            this.chkAddressLayers.TabIndex = 6;
            this.chkAddressLayers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkAddressLayers_ItemCheck);
            // 
            // cmbNotesLayer
            // 
            this.cmbNotesLayer.FormattingEnabled = true;
            this.cmbNotesLayer.Location = new System.Drawing.Point(113, 273);
            this.cmbNotesLayer.Name = "cmbNotesLayer";
            this.cmbNotesLayer.Size = new System.Drawing.Size(199, 21);
            this.cmbNotesLayer.TabIndex = 5;
            this.cmbNotesLayer.SelectedIndexChanged += new System.EventHandler(this.cmbNotesLayer_SelectedIndexChanged);
            // 
            // adminTab_ProjectSettings
            // 
            this.adminTab_ProjectSettings.Controls.Add(this.label4);
            this.adminTab_ProjectSettings.Controls.Add(this.ptSymbolColorSlider);
            this.adminTab_ProjectSettings.Controls.Add(this.lineSymbolColorSlider);
            this.adminTab_ProjectSettings.Controls.Add(this.lineSymbolBorderColor);
            this.adminTab_ProjectSettings.Controls.Add(this.lineSymbolCap);
            this.adminTab_ProjectSettings.Controls.Add(this.lineSymbolSize);
            this.adminTab_ProjectSettings.Controls.Add(this.lineSymbolStyle);
            this.adminTab_ProjectSettings.Controls.Add(this.lineSymbolColor);
            this.adminTab_ProjectSettings.Controls.Add(this.ptSymbolSize);
            this.adminTab_ProjectSettings.Controls.Add(this.ptSymbolColor);
            this.adminTab_ProjectSettings.Controls.Add(this.ptSymbolStyle);
            this.adminTab_ProjectSettings.Controls.Add(this.lineSymbolGraphic);
            this.adminTab_ProjectSettings.Controls.Add(this.ptSymbolGraphic);
            this.adminTab_ProjectSettings.Location = new System.Drawing.Point(4, 22);
            this.adminTab_ProjectSettings.Name = "adminTab_ProjectSettings";
            this.adminTab_ProjectSettings.Padding = new System.Windows.Forms.Padding(3);
            this.adminTab_ProjectSettings.Size = new System.Drawing.Size(601, 729);
            this.adminTab_ProjectSettings.TabIndex = 5;
            this.adminTab_ProjectSettings.Text = "Project Settings";
            this.adminTab_ProjectSettings.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(55, 219);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(268, 13);
            this.label4.TabIndex = 48;
            this.label4.Text = "TODO: add settings for label orientation and what else?";
            // 
            // ptSymbolColorSlider
            // 
            this.ptSymbolColorSlider.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ptSymbolColorSlider.ColorButton = null;
            this.ptSymbolColorSlider.FlipRamp = false;
            this.ptSymbolColorSlider.FlipText = false;
            this.ptSymbolColorSlider.InvertRamp = false;
            this.ptSymbolColorSlider.Location = new System.Drawing.Point(148, 35);
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
            this.lineSymbolColorSlider.Location = new System.Drawing.Point(148, 107);
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
            // lineSymbolBorderColor
            // 
            this.lineSymbolBorderColor.Location = new System.Drawing.Point(93, 140);
            this.lineSymbolBorderColor.Name = "lineSymbolBorderColor";
            this.lineSymbolBorderColor.Size = new System.Drawing.Size(34, 36);
            this.lineSymbolBorderColor.TabIndex = 36;
            // 
            // lineSymbolCap
            // 
            this.lineSymbolCap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lineSymbolCap.FormattingEnabled = true;
            this.lineSymbolCap.Location = new System.Drawing.Point(148, 140);
            this.lineSymbolCap.Name = "lineSymbolCap";
            this.lineSymbolCap.Size = new System.Drawing.Size(206, 21);
            this.lineSymbolCap.TabIndex = 35;
            // 
            // lineSymbolSize
            // 
            this.lineSymbolSize.Location = new System.Drawing.Point(547, 110);
            this.lineSymbolSize.Name = "lineSymbolSize";
            this.lineSymbolSize.Size = new System.Drawing.Size(46, 20);
            this.lineSymbolSize.TabIndex = 34;
            // 
            // lineSymbolStyle
            // 
            this.lineSymbolStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lineSymbolStyle.FormattingEnabled = true;
            this.lineSymbolStyle.Location = new System.Drawing.Point(297, 109);
            this.lineSymbolStyle.Name = "lineSymbolStyle";
            this.lineSymbolStyle.Size = new System.Drawing.Size(244, 21);
            this.lineSymbolStyle.TabIndex = 33;
            // 
            // lineSymbolColor
            // 
            this.lineSymbolColor.Location = new System.Drawing.Point(93, 94);
            this.lineSymbolColor.Name = "lineSymbolColor";
            this.lineSymbolColor.Size = new System.Drawing.Size(34, 36);
            this.lineSymbolColor.TabIndex = 32;
            // 
            // ptSymbolSize
            // 
            this.ptSymbolSize.Location = new System.Drawing.Point(491, 35);
            this.ptSymbolSize.Name = "ptSymbolSize";
            this.ptSymbolSize.Size = new System.Drawing.Size(72, 20);
            this.ptSymbolSize.TabIndex = 31;
            // 
            // ptSymbolColor
            // 
            this.ptSymbolColor.Location = new System.Drawing.Point(120, 35);
            this.ptSymbolColor.Name = "ptSymbolColor";
            this.ptSymbolColor.Size = new System.Drawing.Size(22, 24);
            this.ptSymbolColor.TabIndex = 30;
            // 
            // ptSymbolStyle
            // 
            this.ptSymbolStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ptSymbolStyle.FormattingEnabled = true;
            this.ptSymbolStyle.Location = new System.Drawing.Point(280, 35);
            this.ptSymbolStyle.Name = "ptSymbolStyle";
            this.ptSymbolStyle.Size = new System.Drawing.Size(205, 21);
            this.ptSymbolStyle.TabIndex = 29;
            // 
            // lineSymbolGraphic
            // 
            this.lineSymbolGraphic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lineSymbolGraphic.Location = new System.Drawing.Point(35, 85);
            this.lineSymbolGraphic.Name = "lineSymbolGraphic";
            this.lineSymbolGraphic.Size = new System.Drawing.Size(45, 45);
            this.lineSymbolGraphic.TabIndex = 28;
            // 
            // ptSymbolGraphic
            // 
            this.ptSymbolGraphic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ptSymbolGraphic.Location = new System.Drawing.Point(35, 15);
            this.ptSymbolGraphic.Margin = new System.Windows.Forms.Padding(0);
            this.ptSymbolGraphic.Name = "ptSymbolGraphic";
            this.ptSymbolGraphic.Size = new System.Drawing.Size(45, 45);
            this.ptSymbolGraphic.TabIndex = 27;
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
            this.adminTab_ProgramManagement.Size = new System.Drawing.Size(601, 729);
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
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvHotKeys.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvHotKeys.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvHotKeys.DefaultCellStyle = dataGridViewCellStyle8;
            this.dgvHotKeys.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvHotKeys.Location = new System.Drawing.Point(24, 218);
            this.dgvHotKeys.MultiSelect = false;
            this.dgvHotKeys.Name = "dgvHotKeys";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvHotKeys.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
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
            // adminTab_SearchSettings
            // 
            this.adminTab_SearchSettings.Controls.Add(this.lblHydrantDist);
            this.adminTab_SearchSettings.Controls.Add(this.lblZoomFactor);
            this.adminTab_SearchSettings.Controls.Add(this.lblBufDist);
            this.adminTab_SearchSettings.Controls.Add(this.lblHydrantCount);
            this.adminTab_SearchSettings.Controls.Add(this.searchHydrantDistance);
            this.adminTab_SearchSettings.Controls.Add(this.searchBufferDistance);
            this.adminTab_SearchSettings.Controls.Add(this.searchZoomFactor);
            this.adminTab_SearchSettings.Controls.Add(this.searchHydrantCount);
            this.adminTab_SearchSettings.Controls.Add(this.chkEnableQueryParserLog);
            this.adminTab_SearchSettings.Controls.Add(this.chkPretypes);
            this.adminTab_SearchSettings.Controls.Add(this.btnRemoveIndex);
            this.adminTab_SearchSettings.Controls.Add(this.lstExistingIndexes);
            this.adminTab_SearchSettings.Controls.Add(this.chkLayersToIndex);
            this.adminTab_SearchSettings.Controls.Add(this.btnAddIndex);
            this.adminTab_SearchSettings.Controls.Add(this.dgvLayerIndex);
            this.adminTab_SearchSettings.Controls.Add(this.btnDeleteIndex);
            this.adminTab_SearchSettings.Controls.Add(this.btnCreateIndex);
            this.adminTab_SearchSettings.Controls.Add(this.cmbLayerIndex);
            this.adminTab_SearchSettings.Controls.Add(this.chkLayerIndex);
            this.adminTab_SearchSettings.Location = new System.Drawing.Point(4, 22);
            this.adminTab_SearchSettings.Name = "adminTab_SearchSettings";
            this.adminTab_SearchSettings.Padding = new System.Windows.Forms.Padding(3);
            this.adminTab_SearchSettings.Size = new System.Drawing.Size(601, 729);
            this.adminTab_SearchSettings.TabIndex = 4;
            this.adminTab_SearchSettings.Text = "Search Settings";
            this.adminTab_SearchSettings.UseVisualStyleBackColor = true;
            // 
            // lblHydrantDist
            // 
            this.lblHydrantDist.AutoSize = true;
            this.lblHydrantDist.Location = new System.Drawing.Point(103, 616);
            this.lblHydrantDist.Name = "lblHydrantDist";
            this.lblHydrantDist.Size = new System.Drawing.Size(89, 13);
            this.lblHydrantDist.TabIndex = 52;
            this.lblHydrantDist.Text = "Hydrant Distance";
            // 
            // lblZoomFactor
            // 
            this.lblZoomFactor.AutoSize = true;
            this.lblZoomFactor.Location = new System.Drawing.Point(199, 616);
            this.lblZoomFactor.Name = "lblZoomFactor";
            this.lblZoomFactor.Size = new System.Drawing.Size(67, 13);
            this.lblZoomFactor.TabIndex = 51;
            this.lblZoomFactor.Text = "Zoom Factor";
            // 
            // lblBufDist
            // 
            this.lblBufDist.AutoSize = true;
            this.lblBufDist.Location = new System.Drawing.Point(365, 616);
            this.lblBufDist.Name = "lblBufDist";
            this.lblBufDist.Size = new System.Drawing.Size(117, 13);
            this.lblBufDist.TabIndex = 50;
            this.lblBufDist.Text = "Search Buffer Distance";
            // 
            // lblHydrantCount
            // 
            this.lblHydrantCount.AutoSize = true;
            this.lblHydrantCount.Location = new System.Drawing.Point(284, 616);
            this.lblHydrantCount.Name = "lblHydrantCount";
            this.lblHydrantCount.Size = new System.Drawing.Size(75, 13);
            this.lblHydrantCount.TabIndex = 49;
            this.lblHydrantCount.Text = "Hydrant Count";
            // 
            // searchHydrantDistance
            // 
            this.searchHydrantDistance.Location = new System.Drawing.Point(106, 635);
            this.searchHydrantDistance.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.searchHydrantDistance.Name = "searchHydrantDistance";
            this.searchHydrantDistance.Size = new System.Drawing.Size(78, 20);
            this.searchHydrantDistance.TabIndex = 48;
            // 
            // searchBufferDistance
            // 
            this.searchBufferDistance.Location = new System.Drawing.Point(373, 632);
            this.searchBufferDistance.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.searchBufferDistance.Name = "searchBufferDistance";
            this.searchBufferDistance.Size = new System.Drawing.Size(77, 20);
            this.searchBufferDistance.TabIndex = 47;
            // 
            // searchZoomFactor
            // 
            this.searchZoomFactor.DecimalPlaces = 2;
            this.searchZoomFactor.Increment = new decimal(new int[] {
            25,
            0,
            0,
            131072});
            this.searchZoomFactor.Location = new System.Drawing.Point(200, 632);
            this.searchZoomFactor.Name = "searchZoomFactor";
            this.searchZoomFactor.Size = new System.Drawing.Size(66, 20);
            this.searchZoomFactor.TabIndex = 46;
            // 
            // searchHydrantCount
            // 
            this.searchHydrantCount.Location = new System.Drawing.Point(287, 632);
            this.searchHydrantCount.Name = "searchHydrantCount";
            this.searchHydrantCount.Size = new System.Drawing.Size(66, 20);
            this.searchHydrantCount.TabIndex = 45;
            // 
            // chkEnableQueryParserLog
            // 
            this.chkEnableQueryParserLog.AutoSize = true;
            this.chkEnableQueryParserLog.Location = new System.Drawing.Point(33, 561);
            this.chkEnableQueryParserLog.Name = "chkEnableQueryParserLog";
            this.chkEnableQueryParserLog.Size = new System.Drawing.Size(164, 17);
            this.chkEnableQueryParserLog.TabIndex = 12;
            this.chkEnableQueryParserLog.Text = "Enable Query Parser Logging";
            this.chkEnableQueryParserLog.UseVisualStyleBackColor = true;
            // 
            // chkPretypes
            // 
            this.chkPretypes.AutoSize = true;
            this.chkPretypes.Location = new System.Drawing.Point(33, 538);
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
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLayerIndex.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dgvLayerIndex.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLayerIndex.DefaultCellStyle = dataGridViewCellStyle11;
            this.dgvLayerIndex.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvLayerIndex.Location = new System.Drawing.Point(202, 39);
            this.dgvLayerIndex.MultiSelect = false;
            this.dgvLayerIndex.Name = "dgvLayerIndex";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLayerIndex.RowHeadersDefaultCellStyle = dataGridViewCellStyle12;
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
            // adminTab_AliSettings
            // 
            this.adminTab_AliSettings.Controls.Add(this.pnlAliEnterpolAvl);
            this.adminTab_AliSettings.Controls.Add(this.chkNetworkfleet);
            this.adminTab_AliSettings.Controls.Add(this.pnlAliNetworkfleet);
            this.adminTab_AliSettings.Controls.Add(this.btnAliValidate);
            this.adminTab_AliSettings.Controls.Add(this.pnlAliEnterpol);
            this.adminTab_AliSettings.Controls.Add(this.pnlAliGlobalCad);
            this.adminTab_AliSettings.Controls.Add(this.pnlAliSdrAliServer);
            this.adminTab_AliSettings.Controls.Add(this.cmbAliMode);
            this.adminTab_AliSettings.Location = new System.Drawing.Point(4, 22);
            this.adminTab_AliSettings.Name = "adminTab_AliSettings";
            this.adminTab_AliSettings.Padding = new System.Windows.Forms.Padding(3);
            this.adminTab_AliSettings.Size = new System.Drawing.Size(601, 733);
            this.adminTab_AliSettings.TabIndex = 6;
            this.adminTab_AliSettings.Text = "ALI Settings";
            this.adminTab_AliSettings.UseVisualStyleBackColor = true;
            // 
            // pnlAliEnterpolAvl
            // 
            this.pnlAliEnterpolAvl.Controls.Add(this.label5);
            this.pnlAliEnterpolAvl.Controls.Add(this.pnlAliAVLInactiveColor);
            this.pnlAliEnterpolAvl.Controls.Add(this.lblAliEnterpolAVLMyVehicleColor);
            this.pnlAliEnterpolAvl.Controls.Add(this.pnlAliEnterpolAVLMyVehicleColor);
            this.pnlAliEnterpolAvl.Controls.Add(this.chkAutoHideInactiveUnits);
            this.pnlAliEnterpolAvl.Controls.Add(this.pnlAliEnterpolAVLPdGraphic);
            this.pnlAliEnterpolAvl.Controls.Add(this.lblAliEnterpolAVLTableName);
            this.pnlAliEnterpolAvl.Controls.Add(this.pnlAliEnterpolAVLFdGraphic);
            this.pnlAliEnterpolAvl.Controls.Add(this.txtAliEnterpolAVLTableName);
            this.pnlAliEnterpolAvl.Controls.Add(this.pnlAliEnterpolAVLEmsGraphic);
            this.pnlAliEnterpolAvl.Controls.Add(this.txtAliEnterpolAVLInitialCatalog);
            this.pnlAliEnterpolAvl.Controls.Add(this.lblAliEnterpolAVLFontSize);
            this.pnlAliEnterpolAvl.Controls.Add(this.txtAliEnterpolAVLSetLocProc);
            this.pnlAliEnterpolAvl.Controls.Add(this.lblAliEnterpolAVLFontName);
            this.pnlAliEnterpolAvl.Controls.Add(this.lblAliEnterpolAVLInitialCatalog);
            this.pnlAliEnterpolAvl.Controls.Add(this.lblAliEnterpolAVLUpdateFreq);
            this.pnlAliEnterpolAvl.Controls.Add(this.numAliEnterpolAVLUpdateFreq);
            this.pnlAliEnterpolAvl.Controls.Add(this.txtAliEnterpolAVLFdChars);
            this.pnlAliEnterpolAvl.Controls.Add(this.txtAliEnterpolAVLWhoAmIProc);
            this.pnlAliEnterpolAvl.Controls.Add(this.txtAliEnterpolAVLPdChars);
            this.pnlAliEnterpolAvl.Controls.Add(this.lblAliEnterpolAVLSetLocProc);
            this.pnlAliEnterpolAvl.Controls.Add(this.txtAliEnterpolAVLEmsChars);
            this.pnlAliEnterpolAvl.Controls.Add(this.lblAliEnterpolAVLWhoAmIProc);
            this.pnlAliEnterpolAvl.Controls.Add(this.btnAliEnterpolAVLFont);
            this.pnlAliEnterpolAvl.Controls.Add(this.pnlAliEnterpolAVLFdColor);
            this.pnlAliEnterpolAvl.Controls.Add(this.lblAliEnterpolAVLReadFreq);
            this.pnlAliEnterpolAvl.Controls.Add(this.numAliEnterpolAVLAge1Freq);
            this.pnlAliEnterpolAvl.Controls.Add(this.pnlAliEnterpolAVLPdColor);
            this.pnlAliEnterpolAvl.Controls.Add(this.numAliEnterpolAVLAge2Freq);
            this.pnlAliEnterpolAvl.Controls.Add(this.numAliEnterpolAVLReadFreq);
            this.pnlAliEnterpolAvl.Controls.Add(this.numAliEnterpolAVLAge3Freq);
            this.pnlAliEnterpolAvl.Controls.Add(this.pnlAliEnterpolAVLEmsColor);
            this.pnlAliEnterpolAvl.Controls.Add(this.lblAliEnterpolAVLAgeFreq);
            this.pnlAliEnterpolAvl.Location = new System.Drawing.Point(128, 335);
            this.pnlAliEnterpolAvl.Name = "pnlAliEnterpolAvl";
            this.pnlAliEnterpolAvl.Size = new System.Drawing.Size(449, 388);
            this.pnlAliEnterpolAvl.TabIndex = 72;
            this.pnlAliEnterpolAvl.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(294, 157);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 13);
            this.label5.TabIndex = 88;
            this.label5.Text = "Inactive Vehicle Color";
            // 
            // pnlAliAVLInactiveColor
            // 
            this.pnlAliAVLInactiveColor.Location = new System.Drawing.Point(322, 173);
            this.pnlAliAVLInactiveColor.Name = "pnlAliAVLInactiveColor";
            this.pnlAliAVLInactiveColor.Size = new System.Drawing.Size(44, 44);
            this.pnlAliAVLInactiveColor.TabIndex = 87;
            // 
            // lblAliEnterpolAVLMyVehicleColor
            // 
            this.lblAliEnterpolAVLMyVehicleColor.AutoSize = true;
            this.lblAliEnterpolAVLMyVehicleColor.Location = new System.Drawing.Point(312, 221);
            this.lblAliEnterpolAVLMyVehicleColor.Name = "lblAliEnterpolAVLMyVehicleColor";
            this.lblAliEnterpolAVLMyVehicleColor.Size = new System.Drawing.Size(86, 13);
            this.lblAliEnterpolAVLMyVehicleColor.TabIndex = 86;
            this.lblAliEnterpolAVLMyVehicleColor.Text = "My Vehicle Color";
            // 
            // pnlAliEnterpolAVLMyVehicleColor
            // 
            this.pnlAliEnterpolAVLMyVehicleColor.Location = new System.Drawing.Point(342, 242);
            this.pnlAliEnterpolAVLMyVehicleColor.Name = "pnlAliEnterpolAVLMyVehicleColor";
            this.pnlAliEnterpolAVLMyVehicleColor.Size = new System.Drawing.Size(29, 30);
            this.pnlAliEnterpolAVLMyVehicleColor.TabIndex = 85;
            // 
            // chkAutoHideInactiveUnits
            // 
            this.chkAutoHideInactiveUnits.AutoSize = true;
            this.chkAutoHideInactiveUnits.Location = new System.Drawing.Point(275, 75);
            this.chkAutoHideInactiveUnits.Name = "chkAutoHideInactiveUnits";
            this.chkAutoHideInactiveUnits.Size = new System.Drawing.Size(136, 17);
            this.chkAutoHideInactiveUnits.TabIndex = 84;
            this.chkAutoHideInactiveUnits.Text = "Autohide Inactive Units";
            this.chkAutoHideInactiveUnits.UseVisualStyleBackColor = true;
            // 
            // pnlAliEnterpolAVLPdGraphic
            // 
            this.pnlAliEnterpolAVLPdGraphic.Location = new System.Drawing.Point(56, 195);
            this.pnlAliEnterpolAVLPdGraphic.Name = "pnlAliEnterpolAVLPdGraphic";
            this.pnlAliEnterpolAVLPdGraphic.Size = new System.Drawing.Size(62, 39);
            this.pnlAliEnterpolAVLPdGraphic.TabIndex = 83;
            // 
            // lblAliEnterpolAVLTableName
            // 
            this.lblAliEnterpolAVLTableName.AutoSize = true;
            this.lblAliEnterpolAVLTableName.Location = new System.Drawing.Point(96, 11);
            this.lblAliEnterpolAVLTableName.Name = "lblAliEnterpolAVLTableName";
            this.lblAliEnterpolAVLTableName.Size = new System.Drawing.Size(84, 13);
            this.lblAliEnterpolAVLTableName.TabIndex = 13;
            this.lblAliEnterpolAVLTableName.Text = "Database Name";
            // 
            // pnlAliEnterpolAVLFdGraphic
            // 
            this.pnlAliEnterpolAVLFdGraphic.Location = new System.Drawing.Point(56, 236);
            this.pnlAliEnterpolAVLFdGraphic.Name = "pnlAliEnterpolAVLFdGraphic";
            this.pnlAliEnterpolAVLFdGraphic.Size = new System.Drawing.Size(62, 40);
            this.pnlAliEnterpolAVLFdGraphic.TabIndex = 82;
            // 
            // txtAliEnterpolAVLTableName
            // 
            this.txtAliEnterpolAVLTableName.Location = new System.Drawing.Point(186, 4);
            this.txtAliEnterpolAVLTableName.Name = "txtAliEnterpolAVLTableName";
            this.txtAliEnterpolAVLTableName.Size = new System.Drawing.Size(206, 20);
            this.txtAliEnterpolAVLTableName.TabIndex = 9;
            // 
            // pnlAliEnterpolAVLEmsGraphic
            // 
            this.pnlAliEnterpolAVLEmsGraphic.Location = new System.Drawing.Point(56, 145);
            this.pnlAliEnterpolAVLEmsGraphic.Name = "pnlAliEnterpolAVLEmsGraphic";
            this.pnlAliEnterpolAVLEmsGraphic.Size = new System.Drawing.Size(61, 44);
            this.pnlAliEnterpolAVLEmsGraphic.TabIndex = 81;
            // 
            // txtAliEnterpolAVLInitialCatalog
            // 
            this.txtAliEnterpolAVLInitialCatalog.Location = new System.Drawing.Point(166, 46);
            this.txtAliEnterpolAVLInitialCatalog.Name = "txtAliEnterpolAVLInitialCatalog";
            this.txtAliEnterpolAVLInitialCatalog.Size = new System.Drawing.Size(147, 20);
            this.txtAliEnterpolAVLInitialCatalog.TabIndex = 10;
            // 
            // lblAliEnterpolAVLFontSize
            // 
            this.lblAliEnterpolAVLFontSize.AutoSize = true;
            this.lblAliEnterpolAVLFontSize.Location = new System.Drawing.Point(294, 130);
            this.lblAliEnterpolAVLFontSize.Name = "lblAliEnterpolAVLFontSize";
            this.lblAliEnterpolAVLFontSize.Size = new System.Drawing.Size(51, 13);
            this.lblAliEnterpolAVLFontSize.TabIndex = 80;
            this.lblAliEnterpolAVLFontSize.Text = "Font Size";
            // 
            // txtAliEnterpolAVLSetLocProc
            // 
            this.txtAliEnterpolAVLSetLocProc.Location = new System.Drawing.Point(204, 292);
            this.txtAliEnterpolAVLSetLocProc.Name = "txtAliEnterpolAVLSetLocProc";
            this.txtAliEnterpolAVLSetLocProc.Size = new System.Drawing.Size(137, 20);
            this.txtAliEnterpolAVLSetLocProc.TabIndex = 11;
            // 
            // lblAliEnterpolAVLFontName
            // 
            this.lblAliEnterpolAVLFontName.AutoSize = true;
            this.lblAliEnterpolAVLFontName.Location = new System.Drawing.Point(215, 130);
            this.lblAliEnterpolAVLFontName.Name = "lblAliEnterpolAVLFontName";
            this.lblAliEnterpolAVLFontName.Size = new System.Drawing.Size(59, 13);
            this.lblAliEnterpolAVLFontName.TabIndex = 79;
            this.lblAliEnterpolAVLFontName.Text = "Font Name";
            // 
            // lblAliEnterpolAVLInitialCatalog
            // 
            this.lblAliEnterpolAVLInitialCatalog.AutoSize = true;
            this.lblAliEnterpolAVLInitialCatalog.Location = new System.Drawing.Point(66, 53);
            this.lblAliEnterpolAVLInitialCatalog.Name = "lblAliEnterpolAVLInitialCatalog";
            this.lblAliEnterpolAVLInitialCatalog.Size = new System.Drawing.Size(89, 13);
            this.lblAliEnterpolAVLInitialCatalog.TabIndex = 12;
            this.lblAliEnterpolAVLInitialCatalog.Text = "Units Table/View";
            // 
            // lblAliEnterpolAVLUpdateFreq
            // 
            this.lblAliEnterpolAVLUpdateFreq.AutoSize = true;
            this.lblAliEnterpolAVLUpdateFreq.Location = new System.Drawing.Point(286, 276);
            this.lblAliEnterpolAVLUpdateFreq.Name = "lblAliEnterpolAVLUpdateFreq";
            this.lblAliEnterpolAVLUpdateFreq.Size = new System.Drawing.Size(117, 13);
            this.lblAliEnterpolAVLUpdateFreq.TabIndex = 19;
            this.lblAliEnterpolAVLUpdateFreq.Text = "Update Frequency (ms)";
            // 
            // numAliEnterpolAVLUpdateFreq
            // 
            this.numAliEnterpolAVLUpdateFreq.Location = new System.Drawing.Point(347, 293);
            this.numAliEnterpolAVLUpdateFreq.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numAliEnterpolAVLUpdateFreq.Name = "numAliEnterpolAVLUpdateFreq";
            this.numAliEnterpolAVLUpdateFreq.Size = new System.Drawing.Size(59, 20);
            this.numAliEnterpolAVLUpdateFreq.TabIndex = 18;
            // 
            // txtAliEnterpolAVLFdChars
            // 
            this.txtAliEnterpolAVLFdChars.Location = new System.Drawing.Point(176, 236);
            this.txtAliEnterpolAVLFdChars.Name = "txtAliEnterpolAVLFdChars";
            this.txtAliEnterpolAVLFdChars.Size = new System.Drawing.Size(100, 20);
            this.txtAliEnterpolAVLFdChars.TabIndex = 78;
            // 
            // txtAliEnterpolAVLWhoAmIProc
            // 
            this.txtAliEnterpolAVLWhoAmIProc.Location = new System.Drawing.Point(202, 319);
            this.txtAliEnterpolAVLWhoAmIProc.Name = "txtAliEnterpolAVLWhoAmIProc";
            this.txtAliEnterpolAVLWhoAmIProc.Size = new System.Drawing.Size(196, 20);
            this.txtAliEnterpolAVLWhoAmIProc.TabIndex = 14;
            // 
            // txtAliEnterpolAVLPdChars
            // 
            this.txtAliEnterpolAVLPdChars.Location = new System.Drawing.Point(182, 197);
            this.txtAliEnterpolAVLPdChars.Name = "txtAliEnterpolAVLPdChars";
            this.txtAliEnterpolAVLPdChars.Size = new System.Drawing.Size(94, 20);
            this.txtAliEnterpolAVLPdChars.TabIndex = 77;
            // 
            // lblAliEnterpolAVLSetLocProc
            // 
            this.lblAliEnterpolAVLSetLocProc.AutoSize = true;
            this.lblAliEnterpolAVLSetLocProc.Location = new System.Drawing.Point(65, 292);
            this.lblAliEnterpolAVLSetLocProc.Name = "lblAliEnterpolAVLSetLocProc";
            this.lblAliEnterpolAVLSetLocProc.Size = new System.Drawing.Size(130, 13);
            this.lblAliEnterpolAVLSetLocProc.TabIndex = 17;
            this.lblAliEnterpolAVLSetLocProc.Text = "SetMyLocation Procedure";
            // 
            // txtAliEnterpolAVLEmsChars
            // 
            this.txtAliEnterpolAVLEmsChars.Location = new System.Drawing.Point(182, 169);
            this.txtAliEnterpolAVLEmsChars.Name = "txtAliEnterpolAVLEmsChars";
            this.txtAliEnterpolAVLEmsChars.Size = new System.Drawing.Size(100, 20);
            this.txtAliEnterpolAVLEmsChars.TabIndex = 76;
            // 
            // lblAliEnterpolAVLWhoAmIProc
            // 
            this.lblAliEnterpolAVLWhoAmIProc.AutoSize = true;
            this.lblAliEnterpolAVLWhoAmIProc.Location = new System.Drawing.Point(96, 322);
            this.lblAliEnterpolAVLWhoAmIProc.Name = "lblAliEnterpolAVLWhoAmIProc";
            this.lblAliEnterpolAVLWhoAmIProc.Size = new System.Drawing.Size(100, 13);
            this.lblAliEnterpolAVLWhoAmIProc.TabIndex = 16;
            this.lblAliEnterpolAVLWhoAmIProc.Text = "WhoAmI Procedure";
            // 
            // btnAliEnterpolAVLFont
            // 
            this.btnAliEnterpolAVLFont.Location = new System.Drawing.Point(129, 125);
            this.btnAliEnterpolAVLFont.Name = "btnAliEnterpolAVLFont";
            this.btnAliEnterpolAVLFont.Size = new System.Drawing.Size(75, 23);
            this.btnAliEnterpolAVLFont.TabIndex = 75;
            this.btnAliEnterpolAVLFont.Text = "Font";
            this.btnAliEnterpolAVLFont.UseVisualStyleBackColor = true;
            this.btnAliEnterpolAVLFont.Click += new System.EventHandler(this.btnAliEnterpolAVLFont_Click);
            // 
            // pnlAliEnterpolAVLFdColor
            // 
            this.pnlAliEnterpolAVLFdColor.Location = new System.Drawing.Point(132, 236);
            this.pnlAliEnterpolAVLFdColor.Name = "pnlAliEnterpolAVLFdColor";
            this.pnlAliEnterpolAVLFdColor.Size = new System.Drawing.Size(25, 22);
            this.pnlAliEnterpolAVLFdColor.TabIndex = 74;
            // 
            // lblAliEnterpolAVLReadFreq
            // 
            this.lblAliEnterpolAVLReadFreq.AutoSize = true;
            this.lblAliEnterpolAVLReadFreq.Location = new System.Drawing.Point(306, 27);
            this.lblAliEnterpolAVLReadFreq.Name = "lblAliEnterpolAVLReadFreq";
            this.lblAliEnterpolAVLReadFreq.Size = new System.Drawing.Size(108, 13);
            this.lblAliEnterpolAVLReadFreq.TabIndex = 25;
            this.lblAliEnterpolAVLReadFreq.Text = "Read Frequency (ms)";
            // 
            // numAliEnterpolAVLAge1Freq
            // 
            this.numAliEnterpolAVLAge1Freq.Location = new System.Drawing.Point(80, 98);
            this.numAliEnterpolAVLAge1Freq.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numAliEnterpolAVLAge1Freq.Name = "numAliEnterpolAVLAge1Freq";
            this.numAliEnterpolAVLAge1Freq.Size = new System.Drawing.Size(84, 20);
            this.numAliEnterpolAVLAge1Freq.TabIndex = 20;
            // 
            // pnlAliEnterpolAVLPdColor
            // 
            this.pnlAliEnterpolAVLPdColor.Location = new System.Drawing.Point(133, 197);
            this.pnlAliEnterpolAVLPdColor.Name = "pnlAliEnterpolAVLPdColor";
            this.pnlAliEnterpolAVLPdColor.Size = new System.Drawing.Size(24, 23);
            this.pnlAliEnterpolAVLPdColor.TabIndex = 73;
            // 
            // numAliEnterpolAVLAge2Freq
            // 
            this.numAliEnterpolAVLAge2Freq.Location = new System.Drawing.Point(182, 98);
            this.numAliEnterpolAVLAge2Freq.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numAliEnterpolAVLAge2Freq.Name = "numAliEnterpolAVLAge2Freq";
            this.numAliEnterpolAVLAge2Freq.Size = new System.Drawing.Size(120, 20);
            this.numAliEnterpolAVLAge2Freq.TabIndex = 21;
            // 
            // numAliEnterpolAVLReadFreq
            // 
            this.numAliEnterpolAVLReadFreq.Location = new System.Drawing.Point(319, 46);
            this.numAliEnterpolAVLReadFreq.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numAliEnterpolAVLReadFreq.Name = "numAliEnterpolAVLReadFreq";
            this.numAliEnterpolAVLReadFreq.Size = new System.Drawing.Size(90, 20);
            this.numAliEnterpolAVLReadFreq.TabIndex = 24;
            // 
            // numAliEnterpolAVLAge3Freq
            // 
            this.numAliEnterpolAVLAge3Freq.Location = new System.Drawing.Point(311, 98);
            this.numAliEnterpolAVLAge3Freq.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numAliEnterpolAVLAge3Freq.Name = "numAliEnterpolAVLAge3Freq";
            this.numAliEnterpolAVLAge3Freq.Size = new System.Drawing.Size(103, 20);
            this.numAliEnterpolAVLAge3Freq.TabIndex = 22;
            // 
            // pnlAliEnterpolAVLEmsColor
            // 
            this.pnlAliEnterpolAVLEmsColor.Location = new System.Drawing.Point(133, 165);
            this.pnlAliEnterpolAVLEmsColor.Name = "pnlAliEnterpolAVLEmsColor";
            this.pnlAliEnterpolAVLEmsColor.Size = new System.Drawing.Size(24, 24);
            this.pnlAliEnterpolAVLEmsColor.TabIndex = 72;
            // 
            // lblAliEnterpolAVLAgeFreq
            // 
            this.lblAliEnterpolAVLAgeFreq.AutoSize = true;
            this.lblAliEnterpolAVLAgeFreq.Location = new System.Drawing.Point(86, 73);
            this.lblAliEnterpolAVLAgeFreq.Name = "lblAliEnterpolAVLAgeFreq";
            this.lblAliEnterpolAVLAgeFreq.Size = new System.Drawing.Size(116, 13);
            this.lblAliEnterpolAVLAgeFreq.TabIndex = 23;
            this.lblAliEnterpolAVLAgeFreq.Text = "AVL Age Frequency (s)";
            // 
            // chkNetworkfleet
            // 
            this.chkNetworkfleet.AutoSize = true;
            this.chkNetworkfleet.Location = new System.Drawing.Point(34, 80);
            this.chkNetworkfleet.Name = "chkNetworkfleet";
            this.chkNetworkfleet.Size = new System.Drawing.Size(146, 17);
            this.chkNetworkfleet.TabIndex = 65;
            this.chkNetworkfleet.Text = "Use Verizon Networkfleet";
            this.chkNetworkfleet.UseVisualStyleBackColor = true;
            this.chkNetworkfleet.CheckedChanged += new System.EventHandler(this.chkNetworkfleet_CheckedChanged);
            // 
            // pnlAliNetworkfleet
            // 
            this.pnlAliNetworkfleet.Controls.Add(this.ptAliNetworkfleetColor);
            this.pnlAliNetworkfleet.Controls.Add(this.ptAliNetworkfleetSize);
            this.pnlAliNetworkfleet.Controls.Add(this.ptAliNetworkfleetFont);
            this.pnlAliNetworkfleet.Controls.Add(this.btnAliNetworkfleetFont);
            this.pnlAliNetworkfleet.Controls.Add(this.ptAliNetworkfleetChar);
            this.pnlAliNetworkfleet.Controls.Add(this.ptAliNetworkfleetGraphic);
            this.pnlAliNetworkfleet.Controls.Add(this.label6);
            this.pnlAliNetworkfleet.Controls.Add(this.lblAliNetworkfleetUdpHost);
            this.pnlAliNetworkfleet.Controls.Add(this.numAliNetworkfleetUdpPort);
            this.pnlAliNetworkfleet.Controls.Add(this.txtAliNetworkfleetUdpHost);
            this.pnlAliNetworkfleet.Location = new System.Drawing.Point(13, 103);
            this.pnlAliNetworkfleet.Name = "pnlAliNetworkfleet";
            this.pnlAliNetworkfleet.Size = new System.Drawing.Size(196, 226);
            this.pnlAliNetworkfleet.TabIndex = 71;
            this.pnlAliNetworkfleet.Visible = false;
            // 
            // ptAliNetworkfleetColor
            // 
            this.ptAliNetworkfleetColor.Location = new System.Drawing.Point(85, 101);
            this.ptAliNetworkfleetColor.Name = "ptAliNetworkfleetColor";
            this.ptAliNetworkfleetColor.Size = new System.Drawing.Size(24, 23);
            this.ptAliNetworkfleetColor.TabIndex = 70;
            // 
            // ptAliNetworkfleetSize
            // 
            this.ptAliNetworkfleetSize.AutoSize = true;
            this.ptAliNetworkfleetSize.Location = new System.Drawing.Point(118, 187);
            this.ptAliNetworkfleetSize.Name = "ptAliNetworkfleetSize";
            this.ptAliNetworkfleetSize.Size = new System.Drawing.Size(48, 13);
            this.ptAliNetworkfleetSize.TabIndex = 65;
            this.ptAliNetworkfleetSize.Text = "FontSize";
            // 
            // ptAliNetworkfleetFont
            // 
            this.ptAliNetworkfleetFont.AutoSize = true;
            this.ptAliNetworkfleetFont.Location = new System.Drawing.Point(62, 167);
            this.ptAliNetworkfleetFont.Name = "ptAliNetworkfleetFont";
            this.ptAliNetworkfleetFont.Size = new System.Drawing.Size(105, 13);
            this.ptAliNetworkfleetFont.TabIndex = 69;
            this.ptAliNetworkfleetFont.Text = "This is the font name";
            // 
            // btnAliNetworkfleetFont
            // 
            this.btnAliNetworkfleetFont.Location = new System.Drawing.Point(14, 162);
            this.btnAliNetworkfleetFont.Name = "btnAliNetworkfleetFont";
            this.btnAliNetworkfleetFont.Size = new System.Drawing.Size(44, 23);
            this.btnAliNetworkfleetFont.TabIndex = 68;
            this.btnAliNetworkfleetFont.Text = "Font";
            this.btnAliNetworkfleetFont.UseVisualStyleBackColor = true;
            this.btnAliNetworkfleetFont.Click += new System.EventHandler(this.btnAliNetworkfleetFont_Click);
            // 
            // ptAliNetworkfleetChar
            // 
            this.ptAliNetworkfleetChar.Location = new System.Drawing.Point(115, 101);
            this.ptAliNetworkfleetChar.MaxLength = 1;
            this.ptAliNetworkfleetChar.Name = "ptAliNetworkfleetChar";
            this.ptAliNetworkfleetChar.Size = new System.Drawing.Size(34, 20);
            this.ptAliNetworkfleetChar.TabIndex = 67;
            // 
            // ptAliNetworkfleetGraphic
            // 
            this.ptAliNetworkfleetGraphic.Location = new System.Drawing.Point(16, 99);
            this.ptAliNetworkfleetGraphic.Name = "ptAliNetworkfleetGraphic";
            this.ptAliNetworkfleetGraphic.Size = new System.Drawing.Size(53, 50);
            this.ptAliNetworkfleetGraphic.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Port:";
            // 
            // lblAliNetworkfleetUdpHost
            // 
            this.lblAliNetworkfleetUdpHost.AutoSize = true;
            this.lblAliNetworkfleetUdpHost.Location = new System.Drawing.Point(11, 9);
            this.lblAliNetworkfleetUdpHost.Name = "lblAliNetworkfleetUdpHost";
            this.lblAliNetworkfleetUdpHost.Size = new System.Drawing.Size(58, 13);
            this.lblAliNetworkfleetUdpHost.TabIndex = 2;
            this.lblAliNetworkfleetUdpHost.Text = "UDP Host:";
            // 
            // numAliNetworkfleetUdpPort
            // 
            this.numAliNetworkfleetUdpPort.Location = new System.Drawing.Point(10, 69);
            this.numAliNetworkfleetUdpPort.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numAliNetworkfleetUdpPort.Name = "numAliNetworkfleetUdpPort";
            this.numAliNetworkfleetUdpPort.Size = new System.Drawing.Size(91, 20);
            this.numAliNetworkfleetUdpPort.TabIndex = 1;
            // 
            // txtAliNetworkfleetUdpHost
            // 
            this.txtAliNetworkfleetUdpHost.Location = new System.Drawing.Point(10, 25);
            this.txtAliNetworkfleetUdpHost.Name = "txtAliNetworkfleetUdpHost";
            this.txtAliNetworkfleetUdpHost.Size = new System.Drawing.Size(171, 20);
            this.txtAliNetworkfleetUdpHost.TabIndex = 0;
            // 
            // btnAliValidate
            // 
            this.btnAliValidate.Location = new System.Drawing.Point(134, 41);
            this.btnAliValidate.Name = "btnAliValidate";
            this.btnAliValidate.Size = new System.Drawing.Size(75, 23);
            this.btnAliValidate.TabIndex = 70;
            this.btnAliValidate.Text = "Validate";
            this.btnAliValidate.UseVisualStyleBackColor = true;
            this.btnAliValidate.Click += new System.EventHandler(this.btnAliValidate_Click);
            // 
            // pnlAliEnterpol
            // 
            this.pnlAliEnterpol.Controls.Add(this.label17);
            this.pnlAliEnterpol.Controls.Add(this.chkEnterpolAvl);
            this.pnlAliEnterpol.Controls.Add(this.lblAliEnterpolInitialCatalog);
            this.pnlAliEnterpol.Controls.Add(this.lblAliEnterpolTableName);
            this.pnlAliEnterpol.Controls.Add(this.lblAliEnterpolDataSource);
            this.pnlAliEnterpol.Controls.Add(this.txtAliEnterpolInitialCatalog);
            this.pnlAliEnterpol.Controls.Add(this.txtAliEnterpolDataSource);
            this.pnlAliEnterpol.Controls.Add(this.txtAliEnterpolTableName);
            this.pnlAliEnterpol.Location = new System.Drawing.Point(215, 207);
            this.pnlAliEnterpol.Name = "pnlAliEnterpol";
            this.pnlAliEnterpol.Size = new System.Drawing.Size(342, 122);
            this.pnlAliEnterpol.TabIndex = 69;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(11, 179);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(0, 13);
            this.label17.TabIndex = 15;
            // 
            // chkEnterpolAvl
            // 
            this.chkEnterpolAvl.AutoSize = true;
            this.chkEnterpolAvl.Location = new System.Drawing.Point(24, 94);
            this.chkEnterpolAvl.Name = "chkEnterpolAvl";
            this.chkEnterpolAvl.Size = new System.Drawing.Size(177, 17);
            this.chkEnterpolAvl.TabIndex = 8;
            this.chkEnterpolAvl.Text = "Use Automatic Vehicle Location";
            this.chkEnterpolAvl.UseVisualStyleBackColor = true;
            this.chkEnterpolAvl.CheckedChanged += new System.EventHandler(this.chkAvl_CheckedChanged);
            // 
            // lblAliEnterpolInitialCatalog
            // 
            this.lblAliEnterpolInitialCatalog.AutoSize = true;
            this.lblAliEnterpolInitialCatalog.Location = new System.Drawing.Point(211, 52);
            this.lblAliEnterpolInitialCatalog.Name = "lblAliEnterpolInitialCatalog";
            this.lblAliEnterpolInitialCatalog.Size = new System.Drawing.Size(108, 13);
            this.lblAliEnterpolInitialCatalog.TabIndex = 7;
            this.lblAliEnterpolInitialCatalog.Text = "Incidents Table/View";
            // 
            // lblAliEnterpolTableName
            // 
            this.lblAliEnterpolTableName.AutoSize = true;
            this.lblAliEnterpolTableName.Location = new System.Drawing.Point(9, 72);
            this.lblAliEnterpolTableName.Name = "lblAliEnterpolTableName";
            this.lblAliEnterpolTableName.Size = new System.Drawing.Size(87, 13);
            this.lblAliEnterpolTableName.TabIndex = 6;
            this.lblAliEnterpolTableName.Text = "Database Name:";
            // 
            // lblAliEnterpolDataSource
            // 
            this.lblAliEnterpolDataSource.AutoSize = true;
            this.lblAliEnterpolDataSource.Location = new System.Drawing.Point(21, 7);
            this.lblAliEnterpolDataSource.Name = "lblAliEnterpolDataSource";
            this.lblAliEnterpolDataSource.Size = new System.Drawing.Size(97, 13);
            this.lblAliEnterpolDataSource.TabIndex = 5;
            this.lblAliEnterpolDataSource.Text = "Database Location";
            // 
            // txtAliEnterpolInitialCatalog
            // 
            this.txtAliEnterpolInitialCatalog.Location = new System.Drawing.Point(214, 68);
            this.txtAliEnterpolInitialCatalog.Name = "txtAliEnterpolInitialCatalog";
            this.txtAliEnterpolInitialCatalog.Size = new System.Drawing.Size(91, 20);
            this.txtAliEnterpolInitialCatalog.TabIndex = 3;
            // 
            // txtAliEnterpolDataSource
            // 
            this.txtAliEnterpolDataSource.Location = new System.Drawing.Point(18, 23);
            this.txtAliEnterpolDataSource.Name = "txtAliEnterpolDataSource";
            this.txtAliEnterpolDataSource.Size = new System.Drawing.Size(263, 20);
            this.txtAliEnterpolDataSource.TabIndex = 2;
            // 
            // txtAliEnterpolTableName
            // 
            this.txtAliEnterpolTableName.Location = new System.Drawing.Point(108, 68);
            this.txtAliEnterpolTableName.Name = "txtAliEnterpolTableName";
            this.txtAliEnterpolTableName.Size = new System.Drawing.Size(100, 20);
            this.txtAliEnterpolTableName.TabIndex = 1;
            // 
            // pnlAliGlobalCad
            // 
            this.pnlAliGlobalCad.Controls.Add(this.btnAliGlobalCadConfigIniBrowse);
            this.pnlAliGlobalCad.Controls.Add(this.btnAliGlobalCadArchivePathBrowse);
            this.pnlAliGlobalCad.Controls.Add(this.txtAliGlobalCadConfigIni);
            this.pnlAliGlobalCad.Controls.Add(this.lblAliGlobalCadConfigIni);
            this.pnlAliGlobalCad.Controls.Add(this.lblAliGlobalCadArchivePath);
            this.pnlAliGlobalCad.Controls.Add(this.txtAliGlobalCadArchivePath);
            this.pnlAliGlobalCad.Controls.Add(this.lblAliGlobalCadLogPath);
            this.pnlAliGlobalCad.Controls.Add(this.btnAliGlobalCadLogPathBrowse);
            this.pnlAliGlobalCad.Controls.Add(this.txtAliGlobalCadLogPath);
            this.pnlAliGlobalCad.Location = new System.Drawing.Point(215, 92);
            this.pnlAliGlobalCad.Name = "pnlAliGlobalCad";
            this.pnlAliGlobalCad.Size = new System.Drawing.Size(379, 109);
            this.pnlAliGlobalCad.TabIndex = 68;
            // 
            // btnAliGlobalCadConfigIniBrowse
            // 
            this.btnAliGlobalCadConfigIniBrowse.Location = new System.Drawing.Point(287, 74);
            this.btnAliGlobalCadConfigIniBrowse.Name = "btnAliGlobalCadConfigIniBrowse";
            this.btnAliGlobalCadConfigIniBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnAliGlobalCadConfigIniBrowse.TabIndex = 8;
            this.btnAliGlobalCadConfigIniBrowse.Text = "Browse";
            this.btnAliGlobalCadConfigIniBrowse.UseVisualStyleBackColor = true;
            this.btnAliGlobalCadConfigIniBrowse.Click += new System.EventHandler(this.btnAliGlobalCadConfigIniBrowse_Click);
            // 
            // btnAliGlobalCadArchivePathBrowse
            // 
            this.btnAliGlobalCadArchivePathBrowse.Location = new System.Drawing.Point(287, 45);
            this.btnAliGlobalCadArchivePathBrowse.Name = "btnAliGlobalCadArchivePathBrowse";
            this.btnAliGlobalCadArchivePathBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnAliGlobalCadArchivePathBrowse.TabIndex = 7;
            this.btnAliGlobalCadArchivePathBrowse.Text = "Browse";
            this.btnAliGlobalCadArchivePathBrowse.UseVisualStyleBackColor = true;
            this.btnAliGlobalCadArchivePathBrowse.Click += new System.EventHandler(this.btnAliGlobalCadArchivePathBrowse_Click);
            // 
            // txtAliGlobalCadConfigIni
            // 
            this.txtAliGlobalCadConfigIni.Location = new System.Drawing.Point(102, 76);
            this.txtAliGlobalCadConfigIni.Name = "txtAliGlobalCadConfigIni";
            this.txtAliGlobalCadConfigIni.Size = new System.Drawing.Size(179, 20);
            this.txtAliGlobalCadConfigIni.TabIndex = 6;
            // 
            // lblAliGlobalCadConfigIni
            // 
            this.lblAliGlobalCadConfigIni.AutoSize = true;
            this.lblAliGlobalCadConfigIni.Location = new System.Drawing.Point(19, 79);
            this.lblAliGlobalCadConfigIni.Name = "lblAliGlobalCadConfigIni";
            this.lblAliGlobalCadConfigIni.Size = new System.Drawing.Size(56, 13);
            this.lblAliGlobalCadConfigIni.TabIndex = 5;
            this.lblAliGlobalCadConfigIni.Text = "Config File";
            // 
            // lblAliGlobalCadArchivePath
            // 
            this.lblAliGlobalCadArchivePath.AutoSize = true;
            this.lblAliGlobalCadArchivePath.Location = new System.Drawing.Point(9, 53);
            this.lblAliGlobalCadArchivePath.Name = "lblAliGlobalCadArchivePath";
            this.lblAliGlobalCadArchivePath.Size = new System.Drawing.Size(87, 13);
            this.lblAliGlobalCadArchivePath.TabIndex = 4;
            this.lblAliGlobalCadArchivePath.Text = "Archive Location";
            // 
            // txtAliGlobalCadArchivePath
            // 
            this.txtAliGlobalCadArchivePath.Location = new System.Drawing.Point(102, 50);
            this.txtAliGlobalCadArchivePath.Name = "txtAliGlobalCadArchivePath";
            this.txtAliGlobalCadArchivePath.Size = new System.Drawing.Size(179, 20);
            this.txtAliGlobalCadArchivePath.TabIndex = 3;
            // 
            // lblAliGlobalCadLogPath
            // 
            this.lblAliGlobalCadLogPath.AutoSize = true;
            this.lblAliGlobalCadLogPath.Location = new System.Drawing.Point(9, 16);
            this.lblAliGlobalCadLogPath.Name = "lblAliGlobalCadLogPath";
            this.lblAliGlobalCadLogPath.Size = new System.Drawing.Size(69, 13);
            this.lblAliGlobalCadLogPath.TabIndex = 2;
            this.lblAliGlobalCadLogPath.Text = "Log Location";
            // 
            // btnAliGlobalCadLogPathBrowse
            // 
            this.btnAliGlobalCadLogPathBrowse.Location = new System.Drawing.Point(287, 11);
            this.btnAliGlobalCadLogPathBrowse.Name = "btnAliGlobalCadLogPathBrowse";
            this.btnAliGlobalCadLogPathBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnAliGlobalCadLogPathBrowse.TabIndex = 1;
            this.btnAliGlobalCadLogPathBrowse.Text = "Browse";
            this.btnAliGlobalCadLogPathBrowse.UseVisualStyleBackColor = true;
            this.btnAliGlobalCadLogPathBrowse.Click += new System.EventHandler(this.btnAliGlobalCadLogPathBrowse_Click);
            // 
            // txtAliGlobalCadLogPath
            // 
            this.txtAliGlobalCadLogPath.Location = new System.Drawing.Point(81, 14);
            this.txtAliGlobalCadLogPath.Name = "txtAliGlobalCadLogPath";
            this.txtAliGlobalCadLogPath.Size = new System.Drawing.Size(200, 20);
            this.txtAliGlobalCadLogPath.TabIndex = 0;
            // 
            // pnlAliSdrAliServer
            // 
            this.pnlAliSdrAliServer.Controls.Add(this.numAliInterfaceUdpPort);
            this.pnlAliSdrAliServer.Controls.Add(this.lblAliInterfaceUdpPort);
            this.pnlAliSdrAliServer.Controls.Add(this.lblAliInterfaceUdpHost);
            this.pnlAliSdrAliServer.Controls.Add(this.lblAliInterfaceDbPath);
            this.pnlAliSdrAliServer.Controls.Add(this.txtAliInterfaceUdpHost);
            this.pnlAliSdrAliServer.Controls.Add(this.btnAliInterfaceDbPathBrowse);
            this.pnlAliSdrAliServer.Controls.Add(this.txtAliInterfaceDbPath);
            this.pnlAliSdrAliServer.Location = new System.Drawing.Point(215, 6);
            this.pnlAliSdrAliServer.Name = "pnlAliSdrAliServer";
            this.pnlAliSdrAliServer.Size = new System.Drawing.Size(375, 80);
            this.pnlAliSdrAliServer.TabIndex = 67;
            // 
            // numAliInterfaceUdpPort
            // 
            this.numAliInterfaceUdpPort.Location = new System.Drawing.Point(271, 7);
            this.numAliInterfaceUdpPort.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numAliInterfaceUdpPort.Name = "numAliInterfaceUdpPort";
            this.numAliInterfaceUdpPort.Size = new System.Drawing.Size(71, 20);
            this.numAliInterfaceUdpPort.TabIndex = 7;
            // 
            // lblAliInterfaceUdpPort
            // 
            this.lblAliInterfaceUdpPort.AutoSize = true;
            this.lblAliInterfaceUdpPort.Location = new System.Drawing.Point(237, 9);
            this.lblAliInterfaceUdpPort.Name = "lblAliInterfaceUdpPort";
            this.lblAliInterfaceUdpPort.Size = new System.Drawing.Size(29, 13);
            this.lblAliInterfaceUdpPort.TabIndex = 6;
            this.lblAliInterfaceUdpPort.Text = "Port:";
            // 
            // lblAliInterfaceUdpHost
            // 
            this.lblAliInterfaceUdpHost.AutoSize = true;
            this.lblAliInterfaceUdpHost.Location = new System.Drawing.Point(18, 12);
            this.lblAliInterfaceUdpHost.Name = "lblAliInterfaceUdpHost";
            this.lblAliInterfaceUdpHost.Size = new System.Drawing.Size(58, 13);
            this.lblAliInterfaceUdpHost.TabIndex = 5;
            this.lblAliInterfaceUdpHost.Text = "UDP Host:";
            // 
            // lblAliInterfaceDbPath
            // 
            this.lblAliInterfaceDbPath.AutoSize = true;
            this.lblAliInterfaceDbPath.Location = new System.Drawing.Point(64, 32);
            this.lblAliInterfaceDbPath.Name = "lblAliInterfaceDbPath";
            this.lblAliInterfaceDbPath.Size = new System.Drawing.Size(97, 13);
            this.lblAliInterfaceDbPath.TabIndex = 4;
            this.lblAliInterfaceDbPath.Text = "Database Location";
            // 
            // txtAliInterfaceUdpHost
            // 
            this.txtAliInterfaceUdpHost.Location = new System.Drawing.Point(77, 7);
            this.txtAliInterfaceUdpHost.Name = "txtAliInterfaceUdpHost";
            this.txtAliInterfaceUdpHost.Size = new System.Drawing.Size(157, 20);
            this.txtAliInterfaceUdpHost.TabIndex = 2;
            // 
            // btnAliInterfaceDbPathBrowse
            // 
            this.btnAliInterfaceDbPathBrowse.Location = new System.Drawing.Point(271, 49);
            this.btnAliInterfaceDbPathBrowse.Name = "btnAliInterfaceDbPathBrowse";
            this.btnAliInterfaceDbPathBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnAliInterfaceDbPathBrowse.TabIndex = 1;
            this.btnAliInterfaceDbPathBrowse.Text = "Browse";
            this.btnAliInterfaceDbPathBrowse.UseVisualStyleBackColor = true;
            this.btnAliInterfaceDbPathBrowse.Click += new System.EventHandler(this.btnAliInterfaceDbPathBrowse_Click);
            // 
            // txtAliInterfaceDbPath
            // 
            this.txtAliInterfaceDbPath.Location = new System.Drawing.Point(28, 51);
            this.txtAliInterfaceDbPath.Name = "txtAliInterfaceDbPath";
            this.txtAliInterfaceDbPath.Size = new System.Drawing.Size(237, 20);
            this.txtAliInterfaceDbPath.TabIndex = 0;
            // 
            // cmbAliMode
            // 
            this.cmbAliMode.FormattingEnabled = true;
            this.cmbAliMode.Location = new System.Drawing.Point(23, 14);
            this.cmbAliMode.Name = "cmbAliMode";
            this.cmbAliMode.Size = new System.Drawing.Size(186, 21);
            this.cmbAliMode.TabIndex = 66;
            this.cmbAliMode.SelectedIndexChanged += new System.EventHandler(this.cmbAliMode_SelectedIndexChanged);
            // 
            // btnSplitSave
            // 
            this.btnSplitSave.AutoSize = true;
            this.btnSplitSave.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnSplitSave.Location = new System.Drawing.Point(520, 762);
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
            this.btnCancel.Location = new System.Drawing.Point(439, 762);
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
            this.ClientSize = new System.Drawing.Size(609, 794);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "AdminForm";
            this.Text = "Go2It Administration";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.adminTab_Control.ResumeLayout(false);
            this.adminTab_MapConfiguration.ResumeLayout(false);
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
            this.adminTab_ProjectSettings.ResumeLayout(false);
            this.adminTab_ProjectSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lineSymbolSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptSymbolSize)).EndInit();
            this.adminTab_ProgramManagement.ResumeLayout(false);
            this.adminTab_ProgramManagement.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHotKeys)).EndInit();
            this.adminTab_SearchSettings.ResumeLayout(false);
            this.adminTab_SearchSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchHydrantDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchBufferDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchZoomFactor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchHydrantCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLayerIndex)).EndInit();
            this.adminTab_AliSettings.ResumeLayout(false);
            this.adminTab_AliSettings.PerformLayout();
            this.pnlAliEnterpolAvl.ResumeLayout(false);
            this.pnlAliEnterpolAvl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLUpdateFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLAge1Freq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLAge2Freq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLReadFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLAge3Freq)).EndInit();
            this.pnlAliNetworkfleet.ResumeLayout(false);
            this.pnlAliNetworkfleet.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAliNetworkfleetUdpPort)).EndInit();
            this.pnlAliEnterpol.ResumeLayout(false);
            this.pnlAliEnterpol.PerformLayout();
            this.pnlAliGlobalCad.ResumeLayout(false);
            this.pnlAliGlobalCad.PerformLayout();
            this.pnlAliSdrAliServer.ResumeLayout(false);
            this.pnlAliSdrAliServer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAliInterfaceUdpPort)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DotSpatial.SDR.Controls.SplitButton btnSplitSave;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabControl adminTab_Control;
        private System.Windows.Forms.TabPage adminTab_MapConfiguration;
        private System.Windows.Forms.SplitContainer adminLayerSplitter;
        private System.Windows.Forms.SplitContainer legendSplitter;
        private System.Windows.Forms.TableLayoutPanel legendButtonTable;
        private System.Windows.Forms.Button btnRemoveLayer;
        private System.Windows.Forms.Button btnAddLayer;
        private System.Windows.Forms.Button btnAddView;
        private System.Windows.Forms.TextBox txtViewName;
        private System.Windows.Forms.Button btnDeleteView;
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
        private System.Windows.Forms.TabPage adminTab_SearchSettings;
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
        private System.Windows.Forms.CheckBox chkEnableQueryParserLog;
        private System.Windows.Forms.TabPage adminTab_ProjectSettings;
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
        private DotSpatial.Symbology.Forms.RampSlider lineSymbolColorSlider;
        private DotSpatial.Symbology.Forms.RampSlider ptSymbolColorSlider;
        private System.Windows.Forms.Label lblHydrantDist;
        private System.Windows.Forms.Label lblZoomFactor;
        private System.Windows.Forms.Label lblBufDist;
        private System.Windows.Forms.Label lblHydrantCount;
        private System.Windows.Forms.NumericUpDown searchHydrantDistance;
        private System.Windows.Forms.NumericUpDown searchBufferDistance;
        private System.Windows.Forms.NumericUpDown searchZoomFactor;
        private System.Windows.Forms.NumericUpDown searchHydrantCount;
        private System.Windows.Forms.TabPage adminTab_AliSettings;
        private System.Windows.Forms.CheckBox chkNetworkfleet;
        private System.Windows.Forms.Panel pnlAliNetworkfleet;
        private System.Windows.Forms.Panel ptAliNetworkfleetColor;
        private System.Windows.Forms.Label ptAliNetworkfleetSize;
        private System.Windows.Forms.Label ptAliNetworkfleetFont;
        private System.Windows.Forms.Button btnAliNetworkfleetFont;
        private System.Windows.Forms.TextBox ptAliNetworkfleetChar;
        private System.Windows.Forms.Panel ptAliNetworkfleetGraphic;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblAliNetworkfleetUdpHost;
        private System.Windows.Forms.NumericUpDown numAliNetworkfleetUdpPort;
        private System.Windows.Forms.TextBox txtAliNetworkfleetUdpHost;
        private System.Windows.Forms.Button btnAliValidate;
        private System.Windows.Forms.Panel pnlAliEnterpol;
        private System.Windows.Forms.TextBox txtAliEnterpolAVLWhoAmIProc;
        private System.Windows.Forms.Label lblAliEnterpolAVLTableName;
        private System.Windows.Forms.Label lblAliEnterpolAVLInitialCatalog;
        private System.Windows.Forms.TextBox txtAliEnterpolAVLSetLocProc;
        private System.Windows.Forms.TextBox txtAliEnterpolAVLInitialCatalog;
        private System.Windows.Forms.TextBox txtAliEnterpolAVLTableName;
        private System.Windows.Forms.CheckBox chkEnterpolAvl;
        private System.Windows.Forms.Label lblAliEnterpolInitialCatalog;
        private System.Windows.Forms.Label lblAliEnterpolTableName;
        private System.Windows.Forms.Label lblAliEnterpolDataSource;
        private System.Windows.Forms.TextBox txtAliEnterpolInitialCatalog;
        private System.Windows.Forms.TextBox txtAliEnterpolDataSource;
        private System.Windows.Forms.TextBox txtAliEnterpolTableName;
        private System.Windows.Forms.Panel pnlAliGlobalCad;
        private System.Windows.Forms.Button btnAliGlobalCadConfigIniBrowse;
        private System.Windows.Forms.Button btnAliGlobalCadArchivePathBrowse;
        private System.Windows.Forms.TextBox txtAliGlobalCadConfigIni;
        private System.Windows.Forms.Label lblAliGlobalCadConfigIni;
        private System.Windows.Forms.Label lblAliGlobalCadArchivePath;
        private System.Windows.Forms.TextBox txtAliGlobalCadArchivePath;
        private System.Windows.Forms.Label lblAliGlobalCadLogPath;
        private System.Windows.Forms.Button btnAliGlobalCadLogPathBrowse;
        private System.Windows.Forms.TextBox txtAliGlobalCadLogPath;
        private System.Windows.Forms.Panel pnlAliSdrAliServer;
        private System.Windows.Forms.NumericUpDown numAliInterfaceUdpPort;
        private System.Windows.Forms.Label lblAliInterfaceUdpPort;
        private System.Windows.Forms.Label lblAliInterfaceUdpHost;
        private System.Windows.Forms.Label lblAliInterfaceDbPath;
        private System.Windows.Forms.TextBox txtAliInterfaceUdpHost;
        private System.Windows.Forms.Button btnAliInterfaceDbPathBrowse;
        private System.Windows.Forms.TextBox txtAliInterfaceDbPath;
        private System.Windows.Forms.ComboBox cmbAliMode;
        private System.Windows.Forms.Label lblAliEnterpolAVLSetLocProc;
        private System.Windows.Forms.Label lblAliEnterpolAVLWhoAmIProc;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lblAliEnterpolAVLAgeFreq;
        private System.Windows.Forms.NumericUpDown numAliEnterpolAVLAge3Freq;
        private System.Windows.Forms.NumericUpDown numAliEnterpolAVLAge2Freq;
        private System.Windows.Forms.NumericUpDown numAliEnterpolAVLAge1Freq;
        private System.Windows.Forms.Label lblAliEnterpolAVLUpdateFreq;
        private System.Windows.Forms.NumericUpDown numAliEnterpolAVLUpdateFreq;
        private System.Windows.Forms.Label lblAliEnterpolAVLReadFreq;
        private System.Windows.Forms.NumericUpDown numAliEnterpolAVLReadFreq;
        private System.Windows.Forms.Panel pnlAliEnterpolAVLPdGraphic;
        private System.Windows.Forms.Panel pnlAliEnterpolAVLFdGraphic;
        private System.Windows.Forms.Panel pnlAliEnterpolAVLEmsGraphic;
        private System.Windows.Forms.Label lblAliEnterpolAVLFontSize;
        private System.Windows.Forms.Label lblAliEnterpolAVLFontName;
        private System.Windows.Forms.TextBox txtAliEnterpolAVLFdChars;
        private System.Windows.Forms.TextBox txtAliEnterpolAVLPdChars;
        private System.Windows.Forms.TextBox txtAliEnterpolAVLEmsChars;
        private System.Windows.Forms.Button btnAliEnterpolAVLFont;
        private System.Windows.Forms.Panel pnlAliEnterpolAVLFdColor;
        private System.Windows.Forms.Panel pnlAliEnterpolAVLPdColor;
        private System.Windows.Forms.Panel pnlAliEnterpolAVLEmsColor;
        private System.Windows.Forms.Panel pnlAliEnterpolAvl;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkAutoHideInactiveUnits;
        private System.Windows.Forms.Label lblAliEnterpolAVLMyVehicleColor;
        private System.Windows.Forms.Panel pnlAliEnterpolAVLMyVehicleColor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel pnlAliAVLInactiveColor;

    }
}