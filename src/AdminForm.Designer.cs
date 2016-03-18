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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.layerName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.fieldName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.add = new System.Windows.Forms.DataGridViewButtonColumn();
            this.remove = new System.Windows.Forms.DataGridViewButtonColumn();
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
            this.aliPanelTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pnlAliEnterpol = new System.Windows.Forms.TableLayoutPanel();
            this.pnlAliEnterpolAvl = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.label10 = new System.Windows.Forms.Label();
            this.numAliEnterpolAVLLabelXOffset = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.numAliEnterpolAVLLabelYOffset = new System.Windows.Forms.NumericUpDown();
            this.btnAliEnterpolAVLSymbolFont = new System.Windows.Forms.Button();
            this.txtAliEnterpolAVLSymbolFontSize = new System.Windows.Forms.TextBox();
            this.lblAliEnterpolAVLSymbolFontSize = new System.Windows.Forms.Label();
            this.txtAliEnterpolAVLLabelFontSize = new System.Windows.Forms.TextBox();
            this.tblEnterpolAvlSymbology = new System.Windows.Forms.TableLayoutPanel();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblAliEnterpolAVLMyVehicleColor = new System.Windows.Forms.Label();
            this.pnlAliEnterpolAVLEmsColor = new System.Windows.Forms.Panel();
            this.pnlAliEnterpolAVLFdColor = new System.Windows.Forms.Panel();
            this.pnlAliEnterpolAVLPdColor = new System.Windows.Forms.Panel();
            this.pnlAliAVLInactiveColor = new System.Windows.Forms.Panel();
            this.pnlAliEnterpolAVLMyVehicleColor = new System.Windows.Forms.Panel();
            this.txtAliEnterpolAVLEmsChars = new System.Windows.Forms.TextBox();
            this.txtAliEnterpolAVLFdChars = new System.Windows.Forms.TextBox();
            this.txtAliEnterpolAVLPdChars = new System.Windows.Forms.TextBox();
            this.pnlAliEnterpolAVLEmsGraphic = new System.Windows.Forms.Panel();
            this.pnlAliEnterpolAVLFdGraphic = new System.Windows.Forms.Panel();
            this.pnlAliEnterpolAVLPdGraphic = new System.Windows.Forms.Panel();
            this.cmbAliEnterpolAVLLabelAlignment = new System.Windows.Forms.ComboBox();
            this.lblAliEnterpolAVLLabelAlignment = new System.Windows.Forms.Label();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.lblAliEnterpolAVLSymbolFontName = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.btnAliEnterpolAVLLabelFont = new System.Windows.Forms.Button();
            this.lblAliEnterpolAVLLabelFontSize = new System.Windows.Forms.Label();
            this.lblAliEnterpolAVLLabelFontName = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.lblAliEnterpolAVLAgeFreq = new System.Windows.Forms.Label();
            this.numAliEnterpolAVLAge1Freq = new System.Windows.Forms.NumericUpDown();
            this.numAliEnterpolAVLAge2Freq = new System.Windows.Forms.NumericUpDown();
            this.numAliEnterpolAVLAge3Freq = new System.Windows.Forms.NumericUpDown();
            this.chkAutoHideInactiveUnits = new System.Windows.Forms.CheckBox();
            this.lblAliEnterpolAVLInitialCatalog = new System.Windows.Forms.Label();
            this.lblAliEnterpolAVLWhoAmIProc = new System.Windows.Forms.Label();
            this.numAliEnterpolAVLReadFreq = new System.Windows.Forms.NumericUpDown();
            this.numAliEnterpolAVLUpdateFreq = new System.Windows.Forms.NumericUpDown();
            this.lblAliEnterpolAVLTableName = new System.Windows.Forms.Label();
            this.txtAliEnterpolAVLTableName = new System.Windows.Forms.TextBox();
            this.lblAliEnterpolAVLUpdateFreq = new System.Windows.Forms.Label();
            this.txtAliEnterpolAVLInitialCatalog = new System.Windows.Forms.TextBox();
            this.lblAliEnterpolAVLReadFreq = new System.Windows.Forms.Label();
            this.txtAliEnterpolAVLWhoAmIProc = new System.Windows.Forms.TextBox();
            this.lblAliEnterpolAVLSetLocProc = new System.Windows.Forms.Label();
            this.txtAliEnterpolAVLSetLocProc = new System.Windows.Forms.TextBox();
            this.chkEnterpolAvl = new System.Windows.Forms.CheckBox();
            this.txtAliEnterpolInitialCatalog = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lblAliEnterpolDataSource = new System.Windows.Forms.Label();
            this.lblAliEnterpolTableName = new System.Windows.Forms.Label();
            this.lblAliEnterpolInitialCatalog = new System.Windows.Forms.Label();
            this.txtAliEnterpolDataSource = new System.Windows.Forms.TextBox();
            this.txtAliEnterpolTableName = new System.Windows.Forms.TextBox();
            this.pnlAliSdrServer = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.btnAliInterfaceDbPathBrowse = new System.Windows.Forms.Button();
            this.lblAliInterfaceDbPath = new System.Windows.Forms.Label();
            this.txtAliInterfaceDbPath = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lblAliInterfaceUdpHost = new System.Windows.Forms.Label();
            this.numAliInterfaceUdpPort = new System.Windows.Forms.NumericUpDown();
            this.lblAliInterfaceUdpPort = new System.Windows.Forms.Label();
            this.txtAliInterfaceUdpHost = new System.Windows.Forms.TextBox();
            this.pnlAliNetworkfleet = new System.Windows.Forms.TableLayoutPanel();
            this.tblAliNetworkfleetSettings = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numAliNetworkfleetAVLAge1Freq = new System.Windows.Forms.NumericUpDown();
            this.chkAutoHideInactiveUnitsNetworkfleet = new System.Windows.Forms.CheckBox();
            this.numAliNetworkfleetAVLAge2Freq = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.numAliNetworkfleetAVLAge3Freq = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numAliNetworkfleetUdpPort = new System.Windows.Forms.NumericUpDown();
            this.lblAliNetworkfleetUdpHost = new System.Windows.Forms.Label();
            this.txtAliNetworkfleetUdpHost = new System.Windows.Forms.TextBox();
            this.tblAliNetworkfleetLabeling = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numAliNetworkfleetLabelYOffset = new System.Windows.Forms.NumericUpDown();
            this.numAliNetworkfleetLabelXOffset = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.txtAliNetworkfleetLabelFontSize = new System.Windows.Forms.TextBox();
            this.btnAliNetworkfleetLabelFont = new System.Windows.Forms.Button();
            this.lblAliNetworkfleetLabelFont = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.dgvNetworkfleetLabelLookup = new System.Windows.Forms.DataGridView();
            this.vehicle_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vehicle_label = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblAliNetworkfleetLabelFontSize = new System.Windows.Forms.Label();
            this.cmbAliNetworkfleetLabelAlignment = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.tblAliNetworkfleetSymbology = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tblNetworkfleetAvlColors = new System.Windows.Forms.TableLayoutPanel();
            this.label21 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.ptAliNetworkfleetColor = new System.Windows.Forms.Panel();
            this.pnlAliNetworkfleetAVLInactiveColor = new System.Windows.Forms.Panel();
            this.pnlAliNetworkfleetAVLMyVehicleColor = new System.Windows.Forms.Panel();
            this.ptAliNetworkfleetGraphic = new System.Windows.Forms.Panel();
            this.label22 = new System.Windows.Forms.Label();
            this.btnAliNetworkfleetFont = new System.Windows.Forms.Button();
            this.lblAliNetworkfleetFont = new System.Windows.Forms.Label();
            this.txtAliNetworkfleetSize = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txtAliNetworkfleetChar = new System.Windows.Forms.TextBox();
            this.pnlAliGlobalCad = new System.Windows.Forms.TableLayoutPanel();
            this.btnAliGlobalCadConfigIniBrowse = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnAliGlobalCadArchivePathBrowse = new System.Windows.Forms.Button();
            this.lblAliGlobalCadLogPath = new System.Windows.Forms.Label();
            this.btnAliGlobalCadLogPathBrowse = new System.Windows.Forms.Button();
            this.txtAliGlobalCadConfigIni = new System.Windows.Forms.TextBox();
            this.lblAliGlobalCadArchivePath = new System.Windows.Forms.Label();
            this.txtAliGlobalCadArchivePath = new System.Windows.Forms.TextBox();
            this.lblAliGlobalCadConfigIni = new System.Windows.Forms.Label();
            this.txtAliGlobalCadLogPath = new System.Windows.Forms.TextBox();
            this.pnlAliValidate = new System.Windows.Forms.TableLayoutPanel();
            this.cmbAliMode = new System.Windows.Forms.ComboBox();
            this.btnAliValidate = new System.Windows.Forms.Button();
            this.chkNetworkfleet = new System.Windows.Forms.CheckBox();
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
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
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
            this.aliPanelTableLayout.SuspendLayout();
            this.pnlAliEnterpol.SuspendLayout();
            this.pnlAliEnterpolAvl.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLLabelXOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLLabelYOffset)).BeginInit();
            this.tblEnterpolAvlSymbology.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLAge1Freq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLAge2Freq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLAge3Freq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLReadFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLUpdateFreq)).BeginInit();
            this.pnlAliSdrServer.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAliInterfaceUdpPort)).BeginInit();
            this.pnlAliNetworkfleet.SuspendLayout();
            this.tblAliNetworkfleetSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAliNetworkfleetAVLAge1Freq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliNetworkfleetAVLAge2Freq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliNetworkfleetAVLAge3Freq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliNetworkfleetUdpPort)).BeginInit();
            this.tblAliNetworkfleetLabeling.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAliNetworkfleetLabelYOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliNetworkfleetLabelXOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNetworkfleetLabelLookup)).BeginInit();
            this.tblAliNetworkfleetSymbology.SuspendLayout();
            this.tblNetworkfleetAvlColors.SuspendLayout();
            this.pnlAliGlobalCad.SuspendLayout();
            this.pnlAliValidate.SuspendLayout();
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(598, 777);
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
            this.adminTab_Control.Size = new System.Drawing.Size(598, 742);
            this.adminTab_Control.TabIndex = 8;
            // 
            // adminTab_MapConfiguration
            // 
            this.adminTab_MapConfiguration.BackColor = System.Drawing.Color.Transparent;
            this.adminTab_MapConfiguration.Controls.Add(this.adminLayerSplitter);
            this.adminTab_MapConfiguration.Location = new System.Drawing.Point(4, 22);
            this.adminTab_MapConfiguration.Margin = new System.Windows.Forms.Padding(0);
            this.adminTab_MapConfiguration.Name = "adminTab_MapConfiguration";
            this.adminTab_MapConfiguration.Size = new System.Drawing.Size(590, 716);
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
            this.adminLayerSplitter.Size = new System.Drawing.Size(590, 716);
            this.adminLayerSplitter.SplitterDistance = 186;
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
            this.legendSplitter.Size = new System.Drawing.Size(184, 714);
            this.legendSplitter.SplitterDistance = 680;
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
            this.legendButtonTable.Size = new System.Drawing.Size(184, 33);
            this.legendButtonTable.TabIndex = 0;
            // 
            // btnRemoveLayer
            // 
            this.btnRemoveLayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRemoveLayer.Location = new System.Drawing.Point(95, 3);
            this.btnRemoveLayer.MaximumSize = new System.Drawing.Size(0, 27);
            this.btnRemoveLayer.MinimumSize = new System.Drawing.Size(55, 23);
            this.btnRemoveLayer.Name = "btnRemoveLayer";
            this.btnRemoveLayer.Size = new System.Drawing.Size(86, 27);
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
            this.btnAddLayer.Size = new System.Drawing.Size(86, 27);
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
            this.adminTab_ProjectSettings.Controls.Add(this.dataGridView1);
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
            this.adminTab_ProjectSettings.Size = new System.Drawing.Size(590, 716);
            this.adminTab_ProjectSettings.TabIndex = 5;
            this.adminTab_ProjectSettings.Text = "Project Settings";
            this.adminTab_ProjectSettings.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.layerName,
            this.fieldName,
            this.add,
            this.remove});
            this.dataGridView1.Location = new System.Drawing.Point(26, 249);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(537, 324);
            this.dataGridView1.TabIndex = 50;
            // 
            // layerName
            // 
            this.layerName.HeaderText = "Layer Name";
            this.layerName.Name = "layerName";
            this.layerName.ReadOnly = true;
            // 
            // fieldName
            // 
            this.fieldName.HeaderText = "Field Name";
            this.fieldName.Name = "fieldName";
            this.fieldName.ReadOnly = true;
            // 
            // add
            // 
            this.add.HeaderText = "Add";
            this.add.Name = "add";
            this.add.ReadOnly = true;
            // 
            // remove
            // 
            this.remove.HeaderText = "Remove";
            this.remove.Name = "remove";
            this.remove.ReadOnly = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(45, 204);
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
            this.lineSymbolSize.Location = new System.Drawing.Point(505, 110);
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
            this.lineSymbolStyle.Size = new System.Drawing.Size(202, 21);
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
            this.adminTab_ProgramManagement.Size = new System.Drawing.Size(590, 716);
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
            this.adminTab_SearchSettings.Size = new System.Drawing.Size(590, 716);
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
            // adminTab_AliSettings
            // 
            this.adminTab_AliSettings.Controls.Add(this.aliPanelTableLayout);
            this.adminTab_AliSettings.Location = new System.Drawing.Point(4, 22);
            this.adminTab_AliSettings.Name = "adminTab_AliSettings";
            this.adminTab_AliSettings.Padding = new System.Windows.Forms.Padding(3);
            this.adminTab_AliSettings.Size = new System.Drawing.Size(590, 716);
            this.adminTab_AliSettings.TabIndex = 6;
            this.adminTab_AliSettings.Text = "ALI Settings";
            this.adminTab_AliSettings.UseVisualStyleBackColor = true;
            // 
            // aliPanelTableLayout
            // 
            this.aliPanelTableLayout.ColumnCount = 2;
            this.aliPanelTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.aliPanelTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.aliPanelTableLayout.Controls.Add(this.pnlAliEnterpol, 1, 3);
            this.aliPanelTableLayout.Controls.Add(this.pnlAliSdrServer, 1, 1);
            this.aliPanelTableLayout.Controls.Add(this.pnlAliNetworkfleet, 0, 1);
            this.aliPanelTableLayout.Controls.Add(this.pnlAliGlobalCad, 1, 2);
            this.aliPanelTableLayout.Controls.Add(this.pnlAliValidate, 0, 0);
            this.aliPanelTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aliPanelTableLayout.Location = new System.Drawing.Point(3, 3);
            this.aliPanelTableLayout.Name = "aliPanelTableLayout";
            this.aliPanelTableLayout.RowCount = 4;
            this.aliPanelTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.aliPanelTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.aliPanelTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.aliPanelTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.aliPanelTableLayout.Size = new System.Drawing.Size(584, 710);
            this.aliPanelTableLayout.TabIndex = 73;
            // 
            // pnlAliEnterpol
            // 
            this.pnlAliEnterpol.ColumnCount = 3;
            this.pnlAliEnterpol.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlAliEnterpol.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlAliEnterpol.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlAliEnterpol.Controls.Add(this.pnlAliEnterpolAvl, 0, 5);
            this.pnlAliEnterpol.Controls.Add(this.chkEnterpolAvl, 0, 4);
            this.pnlAliEnterpol.Controls.Add(this.txtAliEnterpolInitialCatalog, 2, 3);
            this.pnlAliEnterpol.Controls.Add(this.groupBox6, 0, 0);
            this.pnlAliEnterpol.Controls.Add(this.lblAliEnterpolDataSource, 0, 1);
            this.pnlAliEnterpol.Controls.Add(this.lblAliEnterpolTableName, 0, 2);
            this.pnlAliEnterpol.Controls.Add(this.lblAliEnterpolInitialCatalog, 0, 3);
            this.pnlAliEnterpol.Controls.Add(this.txtAliEnterpolDataSource, 1, 1);
            this.pnlAliEnterpol.Controls.Add(this.txtAliEnterpolTableName, 1, 2);
            this.pnlAliEnterpol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAliEnterpol.Location = new System.Drawing.Point(241, 266);
            this.pnlAliEnterpol.Margin = new System.Windows.Forms.Padding(8, 0, 0, 3);
            this.pnlAliEnterpol.MinimumSize = new System.Drawing.Size(0, 644);
            this.pnlAliEnterpol.Name = "pnlAliEnterpol";
            this.pnlAliEnterpol.RowCount = 7;
            this.pnlAliEnterpol.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliEnterpol.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliEnterpol.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliEnterpol.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliEnterpol.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliEnterpol.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliEnterpol.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlAliEnterpol.Size = new System.Drawing.Size(343, 644);
            this.pnlAliEnterpol.TabIndex = 74;
            // 
            // pnlAliEnterpolAvl
            // 
            this.pnlAliEnterpolAvl.ColumnCount = 4;
            this.pnlAliEnterpol.SetColumnSpan(this.pnlAliEnterpolAvl, 3);
            this.pnlAliEnterpolAvl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlAliEnterpolAvl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlAliEnterpolAvl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 99F));
            this.pnlAliEnterpolAvl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlAliEnterpolAvl.Controls.Add(this.tableLayoutPanel7, 0, 8);
            this.pnlAliEnterpolAvl.Controls.Add(this.groupBox7, 0, 0);
            this.pnlAliEnterpolAvl.Controls.Add(this.tableLayoutPanel4, 0, 7);
            this.pnlAliEnterpolAvl.Controls.Add(this.lblAliEnterpolAVLInitialCatalog, 0, 2);
            this.pnlAliEnterpolAvl.Controls.Add(this.lblAliEnterpolAVLWhoAmIProc, 0, 6);
            this.pnlAliEnterpolAvl.Controls.Add(this.numAliEnterpolAVLReadFreq, 2, 3);
            this.pnlAliEnterpolAvl.Controls.Add(this.numAliEnterpolAVLUpdateFreq, 2, 5);
            this.pnlAliEnterpolAvl.Controls.Add(this.lblAliEnterpolAVLTableName, 0, 1);
            this.pnlAliEnterpolAvl.Controls.Add(this.txtAliEnterpolAVLTableName, 1, 1);
            this.pnlAliEnterpolAvl.Controls.Add(this.lblAliEnterpolAVLUpdateFreq, 0, 5);
            this.pnlAliEnterpolAvl.Controls.Add(this.txtAliEnterpolAVLInitialCatalog, 2, 2);
            this.pnlAliEnterpolAvl.Controls.Add(this.lblAliEnterpolAVLReadFreq, 0, 3);
            this.pnlAliEnterpolAvl.Controls.Add(this.txtAliEnterpolAVLWhoAmIProc, 2, 6);
            this.pnlAliEnterpolAvl.Controls.Add(this.lblAliEnterpolAVLSetLocProc, 0, 4);
            this.pnlAliEnterpolAvl.Controls.Add(this.txtAliEnterpolAVLSetLocProc, 2, 4);
            this.pnlAliEnterpolAvl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAliEnterpolAvl.Location = new System.Drawing.Point(0, 124);
            this.pnlAliEnterpolAvl.Margin = new System.Windows.Forms.Padding(0);
            this.pnlAliEnterpolAvl.MinimumSize = new System.Drawing.Size(0, 520);
            this.pnlAliEnterpolAvl.Name = "pnlAliEnterpolAvl";
            this.pnlAliEnterpolAvl.RowCount = 10;
            this.pnlAliEnterpolAvl.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliEnterpolAvl.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliEnterpolAvl.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliEnterpolAvl.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliEnterpolAvl.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliEnterpolAvl.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliEnterpolAvl.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliEnterpolAvl.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliEnterpolAvl.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliEnterpolAvl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlAliEnterpolAvl.Size = new System.Drawing.Size(343, 536);
            this.pnlAliEnterpolAvl.TabIndex = 110;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 5;
            this.pnlAliEnterpolAvl.SetColumnSpan(this.tableLayoutPanel7, 4);
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel8, 0, 6);
            this.tableLayoutPanel7.Controls.Add(this.btnAliEnterpolAVLSymbolFont, 0, 1);
            this.tableLayoutPanel7.Controls.Add(this.txtAliEnterpolAVLSymbolFontSize, 4, 1);
            this.tableLayoutPanel7.Controls.Add(this.lblAliEnterpolAVLSymbolFontSize, 3, 1);
            this.tableLayoutPanel7.Controls.Add(this.txtAliEnterpolAVLLabelFontSize, 4, 4);
            this.tableLayoutPanel7.Controls.Add(this.tblEnterpolAvlSymbology, 0, 2);
            this.tableLayoutPanel7.Controls.Add(this.cmbAliEnterpolAVLLabelAlignment, 1, 5);
            this.tableLayoutPanel7.Controls.Add(this.lblAliEnterpolAVLLabelAlignment, 0, 5);
            this.tableLayoutPanel7.Controls.Add(this.groupBox9, 0, 3);
            this.tableLayoutPanel7.Controls.Add(this.lblAliEnterpolAVLSymbolFontName, 1, 1);
            this.tableLayoutPanel7.Controls.Add(this.groupBox8, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.btnAliEnterpolAVLLabelFont, 0, 4);
            this.tableLayoutPanel7.Controls.Add(this.lblAliEnterpolAVLLabelFontSize, 3, 4);
            this.tableLayoutPanel7.Controls.Add(this.lblAliEnterpolAVLLabelFontName, 1, 4);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(0, 248);
            this.tableLayoutPanel7.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel7.MinimumSize = new System.Drawing.Size(0, 272);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 8;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(343, 280);
            this.tableLayoutPanel7.TabIndex = 111;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 4;
            this.tableLayoutPanel7.SetColumnSpan(this.tableLayoutPanel8, 5);
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.Controls.Add(this.label10, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.numAliEnterpolAVLLabelXOffset, 1, 0);
            this.tableLayoutPanel8.Controls.Add(this.label11, 2, 0);
            this.tableLayoutPanel8.Controls.Add(this.numAliEnterpolAVLLabelYOffset, 3, 0);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(0, 245);
            this.tableLayoutPanel8.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel8.MinimumSize = new System.Drawing.Size(0, 27);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 2;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(343, 27);
            this.tableLayoutPanel8.TabIndex = 112;
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 6);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 13);
            this.label10.TabIndex = 96;
            this.label10.Text = "X Offset:";
            // 
            // numAliEnterpolAVLLabelXOffset
            // 
            this.numAliEnterpolAVLLabelXOffset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numAliEnterpolAVLLabelXOffset.Location = new System.Drawing.Point(57, 3);
            this.numAliEnterpolAVLLabelXOffset.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numAliEnterpolAVLLabelXOffset.Name = "numAliEnterpolAVLLabelXOffset";
            this.numAliEnterpolAVLLabelXOffset.Size = new System.Drawing.Size(111, 20);
            this.numAliEnterpolAVLLabelXOffset.TabIndex = 93;
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(174, 6);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(48, 13);
            this.label11.TabIndex = 97;
            this.label11.Text = "Y Offset:";
            // 
            // numAliEnterpolAVLLabelYOffset
            // 
            this.numAliEnterpolAVLLabelYOffset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numAliEnterpolAVLLabelYOffset.Location = new System.Drawing.Point(228, 3);
            this.numAliEnterpolAVLLabelYOffset.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numAliEnterpolAVLLabelYOffset.Name = "numAliEnterpolAVLLabelYOffset";
            this.numAliEnterpolAVLLabelYOffset.Size = new System.Drawing.Size(112, 20);
            this.numAliEnterpolAVLLabelYOffset.TabIndex = 94;
            // 
            // btnAliEnterpolAVLSymbolFont
            // 
            this.btnAliEnterpolAVLSymbolFont.Location = new System.Drawing.Point(3, 24);
            this.btnAliEnterpolAVLSymbolFont.Name = "btnAliEnterpolAVLSymbolFont";
            this.btnAliEnterpolAVLSymbolFont.Size = new System.Drawing.Size(70, 23);
            this.btnAliEnterpolAVLSymbolFont.TabIndex = 75;
            this.btnAliEnterpolAVLSymbolFont.Text = "Select Font";
            this.btnAliEnterpolAVLSymbolFont.UseVisualStyleBackColor = true;
            this.btnAliEnterpolAVLSymbolFont.Click += new System.EventHandler(this.btnAliEnterpolAVLFont_Click);
            // 
            // txtAliEnterpolAVLSymbolFontSize
            // 
            this.txtAliEnterpolAVLSymbolFontSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAliEnterpolAVLSymbolFontSize.Location = new System.Drawing.Point(287, 24);
            this.txtAliEnterpolAVLSymbolFontSize.Name = "txtAliEnterpolAVLSymbolFontSize";
            this.txtAliEnterpolAVLSymbolFontSize.ReadOnly = true;
            this.txtAliEnterpolAVLSymbolFontSize.Size = new System.Drawing.Size(53, 20);
            this.txtAliEnterpolAVLSymbolFontSize.TabIndex = 106;
            // 
            // lblAliEnterpolAVLSymbolFontSize
            // 
            this.lblAliEnterpolAVLSymbolFontSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAliEnterpolAVLSymbolFontSize.AutoSize = true;
            this.lblAliEnterpolAVLSymbolFontSize.Location = new System.Drawing.Point(227, 29);
            this.lblAliEnterpolAVLSymbolFontSize.Name = "lblAliEnterpolAVLSymbolFontSize";
            this.lblAliEnterpolAVLSymbolFontSize.Size = new System.Drawing.Size(54, 13);
            this.lblAliEnterpolAVLSymbolFontSize.TabIndex = 80;
            this.lblAliEnterpolAVLSymbolFontSize.Text = "Font Size:";
            // 
            // txtAliEnterpolAVLLabelFontSize
            // 
            this.txtAliEnterpolAVLLabelFontSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAliEnterpolAVLLabelFontSize.Location = new System.Drawing.Point(287, 192);
            this.txtAliEnterpolAVLLabelFontSize.Name = "txtAliEnterpolAVLLabelFontSize";
            this.txtAliEnterpolAVLLabelFontSize.ReadOnly = true;
            this.txtAliEnterpolAVLLabelFontSize.Size = new System.Drawing.Size(53, 20);
            this.txtAliEnterpolAVLLabelFontSize.TabIndex = 108;
            // 
            // tblEnterpolAvlSymbology
            // 
            this.tblEnterpolAvlSymbology.ColumnCount = 5;
            this.tableLayoutPanel7.SetColumnSpan(this.tblEnterpolAvlSymbology, 5);
            this.tblEnterpolAvlSymbology.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tblEnterpolAvlSymbology.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tblEnterpolAvlSymbology.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tblEnterpolAvlSymbology.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tblEnterpolAvlSymbology.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tblEnterpolAvlSymbology.Controls.Add(this.label7, 0, 0);
            this.tblEnterpolAvlSymbology.Controls.Add(this.label9, 1, 0);
            this.tblEnterpolAvlSymbology.Controls.Add(this.label8, 2, 0);
            this.tblEnterpolAvlSymbology.Controls.Add(this.label5, 3, 0);
            this.tblEnterpolAvlSymbology.Controls.Add(this.lblAliEnterpolAVLMyVehicleColor, 4, 0);
            this.tblEnterpolAvlSymbology.Controls.Add(this.pnlAliEnterpolAVLEmsColor, 0, 1);
            this.tblEnterpolAvlSymbology.Controls.Add(this.pnlAliEnterpolAVLFdColor, 1, 1);
            this.tblEnterpolAvlSymbology.Controls.Add(this.pnlAliEnterpolAVLPdColor, 2, 1);
            this.tblEnterpolAvlSymbology.Controls.Add(this.pnlAliAVLInactiveColor, 3, 1);
            this.tblEnterpolAvlSymbology.Controls.Add(this.pnlAliEnterpolAVLMyVehicleColor, 4, 1);
            this.tblEnterpolAvlSymbology.Controls.Add(this.txtAliEnterpolAVLEmsChars, 0, 2);
            this.tblEnterpolAvlSymbology.Controls.Add(this.txtAliEnterpolAVLFdChars, 1, 2);
            this.tblEnterpolAvlSymbology.Controls.Add(this.txtAliEnterpolAVLPdChars, 2, 2);
            this.tblEnterpolAvlSymbology.Controls.Add(this.pnlAliEnterpolAVLEmsGraphic, 0, 3);
            this.tblEnterpolAvlSymbology.Controls.Add(this.pnlAliEnterpolAVLFdGraphic, 1, 3);
            this.tblEnterpolAvlSymbology.Controls.Add(this.pnlAliEnterpolAVLPdGraphic, 2, 3);
            this.tblEnterpolAvlSymbology.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblEnterpolAvlSymbology.Location = new System.Drawing.Point(0, 50);
            this.tblEnterpolAvlSymbology.Margin = new System.Windows.Forms.Padding(0);
            this.tblEnterpolAvlSymbology.MinimumSize = new System.Drawing.Size(0, 116);
            this.tblEnterpolAvlSymbology.Name = "tblEnterpolAvlSymbology";
            this.tblEnterpolAvlSymbology.RowCount = 5;
            this.tblEnterpolAvlSymbology.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblEnterpolAvlSymbology.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblEnterpolAvlSymbology.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblEnterpolAvlSymbology.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblEnterpolAvlSymbology.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblEnterpolAvlSymbology.Size = new System.Drawing.Size(343, 116);
            this.tblEnterpolAvlSymbology.TabIndex = 107;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 13);
            this.label7.TabIndex = 98;
            this.label7.Text = "EMS";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(91, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(21, 13);
            this.label9.TabIndex = 100;
            this.label9.Text = "FD";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(160, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(20, 13);
            this.label8.TabIndex = 99;
            this.label8.Text = "LE";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(215, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 88;
            this.label5.Text = "Inactive";
            // 
            // lblAliEnterpolAVLMyVehicleColor
            // 
            this.lblAliEnterpolAVLMyVehicleColor.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblAliEnterpolAVLMyVehicleColor.AutoSize = true;
            this.lblAliEnterpolAVLMyVehicleColor.Location = new System.Drawing.Point(286, 0);
            this.lblAliEnterpolAVLMyVehicleColor.Name = "lblAliEnterpolAVLMyVehicleColor";
            this.lblAliEnterpolAVLMyVehicleColor.Size = new System.Drawing.Size(43, 13);
            this.lblAliEnterpolAVLMyVehicleColor.TabIndex = 86;
            this.lblAliEnterpolAVLMyVehicleColor.Text = "My Unit";
            // 
            // pnlAliEnterpolAVLEmsColor
            // 
            this.pnlAliEnterpolAVLEmsColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAliEnterpolAVLEmsColor.Location = new System.Drawing.Point(3, 16);
            this.pnlAliEnterpolAVLEmsColor.MaximumSize = new System.Drawing.Size(0, 50);
            this.pnlAliEnterpolAVLEmsColor.MinimumSize = new System.Drawing.Size(0, 24);
            this.pnlAliEnterpolAVLEmsColor.Name = "pnlAliEnterpolAVLEmsColor";
            this.pnlAliEnterpolAVLEmsColor.Size = new System.Drawing.Size(62, 24);
            this.pnlAliEnterpolAVLEmsColor.TabIndex = 72;
            // 
            // pnlAliEnterpolAVLFdColor
            // 
            this.pnlAliEnterpolAVLFdColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAliEnterpolAVLFdColor.Location = new System.Drawing.Point(71, 16);
            this.pnlAliEnterpolAVLFdColor.MaximumSize = new System.Drawing.Size(0, 50);
            this.pnlAliEnterpolAVLFdColor.MinimumSize = new System.Drawing.Size(0, 24);
            this.pnlAliEnterpolAVLFdColor.Name = "pnlAliEnterpolAVLFdColor";
            this.pnlAliEnterpolAVLFdColor.Size = new System.Drawing.Size(62, 24);
            this.pnlAliEnterpolAVLFdColor.TabIndex = 74;
            // 
            // pnlAliEnterpolAVLPdColor
            // 
            this.pnlAliEnterpolAVLPdColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAliEnterpolAVLPdColor.Location = new System.Drawing.Point(139, 16);
            this.pnlAliEnterpolAVLPdColor.MaximumSize = new System.Drawing.Size(0, 50);
            this.pnlAliEnterpolAVLPdColor.MinimumSize = new System.Drawing.Size(0, 24);
            this.pnlAliEnterpolAVLPdColor.Name = "pnlAliEnterpolAVLPdColor";
            this.pnlAliEnterpolAVLPdColor.Size = new System.Drawing.Size(62, 24);
            this.pnlAliEnterpolAVLPdColor.TabIndex = 73;
            // 
            // pnlAliAVLInactiveColor
            // 
            this.pnlAliAVLInactiveColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAliAVLInactiveColor.Location = new System.Drawing.Point(207, 16);
            this.pnlAliAVLInactiveColor.MaximumSize = new System.Drawing.Size(0, 50);
            this.pnlAliAVLInactiveColor.MinimumSize = new System.Drawing.Size(0, 24);
            this.pnlAliAVLInactiveColor.Name = "pnlAliAVLInactiveColor";
            this.pnlAliAVLInactiveColor.Size = new System.Drawing.Size(62, 24);
            this.pnlAliAVLInactiveColor.TabIndex = 87;
            // 
            // pnlAliEnterpolAVLMyVehicleColor
            // 
            this.pnlAliEnterpolAVLMyVehicleColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAliEnterpolAVLMyVehicleColor.Location = new System.Drawing.Point(275, 16);
            this.pnlAliEnterpolAVLMyVehicleColor.MaximumSize = new System.Drawing.Size(0, 50);
            this.pnlAliEnterpolAVLMyVehicleColor.MinimumSize = new System.Drawing.Size(0, 24);
            this.pnlAliEnterpolAVLMyVehicleColor.Name = "pnlAliEnterpolAVLMyVehicleColor";
            this.pnlAliEnterpolAVLMyVehicleColor.Size = new System.Drawing.Size(65, 24);
            this.pnlAliEnterpolAVLMyVehicleColor.TabIndex = 85;
            // 
            // txtAliEnterpolAVLEmsChars
            // 
            this.txtAliEnterpolAVLEmsChars.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAliEnterpolAVLEmsChars.Location = new System.Drawing.Point(3, 46);
            this.txtAliEnterpolAVLEmsChars.Name = "txtAliEnterpolAVLEmsChars";
            this.txtAliEnterpolAVLEmsChars.Size = new System.Drawing.Size(62, 20);
            this.txtAliEnterpolAVLEmsChars.TabIndex = 76;
            // 
            // txtAliEnterpolAVLFdChars
            // 
            this.txtAliEnterpolAVLFdChars.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAliEnterpolAVLFdChars.Location = new System.Drawing.Point(71, 46);
            this.txtAliEnterpolAVLFdChars.Name = "txtAliEnterpolAVLFdChars";
            this.txtAliEnterpolAVLFdChars.Size = new System.Drawing.Size(62, 20);
            this.txtAliEnterpolAVLFdChars.TabIndex = 78;
            // 
            // txtAliEnterpolAVLPdChars
            // 
            this.txtAliEnterpolAVLPdChars.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAliEnterpolAVLPdChars.Location = new System.Drawing.Point(139, 46);
            this.txtAliEnterpolAVLPdChars.Name = "txtAliEnterpolAVLPdChars";
            this.txtAliEnterpolAVLPdChars.Size = new System.Drawing.Size(62, 20);
            this.txtAliEnterpolAVLPdChars.TabIndex = 77;
            // 
            // pnlAliEnterpolAVLEmsGraphic
            // 
            this.pnlAliEnterpolAVLEmsGraphic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAliEnterpolAVLEmsGraphic.Location = new System.Drawing.Point(3, 72);
            this.pnlAliEnterpolAVLEmsGraphic.MinimumSize = new System.Drawing.Size(0, 40);
            this.pnlAliEnterpolAVLEmsGraphic.Name = "pnlAliEnterpolAVLEmsGraphic";
            this.pnlAliEnterpolAVLEmsGraphic.Size = new System.Drawing.Size(62, 40);
            this.pnlAliEnterpolAVLEmsGraphic.TabIndex = 81;
            // 
            // pnlAliEnterpolAVLFdGraphic
            // 
            this.pnlAliEnterpolAVLFdGraphic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAliEnterpolAVLFdGraphic.Location = new System.Drawing.Point(71, 72);
            this.pnlAliEnterpolAVLFdGraphic.MinimumSize = new System.Drawing.Size(0, 40);
            this.pnlAliEnterpolAVLFdGraphic.Name = "pnlAliEnterpolAVLFdGraphic";
            this.pnlAliEnterpolAVLFdGraphic.Size = new System.Drawing.Size(62, 40);
            this.pnlAliEnterpolAVLFdGraphic.TabIndex = 82;
            // 
            // pnlAliEnterpolAVLPdGraphic
            // 
            this.pnlAliEnterpolAVLPdGraphic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAliEnterpolAVLPdGraphic.Location = new System.Drawing.Point(139, 72);
            this.pnlAliEnterpolAVLPdGraphic.MinimumSize = new System.Drawing.Size(0, 40);
            this.pnlAliEnterpolAVLPdGraphic.Name = "pnlAliEnterpolAVLPdGraphic";
            this.pnlAliEnterpolAVLPdGraphic.Size = new System.Drawing.Size(62, 40);
            this.pnlAliEnterpolAVLPdGraphic.TabIndex = 83;
            // 
            // cmbAliEnterpolAVLLabelAlignment
            // 
            this.tableLayoutPanel7.SetColumnSpan(this.cmbAliEnterpolAVLLabelAlignment, 4);
            this.cmbAliEnterpolAVLLabelAlignment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbAliEnterpolAVLLabelAlignment.FormattingEnabled = true;
            this.cmbAliEnterpolAVLLabelAlignment.Items.AddRange(new object[] {
            "Above",
            "Below",
            "Right",
            "Left"});
            this.cmbAliEnterpolAVLLabelAlignment.Location = new System.Drawing.Point(79, 221);
            this.cmbAliEnterpolAVLLabelAlignment.Name = "cmbAliEnterpolAVLLabelAlignment";
            this.cmbAliEnterpolAVLLabelAlignment.Size = new System.Drawing.Size(261, 21);
            this.cmbAliEnterpolAVLLabelAlignment.TabIndex = 92;
            this.cmbAliEnterpolAVLLabelAlignment.SelectedIndexChanged += new System.EventHandler(this.cmbAliEnterpolAVLLabelAlignment_SelectedIndexChanged);
            // 
            // lblAliEnterpolAVLLabelAlignment
            // 
            this.lblAliEnterpolAVLLabelAlignment.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAliEnterpolAVLLabelAlignment.AutoSize = true;
            this.lblAliEnterpolAVLLabelAlignment.Location = new System.Drawing.Point(17, 225);
            this.lblAliEnterpolAVLLabelAlignment.Name = "lblAliEnterpolAVLLabelAlignment";
            this.lblAliEnterpolAVLLabelAlignment.Size = new System.Drawing.Size(56, 13);
            this.lblAliEnterpolAVLLabelAlignment.TabIndex = 95;
            this.lblAliEnterpolAVLLabelAlignment.Text = "Alignment:";
            // 
            // groupBox9
            // 
            this.tableLayoutPanel7.SetColumnSpan(this.groupBox9, 5);
            this.groupBox9.Location = new System.Drawing.Point(0, 169);
            this.groupBox9.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.groupBox9.MinimumSize = new System.Drawing.Size(0, 15);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.groupBox9.Size = new System.Drawing.Size(335, 17);
            this.groupBox9.TabIndex = 105;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Enterpol AVL Labeling";
            // 
            // lblAliEnterpolAVLSymbolFontName
            // 
            this.lblAliEnterpolAVLSymbolFontName.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel7.SetColumnSpan(this.lblAliEnterpolAVLSymbolFontName, 2);
            this.lblAliEnterpolAVLSymbolFontName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAliEnterpolAVLSymbolFontName.Location = new System.Drawing.Point(79, 21);
            this.lblAliEnterpolAVLSymbolFontName.Name = "lblAliEnterpolAVLSymbolFontName";
            this.lblAliEnterpolAVLSymbolFontName.Size = new System.Drawing.Size(142, 29);
            this.lblAliEnterpolAVLSymbolFontName.TabIndex = 79;
            this.lblAliEnterpolAVLSymbolFontName.Text = "This is the font name";
            // 
            // groupBox8
            // 
            this.tableLayoutPanel7.SetColumnSpan(this.groupBox8, 5);
            this.groupBox8.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox8.Location = new System.Drawing.Point(0, 3);
            this.groupBox8.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.groupBox8.MinimumSize = new System.Drawing.Size(0, 15);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.groupBox8.Size = new System.Drawing.Size(343, 15);
            this.groupBox8.TabIndex = 104;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Enterpol AVL Symbology";
            // 
            // btnAliEnterpolAVLLabelFont
            // 
            this.btnAliEnterpolAVLLabelFont.Location = new System.Drawing.Point(3, 192);
            this.btnAliEnterpolAVLLabelFont.Name = "btnAliEnterpolAVLLabelFont";
            this.btnAliEnterpolAVLLabelFont.Size = new System.Drawing.Size(70, 23);
            this.btnAliEnterpolAVLLabelFont.TabIndex = 89;
            this.btnAliEnterpolAVLLabelFont.Text = "Select Font";
            this.btnAliEnterpolAVLLabelFont.UseVisualStyleBackColor = true;
            this.btnAliEnterpolAVLLabelFont.Click += new System.EventHandler(this.btnAliEnterpolAVLLabelFont_Click);
            // 
            // lblAliEnterpolAVLLabelFontSize
            // 
            this.lblAliEnterpolAVLLabelFontSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAliEnterpolAVLLabelFontSize.AutoSize = true;
            this.lblAliEnterpolAVLLabelFontSize.Location = new System.Drawing.Point(227, 197);
            this.lblAliEnterpolAVLLabelFontSize.Name = "lblAliEnterpolAVLLabelFontSize";
            this.lblAliEnterpolAVLLabelFontSize.Size = new System.Drawing.Size(54, 13);
            this.lblAliEnterpolAVLLabelFontSize.TabIndex = 91;
            this.lblAliEnterpolAVLLabelFontSize.Text = "Font Size:";
            // 
            // lblAliEnterpolAVLLabelFontName
            // 
            this.lblAliEnterpolAVLLabelFontName.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel7.SetColumnSpan(this.lblAliEnterpolAVLLabelFontName, 2);
            this.lblAliEnterpolAVLLabelFontName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAliEnterpolAVLLabelFontName.Location = new System.Drawing.Point(79, 189);
            this.lblAliEnterpolAVLLabelFontName.Name = "lblAliEnterpolAVLLabelFontName";
            this.lblAliEnterpolAVLLabelFontName.Size = new System.Drawing.Size(142, 29);
            this.lblAliEnterpolAVLLabelFontName.TabIndex = 90;
            this.lblAliEnterpolAVLLabelFontName.Text = "This is the font name";
            // 
            // groupBox7
            // 
            this.groupBox7.BackColor = System.Drawing.Color.Transparent;
            this.pnlAliEnterpolAvl.SetColumnSpan(this.groupBox7, 4);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox7.Location = new System.Drawing.Point(0, 3);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.groupBox7.MinimumSize = new System.Drawing.Size(0, 15);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.groupBox7.Size = new System.Drawing.Size(343, 17);
            this.groupBox7.TabIndex = 102;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Enterpol AVL Settings";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 3;
            this.pnlAliEnterpolAvl.SetColumnSpan(this.tableLayoutPanel4, 4);
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel4.Controls.Add(this.lblAliEnterpolAVLAgeFreq, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.numAliEnterpolAVLAge1Freq, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.numAliEnterpolAVLAge2Freq, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.numAliEnterpolAVLAge3Freq, 2, 1);
            this.tableLayoutPanel4.Controls.Add(this.chkAutoHideInactiveUnits, 0, 2);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 179);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.MinimumSize = new System.Drawing.Size(0, 69);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 4;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(343, 69);
            this.tableLayoutPanel4.TabIndex = 109;
            // 
            // lblAliEnterpolAVLAgeFreq
            // 
            this.lblAliEnterpolAVLAgeFreq.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblAliEnterpolAVLAgeFreq.AutoSize = true;
            this.tableLayoutPanel4.SetColumnSpan(this.lblAliEnterpolAVLAgeFreq, 3);
            this.lblAliEnterpolAVLAgeFreq.Location = new System.Drawing.Point(113, 0);
            this.lblAliEnterpolAVLAgeFreq.Name = "lblAliEnterpolAVLAgeFreq";
            this.lblAliEnterpolAVLAgeFreq.Padding = new System.Windows.Forms.Padding(0, 5, 0, 3);
            this.lblAliEnterpolAVLAgeFreq.Size = new System.Drawing.Size(116, 21);
            this.lblAliEnterpolAVLAgeFreq.TabIndex = 23;
            this.lblAliEnterpolAVLAgeFreq.Text = "AVL Age Frequency (s)";
            // 
            // numAliEnterpolAVLAge1Freq
            // 
            this.numAliEnterpolAVLAge1Freq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numAliEnterpolAVLAge1Freq.Location = new System.Drawing.Point(3, 24);
            this.numAliEnterpolAVLAge1Freq.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numAliEnterpolAVLAge1Freq.Name = "numAliEnterpolAVLAge1Freq";
            this.numAliEnterpolAVLAge1Freq.Size = new System.Drawing.Size(108, 20);
            this.numAliEnterpolAVLAge1Freq.TabIndex = 20;
            // 
            // numAliEnterpolAVLAge2Freq
            // 
            this.numAliEnterpolAVLAge2Freq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numAliEnterpolAVLAge2Freq.Location = new System.Drawing.Point(117, 24);
            this.numAliEnterpolAVLAge2Freq.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numAliEnterpolAVLAge2Freq.Name = "numAliEnterpolAVLAge2Freq";
            this.numAliEnterpolAVLAge2Freq.Size = new System.Drawing.Size(108, 20);
            this.numAliEnterpolAVLAge2Freq.TabIndex = 21;
            // 
            // numAliEnterpolAVLAge3Freq
            // 
            this.numAliEnterpolAVLAge3Freq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numAliEnterpolAVLAge3Freq.Location = new System.Drawing.Point(231, 24);
            this.numAliEnterpolAVLAge3Freq.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numAliEnterpolAVLAge3Freq.Name = "numAliEnterpolAVLAge3Freq";
            this.numAliEnterpolAVLAge3Freq.Size = new System.Drawing.Size(109, 20);
            this.numAliEnterpolAVLAge3Freq.TabIndex = 22;
            // 
            // chkAutoHideInactiveUnits
            // 
            this.chkAutoHideInactiveUnits.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkAutoHideInactiveUnits.AutoSize = true;
            this.tableLayoutPanel4.SetColumnSpan(this.chkAutoHideInactiveUnits, 3);
            this.chkAutoHideInactiveUnits.Location = new System.Drawing.Point(103, 50);
            this.chkAutoHideInactiveUnits.Name = "chkAutoHideInactiveUnits";
            this.chkAutoHideInactiveUnits.Size = new System.Drawing.Size(136, 17);
            this.chkAutoHideInactiveUnits.TabIndex = 84;
            this.chkAutoHideInactiveUnits.Text = "Autohide Inactive Units";
            this.chkAutoHideInactiveUnits.UseVisualStyleBackColor = true;
            // 
            // lblAliEnterpolAVLInitialCatalog
            // 
            this.lblAliEnterpolAVLInitialCatalog.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAliEnterpolAVLInitialCatalog.AutoSize = true;
            this.pnlAliEnterpolAvl.SetColumnSpan(this.lblAliEnterpolAVLInitialCatalog, 2);
            this.lblAliEnterpolAVLInitialCatalog.Location = new System.Drawing.Point(13, 55);
            this.lblAliEnterpolAVLInitialCatalog.Name = "lblAliEnterpolAVLInitialCatalog";
            this.lblAliEnterpolAVLInitialCatalog.Size = new System.Drawing.Size(123, 13);
            this.lblAliEnterpolAVLInitialCatalog.TabIndex = 12;
            this.lblAliEnterpolAVLInitialCatalog.Text = "Units Table/View Name:";
            // 
            // lblAliEnterpolAVLWhoAmIProc
            // 
            this.lblAliEnterpolAVLWhoAmIProc.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAliEnterpolAVLWhoAmIProc.AutoSize = true;
            this.pnlAliEnterpolAvl.SetColumnSpan(this.lblAliEnterpolAVLWhoAmIProc, 2);
            this.lblAliEnterpolAVLWhoAmIProc.Location = new System.Drawing.Point(33, 159);
            this.lblAliEnterpolAVLWhoAmIProc.Name = "lblAliEnterpolAVLWhoAmIProc";
            this.lblAliEnterpolAVLWhoAmIProc.Size = new System.Drawing.Size(103, 13);
            this.lblAliEnterpolAVLWhoAmIProc.TabIndex = 16;
            this.lblAliEnterpolAVLWhoAmIProc.Text = "WhoAmI Procedure:";
            // 
            // numAliEnterpolAVLReadFreq
            // 
            this.numAliEnterpolAVLReadFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numAliEnterpolAVLReadFreq.Location = new System.Drawing.Point(142, 78);
            this.numAliEnterpolAVLReadFreq.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numAliEnterpolAVLReadFreq.Name = "numAliEnterpolAVLReadFreq";
            this.numAliEnterpolAVLReadFreq.Size = new System.Drawing.Size(93, 20);
            this.numAliEnterpolAVLReadFreq.TabIndex = 24;
            // 
            // numAliEnterpolAVLUpdateFreq
            // 
            this.numAliEnterpolAVLUpdateFreq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numAliEnterpolAVLUpdateFreq.Location = new System.Drawing.Point(142, 130);
            this.numAliEnterpolAVLUpdateFreq.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numAliEnterpolAVLUpdateFreq.Name = "numAliEnterpolAVLUpdateFreq";
            this.numAliEnterpolAVLUpdateFreq.Size = new System.Drawing.Size(93, 20);
            this.numAliEnterpolAVLUpdateFreq.TabIndex = 18;
            // 
            // lblAliEnterpolAVLTableName
            // 
            this.lblAliEnterpolAVLTableName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAliEnterpolAVLTableName.AutoSize = true;
            this.lblAliEnterpolAVLTableName.Location = new System.Drawing.Point(3, 29);
            this.lblAliEnterpolAVLTableName.Name = "lblAliEnterpolAVLTableName";
            this.lblAliEnterpolAVLTableName.Size = new System.Drawing.Size(87, 13);
            this.lblAliEnterpolAVLTableName.TabIndex = 13;
            this.lblAliEnterpolAVLTableName.Text = "Database Name:";
            // 
            // txtAliEnterpolAVLTableName
            // 
            this.pnlAliEnterpolAvl.SetColumnSpan(this.txtAliEnterpolAVLTableName, 3);
            this.txtAliEnterpolAVLTableName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAliEnterpolAVLTableName.Location = new System.Drawing.Point(96, 26);
            this.txtAliEnterpolAVLTableName.Name = "txtAliEnterpolAVLTableName";
            this.txtAliEnterpolAVLTableName.Size = new System.Drawing.Size(244, 20);
            this.txtAliEnterpolAVLTableName.TabIndex = 9;
            // 
            // lblAliEnterpolAVLUpdateFreq
            // 
            this.lblAliEnterpolAVLUpdateFreq.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAliEnterpolAVLUpdateFreq.AutoSize = true;
            this.pnlAliEnterpolAvl.SetColumnSpan(this.lblAliEnterpolAVLUpdateFreq, 2);
            this.lblAliEnterpolAVLUpdateFreq.Location = new System.Drawing.Point(16, 133);
            this.lblAliEnterpolAVLUpdateFreq.Name = "lblAliEnterpolAVLUpdateFreq";
            this.lblAliEnterpolAVLUpdateFreq.Size = new System.Drawing.Size(120, 13);
            this.lblAliEnterpolAVLUpdateFreq.TabIndex = 19;
            this.lblAliEnterpolAVLUpdateFreq.Text = "Update Frequency (ms):";
            // 
            // txtAliEnterpolAVLInitialCatalog
            // 
            this.pnlAliEnterpolAvl.SetColumnSpan(this.txtAliEnterpolAVLInitialCatalog, 2);
            this.txtAliEnterpolAVLInitialCatalog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAliEnterpolAVLInitialCatalog.Location = new System.Drawing.Point(142, 52);
            this.txtAliEnterpolAVLInitialCatalog.Name = "txtAliEnterpolAVLInitialCatalog";
            this.txtAliEnterpolAVLInitialCatalog.Size = new System.Drawing.Size(198, 20);
            this.txtAliEnterpolAVLInitialCatalog.TabIndex = 10;
            // 
            // lblAliEnterpolAVLReadFreq
            // 
            this.lblAliEnterpolAVLReadFreq.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAliEnterpolAVLReadFreq.AutoSize = true;
            this.pnlAliEnterpolAvl.SetColumnSpan(this.lblAliEnterpolAVLReadFreq, 2);
            this.lblAliEnterpolAVLReadFreq.Location = new System.Drawing.Point(25, 81);
            this.lblAliEnterpolAVLReadFreq.Name = "lblAliEnterpolAVLReadFreq";
            this.lblAliEnterpolAVLReadFreq.Size = new System.Drawing.Size(111, 13);
            this.lblAliEnterpolAVLReadFreq.TabIndex = 25;
            this.lblAliEnterpolAVLReadFreq.Text = "Read Frequency (ms):";
            // 
            // txtAliEnterpolAVLWhoAmIProc
            // 
            this.pnlAliEnterpolAvl.SetColumnSpan(this.txtAliEnterpolAVLWhoAmIProc, 2);
            this.txtAliEnterpolAVLWhoAmIProc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAliEnterpolAVLWhoAmIProc.Location = new System.Drawing.Point(142, 156);
            this.txtAliEnterpolAVLWhoAmIProc.Name = "txtAliEnterpolAVLWhoAmIProc";
            this.txtAliEnterpolAVLWhoAmIProc.Size = new System.Drawing.Size(198, 20);
            this.txtAliEnterpolAVLWhoAmIProc.TabIndex = 14;
            // 
            // lblAliEnterpolAVLSetLocProc
            // 
            this.lblAliEnterpolAVLSetLocProc.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAliEnterpolAVLSetLocProc.AutoSize = true;
            this.pnlAliEnterpolAvl.SetColumnSpan(this.lblAliEnterpolAVLSetLocProc, 2);
            this.lblAliEnterpolAVLSetLocProc.Location = new System.Drawing.Point(3, 107);
            this.lblAliEnterpolAVLSetLocProc.Name = "lblAliEnterpolAVLSetLocProc";
            this.lblAliEnterpolAVLSetLocProc.Size = new System.Drawing.Size(133, 13);
            this.lblAliEnterpolAVLSetLocProc.TabIndex = 17;
            this.lblAliEnterpolAVLSetLocProc.Text = "SetMyLocation Procedure:";
            // 
            // txtAliEnterpolAVLSetLocProc
            // 
            this.pnlAliEnterpolAvl.SetColumnSpan(this.txtAliEnterpolAVLSetLocProc, 2);
            this.txtAliEnterpolAVLSetLocProc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAliEnterpolAVLSetLocProc.Location = new System.Drawing.Point(142, 104);
            this.txtAliEnterpolAVLSetLocProc.Name = "txtAliEnterpolAVLSetLocProc";
            this.txtAliEnterpolAVLSetLocProc.Size = new System.Drawing.Size(198, 20);
            this.txtAliEnterpolAVLSetLocProc.TabIndex = 11;
            // 
            // chkEnterpolAvl
            // 
            this.chkEnterpolAvl.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.chkEnterpolAvl.AutoSize = true;
            this.chkEnterpolAvl.Checked = true;
            this.chkEnterpolAvl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.pnlAliEnterpol.SetColumnSpan(this.chkEnterpolAvl, 3);
            this.chkEnterpolAvl.Location = new System.Drawing.Point(62, 104);
            this.chkEnterpolAvl.Name = "chkEnterpolAvl";
            this.chkEnterpolAvl.Size = new System.Drawing.Size(219, 17);
            this.chkEnterpolAvl.TabIndex = 8;
            this.chkEnterpolAvl.Text = "Use Enterpol Automatic Vehicle Location";
            this.chkEnterpolAvl.UseVisualStyleBackColor = true;
            this.chkEnterpolAvl.CheckedChanged += new System.EventHandler(this.chkAvl_CheckedChanged);
            // 
            // txtAliEnterpolInitialCatalog
            // 
            this.txtAliEnterpolInitialCatalog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAliEnterpolInitialCatalog.Location = new System.Drawing.Point(151, 78);
            this.txtAliEnterpolInitialCatalog.Name = "txtAliEnterpolInitialCatalog";
            this.txtAliEnterpolInitialCatalog.Size = new System.Drawing.Size(189, 20);
            this.txtAliEnterpolInitialCatalog.TabIndex = 3;
            // 
            // groupBox6
            // 
            this.pnlAliEnterpol.SetColumnSpan(this.groupBox6, 3);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox6.Location = new System.Drawing.Point(0, 3);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.groupBox6.MinimumSize = new System.Drawing.Size(0, 15);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.groupBox6.Size = new System.Drawing.Size(343, 17);
            this.groupBox6.TabIndex = 101;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Enterpol Database Settings";
            // 
            // lblAliEnterpolDataSource
            // 
            this.lblAliEnterpolDataSource.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAliEnterpolDataSource.AutoSize = true;
            this.lblAliEnterpolDataSource.Location = new System.Drawing.Point(3, 29);
            this.lblAliEnterpolDataSource.Name = "lblAliEnterpolDataSource";
            this.lblAliEnterpolDataSource.Size = new System.Drawing.Size(100, 13);
            this.lblAliEnterpolDataSource.TabIndex = 5;
            this.lblAliEnterpolDataSource.Text = "Database Location:";
            // 
            // lblAliEnterpolTableName
            // 
            this.lblAliEnterpolTableName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAliEnterpolTableName.AutoSize = true;
            this.lblAliEnterpolTableName.Location = new System.Drawing.Point(16, 55);
            this.lblAliEnterpolTableName.Name = "lblAliEnterpolTableName";
            this.lblAliEnterpolTableName.Size = new System.Drawing.Size(87, 13);
            this.lblAliEnterpolTableName.TabIndex = 6;
            this.lblAliEnterpolTableName.Text = "Database Name:";
            // 
            // lblAliEnterpolInitialCatalog
            // 
            this.lblAliEnterpolInitialCatalog.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAliEnterpolInitialCatalog.AutoSize = true;
            this.pnlAliEnterpol.SetColumnSpan(this.lblAliEnterpolInitialCatalog, 2);
            this.lblAliEnterpolInitialCatalog.Location = new System.Drawing.Point(3, 81);
            this.lblAliEnterpolInitialCatalog.Name = "lblAliEnterpolInitialCatalog";
            this.lblAliEnterpolInitialCatalog.Size = new System.Drawing.Size(142, 13);
            this.lblAliEnterpolInitialCatalog.TabIndex = 7;
            this.lblAliEnterpolInitialCatalog.Text = "Incidents Table/View Name:";
            // 
            // txtAliEnterpolDataSource
            // 
            this.pnlAliEnterpol.SetColumnSpan(this.txtAliEnterpolDataSource, 2);
            this.txtAliEnterpolDataSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAliEnterpolDataSource.Location = new System.Drawing.Point(109, 26);
            this.txtAliEnterpolDataSource.Name = "txtAliEnterpolDataSource";
            this.txtAliEnterpolDataSource.Size = new System.Drawing.Size(231, 20);
            this.txtAliEnterpolDataSource.TabIndex = 2;
            // 
            // txtAliEnterpolTableName
            // 
            this.pnlAliEnterpol.SetColumnSpan(this.txtAliEnterpolTableName, 2);
            this.txtAliEnterpolTableName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAliEnterpolTableName.Location = new System.Drawing.Point(109, 52);
            this.txtAliEnterpolTableName.Name = "txtAliEnterpolTableName";
            this.txtAliEnterpolTableName.Size = new System.Drawing.Size(231, 20);
            this.txtAliEnterpolTableName.TabIndex = 1;
            // 
            // pnlAliSdrServer
            // 
            this.pnlAliSdrServer.ColumnCount = 4;
            this.pnlAliSdrServer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlAliSdrServer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 71.42857F));
            this.pnlAliSdrServer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlAliSdrServer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.57143F));
            this.pnlAliSdrServer.Controls.Add(this.tableLayoutPanel6, 0, 1);
            this.pnlAliSdrServer.Controls.Add(this.groupBox5, 0, 0);
            this.pnlAliSdrServer.Controls.Add(this.lblAliInterfaceUdpHost, 0, 2);
            this.pnlAliSdrServer.Controls.Add(this.numAliInterfaceUdpPort, 3, 2);
            this.pnlAliSdrServer.Controls.Add(this.lblAliInterfaceUdpPort, 2, 2);
            this.pnlAliSdrServer.Controls.Add(this.txtAliInterfaceUdpHost, 1, 2);
            this.pnlAliSdrServer.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlAliSdrServer.Location = new System.Drawing.Point(241, 61);
            this.pnlAliSdrServer.Margin = new System.Windows.Forms.Padding(8, 0, 0, 3);
            this.pnlAliSdrServer.MinimumSize = new System.Drawing.Size(0, 79);
            this.pnlAliSdrServer.Name = "pnlAliSdrServer";
            this.pnlAliSdrServer.RowCount = 4;
            this.pnlAliSdrServer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliSdrServer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliSdrServer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliSdrServer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.pnlAliSdrServer.Size = new System.Drawing.Size(343, 84);
            this.pnlAliSdrServer.TabIndex = 102;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 3;
            this.pnlAliSdrServer.SetColumnSpan(this.tableLayoutPanel6, 4);
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.Controls.Add(this.btnAliInterfaceDbPathBrowse, 2, 0);
            this.tableLayoutPanel6.Controls.Add(this.lblAliInterfaceDbPath, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.txtAliInterfaceDbPath, 1, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 23);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel6.MinimumSize = new System.Drawing.Size(0, 30);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(343, 30);
            this.tableLayoutPanel6.TabIndex = 103;
            // 
            // btnAliInterfaceDbPathBrowse
            // 
            this.btnAliInterfaceDbPathBrowse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAliInterfaceDbPathBrowse.Location = new System.Drawing.Point(276, 3);
            this.btnAliInterfaceDbPathBrowse.Name = "btnAliInterfaceDbPathBrowse";
            this.btnAliInterfaceDbPathBrowse.Size = new System.Drawing.Size(64, 24);
            this.btnAliInterfaceDbPathBrowse.TabIndex = 1;
            this.btnAliInterfaceDbPathBrowse.Text = "Browse";
            this.btnAliInterfaceDbPathBrowse.UseVisualStyleBackColor = true;
            this.btnAliInterfaceDbPathBrowse.Click += new System.EventHandler(this.btnAliInterfaceDbPathBrowse_Click);
            // 
            // lblAliInterfaceDbPath
            // 
            this.lblAliInterfaceDbPath.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAliInterfaceDbPath.AutoSize = true;
            this.lblAliInterfaceDbPath.Location = new System.Drawing.Point(3, 8);
            this.lblAliInterfaceDbPath.Name = "lblAliInterfaceDbPath";
            this.lblAliInterfaceDbPath.Size = new System.Drawing.Size(100, 13);
            this.lblAliInterfaceDbPath.TabIndex = 4;
            this.lblAliInterfaceDbPath.Text = "Database Location:";
            // 
            // txtAliInterfaceDbPath
            // 
            this.txtAliInterfaceDbPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAliInterfaceDbPath.Location = new System.Drawing.Point(109, 3);
            this.txtAliInterfaceDbPath.Name = "txtAliInterfaceDbPath";
            this.txtAliInterfaceDbPath.Size = new System.Drawing.Size(161, 20);
            this.txtAliInterfaceDbPath.TabIndex = 0;
            // 
            // groupBox5
            // 
            this.pnlAliSdrServer.SetColumnSpan(this.groupBox5, 4);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox5.Location = new System.Drawing.Point(0, 3);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.groupBox5.MinimumSize = new System.Drawing.Size(0, 15);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.groupBox5.Size = new System.Drawing.Size(343, 17);
            this.groupBox5.TabIndex = 101;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "SDR AliServer Settings";
            // 
            // lblAliInterfaceUdpHost
            // 
            this.lblAliInterfaceUdpHost.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblAliInterfaceUdpHost.AutoSize = true;
            this.lblAliInterfaceUdpHost.Location = new System.Drawing.Point(3, 59);
            this.lblAliInterfaceUdpHost.Name = "lblAliInterfaceUdpHost";
            this.lblAliInterfaceUdpHost.Size = new System.Drawing.Size(58, 13);
            this.lblAliInterfaceUdpHost.TabIndex = 5;
            this.lblAliInterfaceUdpHost.Text = "UDP Host:";
            // 
            // numAliInterfaceUdpPort
            // 
            this.numAliInterfaceUdpPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numAliInterfaceUdpPort.Location = new System.Drawing.Point(276, 56);
            this.numAliInterfaceUdpPort.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numAliInterfaceUdpPort.Name = "numAliInterfaceUdpPort";
            this.numAliInterfaceUdpPort.Size = new System.Drawing.Size(64, 20);
            this.numAliInterfaceUdpPort.TabIndex = 7;
            // 
            // lblAliInterfaceUdpPort
            // 
            this.lblAliInterfaceUdpPort.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAliInterfaceUdpPort.AutoSize = true;
            this.lblAliInterfaceUdpPort.Location = new System.Drawing.Point(241, 59);
            this.lblAliInterfaceUdpPort.Name = "lblAliInterfaceUdpPort";
            this.lblAliInterfaceUdpPort.Size = new System.Drawing.Size(29, 13);
            this.lblAliInterfaceUdpPort.TabIndex = 6;
            this.lblAliInterfaceUdpPort.Text = "Port:";
            // 
            // txtAliInterfaceUdpHost
            // 
            this.txtAliInterfaceUdpHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAliInterfaceUdpHost.Location = new System.Drawing.Point(67, 56);
            this.txtAliInterfaceUdpHost.Name = "txtAliInterfaceUdpHost";
            this.txtAliInterfaceUdpHost.Size = new System.Drawing.Size(168, 20);
            this.txtAliInterfaceUdpHost.TabIndex = 2;
            // 
            // pnlAliNetworkfleet
            // 
            this.pnlAliNetworkfleet.ColumnCount = 1;
            this.pnlAliNetworkfleet.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlAliNetworkfleet.Controls.Add(this.tblAliNetworkfleetSettings, 0, 0);
            this.pnlAliNetworkfleet.Controls.Add(this.tblAliNetworkfleetLabeling, 0, 2);
            this.pnlAliNetworkfleet.Controls.Add(this.tblAliNetworkfleetSymbology, 0, 1);
            this.pnlAliNetworkfleet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAliNetworkfleet.Location = new System.Drawing.Point(0, 61);
            this.pnlAliNetworkfleet.Margin = new System.Windows.Forms.Padding(0);
            this.pnlAliNetworkfleet.Name = "pnlAliNetworkfleet";
            this.pnlAliNetworkfleet.RowCount = 3;
            this.aliPanelTableLayout.SetRowSpan(this.pnlAliNetworkfleet, 3);
            this.pnlAliNetworkfleet.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliNetworkfleet.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliNetworkfleet.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlAliNetworkfleet.Size = new System.Drawing.Size(233, 649);
            this.pnlAliNetworkfleet.TabIndex = 107;
            // 
            // tblAliNetworkfleetSettings
            // 
            this.tblAliNetworkfleetSettings.ColumnCount = 3;
            this.tblAliNetworkfleetSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.tblAliNetworkfleetSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tblAliNetworkfleetSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tblAliNetworkfleetSettings.Controls.Add(this.groupBox3, 0, 0);
            this.tblAliNetworkfleetSettings.Controls.Add(this.numAliNetworkfleetAVLAge1Freq, 0, 4);
            this.tblAliNetworkfleetSettings.Controls.Add(this.chkAutoHideInactiveUnitsNetworkfleet, 0, 5);
            this.tblAliNetworkfleetSettings.Controls.Add(this.numAliNetworkfleetAVLAge2Freq, 1, 4);
            this.tblAliNetworkfleetSettings.Controls.Add(this.label16, 0, 3);
            this.tblAliNetworkfleetSettings.Controls.Add(this.numAliNetworkfleetAVLAge3Freq, 2, 4);
            this.tblAliNetworkfleetSettings.Controls.Add(this.label6, 0, 2);
            this.tblAliNetworkfleetSettings.Controls.Add(this.numAliNetworkfleetUdpPort, 1, 2);
            this.tblAliNetworkfleetSettings.Controls.Add(this.lblAliNetworkfleetUdpHost, 0, 1);
            this.tblAliNetworkfleetSettings.Controls.Add(this.txtAliNetworkfleetUdpHost, 1, 1);
            this.tblAliNetworkfleetSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblAliNetworkfleetSettings.Location = new System.Drawing.Point(0, 0);
            this.tblAliNetworkfleetSettings.Margin = new System.Windows.Forms.Padding(0);
            this.tblAliNetworkfleetSettings.MinimumSize = new System.Drawing.Size(197, 144);
            this.tblAliNetworkfleetSettings.Name = "tblAliNetworkfleetSettings";
            this.tblAliNetworkfleetSettings.RowCount = 6;
            this.tblAliNetworkfleetSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAliNetworkfleetSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAliNetworkfleetSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAliNetworkfleetSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAliNetworkfleetSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAliNetworkfleetSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAliNetworkfleetSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblAliNetworkfleetSettings.Size = new System.Drawing.Size(233, 144);
            this.tblAliNetworkfleetSettings.TabIndex = 96;
            // 
            // groupBox3
            // 
            this.tblAliNetworkfleetSettings.SetColumnSpan(this.groupBox3, 3);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(0, 3);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.groupBox3.MinimumSize = new System.Drawing.Size(0, 15);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.groupBox3.Size = new System.Drawing.Size(233, 17);
            this.groupBox3.TabIndex = 100;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Networkfleet Settings";
            // 
            // numAliNetworkfleetAVLAge1Freq
            // 
            this.numAliNetworkfleetAVLAge1Freq.AutoSize = true;
            this.numAliNetworkfleetAVLAge1Freq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numAliNetworkfleetAVLAge1Freq.Location = new System.Drawing.Point(3, 99);
            this.numAliNetworkfleetAVLAge1Freq.Name = "numAliNetworkfleetAVLAge1Freq";
            this.numAliNetworkfleetAVLAge1Freq.Size = new System.Drawing.Size(71, 20);
            this.numAliNetworkfleetAVLAge1Freq.TabIndex = 81;
            // 
            // chkAutoHideInactiveUnitsNetworkfleet
            // 
            this.chkAutoHideInactiveUnitsNetworkfleet.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.chkAutoHideInactiveUnitsNetworkfleet.AutoSize = true;
            this.tblAliNetworkfleetSettings.SetColumnSpan(this.chkAutoHideInactiveUnitsNetworkfleet, 3);
            this.chkAutoHideInactiveUnitsNetworkfleet.Location = new System.Drawing.Point(48, 125);
            this.chkAutoHideInactiveUnitsNetworkfleet.Name = "chkAutoHideInactiveUnitsNetworkfleet";
            this.chkAutoHideInactiveUnitsNetworkfleet.Size = new System.Drawing.Size(136, 17);
            this.chkAutoHideInactiveUnitsNetworkfleet.TabIndex = 93;
            this.chkAutoHideInactiveUnitsNetworkfleet.Text = "Autohide Inactive Units";
            this.chkAutoHideInactiveUnitsNetworkfleet.UseVisualStyleBackColor = true;
            // 
            // numAliNetworkfleetAVLAge2Freq
            // 
            this.numAliNetworkfleetAVLAge2Freq.AutoSize = true;
            this.numAliNetworkfleetAVLAge2Freq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numAliNetworkfleetAVLAge2Freq.Location = new System.Drawing.Point(80, 99);
            this.numAliNetworkfleetAVLAge2Freq.Name = "numAliNetworkfleetAVLAge2Freq";
            this.numAliNetworkfleetAVLAge2Freq.Size = new System.Drawing.Size(71, 20);
            this.numAliNetworkfleetAVLAge2Freq.TabIndex = 82;
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label16.AutoSize = true;
            this.tblAliNetworkfleetSettings.SetColumnSpan(this.label16, 3);
            this.label16.Location = new System.Drawing.Point(58, 75);
            this.label16.Margin = new System.Windows.Forms.Padding(0);
            this.label16.Name = "label16";
            this.label16.Padding = new System.Windows.Forms.Padding(0, 5, 0, 3);
            this.label16.Size = new System.Drawing.Size(116, 21);
            this.label16.TabIndex = 90;
            this.label16.Text = "AVL Age Frequency (s)";
            // 
            // numAliNetworkfleetAVLAge3Freq
            // 
            this.numAliNetworkfleetAVLAge3Freq.AutoSize = true;
            this.numAliNetworkfleetAVLAge3Freq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numAliNetworkfleetAVLAge3Freq.Location = new System.Drawing.Point(157, 99);
            this.numAliNetworkfleetAVLAge3Freq.Name = "numAliNetworkfleetAVLAge3Freq";
            this.numAliNetworkfleetAVLAge3Freq.Size = new System.Drawing.Size(73, 20);
            this.numAliNetworkfleetAVLAge3Freq.TabIndex = 83;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(45, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Port:";
            // 
            // numAliNetworkfleetUdpPort
            // 
            this.tblAliNetworkfleetSettings.SetColumnSpan(this.numAliNetworkfleetUdpPort, 2);
            this.numAliNetworkfleetUdpPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numAliNetworkfleetUdpPort.Location = new System.Drawing.Point(80, 52);
            this.numAliNetworkfleetUdpPort.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numAliNetworkfleetUdpPort.Name = "numAliNetworkfleetUdpPort";
            this.numAliNetworkfleetUdpPort.Size = new System.Drawing.Size(150, 20);
            this.numAliNetworkfleetUdpPort.TabIndex = 1;
            // 
            // lblAliNetworkfleetUdpHost
            // 
            this.lblAliNetworkfleetUdpHost.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAliNetworkfleetUdpHost.AutoSize = true;
            this.lblAliNetworkfleetUdpHost.Location = new System.Drawing.Point(16, 29);
            this.lblAliNetworkfleetUdpHost.Name = "lblAliNetworkfleetUdpHost";
            this.lblAliNetworkfleetUdpHost.Size = new System.Drawing.Size(58, 13);
            this.lblAliNetworkfleetUdpHost.TabIndex = 2;
            this.lblAliNetworkfleetUdpHost.Text = "UDP Host:";
            // 
            // txtAliNetworkfleetUdpHost
            // 
            this.tblAliNetworkfleetSettings.SetColumnSpan(this.txtAliNetworkfleetUdpHost, 2);
            this.txtAliNetworkfleetUdpHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAliNetworkfleetUdpHost.Location = new System.Drawing.Point(80, 26);
            this.txtAliNetworkfleetUdpHost.Name = "txtAliNetworkfleetUdpHost";
            this.txtAliNetworkfleetUdpHost.Size = new System.Drawing.Size(150, 20);
            this.txtAliNetworkfleetUdpHost.TabIndex = 0;
            // 
            // tblAliNetworkfleetLabeling
            // 
            this.tblAliNetworkfleetLabeling.ColumnCount = 3;
            this.tblAliNetworkfleetLabeling.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblAliNetworkfleetLabeling.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 74.07407F));
            this.tblAliNetworkfleetLabeling.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.92593F));
            this.tblAliNetworkfleetLabeling.Controls.Add(this.groupBox1, 0, 0);
            this.tblAliNetworkfleetLabeling.Controls.Add(this.numAliNetworkfleetLabelYOffset, 1, 6);
            this.tblAliNetworkfleetLabeling.Controls.Add(this.numAliNetworkfleetLabelXOffset, 1, 5);
            this.tblAliNetworkfleetLabeling.Controls.Add(this.label15, 0, 6);
            this.tblAliNetworkfleetLabeling.Controls.Add(this.txtAliNetworkfleetLabelFontSize, 1, 2);
            this.tblAliNetworkfleetLabeling.Controls.Add(this.btnAliNetworkfleetLabelFont, 0, 1);
            this.tblAliNetworkfleetLabeling.Controls.Add(this.lblAliNetworkfleetLabelFont, 1, 1);
            this.tblAliNetworkfleetLabeling.Controls.Add(this.label13, 0, 4);
            this.tblAliNetworkfleetLabeling.Controls.Add(this.dgvNetworkfleetLabelLookup, 0, 7);
            this.tblAliNetworkfleetLabeling.Controls.Add(this.lblAliNetworkfleetLabelFontSize, 0, 2);
            this.tblAliNetworkfleetLabeling.Controls.Add(this.cmbAliNetworkfleetLabelAlignment, 1, 4);
            this.tblAliNetworkfleetLabeling.Controls.Add(this.label14, 0, 5);
            this.tblAliNetworkfleetLabeling.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblAliNetworkfleetLabeling.Location = new System.Drawing.Point(0, 298);
            this.tblAliNetworkfleetLabeling.Margin = new System.Windows.Forms.Padding(0);
            this.tblAliNetworkfleetLabeling.Name = "tblAliNetworkfleetLabeling";
            this.tblAliNetworkfleetLabeling.RowCount = 8;
            this.tblAliNetworkfleetLabeling.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAliNetworkfleetLabeling.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAliNetworkfleetLabeling.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAliNetworkfleetLabeling.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAliNetworkfleetLabeling.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAliNetworkfleetLabeling.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAliNetworkfleetLabeling.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAliNetworkfleetLabeling.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblAliNetworkfleetLabeling.Size = new System.Drawing.Size(233, 351);
            this.tblAliNetworkfleetLabeling.TabIndex = 106;
            // 
            // groupBox1
            // 
            this.tblAliNetworkfleetLabeling.SetColumnSpan(this.groupBox1, 3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 3);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.groupBox1.MinimumSize = new System.Drawing.Size(0, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.groupBox1.Size = new System.Drawing.Size(233, 17);
            this.groupBox1.TabIndex = 104;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Networkfleet Labeling";
            // 
            // numAliNetworkfleetLabelYOffset
            // 
            this.numAliNetworkfleetLabelYOffset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numAliNetworkfleetLabelYOffset.Location = new System.Drawing.Point(79, 134);
            this.numAliNetworkfleetLabelYOffset.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numAliNetworkfleetLabelYOffset.Name = "numAliNetworkfleetLabelYOffset";
            this.numAliNetworkfleetLabelYOffset.Size = new System.Drawing.Size(110, 20);
            this.numAliNetworkfleetLabelYOffset.TabIndex = 74;
            // 
            // numAliNetworkfleetLabelXOffset
            // 
            this.numAliNetworkfleetLabelXOffset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numAliNetworkfleetLabelXOffset.Location = new System.Drawing.Point(79, 108);
            this.numAliNetworkfleetLabelXOffset.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numAliNetworkfleetLabelXOffset.Name = "numAliNetworkfleetLabelXOffset";
            this.numAliNetworkfleetLabelXOffset.Size = new System.Drawing.Size(110, 20);
            this.numAliNetworkfleetLabelXOffset.TabIndex = 73;
            // 
            // label15
            // 
            this.label15.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(25, 140);
            this.label15.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(48, 13);
            this.label15.TabIndex = 76;
            this.label15.Text = "Y Offset:";
            // 
            // txtAliNetworkfleetLabelFontSize
            // 
            this.txtAliNetworkfleetLabelFontSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAliNetworkfleetLabelFontSize.Location = new System.Drawing.Point(79, 55);
            this.txtAliNetworkfleetLabelFontSize.Name = "txtAliNetworkfleetLabelFontSize";
            this.txtAliNetworkfleetLabelFontSize.ReadOnly = true;
            this.txtAliNetworkfleetLabelFontSize.Size = new System.Drawing.Size(110, 20);
            this.txtAliNetworkfleetLabelFontSize.TabIndex = 105;
            // 
            // btnAliNetworkfleetLabelFont
            // 
            this.btnAliNetworkfleetLabelFont.Location = new System.Drawing.Point(3, 26);
            this.btnAliNetworkfleetLabelFont.Name = "btnAliNetworkfleetLabelFont";
            this.btnAliNetworkfleetLabelFont.Size = new System.Drawing.Size(70, 23);
            this.btnAliNetworkfleetLabelFont.TabIndex = 77;
            this.btnAliNetworkfleetLabelFont.Text = "Select Font";
            this.btnAliNetworkfleetLabelFont.UseVisualStyleBackColor = true;
            this.btnAliNetworkfleetLabelFont.Click += new System.EventHandler(this.btnAliNetworkfleetLabelFont_Click);
            // 
            // lblAliNetworkfleetLabelFont
            // 
            this.lblAliNetworkfleetLabelFont.BackColor = System.Drawing.Color.Transparent;
            this.tblAliNetworkfleetLabeling.SetColumnSpan(this.lblAliNetworkfleetLabelFont, 2);
            this.lblAliNetworkfleetLabelFont.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAliNetworkfleetLabelFont.Location = new System.Drawing.Point(79, 23);
            this.lblAliNetworkfleetLabelFont.Margin = new System.Windows.Forms.Padding(3, 0, 3, 5);
            this.lblAliNetworkfleetLabelFont.Name = "lblAliNetworkfleetLabelFont";
            this.lblAliNetworkfleetLabelFont.Size = new System.Drawing.Size(151, 24);
            this.lblAliNetworkfleetLabelFont.TabIndex = 78;
            this.lblAliNetworkfleetLabelFont.Text = "This is the font name";
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(17, 85);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(56, 13);
            this.label13.TabIndex = 71;
            this.label13.Text = "Alignment:";
            // 
            // dgvNetworkfleetLabelLookup
            // 
            this.dgvNetworkfleetLabelLookup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNetworkfleetLabelLookup.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.vehicle_id,
            this.vehicle_label});
            this.tblAliNetworkfleetLabeling.SetColumnSpan(this.dgvNetworkfleetLabelLookup, 3);
            this.dgvNetworkfleetLabelLookup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvNetworkfleetLabelLookup.Location = new System.Drawing.Point(0, 162);
            this.dgvNetworkfleetLabelLookup.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.dgvNetworkfleetLabelLookup.Name = "dgvNetworkfleetLabelLookup";
            this.dgvNetworkfleetLabelLookup.RowHeadersVisible = false;
            this.dgvNetworkfleetLabelLookup.Size = new System.Drawing.Size(233, 189);
            this.dgvNetworkfleetLabelLookup.TabIndex = 80;
            // 
            // vehicle_id
            // 
            this.vehicle_id.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.vehicle_id.HeaderText = "Vehicle ID";
            this.vehicle_id.Name = "vehicle_id";
            this.vehicle_id.Width = 81;
            // 
            // vehicle_label
            // 
            this.vehicle_label.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.vehicle_label.HeaderText = "Label";
            this.vehicle_label.Name = "vehicle_label";
            // 
            // lblAliNetworkfleetLabelFontSize
            // 
            this.lblAliNetworkfleetLabelFontSize.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAliNetworkfleetLabelFontSize.AutoSize = true;
            this.lblAliNetworkfleetLabelFontSize.Location = new System.Drawing.Point(19, 58);
            this.lblAliNetworkfleetLabelFontSize.Name = "lblAliNetworkfleetLabelFontSize";
            this.lblAliNetworkfleetLabelFontSize.Size = new System.Drawing.Size(54, 13);
            this.lblAliNetworkfleetLabelFontSize.TabIndex = 79;
            this.lblAliNetworkfleetLabelFontSize.Text = "Font Size:";
            // 
            // cmbAliNetworkfleetLabelAlignment
            // 
            this.tblAliNetworkfleetLabeling.SetColumnSpan(this.cmbAliNetworkfleetLabelAlignment, 2);
            this.cmbAliNetworkfleetLabelAlignment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbAliNetworkfleetLabelAlignment.FormattingEnabled = true;
            this.cmbAliNetworkfleetLabelAlignment.Items.AddRange(new object[] {
            "Above",
            "Below",
            "Right",
            "Left"});
            this.cmbAliNetworkfleetLabelAlignment.Location = new System.Drawing.Point(79, 81);
            this.cmbAliNetworkfleetLabelAlignment.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.cmbAliNetworkfleetLabelAlignment.Name = "cmbAliNetworkfleetLabelAlignment";
            this.cmbAliNetworkfleetLabelAlignment.Size = new System.Drawing.Size(144, 21);
            this.cmbAliNetworkfleetLabelAlignment.TabIndex = 72;
            this.cmbAliNetworkfleetLabelAlignment.SelectedIndexChanged += new System.EventHandler(this.cmbAliNetworkfleetLabelAlignment_SelectedIndexChanged);
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(25, 114);
            this.label14.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(48, 13);
            this.label14.TabIndex = 75;
            this.label14.Text = "X Offset:";
            // 
            // tblAliNetworkfleetSymbology
            // 
            this.tblAliNetworkfleetSymbology.ColumnCount = 3;
            this.tblAliNetworkfleetSymbology.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblAliNetworkfleetSymbology.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblAliNetworkfleetSymbology.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 87F));
            this.tblAliNetworkfleetSymbology.Controls.Add(this.groupBox2, 0, 0);
            this.tblAliNetworkfleetSymbology.Controls.Add(this.tblNetworkfleetAvlColors, 0, 4);
            this.tblAliNetworkfleetSymbology.Controls.Add(this.ptAliNetworkfleetGraphic, 0, 2);
            this.tblAliNetworkfleetSymbology.Controls.Add(this.label22, 1, 2);
            this.tblAliNetworkfleetSymbology.Controls.Add(this.btnAliNetworkfleetFont, 0, 1);
            this.tblAliNetworkfleetSymbology.Controls.Add(this.lblAliNetworkfleetFont, 1, 1);
            this.tblAliNetworkfleetSymbology.Controls.Add(this.txtAliNetworkfleetSize, 2, 2);
            this.tblAliNetworkfleetSymbology.Controls.Add(this.label20, 1, 3);
            this.tblAliNetworkfleetSymbology.Controls.Add(this.txtAliNetworkfleetChar, 2, 3);
            this.tblAliNetworkfleetSymbology.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblAliNetworkfleetSymbology.Location = new System.Drawing.Point(0, 144);
            this.tblAliNetworkfleetSymbology.Margin = new System.Windows.Forms.Padding(0);
            this.tblAliNetworkfleetSymbology.Name = "tblAliNetworkfleetSymbology";
            this.tblAliNetworkfleetSymbology.RowCount = 5;
            this.tblAliNetworkfleetSymbology.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAliNetworkfleetSymbology.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAliNetworkfleetSymbology.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAliNetworkfleetSymbology.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAliNetworkfleetSymbology.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblAliNetworkfleetSymbology.Size = new System.Drawing.Size(233, 154);
            this.tblAliNetworkfleetSymbology.TabIndex = 103;
            // 
            // groupBox2
            // 
            this.tblAliNetworkfleetSymbology.SetColumnSpan(this.groupBox2, 3);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 3);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.groupBox2.MinimumSize = new System.Drawing.Size(0, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.groupBox2.Size = new System.Drawing.Size(233, 15);
            this.groupBox2.TabIndex = 99;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Networkfleet Symbology";
            // 
            // tblNetworkfleetAvlColors
            // 
            this.tblNetworkfleetAvlColors.ColumnCount = 3;
            this.tblAliNetworkfleetSymbology.SetColumnSpan(this.tblNetworkfleetAvlColors, 3);
            this.tblNetworkfleetAvlColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tblNetworkfleetAvlColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tblNetworkfleetAvlColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tblNetworkfleetAvlColors.Controls.Add(this.label21, 0, 0);
            this.tblNetworkfleetAvlColors.Controls.Add(this.label18, 1, 0);
            this.tblNetworkfleetAvlColors.Controls.Add(this.label19, 2, 0);
            this.tblNetworkfleetAvlColors.Controls.Add(this.ptAliNetworkfleetColor, 0, 1);
            this.tblNetworkfleetAvlColors.Controls.Add(this.pnlAliNetworkfleetAVLInactiveColor, 1, 1);
            this.tblNetworkfleetAvlColors.Controls.Add(this.pnlAliNetworkfleetAVLMyVehicleColor, 2, 1);
            this.tblNetworkfleetAvlColors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblNetworkfleetAvlColors.Location = new System.Drawing.Point(0, 106);
            this.tblNetworkfleetAvlColors.Margin = new System.Windows.Forms.Padding(0);
            this.tblNetworkfleetAvlColors.Name = "tblNetworkfleetAvlColors";
            this.tblNetworkfleetAvlColors.RowCount = 2;
            this.tblNetworkfleetAvlColors.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblNetworkfleetAvlColors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblNetworkfleetAvlColors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblNetworkfleetAvlColors.Size = new System.Drawing.Size(233, 48);
            this.tblNetworkfleetAvlColors.TabIndex = 100;
            // 
            // label21
            // 
            this.label21.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(20, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(37, 13);
            this.label21.TabIndex = 95;
            this.label21.Text = "Active";
            // 
            // label18
            // 
            this.label18.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(93, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(45, 13);
            this.label18.TabIndex = 91;
            this.label18.Text = "Inactive";
            // 
            // label19
            // 
            this.label19.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(172, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(43, 13);
            this.label19.TabIndex = 92;
            this.label19.Text = "My Unit";
            // 
            // ptAliNetworkfleetColor
            // 
            this.ptAliNetworkfleetColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ptAliNetworkfleetColor.Location = new System.Drawing.Point(3, 16);
            this.ptAliNetworkfleetColor.Name = "ptAliNetworkfleetColor";
            this.ptAliNetworkfleetColor.Size = new System.Drawing.Size(71, 29);
            this.ptAliNetworkfleetColor.TabIndex = 70;
            // 
            // pnlAliNetworkfleetAVLInactiveColor
            // 
            this.pnlAliNetworkfleetAVLInactiveColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAliNetworkfleetAVLInactiveColor.Location = new System.Drawing.Point(80, 16);
            this.pnlAliNetworkfleetAVLInactiveColor.Name = "pnlAliNetworkfleetAVLInactiveColor";
            this.pnlAliNetworkfleetAVLInactiveColor.Size = new System.Drawing.Size(71, 29);
            this.pnlAliNetworkfleetAVLInactiveColor.TabIndex = 88;
            // 
            // pnlAliNetworkfleetAVLMyVehicleColor
            // 
            this.pnlAliNetworkfleetAVLMyVehicleColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAliNetworkfleetAVLMyVehicleColor.Location = new System.Drawing.Point(157, 16);
            this.pnlAliNetworkfleetAVLMyVehicleColor.Name = "pnlAliNetworkfleetAVLMyVehicleColor";
            this.pnlAliNetworkfleetAVLMyVehicleColor.Size = new System.Drawing.Size(73, 29);
            this.pnlAliNetworkfleetAVLMyVehicleColor.TabIndex = 89;
            // 
            // ptAliNetworkfleetGraphic
            // 
            this.ptAliNetworkfleetGraphic.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ptAliNetworkfleetGraphic.Location = new System.Drawing.Point(13, 53);
            this.ptAliNetworkfleetGraphic.Name = "ptAliNetworkfleetGraphic";
            this.tblAliNetworkfleetSymbology.SetRowSpan(this.ptAliNetworkfleetGraphic, 2);
            this.ptAliNetworkfleetGraphic.Size = new System.Drawing.Size(50, 50);
            this.ptAliNetworkfleetGraphic.TabIndex = 4;
            // 
            // label22
            // 
            this.label22.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(89, 56);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(54, 13);
            this.label22.TabIndex = 102;
            this.label22.Text = "Font Size:";
            // 
            // btnAliNetworkfleetFont
            // 
            this.btnAliNetworkfleetFont.Location = new System.Drawing.Point(3, 24);
            this.btnAliNetworkfleetFont.Name = "btnAliNetworkfleetFont";
            this.btnAliNetworkfleetFont.Size = new System.Drawing.Size(70, 23);
            this.btnAliNetworkfleetFont.TabIndex = 68;
            this.btnAliNetworkfleetFont.Text = "Select Font";
            this.btnAliNetworkfleetFont.UseVisualStyleBackColor = true;
            this.btnAliNetworkfleetFont.Click += new System.EventHandler(this.btnAliNetworkfleetFont_Click);
            // 
            // lblAliNetworkfleetFont
            // 
            this.lblAliNetworkfleetFont.BackColor = System.Drawing.Color.Transparent;
            this.tblAliNetworkfleetSymbology.SetColumnSpan(this.lblAliNetworkfleetFont, 2);
            this.lblAliNetworkfleetFont.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAliNetworkfleetFont.Location = new System.Drawing.Point(79, 21);
            this.lblAliNetworkfleetFont.Name = "lblAliNetworkfleetFont";
            this.lblAliNetworkfleetFont.Size = new System.Drawing.Size(151, 29);
            this.lblAliNetworkfleetFont.TabIndex = 69;
            this.lblAliNetworkfleetFont.Text = "This is the font name";
            // 
            // txtAliNetworkfleetSize
            // 
            this.txtAliNetworkfleetSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.txtAliNetworkfleetSize.Location = new System.Drawing.Point(149, 53);
            this.txtAliNetworkfleetSize.Name = "txtAliNetworkfleetSize";
            this.txtAliNetworkfleetSize.ReadOnly = true;
            this.txtAliNetworkfleetSize.Size = new System.Drawing.Size(34, 20);
            this.txtAliNetworkfleetSize.TabIndex = 101;
            // 
            // label20
            // 
            this.label20.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(87, 84);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(56, 13);
            this.label20.TabIndex = 94;
            this.label20.Text = "Character:";
            // 
            // txtAliNetworkfleetChar
            // 
            this.txtAliNetworkfleetChar.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.txtAliNetworkfleetChar.Location = new System.Drawing.Point(149, 81);
            this.txtAliNetworkfleetChar.MaxLength = 1;
            this.txtAliNetworkfleetChar.Name = "txtAliNetworkfleetChar";
            this.txtAliNetworkfleetChar.Size = new System.Drawing.Size(34, 20);
            this.txtAliNetworkfleetChar.TabIndex = 67;
            // 
            // pnlAliGlobalCad
            // 
            this.pnlAliGlobalCad.ColumnCount = 3;
            this.pnlAliGlobalCad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlAliGlobalCad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 71.42857F));
            this.pnlAliGlobalCad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.57143F));
            this.pnlAliGlobalCad.Controls.Add(this.btnAliGlobalCadConfigIniBrowse, 2, 3);
            this.pnlAliGlobalCad.Controls.Add(this.groupBox4, 0, 0);
            this.pnlAliGlobalCad.Controls.Add(this.btnAliGlobalCadArchivePathBrowse, 2, 2);
            this.pnlAliGlobalCad.Controls.Add(this.lblAliGlobalCadLogPath, 0, 1);
            this.pnlAliGlobalCad.Controls.Add(this.btnAliGlobalCadLogPathBrowse, 2, 1);
            this.pnlAliGlobalCad.Controls.Add(this.txtAliGlobalCadConfigIni, 1, 3);
            this.pnlAliGlobalCad.Controls.Add(this.lblAliGlobalCadArchivePath, 0, 2);
            this.pnlAliGlobalCad.Controls.Add(this.txtAliGlobalCadArchivePath, 1, 2);
            this.pnlAliGlobalCad.Controls.Add(this.lblAliGlobalCadConfigIni, 0, 3);
            this.pnlAliGlobalCad.Controls.Add(this.txtAliGlobalCadLogPath, 1, 1);
            this.pnlAliGlobalCad.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlAliGlobalCad.Location = new System.Drawing.Point(241, 148);
            this.pnlAliGlobalCad.Margin = new System.Windows.Forms.Padding(8, 0, 0, 3);
            this.pnlAliGlobalCad.MinimumSize = new System.Drawing.Size(0, 112);
            this.pnlAliGlobalCad.Name = "pnlAliGlobalCad";
            this.pnlAliGlobalCad.RowCount = 5;
            this.pnlAliGlobalCad.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliGlobalCad.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliGlobalCad.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliGlobalCad.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliGlobalCad.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.pnlAliGlobalCad.Size = new System.Drawing.Size(343, 115);
            this.pnlAliGlobalCad.TabIndex = 74;
            // 
            // btnAliGlobalCadConfigIniBrowse
            // 
            this.btnAliGlobalCadConfigIniBrowse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAliGlobalCadConfigIniBrowse.Location = new System.Drawing.Point(275, 84);
            this.btnAliGlobalCadConfigIniBrowse.Name = "btnAliGlobalCadConfigIniBrowse";
            this.btnAliGlobalCadConfigIniBrowse.Size = new System.Drawing.Size(65, 23);
            this.btnAliGlobalCadConfigIniBrowse.TabIndex = 8;
            this.btnAliGlobalCadConfigIniBrowse.Text = "Browse";
            this.btnAliGlobalCadConfigIniBrowse.UseVisualStyleBackColor = true;
            this.btnAliGlobalCadConfigIniBrowse.Click += new System.EventHandler(this.btnAliGlobalCadConfigIniBrowse_Click);
            // 
            // groupBox4
            // 
            this.pnlAliGlobalCad.SetColumnSpan(this.groupBox4, 3);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox4.Location = new System.Drawing.Point(0, 3);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.groupBox4.MinimumSize = new System.Drawing.Size(0, 15);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.groupBox4.Size = new System.Drawing.Size(343, 17);
            this.groupBox4.TabIndex = 100;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "GlobalCAD Log Settings";
            // 
            // btnAliGlobalCadArchivePathBrowse
            // 
            this.btnAliGlobalCadArchivePathBrowse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAliGlobalCadArchivePathBrowse.Location = new System.Drawing.Point(275, 55);
            this.btnAliGlobalCadArchivePathBrowse.Name = "btnAliGlobalCadArchivePathBrowse";
            this.btnAliGlobalCadArchivePathBrowse.Size = new System.Drawing.Size(65, 23);
            this.btnAliGlobalCadArchivePathBrowse.TabIndex = 7;
            this.btnAliGlobalCadArchivePathBrowse.Text = "Browse";
            this.btnAliGlobalCadArchivePathBrowse.UseVisualStyleBackColor = true;
            this.btnAliGlobalCadArchivePathBrowse.Click += new System.EventHandler(this.btnAliGlobalCadArchivePathBrowse_Click);
            // 
            // lblAliGlobalCadLogPath
            // 
            this.lblAliGlobalCadLogPath.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAliGlobalCadLogPath.AutoSize = true;
            this.lblAliGlobalCadLogPath.Location = new System.Drawing.Point(22, 31);
            this.lblAliGlobalCadLogPath.Name = "lblAliGlobalCadLogPath";
            this.lblAliGlobalCadLogPath.Size = new System.Drawing.Size(72, 13);
            this.lblAliGlobalCadLogPath.TabIndex = 2;
            this.lblAliGlobalCadLogPath.Text = "Log Location:";
            // 
            // btnAliGlobalCadLogPathBrowse
            // 
            this.btnAliGlobalCadLogPathBrowse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAliGlobalCadLogPathBrowse.Location = new System.Drawing.Point(275, 26);
            this.btnAliGlobalCadLogPathBrowse.Name = "btnAliGlobalCadLogPathBrowse";
            this.btnAliGlobalCadLogPathBrowse.Size = new System.Drawing.Size(65, 23);
            this.btnAliGlobalCadLogPathBrowse.TabIndex = 1;
            this.btnAliGlobalCadLogPathBrowse.Text = "Browse";
            this.btnAliGlobalCadLogPathBrowse.UseVisualStyleBackColor = true;
            this.btnAliGlobalCadLogPathBrowse.Click += new System.EventHandler(this.btnAliGlobalCadLogPathBrowse_Click);
            // 
            // txtAliGlobalCadConfigIni
            // 
            this.txtAliGlobalCadConfigIni.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAliGlobalCadConfigIni.Location = new System.Drawing.Point(100, 84);
            this.txtAliGlobalCadConfigIni.Name = "txtAliGlobalCadConfigIni";
            this.txtAliGlobalCadConfigIni.Size = new System.Drawing.Size(169, 20);
            this.txtAliGlobalCadConfigIni.TabIndex = 6;
            // 
            // lblAliGlobalCadArchivePath
            // 
            this.lblAliGlobalCadArchivePath.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAliGlobalCadArchivePath.AutoSize = true;
            this.lblAliGlobalCadArchivePath.Location = new System.Drawing.Point(4, 60);
            this.lblAliGlobalCadArchivePath.Name = "lblAliGlobalCadArchivePath";
            this.lblAliGlobalCadArchivePath.Size = new System.Drawing.Size(90, 13);
            this.lblAliGlobalCadArchivePath.TabIndex = 4;
            this.lblAliGlobalCadArchivePath.Text = "Archive Location:";
            // 
            // txtAliGlobalCadArchivePath
            // 
            this.txtAliGlobalCadArchivePath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAliGlobalCadArchivePath.Location = new System.Drawing.Point(100, 55);
            this.txtAliGlobalCadArchivePath.Name = "txtAliGlobalCadArchivePath";
            this.txtAliGlobalCadArchivePath.Size = new System.Drawing.Size(169, 20);
            this.txtAliGlobalCadArchivePath.TabIndex = 3;
            // 
            // lblAliGlobalCadConfigIni
            // 
            this.lblAliGlobalCadConfigIni.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAliGlobalCadConfigIni.AutoSize = true;
            this.lblAliGlobalCadConfigIni.Location = new System.Drawing.Point(3, 89);
            this.lblAliGlobalCadConfigIni.Name = "lblAliGlobalCadConfigIni";
            this.lblAliGlobalCadConfigIni.Size = new System.Drawing.Size(91, 13);
            this.lblAliGlobalCadConfigIni.TabIndex = 5;
            this.lblAliGlobalCadConfigIni.Text = "Configuration File:";
            // 
            // txtAliGlobalCadLogPath
            // 
            this.txtAliGlobalCadLogPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAliGlobalCadLogPath.Location = new System.Drawing.Point(100, 26);
            this.txtAliGlobalCadLogPath.Name = "txtAliGlobalCadLogPath";
            this.txtAliGlobalCadLogPath.Size = new System.Drawing.Size(169, 20);
            this.txtAliGlobalCadLogPath.TabIndex = 0;
            // 
            // pnlAliValidate
            // 
            this.pnlAliValidate.ColumnCount = 2;
            this.aliPanelTableLayout.SetColumnSpan(this.pnlAliValidate, 2);
            this.pnlAliValidate.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlAliValidate.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlAliValidate.Controls.Add(this.cmbAliMode, 0, 0);
            this.pnlAliValidate.Controls.Add(this.btnAliValidate, 1, 0);
            this.pnlAliValidate.Controls.Add(this.chkNetworkfleet, 0, 1);
            this.pnlAliValidate.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlAliValidate.Location = new System.Drawing.Point(0, 8);
            this.pnlAliValidate.Margin = new System.Windows.Forms.Padding(0, 8, 0, 3);
            this.pnlAliValidate.MinimumSize = new System.Drawing.Size(0, 50);
            this.pnlAliValidate.Name = "pnlAliValidate";
            this.pnlAliValidate.RowCount = 2;
            this.pnlAliValidate.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliValidate.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlAliValidate.Size = new System.Drawing.Size(348, 50);
            this.pnlAliValidate.TabIndex = 74;
            // 
            // cmbAliMode
            // 
            this.cmbAliMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbAliMode.FormattingEnabled = true;
            this.cmbAliMode.Location = new System.Drawing.Point(3, 3);
            this.cmbAliMode.Name = "cmbAliMode";
            this.cmbAliMode.Size = new System.Drawing.Size(261, 21);
            this.cmbAliMode.TabIndex = 66;
            this.cmbAliMode.SelectedIndexChanged += new System.EventHandler(this.cmbAliMode_SelectedIndexChanged);
            // 
            // btnAliValidate
            // 
            this.btnAliValidate.Location = new System.Drawing.Point(270, 3);
            this.btnAliValidate.Name = "btnAliValidate";
            this.btnAliValidate.Size = new System.Drawing.Size(75, 23);
            this.btnAliValidate.TabIndex = 70;
            this.btnAliValidate.Text = "Validate";
            this.btnAliValidate.UseVisualStyleBackColor = true;
            this.btnAliValidate.Click += new System.EventHandler(this.btnAliValidate_Click);
            // 
            // chkNetworkfleet
            // 
            this.chkNetworkfleet.AutoSize = true;
            this.chkNetworkfleet.Checked = true;
            this.chkNetworkfleet.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNetworkfleet.Location = new System.Drawing.Point(20, 32);
            this.chkNetworkfleet.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.chkNetworkfleet.Name = "chkNetworkfleet";
            this.chkNetworkfleet.Size = new System.Drawing.Size(146, 17);
            this.chkNetworkfleet.TabIndex = 65;
            this.chkNetworkfleet.Text = "Use Verizon Networkfleet";
            this.chkNetworkfleet.UseVisualStyleBackColor = true;
            this.chkNetworkfleet.CheckedChanged += new System.EventHandler(this.chkNetworkfleet_CheckedChanged);
            // 
            // btnSplitSave
            // 
            this.btnSplitSave.AutoSize = true;
            this.btnSplitSave.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnSplitSave.Location = new System.Drawing.Point(509, 745);
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
            this.btnCancel.Location = new System.Drawing.Point(428, 745);
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
            this.ClientSize = new System.Drawing.Size(598, 777);
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
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
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
            this.aliPanelTableLayout.ResumeLayout(false);
            this.pnlAliEnterpol.ResumeLayout(false);
            this.pnlAliEnterpol.PerformLayout();
            this.pnlAliEnterpolAvl.ResumeLayout(false);
            this.pnlAliEnterpolAvl.PerformLayout();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLLabelXOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLLabelYOffset)).EndInit();
            this.tblEnterpolAvlSymbology.ResumeLayout(false);
            this.tblEnterpolAvlSymbology.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLAge1Freq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLAge2Freq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLAge3Freq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLReadFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliEnterpolAVLUpdateFreq)).EndInit();
            this.pnlAliSdrServer.ResumeLayout(false);
            this.pnlAliSdrServer.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAliInterfaceUdpPort)).EndInit();
            this.pnlAliNetworkfleet.ResumeLayout(false);
            this.tblAliNetworkfleetSettings.ResumeLayout(false);
            this.tblAliNetworkfleetSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAliNetworkfleetAVLAge1Freq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliNetworkfleetAVLAge2Freq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliNetworkfleetAVLAge3Freq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliNetworkfleetUdpPort)).EndInit();
            this.tblAliNetworkfleetLabeling.ResumeLayout(false);
            this.tblAliNetworkfleetLabeling.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAliNetworkfleetLabelYOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAliNetworkfleetLabelXOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNetworkfleetLabelLookup)).EndInit();
            this.tblAliNetworkfleetSymbology.ResumeLayout(false);
            this.tblAliNetworkfleetSymbology.PerformLayout();
            this.tblNetworkfleetAvlColors.ResumeLayout(false);
            this.tblNetworkfleetAvlColors.PerformLayout();
            this.pnlAliGlobalCad.ResumeLayout(false);
            this.pnlAliGlobalCad.PerformLayout();
            this.pnlAliValidate.ResumeLayout(false);
            this.pnlAliValidate.PerformLayout();
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
        private System.Windows.Forms.Panel ptAliNetworkfleetColor;
        private System.Windows.Forms.Label lblAliNetworkfleetFont;
        private System.Windows.Forms.Button btnAliNetworkfleetFont;
        private System.Windows.Forms.TextBox txtAliNetworkfleetChar;
        private System.Windows.Forms.Panel ptAliNetworkfleetGraphic;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblAliNetworkfleetUdpHost;
        private System.Windows.Forms.NumericUpDown numAliNetworkfleetUdpPort;
        private System.Windows.Forms.TextBox txtAliNetworkfleetUdpHost;
        private System.Windows.Forms.Button btnAliValidate;
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
        private System.Windows.Forms.Button btnAliGlobalCadConfigIniBrowse;
        private System.Windows.Forms.Button btnAliGlobalCadArchivePathBrowse;
        private System.Windows.Forms.TextBox txtAliGlobalCadConfigIni;
        private System.Windows.Forms.Label lblAliGlobalCadConfigIni;
        private System.Windows.Forms.Label lblAliGlobalCadArchivePath;
        private System.Windows.Forms.TextBox txtAliGlobalCadArchivePath;
        private System.Windows.Forms.Label lblAliGlobalCadLogPath;
        private System.Windows.Forms.Button btnAliGlobalCadLogPathBrowse;
        private System.Windows.Forms.TextBox txtAliGlobalCadLogPath;
        private System.Windows.Forms.Label lblAliInterfaceUdpPort;
        private System.Windows.Forms.Label lblAliInterfaceUdpHost;
        private System.Windows.Forms.Label lblAliInterfaceDbPath;
        private System.Windows.Forms.TextBox txtAliInterfaceUdpHost;
        private System.Windows.Forms.Button btnAliInterfaceDbPathBrowse;
        private System.Windows.Forms.TextBox txtAliInterfaceDbPath;
        private System.Windows.Forms.ComboBox cmbAliMode;
        private System.Windows.Forms.Label lblAliEnterpolAVLSetLocProc;
        private System.Windows.Forms.Label lblAliEnterpolAVLWhoAmIProc;
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
        private System.Windows.Forms.Label lblAliEnterpolAVLSymbolFontSize;
        private System.Windows.Forms.Label lblAliEnterpolAVLSymbolFontName;
        private System.Windows.Forms.TextBox txtAliEnterpolAVLFdChars;
        private System.Windows.Forms.TextBox txtAliEnterpolAVLPdChars;
        private System.Windows.Forms.TextBox txtAliEnterpolAVLEmsChars;
        private System.Windows.Forms.Button btnAliEnterpolAVLSymbolFont;
        private System.Windows.Forms.Panel pnlAliEnterpolAVLFdColor;
        private System.Windows.Forms.Panel pnlAliEnterpolAVLPdColor;
        private System.Windows.Forms.Panel pnlAliEnterpolAVLEmsColor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkAutoHideInactiveUnits;
        private System.Windows.Forms.Label lblAliEnterpolAVLMyVehicleColor;
        private System.Windows.Forms.Panel pnlAliEnterpolAVLMyVehicleColor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel pnlAliAVLInactiveColor;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblAliEnterpolAVLLabelAlignment;
        private System.Windows.Forms.NumericUpDown numAliEnterpolAVLLabelYOffset;
        private System.Windows.Forms.NumericUpDown numAliEnterpolAVLLabelXOffset;
        private System.Windows.Forms.ComboBox cmbAliEnterpolAVLLabelAlignment;
        private System.Windows.Forms.Label lblAliEnterpolAVLLabelFontSize;
        private System.Windows.Forms.Label lblAliEnterpolAVLLabelFontName;
        private System.Windows.Forms.Button btnAliEnterpolAVLLabelFont;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown numAliNetworkfleetLabelYOffset;
        private System.Windows.Forms.NumericUpDown numAliNetworkfleetLabelXOffset;
        private System.Windows.Forms.ComboBox cmbAliNetworkfleetLabelAlignment;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblAliNetworkfleetLabelFontSize;
        private System.Windows.Forms.Label lblAliNetworkfleetLabelFont;
        private System.Windows.Forms.Button btnAliNetworkfleetLabelFont;
        private System.Windows.Forms.DataGridView dgvNetworkfleetLabelLookup;
        private System.Windows.Forms.Panel pnlAliNetworkfleetAVLMyVehicleColor;
        private System.Windows.Forms.Panel pnlAliNetworkfleetAVLInactiveColor;
        private System.Windows.Forms.NumericUpDown numAliNetworkfleetAVLAge3Freq;
        private System.Windows.Forms.NumericUpDown numAliNetworkfleetAVLAge2Freq;
        private System.Windows.Forms.NumericUpDown numAliNetworkfleetAVLAge1Freq;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.CheckBox chkAutoHideInactiveUnitsNetworkfleet;
        private System.Windows.Forms.TableLayoutPanel aliPanelTableLayout;
        private System.Windows.Forms.TableLayoutPanel pnlAliValidate;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TableLayoutPanel tblAliNetworkfleetSettings;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TableLayoutPanel tblNetworkfleetAvlColors;
        private System.Windows.Forms.TextBox txtAliNetworkfleetSize;
        private System.Windows.Forms.TableLayoutPanel tblAliNetworkfleetSymbology;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn vehicle_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn vehicle_label;
        private System.Windows.Forms.TextBox txtAliNetworkfleetLabelFontSize;
        private System.Windows.Forms.TableLayoutPanel tblAliNetworkfleetLabeling;
        private System.Windows.Forms.TableLayoutPanel pnlAliNetworkfleet;
        private System.Windows.Forms.TableLayoutPanel pnlAliGlobalCad;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TableLayoutPanel pnlAliSdrServer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.NumericUpDown numAliInterfaceUdpPort;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TableLayoutPanel pnlAliEnterpol;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.TextBox txtAliEnterpolAVLLabelFontSize;
        private System.Windows.Forms.TableLayoutPanel tblEnterpolAvlSymbology;
        private System.Windows.Forms.TextBox txtAliEnterpolAVLSymbolFontSize;
        private System.Windows.Forms.TableLayoutPanel pnlAliEnterpolAvl;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewComboBoxColumn layerName;
        private System.Windows.Forms.DataGridViewComboBoxColumn fieldName;
        private System.Windows.Forms.DataGridViewButtonColumn add;
        private System.Windows.Forms.DataGridViewButtonColumn remove;

    }
}