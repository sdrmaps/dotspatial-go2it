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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdminForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSplitSave = new DotSpatial.SDR.Controls.SplitButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.adminTab_SearchProperties = new System.Windows.Forms.TabPage();
            this.chkLayerIndex = new System.Windows.Forms.CheckedListBox();
            this.cmbLayerIndex = new System.Windows.Forms.ComboBox();
            this.btnCreateIndex = new System.Windows.Forms.Button();
            this.btnDeleteIndex = new System.Windows.Forms.Button();
            this.dgvLayerIndex = new System.Windows.Forms.DataGridView();
            this.idxProgressBar = new System.Windows.Forms.ProgressBar();
            this.btnIndexCancel = new System.Windows.Forms.Button();
            this.adminTab_UserManagement = new System.Windows.Forms.TabPage();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtVerifyPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lstUsers = new System.Windows.Forms.ListBox();
            this.btnUsersAddUpdate = new System.Windows.Forms.Button();
            this.btnUsersDelete = new System.Windows.Forms.Button();
            this.adminTab_LayerManagement = new System.Windows.Forms.TabPage();
            this.adminLayerSplitter = new System.Windows.Forms.SplitContainer();
            this.cmbNotesLayer = new System.Windows.Forms.ComboBox();
            this.chkAddressLayers = new System.Windows.Forms.CheckedListBox();
            this.chkRoadLayers = new System.Windows.Forms.CheckedListBox();
            this.cmbCityLimitLayer = new System.Windows.Forms.ComboBox();
            this.lblNotes = new System.Windows.Forms.Label();
            this.lblCityLimits = new System.Windows.Forms.Label();
            this.cmbCellSectorLayer = new System.Windows.Forms.ComboBox();
            this.cmbESNLayer = new System.Windows.Forms.ComboBox();
            this.cmbParcelsLayer = new System.Windows.Forms.ComboBox();
            this.chkKeyLocationsLayers = new System.Windows.Forms.CheckedListBox();
            this.lblAddresses = new System.Windows.Forms.Label();
            this.lblRoads = new System.Windows.Forms.Label();
            this.cmbHydrantsLayer = new System.Windows.Forms.ComboBox();
            this.lblCellSector = new System.Windows.Forms.Label();
            this.lblEsn = new System.Windows.Forms.Label();
            this.lblParcels = new System.Windows.Forms.Label();
            this.lblHydrants = new System.Windows.Forms.Label();
            this.lblKeyLocations = new System.Windows.Forms.Label();
            this.panelRadAddress = new System.Windows.Forms.Panel();
            this.radAddressPolygons = new System.Windows.Forms.RadioButton();
            this.radAddressPoints = new System.Windows.Forms.RadioButton();
            this.panelRadKeyLocations = new System.Windows.Forms.Panel();
            this.radKeyLocationsPoints = new System.Windows.Forms.RadioButton();
            this.radKeyLocationsPolygons = new System.Windows.Forms.RadioButton();
            this.mapBGColorPanel = new System.Windows.Forms.Panel();
            this.lblMapBGColor = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.cmbActiveMapTab = new System.Windows.Forms.ComboBox();
            this.chkViewLayers = new System.Windows.Forms.CheckedListBox();
            this.btnDeleteView = new System.Windows.Forms.Button();
            this.txtViewName = new System.Windows.Forms.TextBox();
            this.btnAddView = new System.Windows.Forms.Button();
            this.legendSplitter = new System.Windows.Forms.SplitContainer();
            this.legendButtonTable = new System.Windows.Forms.TableLayoutPanel();
            this.btnAddLayer = new System.Windows.Forms.Button();
            this.btnRemoveLayer = new System.Windows.Forms.Button();
            this.adminTab_Control = new System.Windows.Forms.TabControl();
            this.tableLayoutPanel1.SuspendLayout();
            this.adminTab_SearchProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLayerIndex)).BeginInit();
            this.adminTab_UserManagement.SuspendLayout();
            this.adminTab_LayerManagement.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.adminLayerSplitter)).BeginInit();
            this.adminLayerSplitter.Panel1.SuspendLayout();
            this.adminLayerSplitter.Panel2.SuspendLayout();
            this.adminLayerSplitter.SuspendLayout();
            this.panelRadAddress.SuspendLayout();
            this.panelRadKeyLocations.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.legendSplitter)).BeginInit();
            this.legendSplitter.Panel2.SuspendLayout();
            this.legendSplitter.SuspendLayout();
            this.legendButtonTable.SuspendLayout();
            this.adminTab_Control.SuspendLayout();
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(843, 731);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // btnSplitSave
            // 
            this.btnSplitSave.AutoSize = true;
            this.btnSplitSave.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnSplitSave.Location = new System.Drawing.Point(754, 699);
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
            this.btnCancel.Location = new System.Drawing.Point(673, 699);
            this.btnCancel.MaximumSize = new System.Drawing.Size(75, 28);
            this.btnCancel.MinimumSize = new System.Drawing.Size(75, 23);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // adminTab_SearchProperties
            // 
            this.adminTab_SearchProperties.Controls.Add(this.btnIndexCancel);
            this.adminTab_SearchProperties.Controls.Add(this.idxProgressBar);
            this.adminTab_SearchProperties.Controls.Add(this.dgvLayerIndex);
            this.adminTab_SearchProperties.Controls.Add(this.btnDeleteIndex);
            this.adminTab_SearchProperties.Controls.Add(this.btnCreateIndex);
            this.adminTab_SearchProperties.Controls.Add(this.cmbLayerIndex);
            this.adminTab_SearchProperties.Controls.Add(this.chkLayerIndex);
            this.adminTab_SearchProperties.Location = new System.Drawing.Point(4, 22);
            this.adminTab_SearchProperties.Name = "adminTab_SearchProperties";
            this.adminTab_SearchProperties.Padding = new System.Windows.Forms.Padding(3);
            this.adminTab_SearchProperties.Size = new System.Drawing.Size(835, 670);
            this.adminTab_SearchProperties.TabIndex = 4;
            this.adminTab_SearchProperties.Text = "Search Properties";
            this.adminTab_SearchProperties.UseVisualStyleBackColor = true;
            // 
            // chkLayerIndex
            // 
            this.chkLayerIndex.CheckOnClick = true;
            this.chkLayerIndex.FormattingEnabled = true;
            this.chkLayerIndex.Location = new System.Drawing.Point(36, 76);
            this.chkLayerIndex.Name = "chkLayerIndex";
            this.chkLayerIndex.Size = new System.Drawing.Size(216, 169);
            this.chkLayerIndex.TabIndex = 0;
            this.chkLayerIndex.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkLayerIndex_ItemCheck);
            // 
            // cmbLayerIndex
            // 
            this.cmbLayerIndex.FormattingEnabled = true;
            this.cmbLayerIndex.Location = new System.Drawing.Point(36, 34);
            this.cmbLayerIndex.Name = "cmbLayerIndex";
            this.cmbLayerIndex.Size = new System.Drawing.Size(344, 21);
            this.cmbLayerIndex.TabIndex = 1;
            this.cmbLayerIndex.SelectedValueChanged += new System.EventHandler(this.cmbLayerIndex_SelectedValueChanged);
            // 
            // btnCreateIndex
            // 
            this.btnCreateIndex.Location = new System.Drawing.Point(177, 251);
            this.btnCreateIndex.Name = "btnCreateIndex";
            this.btnCreateIndex.Size = new System.Drawing.Size(75, 23);
            this.btnCreateIndex.TabIndex = 2;
            this.btnCreateIndex.Text = "Create Index";
            this.btnCreateIndex.UseVisualStyleBackColor = true;
            this.btnCreateIndex.Click += new System.EventHandler(this.btnCreateIndex_Click);
            // 
            // btnDeleteIndex
            // 
            this.btnDeleteIndex.Location = new System.Drawing.Point(96, 251);
            this.btnDeleteIndex.Name = "btnDeleteIndex";
            this.btnDeleteIndex.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteIndex.TabIndex = 3;
            this.btnDeleteIndex.Text = "Delete Index";
            this.btnDeleteIndex.UseVisualStyleBackColor = true;
            this.btnDeleteIndex.Click += new System.EventHandler(this.btnDeleteIndex_Click);
            // 
            // dgvLayerIndex
            // 
            this.dgvLayerIndex.AllowUserToAddRows = false;
            this.dgvLayerIndex.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLayerIndex.Location = new System.Drawing.Point(262, 76);
            this.dgvLayerIndex.MultiSelect = false;
            this.dgvLayerIndex.Name = "dgvLayerIndex";
            this.dgvLayerIndex.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLayerIndex.Size = new System.Drawing.Size(329, 415);
            this.dgvLayerIndex.TabIndex = 4;
            // 
            // idxProgressBar
            // 
            this.idxProgressBar.Location = new System.Drawing.Point(20, 513);
            this.idxProgressBar.Name = "idxProgressBar";
            this.idxProgressBar.Size = new System.Drawing.Size(571, 23);
            this.idxProgressBar.TabIndex = 5;
            this.idxProgressBar.Visible = false;
            // 
            // btnIndexCancel
            // 
            this.btnIndexCancel.Location = new System.Drawing.Point(134, 302);
            this.btnIndexCancel.Name = "btnIndexCancel";
            this.btnIndexCancel.Size = new System.Drawing.Size(75, 23);
            this.btnIndexCancel.TabIndex = 6;
            this.btnIndexCancel.Text = "Cancel";
            this.btnIndexCancel.UseVisualStyleBackColor = true;
            this.btnIndexCancel.Click += new System.EventHandler(this.btnIndexCancel_Click);
            // 
            // adminTab_UserManagement
            // 
            this.adminTab_UserManagement.Controls.Add(this.btnUsersDelete);
            this.adminTab_UserManagement.Controls.Add(this.btnUsersAddUpdate);
            this.adminTab_UserManagement.Controls.Add(this.lstUsers);
            this.adminTab_UserManagement.Controls.Add(this.label3);
            this.adminTab_UserManagement.Controls.Add(this.label2);
            this.adminTab_UserManagement.Controls.Add(this.label1);
            this.adminTab_UserManagement.Controls.Add(this.txtUsername);
            this.adminTab_UserManagement.Controls.Add(this.txtVerifyPassword);
            this.adminTab_UserManagement.Controls.Add(this.txtPassword);
            this.adminTab_UserManagement.Location = new System.Drawing.Point(4, 22);
            this.adminTab_UserManagement.Name = "adminTab_UserManagement";
            this.adminTab_UserManagement.Padding = new System.Windows.Forms.Padding(3);
            this.adminTab_UserManagement.Size = new System.Drawing.Size(835, 670);
            this.adminTab_UserManagement.TabIndex = 3;
            this.adminTab_UserManagement.Text = "User Management";
            this.adminTab_UserManagement.UseVisualStyleBackColor = true;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(252, 165);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(168, 20);
            this.txtPassword.TabIndex = 0;
            // 
            // txtVerifyPassword
            // 
            this.txtVerifyPassword.Location = new System.Drawing.Point(252, 199);
            this.txtVerifyPassword.Name = "txtVerifyPassword";
            this.txtVerifyPassword.PasswordChar = '*';
            this.txtVerifyPassword.Size = new System.Drawing.Size(168, 20);
            this.txtVerifyPassword.TabIndex = 1;
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(228, 123);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(178, 20);
            this.txtUsername.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(178, 172);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(164, 202);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Verify Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(167, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Username";
            // 
            // lstUsers
            // 
            this.lstUsers.FormattingEnabled = true;
            this.lstUsers.Location = new System.Drawing.Point(27, 97);
            this.lstUsers.Name = "lstUsers";
            this.lstUsers.Size = new System.Drawing.Size(134, 173);
            this.lstUsers.TabIndex = 6;
            this.lstUsers.DoubleClick += new System.EventHandler(this.lstUsers_DoubleClick);
            // 
            // btnUsersAddUpdate
            // 
            this.btnUsersAddUpdate.Location = new System.Drawing.Point(215, 247);
            this.btnUsersAddUpdate.Name = "btnUsersAddUpdate";
            this.btnUsersAddUpdate.Size = new System.Drawing.Size(81, 23);
            this.btnUsersAddUpdate.TabIndex = 7;
            this.btnUsersAddUpdate.Text = "Add/Update";
            this.btnUsersAddUpdate.UseVisualStyleBackColor = true;
            this.btnUsersAddUpdate.Click += new System.EventHandler(this.btnUsersAddUpdate_Click);
            // 
            // btnUsersDelete
            // 
            this.btnUsersDelete.Location = new System.Drawing.Point(302, 247);
            this.btnUsersDelete.Name = "btnUsersDelete";
            this.btnUsersDelete.Size = new System.Drawing.Size(75, 23);
            this.btnUsersDelete.TabIndex = 8;
            this.btnUsersDelete.Text = "Delete";
            this.btnUsersDelete.UseVisualStyleBackColor = true;
            this.btnUsersDelete.Click += new System.EventHandler(this.btnUsersDelete_Click);
            // 
            // adminTab_LayerManagement
            // 
            this.adminTab_LayerManagement.BackColor = System.Drawing.Color.Transparent;
            this.adminTab_LayerManagement.Controls.Add(this.adminLayerSplitter);
            this.adminTab_LayerManagement.Location = new System.Drawing.Point(4, 22);
            this.adminTab_LayerManagement.Margin = new System.Windows.Forms.Padding(0);
            this.adminTab_LayerManagement.Name = "adminTab_LayerManagement";
            this.adminTab_LayerManagement.Size = new System.Drawing.Size(835, 670);
            this.adminTab_LayerManagement.TabIndex = 0;
            this.adminTab_LayerManagement.Text = "Layer Management";
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
            this.adminLayerSplitter.Panel2.Controls.Add(this.button1);
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
            this.adminLayerSplitter.Size = new System.Drawing.Size(835, 670);
            this.adminLayerSplitter.SplitterDistance = 202;
            this.adminLayerSplitter.SplitterWidth = 10;
            this.adminLayerSplitter.TabIndex = 8;
            this.adminLayerSplitter.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.adminLayerSplitter_SplitterMoved);
            // 
            // cmbNotesLayer
            // 
            this.cmbNotesLayer.FormattingEnabled = true;
            this.cmbNotesLayer.Location = new System.Drawing.Point(108, 361);
            this.cmbNotesLayer.Name = "cmbNotesLayer";
            this.cmbNotesLayer.Size = new System.Drawing.Size(199, 21);
            this.cmbNotesLayer.TabIndex = 5;
            this.cmbNotesLayer.SelectionChangeCommitted += new System.EventHandler(this.cmbNotesLayer_SelectionChangeCommitted);
            // 
            // chkAddressLayers
            // 
            this.chkAddressLayers.CheckOnClick = true;
            this.chkAddressLayers.FormattingEnabled = true;
            this.chkAddressLayers.Location = new System.Drawing.Point(108, 161);
            this.chkAddressLayers.Name = "chkAddressLayers";
            this.chkAddressLayers.Size = new System.Drawing.Size(199, 94);
            this.chkAddressLayers.TabIndex = 6;
            this.chkAddressLayers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkAddressLayers_ItemCheck);
            // 
            // chkRoadLayers
            // 
            this.chkRoadLayers.CheckOnClick = true;
            this.chkRoadLayers.FormattingEnabled = true;
            this.chkRoadLayers.Location = new System.Drawing.Point(108, 261);
            this.chkRoadLayers.Name = "chkRoadLayers";
            this.chkRoadLayers.Size = new System.Drawing.Size(199, 94);
            this.chkRoadLayers.TabIndex = 7;
            this.chkRoadLayers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkRoadLayers_ItemCheck);
            // 
            // cmbCityLimitLayer
            // 
            this.cmbCityLimitLayer.FormattingEnabled = true;
            this.cmbCityLimitLayer.Location = new System.Drawing.Point(108, 388);
            this.cmbCityLimitLayer.Name = "cmbCityLimitLayer";
            this.cmbCityLimitLayer.Size = new System.Drawing.Size(199, 21);
            this.cmbCityLimitLayer.TabIndex = 8;
            this.cmbCityLimitLayer.SelectionChangeCommitted += new System.EventHandler(this.cmbCityLimitLayer_SelectionChangeCommitted);
            // 
            // lblNotes
            // 
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(64, 364);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(38, 13);
            this.lblNotes.TabIndex = 15;
            this.lblNotes.Text = "Notes:";
            // 
            // lblCityLimits
            // 
            this.lblCityLimits.AutoSize = true;
            this.lblCityLimits.Location = new System.Drawing.Point(46, 391);
            this.lblCityLimits.Name = "lblCityLimits";
            this.lblCityLimits.Size = new System.Drawing.Size(56, 13);
            this.lblCityLimits.TabIndex = 16;
            this.lblCityLimits.Text = "City Limits:";
            // 
            // cmbCellSectorLayer
            // 
            this.cmbCellSectorLayer.FormattingEnabled = true;
            this.cmbCellSectorLayer.Location = new System.Drawing.Point(108, 415);
            this.cmbCellSectorLayer.Name = "cmbCellSectorLayer";
            this.cmbCellSectorLayer.Size = new System.Drawing.Size(199, 21);
            this.cmbCellSectorLayer.TabIndex = 18;
            this.cmbCellSectorLayer.SelectionChangeCommitted += new System.EventHandler(this.cmbCellSectorLayer_SelectionChangeCommitted);
            // 
            // cmbESNLayer
            // 
            this.cmbESNLayer.FormattingEnabled = true;
            this.cmbESNLayer.Location = new System.Drawing.Point(108, 442);
            this.cmbESNLayer.Name = "cmbESNLayer";
            this.cmbESNLayer.Size = new System.Drawing.Size(199, 21);
            this.cmbESNLayer.TabIndex = 20;
            this.cmbESNLayer.SelectionChangeCommitted += new System.EventHandler(this.cmbESNLayer_SelectionChangeCommitted);
            // 
            // cmbParcelsLayer
            // 
            this.cmbParcelsLayer.FormattingEnabled = true;
            this.cmbParcelsLayer.Location = new System.Drawing.Point(108, 469);
            this.cmbParcelsLayer.Name = "cmbParcelsLayer";
            this.cmbParcelsLayer.Size = new System.Drawing.Size(199, 21);
            this.cmbParcelsLayer.TabIndex = 21;
            this.cmbParcelsLayer.SelectionChangeCommitted += new System.EventHandler(this.cmbParcelsLayer_SelectionChangeCommitted);
            // 
            // chkKeyLocationsLayers
            // 
            this.chkKeyLocationsLayers.CheckOnClick = true;
            this.chkKeyLocationsLayers.FormattingEnabled = true;
            this.chkKeyLocationsLayers.Location = new System.Drawing.Point(108, 523);
            this.chkKeyLocationsLayers.Name = "chkKeyLocationsLayers";
            this.chkKeyLocationsLayers.Size = new System.Drawing.Size(199, 94);
            this.chkKeyLocationsLayers.TabIndex = 22;
            this.chkKeyLocationsLayers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkKeyLocationsLayers_ItemCheck);
            // 
            // lblAddresses
            // 
            this.lblAddresses.AutoSize = true;
            this.lblAddresses.Location = new System.Drawing.Point(46, 161);
            this.lblAddresses.Name = "lblAddresses";
            this.lblAddresses.Size = new System.Drawing.Size(59, 13);
            this.lblAddresses.TabIndex = 23;
            this.lblAddresses.Text = "Addresses:";
            // 
            // lblRoads
            // 
            this.lblRoads.AutoSize = true;
            this.lblRoads.Location = new System.Drawing.Point(61, 261);
            this.lblRoads.Name = "lblRoads";
            this.lblRoads.Size = new System.Drawing.Size(41, 13);
            this.lblRoads.TabIndex = 24;
            this.lblRoads.Text = "Roads:";
            // 
            // cmbHydrantsLayer
            // 
            this.cmbHydrantsLayer.FormattingEnabled = true;
            this.cmbHydrantsLayer.Location = new System.Drawing.Point(108, 496);
            this.cmbHydrantsLayer.Name = "cmbHydrantsLayer";
            this.cmbHydrantsLayer.Size = new System.Drawing.Size(199, 21);
            this.cmbHydrantsLayer.TabIndex = 26;
            this.cmbHydrantsLayer.SelectionChangeCommitted += new System.EventHandler(this.cmbHydrantsLayer_SelectionChangeCommitted);
            // 
            // lblCellSector
            // 
            this.lblCellSector.AutoSize = true;
            this.lblCellSector.Location = new System.Drawing.Point(36, 418);
            this.lblCellSector.Name = "lblCellSector";
            this.lblCellSector.Size = new System.Drawing.Size(66, 13);
            this.lblCellSector.TabIndex = 27;
            this.lblCellSector.Text = "Cell Sectors:";
            // 
            // lblEsn
            // 
            this.lblEsn.AutoSize = true;
            this.lblEsn.Location = new System.Drawing.Point(73, 445);
            this.lblEsn.Name = "lblEsn";
            this.lblEsn.Size = new System.Drawing.Size(32, 13);
            this.lblEsn.TabIndex = 29;
            this.lblEsn.Text = "ESN:";
            // 
            // lblParcels
            // 
            this.lblParcels.AutoSize = true;
            this.lblParcels.Location = new System.Drawing.Point(58, 472);
            this.lblParcels.Name = "lblParcels";
            this.lblParcels.Size = new System.Drawing.Size(45, 13);
            this.lblParcels.TabIndex = 30;
            this.lblParcels.Text = "Parcels:";
            // 
            // lblHydrants
            // 
            this.lblHydrants.AutoSize = true;
            this.lblHydrants.Location = new System.Drawing.Point(50, 499);
            this.lblHydrants.Name = "lblHydrants";
            this.lblHydrants.Size = new System.Drawing.Size(52, 13);
            this.lblHydrants.TabIndex = 31;
            this.lblHydrants.Text = "Hydrants:";
            // 
            // lblKeyLocations
            // 
            this.lblKeyLocations.AutoSize = true;
            this.lblKeyLocations.Location = new System.Drawing.Point(25, 523);
            this.lblKeyLocations.Name = "lblKeyLocations";
            this.lblKeyLocations.Size = new System.Drawing.Size(77, 13);
            this.lblKeyLocations.TabIndex = 32;
            this.lblKeyLocations.Text = "Key Locations:";
            // 
            // panelRadAddress
            // 
            this.panelRadAddress.Controls.Add(this.radAddressPoints);
            this.panelRadAddress.Controls.Add(this.radAddressPolygons);
            this.panelRadAddress.Location = new System.Drawing.Point(28, 177);
            this.panelRadAddress.Name = "panelRadAddress";
            this.panelRadAddress.Size = new System.Drawing.Size(77, 50);
            this.panelRadAddress.TabIndex = 35;
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
            // panelRadKeyLocations
            // 
            this.panelRadKeyLocations.Controls.Add(this.radKeyLocationsPolygons);
            this.panelRadKeyLocations.Controls.Add(this.radKeyLocationsPoints);
            this.panelRadKeyLocations.Location = new System.Drawing.Point(28, 539);
            this.panelRadKeyLocations.Name = "panelRadKeyLocations";
            this.panelRadKeyLocations.Size = new System.Drawing.Size(77, 55);
            this.panelRadKeyLocations.TabIndex = 36;
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
            // mapBGColorPanel
            // 
            this.mapBGColorPanel.BackColor = System.Drawing.Color.Black;
            this.mapBGColorPanel.Location = new System.Drawing.Point(108, 622);
            this.mapBGColorPanel.Name = "mapBGColorPanel";
            this.mapBGColorPanel.Size = new System.Drawing.Size(48, 26);
            this.mapBGColorPanel.TabIndex = 37;
            this.mapBGColorPanel.Click += new System.EventHandler(this.mapBGColorPanel_Click);
            // 
            // lblMapBGColor
            // 
            this.lblMapBGColor.AutoSize = true;
            this.lblMapBGColor.Location = new System.Drawing.Point(175, 630);
            this.lblMapBGColor.Name = "lblMapBGColor";
            this.lblMapBGColor.Size = new System.Drawing.Size(116, 13);
            this.lblMapBGColor.TabIndex = 38;
            this.lblMapBGColor.Text = "Map Background Color";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(216, 132);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 39;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cmbActiveMapTab
            // 
            this.cmbActiveMapTab.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActiveMapTab.FormattingEnabled = true;
            this.cmbActiveMapTab.Location = new System.Drawing.Point(330, 123);
            this.cmbActiveMapTab.Name = "cmbActiveMapTab";
            this.cmbActiveMapTab.Size = new System.Drawing.Size(269, 21);
            this.cmbActiveMapTab.TabIndex = 40;
            this.cmbActiveMapTab.SelectedIndexChanged += new System.EventHandler(this.cmbActiveMapTab_SelectedIndexChanged);
            // 
            // chkViewLayers
            // 
            this.chkViewLayers.CheckOnClick = true;
            this.chkViewLayers.FormattingEnabled = true;
            this.chkViewLayers.Location = new System.Drawing.Point(346, 179);
            this.chkViewLayers.Name = "chkViewLayers";
            this.chkViewLayers.Size = new System.Drawing.Size(241, 169);
            this.chkViewLayers.TabIndex = 41;
            // 
            // btnDeleteView
            // 
            this.btnDeleteView.Location = new System.Drawing.Point(524, 150);
            this.btnDeleteView.Name = "btnDeleteView";
            this.btnDeleteView.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteView.TabIndex = 42;
            this.btnDeleteView.Text = "Delete View";
            this.btnDeleteView.UseVisualStyleBackColor = true;
            this.btnDeleteView.Click += new System.EventHandler(this.btnRemoveView_Click);
            // 
            // txtViewName
            // 
            this.txtViewName.Location = new System.Drawing.Point(400, 96);
            this.txtViewName.Name = "txtViewName";
            this.txtViewName.Size = new System.Drawing.Size(199, 20);
            this.txtViewName.TabIndex = 43;
            // 
            // btnAddView
            // 
            this.btnAddView.Location = new System.Drawing.Point(319, 96);
            this.btnAddView.Name = "btnAddView";
            this.btnAddView.Size = new System.Drawing.Size(75, 23);
            this.btnAddView.TabIndex = 44;
            this.btnAddView.Text = "Add View";
            this.btnAddView.UseVisualStyleBackColor = true;
            this.btnAddView.Click += new System.EventHandler(this.btnAddView_Click);
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
            // legendSplitter.Panel2
            // 
            this.legendSplitter.Panel2.BackColor = System.Drawing.Color.Transparent;
            this.legendSplitter.Panel2.Controls.Add(this.legendButtonTable);
            this.legendSplitter.Panel2MinSize = 33;
            this.legendSplitter.Size = new System.Drawing.Size(200, 668);
            this.legendSplitter.SplitterDistance = 634;
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
            this.legendButtonTable.Size = new System.Drawing.Size(200, 33);
            this.legendButtonTable.TabIndex = 0;
            // 
            // btnAddLayer
            // 
            this.btnAddLayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddLayer.Location = new System.Drawing.Point(3, 3);
            this.btnAddLayer.MaximumSize = new System.Drawing.Size(0, 27);
            this.btnAddLayer.MinimumSize = new System.Drawing.Size(55, 23);
            this.btnAddLayer.Name = "btnAddLayer";
            this.btnAddLayer.Size = new System.Drawing.Size(94, 27);
            this.btnAddLayer.TabIndex = 2;
            this.btnAddLayer.Text = "Add";
            this.btnAddLayer.UseVisualStyleBackColor = true;
            this.btnAddLayer.Click += new System.EventHandler(this.btnAddLayer_Click);
            // 
            // btnRemoveLayer
            // 
            this.btnRemoveLayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRemoveLayer.Location = new System.Drawing.Point(103, 3);
            this.btnRemoveLayer.MaximumSize = new System.Drawing.Size(0, 27);
            this.btnRemoveLayer.MinimumSize = new System.Drawing.Size(55, 23);
            this.btnRemoveLayer.Name = "btnRemoveLayer";
            this.btnRemoveLayer.Size = new System.Drawing.Size(94, 27);
            this.btnRemoveLayer.TabIndex = 1;
            this.btnRemoveLayer.Text = "Remove";
            this.btnRemoveLayer.UseVisualStyleBackColor = true;
            this.btnRemoveLayer.Click += new System.EventHandler(this.btnRemoveLayer_Click);
            // 
            // adminTab_Control
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.adminTab_Control, 2);
            this.adminTab_Control.Controls.Add(this.adminTab_LayerManagement);
            this.adminTab_Control.Controls.Add(this.adminTab_UserManagement);
            this.adminTab_Control.Controls.Add(this.adminTab_SearchProperties);
            this.adminTab_Control.Dock = System.Windows.Forms.DockStyle.Fill;
            this.adminTab_Control.Location = new System.Drawing.Point(0, 0);
            this.adminTab_Control.Margin = new System.Windows.Forms.Padding(0);
            this.adminTab_Control.Name = "adminTab_Control";
            this.adminTab_Control.Padding = new System.Drawing.Point(0, 0);
            this.adminTab_Control.SelectedIndex = 0;
            this.adminTab_Control.Size = new System.Drawing.Size(843, 696);
            this.adminTab_Control.TabIndex = 8;
            // 
            // AdminForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 731);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AdminForm";
            this.Text = "Go2It Administration";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.adminTab_SearchProperties.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLayerIndex)).EndInit();
            this.adminTab_UserManagement.ResumeLayout(false);
            this.adminTab_UserManagement.PerformLayout();
            this.adminTab_LayerManagement.ResumeLayout(false);
            this.adminLayerSplitter.Panel1.ResumeLayout(false);
            this.adminLayerSplitter.Panel2.ResumeLayout(false);
            this.adminLayerSplitter.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.adminLayerSplitter)).EndInit();
            this.adminLayerSplitter.ResumeLayout(false);
            this.panelRadAddress.ResumeLayout(false);
            this.panelRadAddress.PerformLayout();
            this.panelRadKeyLocations.ResumeLayout(false);
            this.panelRadKeyLocations.PerformLayout();
            this.legendSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.legendSplitter)).EndInit();
            this.legendSplitter.ResumeLayout(false);
            this.legendButtonTable.ResumeLayout(false);
            this.adminTab_Control.ResumeLayout(false);
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
        private System.Windows.Forms.Button button1;
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
        private System.Windows.Forms.TabPage adminTab_UserManagement;
        private System.Windows.Forms.Button btnUsersDelete;
        private System.Windows.Forms.Button btnUsersAddUpdate;
        private System.Windows.Forms.ListBox lstUsers;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtVerifyPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TabPage adminTab_SearchProperties;
        private System.Windows.Forms.Button btnIndexCancel;
        private System.Windows.Forms.ProgressBar idxProgressBar;
        private System.Windows.Forms.DataGridView dgvLayerIndex;
        private System.Windows.Forms.Button btnDeleteIndex;
        private System.Windows.Forms.Button btnCreateIndex;
        private System.Windows.Forms.ComboBox cmbLayerIndex;
        private System.Windows.Forms.CheckedListBox chkLayerIndex;

    }
}