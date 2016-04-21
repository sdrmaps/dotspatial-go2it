﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using DotSpatial.Projections;
using DotSpatial.SDR.Controls;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using DotSpatial.Topology.Utilities;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Spatial;
using Lucene.Net.Spatial.Prefix;
using Lucene.Net.Spatial.Prefix.Tree;
using Lucene.Net.Store;
using SDR.Common;
using SDR.Common.UserMessage;
using SDR.Network;
using Spatial4n.Core.Context.Nts;
using Version = Lucene.Net.Util.Version;
using LDirectory = Lucene.Net.Store.Directory;
using Field = Lucene.Net.Documents.Field;
using DotSpatial.Data;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls;
using SDR.Authentication;
using SDR.Data.Database;
using Go2It.Properties;
using Directory = System.IO.Directory;
using IGeometry = DotSpatial.Topology.IGeometry;
using Point = System.Drawing.Point;
using PointShape = DotSpatial.Symbology.PointShape;
using SdrConfig = SDR.Configuration;

namespace Go2It
{
    public partial class AdminForm : Form
    {
        private const string AliPlugin = "DotSpatial.SDR.Plugins.ALI";

        // ali interface options
        private Dictionary<string, string> _aliInterfaces;
        private Font _networkFleetSymbolFont;
        private Font _networkFleetLabelFont;
        private Font _enterpolAvlSymbolFont;
        private Font _enterpolAvlLabelFont;

        // name of the initial map tab, if no map tabs currently exist
        private const string MapTabDefaultCaption = "My Map";

        // admin form controls
        private Legend _adminLegend;
        private readonly AppManager _appManager;
        private readonly DockingControl _dockingControl;
        private readonly ProjectManager _projectManager;

        // internal lookup names used by lucene to get feature from the dataset also stores ft shape (normalized)
        private const string Fid = "FID";
        private const string Lyrname = "LYRNAME";
        private const string Geoshape = "GEOSHAPE";

        // bool to prevent circular tab selection when adding a new map view tab
        private bool _isCreatingNewView;

        // store all avalable layers by type
        private readonly List<IFeatureSet> _pointLayers = new List<IFeatureSet>();
        private readonly List<IFeatureSet> _polygonLayers = new List<IFeatureSet>();
        private readonly List<IFeatureSet> _lineLayers = new List<IFeatureSet>();

        // switch handlers to control state of check/uncheck controls on admin form
        private readonly SelectionsHandler _pointLayerSwitcher = new SelectionsHandler();
        private readonly SelectionsHandler _lineLayerSwitcher = new SelectionsHandler();
        private readonly SelectionsHandler _polygonLayerSwitcher = new SelectionsHandler();

        // lookup of all layers available to this project
        private readonly Dictionary<string, IMapLayer> _layerLookup = new Dictionary<string, IMapLayer>();

        // row template for handling maptips as they are added and removed
        private DataGridViewRow _mapTipsDgvRowTemplate = new DataGridViewRow();

        // background worker handles the indexing process
        private readonly BackgroundWorker _idxWorker = new BackgroundWorker();
        private ProgressPanel _progressPanel;  // indicate progress of index worker

        // invalid file name chars array for validation
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();
        // numeric digits only regex
        private static readonly Regex DigitsOnly = new Regex(@"[^\d]");

        // queued indexes awaiting processing, and another to hold saved indexes, for copy to save dir on event
        // -> the inner most list stores all the rows (field maps)
        // -> the inner dict stores the type/lookups per row
        // -> the outer dict stores the layer name and another list with all the inner dicts
        private readonly Dictionary<string, List<Dictionary<string, string>>> _indexQueue = new Dictionary<string, List<Dictionary<string, string>>>();
        private readonly Dictionary<string, List<Dictionary<string, string>>> _savedIndexes = new Dictionary<string, List<Dictionary<string, string>>>();
        // track any indexes the user chooses to delete, for use on serializing event
        private readonly List<string> _deleteIndexes = new List<string>();

        // temp directory for storage of unsaved project files and temp storage of indexes
        private static string _tempIdxPath = string.Empty;
        private static string TempIndexDir
        {
            get { return _tempIdxPath; }
        }
        
        /// <summary>
        /// Assigns a temporary directory in the instance that the user is building a project but has not yet saved it
        /// </summary>
        private static void AssignTempIndexDir()
        {
            string unqTmpId = string.Format("{0}_{1}{2}", DateTime.Now.Date.ToString("yyyy-MM-dd"), DateTime.Now.Hour,DateTime.Now.Minute);
            // find or create a temp directory to hold any indexes generated before the OnSave Event is fired
            string tempDir = FindOrCreateTempDirectory(SdrConfig.Settings.Instance.ApplicationName + "\\" + unqTmpId);
            // validate we can write to temp access directory
            if (HasWriteAccessToFolder(tempDir))
            {
                _tempIdxPath = tempDir;
            }
        }

        private static bool HasWriteAccessToFolder(string folderPath)
        {
            try
            {
                // attempt to get a list of security permissions from the folder. 
                // this will raise an exception if the path is read only or does not have access to view the permissions.
                Directory.GetAccessControl(folderPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        public static string FindOrCreateTempDirectory(string appName)
        {
            string basePath = Path.GetTempPath();
            // check if this directory can be created             
            string theTempDir = Path.Combine(basePath, appName);
            CheckDirectory(theTempDir);
            return theTempDir;
        }

        private static void CheckDirectory(string directoryName)
        {
            if (Directory.Exists(directoryName)) return;
            try
            {
                Directory.CreateDirectory(directoryName);
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("Error creating directory " + directoryName + ". " + ex.Message);
            }
        }

        // index object for tracking and storing various layer/featureset index information
        internal class IndexObject
        {
            public IFeatureSet FeatureSet { get; private set; }
            public string LayerName { get; private set; }
            public ProjectionInfo LayerProjection { get; set; }
            public string IndexType { get; private set; }
            public List<KeyValuePair<string, string>> FieldLookup { get; private set; }
            public IndexObject(IFeatureSet featureSet, List<KeyValuePair<string, string>> fieldLookup, string layerName, ProjectionInfo projectionInfo, string indexType)
            {
                FeatureSet = featureSet;
                FieldLookup = fieldLookup;
                LayerName = layerName;
                LayerProjection = projectionInfo;
                IndexType = indexType;
            }
        }

        private void InitializeAliModesDict()
        {
            _aliInterfaces = SdrConfig.Plugins.GetPluginApplicationConfigSectionAsDict(AliPlugin, "AliInterfaceModes");
        }

        private void PopulateNetworkFleetLabels()
        {
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.NetworkfleetLabels.Count <= 0) return;
            foreach (var nfLabel in SdrConfig.Project.Go2ItProjectSettings.Instance.NetworkfleetLabels)
            {
                var arr = nfLabel.Split('=');
                object[] row = { arr[0], arr[1] };
                dgvNetworkfleetLabelLookup.Rows.Add(row);
            }
        }

        /// <summary>
        /// generate a DataGridViewRow that is a clone of the row passed in
        /// </summary>
        /// <param name="row">DataGridViewRow to clone</param>
        /// <returns>A New DataGridViewRow cloned from the input row</returns>
        public DataGridViewRow CloneDataGridViewRowTemplate(DataGridViewRow row)
        {
            var clonedRow = (DataGridViewRow)row.Clone();
            for (var i = 0; i < row.Cells.Count; i++)
            {
                if (clonedRow != null) clonedRow.Cells[i].Value = row.Cells[i].Value;
            }
            return clonedRow;
        }

        /// <summary>
        /// Create the basic maptips row template and populate basic information to the admin dgv for maptips
        /// </summary>
        private void InitializeMapTipsTemplate()
        {
            dgvMapTips.Rows.Clear();
            dgvMapTips.Columns.Clear();

            // generate the columns for the maptips datagridview
            var lyrNameCol = new DataGridViewComboBoxColumn
            {
                HeaderText = @"Layer Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                Tag = "LayerNameColumn"
            };
            dgvMapTips.Columns.Add(lyrNameCol);
            var fldNameCol = new DataGridViewComboBoxColumn
            {
                HeaderText = @"Field Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                Tag = "FieldNameColumn"
            };
            dgvMapTips.Columns.Add(fldNameCol);
            var addTipCol = new DataGridViewDisableButtonColumn
            {
                HeaderText = @"Add",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet,
                Width = 50,
                Tag = "AddButtonColumn"
            };
            dgvMapTips.Columns.Add(addTipCol);
            var delTipCol = new DataGridViewDisableButtonColumn
            {
                HeaderText = @"Delete",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet,
                Width = 50,
                Tag = "RemoveButtonColumn"
            };
            dgvMapTips.Columns.Add(delTipCol);

            // add a cloned row of our template to the datagridview
            dgvMapTips.Rows.Add(CloneDataGridViewRowTemplate(_mapTipsDgvRowTemplate));
            // generate the cells for the maptips datagridview row
            dgvMapTips.Rows[0].Cells[0] = new DataGridViewComboBoxCell();
            dgvMapTips.Rows[0].Cells[1] = new DataGridViewComboBoxCell();
            dgvMapTips.Rows[0].Cells[2] = new DataGridViewDisableButtonCell { Value = "+", Enabled = true };
            dgvMapTips.Rows[0].Cells[3] = new DataGridViewDisableButtonCell { Value = "-", Enabled = true };
            // clone the first row back to the template for later use
            _mapTipsDgvRowTemplate = CloneDataGridViewRowTemplate(dgvMapTips.Rows[0]);
            // layer combobox selected index changed event (ridiculous roundabout approach ms forced us into)
            dgvMapTips.EditingControlShowing += DgvMapTipsOnEditingControlShowing;
            // add/remove maptip click event handler
            dgvMapTips.CellContentClick += DgvMapTipsOnCellContentClick;
        }

        private void DgvMapTipsOnEditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (!(e.Control is ComboBox)) return;

            ((ComboBox)e.Control).SelectedIndexChanged -= OnSelectedIndexChanged; 
            ((ComboBox)e.Control).SelectedIndexChanged += OnSelectedIndexChanged;
        }

        private void OnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            var dgvec = (DataGridViewComboBoxEditingControl)sender;
            var colIdx = dgvec.EditingControlDataGridView.CurrentCell.ColumnIndex;

            if (colIdx == 0) // a layer combobox selection has changed
            {
                var lyr = dgvec.EditingControlFormattedValue;
                if (lyr.ToString().Length == 0) return;

                // open up the dataset and add all the data field columns the combobox for fields
                IMapLayer mapLyr;
                _layerLookup.TryGetValue(lyr.ToString(), out mapLyr);
                var mfl = mapLyr as IMapFeatureLayer;

                var fields = new List<string>();
                if (mfl != null && mfl.DataSet != null)
                {
                    IFeatureSet fl = mfl.DataSet;
                    fields = (from DataColumn dc in fl.DataTable.Columns select dc.ColumnName).ToList();
                }
                // populate the fields from the selected layer to the next column
                var dgv = dgvec.EditingControlDataGridView;
                var rowIdx = dgvec.EditingControlRowIndex;
                var cmb = (DataGridViewComboBoxCell) dgv.Rows[rowIdx].Cells[1];
                cmb.Items.Clear();
                cmb.Items.AddRange(fields.ToArray());
            }
        }

        private void FillMapTipFieldsComboBox(string lyr, DataGridViewComboBoxCell cmb)
        {
            if (lyr.Length == 0) return;

            // open up the dataset and add all the data field columns the combobox for fields
            IMapLayer mapLyr;
            _layerLookup.TryGetValue(lyr, out mapLyr);
            var mfl = mapLyr as IMapFeatureLayer;

            var fields = new List<string>();
            if (mfl != null && mfl.DataSet != null)
            {
                IFeatureSet fl = mfl.DataSet;
                fields = (from DataColumn dc in fl.DataTable.Columns select dc.ColumnName).ToList();
            }
            cmb.Items.Clear();
            cmb.Items.AddRange(fields.ToArray());
        }

        private void PopulateMapTipsToForm()
        {
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.MapTips.Count <= 0) return;
            for (int i = 0; i <= SdrConfig.Project.Go2ItProjectSettings.Instance.MapTips.Count - 1; i++)
            {
                var arr = SdrConfig.Project.Go2ItProjectSettings.Instance.MapTips[i].Split(',');
                dgvMapTips.Rows.Add(CloneDataGridViewRowTemplate(_mapTipsDgvRowTemplate));
                // generate the cells for the maptips datagridview row
                FillMapTipFieldsComboBox(arr[0], (DataGridViewComboBoxCell)dgvMapTips.Rows[i].Cells[1]);
                dgvMapTips.Rows[i].Cells[0].Value = arr[0];
                dgvMapTips.Rows[i].Cells[1].Value = arr[1];
            }
            // remove the last row if it's empty (result of the template load on start)
            if (dgvMapTips.Rows[dgvMapTips.RowCount - 1].Cells[0].Value == null || dgvMapTips.Rows[dgvMapTips.RowCount - 1].Cells[1].Value == null)
            {
                dgvMapTips.Rows.RemoveAt(dgvMapTips.RowCount - 1);
            }
        }

        /// <summary>
        /// Handle the "add" and "remove" maptips buttons on click event
        /// </summary>
        /// <param name="sender">MapTips DataGridView</param>
        /// <param name="e">ActiveCell DataGridViewCellEventArgs</param>
        private void DgvMapTipsOnCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var dgv = (DataGridView) sender;
            if (dgv.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 2)  // add maptip button clicked (add a new maptip row below this row)
                {
                    dgv.Rows.Insert(e.RowIndex + 1, CloneDataGridViewRowTemplate(_mapTipsDgvRowTemplate));
                }
                if (e.ColumnIndex == 3)  // remove maptip button clicked (remove this maptip row)
                {
                    if (dgv.RowCount >= 1)
                    {
                        dgv.Rows.RemoveAt(e.RowIndex);
                        if (dgv.RowCount == 0)  // the only row available was removed, need to insert a new blank one
                        {
                            dgv.Rows.Add(CloneDataGridViewRowTemplate(_mapTipsDgvRowTemplate));
                        }
                    }
                }
            }
        }

        public AdminForm(AppManager app)
        {
            InitializeComponent();
            Height = 815;
            InitializeSaveSplitButton();
            InitializeAliModesDict();
            HandleApplicationModeDiffs();
            InitializeMapTipsTemplate();

            // assign all the admin form elements
            _appManager = app;
            
            _projectManager = (ProjectManager)app.SerializationManager;
            _dockingControl = (DockingControl) app.DockManager;

            AssignTempIndexDir();  // temp directory for working with indexes before save event

            // set options on our indexing bgworker
            _idxWorker.WorkerReportsProgress = false;
            _idxWorker.WorkerSupportsCancellation = false;

            // splitter stuff
            adminLayerSplitter.SplitterWidth = 10;
            adminLayerSplitter.Paint += Splitter_Paint;

            // setup docking control events
            _appManager.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;
            _appManager.DockManager.PanelHidden += DockManagerOnPanelHidden;

            // check if there is a valid map loaded to the application
            if (_appManager.Map == null)
            {
                _projectManager.CreateNewProject();  // basically just resets all paths and panels

                // generate a default map for display purposes
                const string caption = MapTabDefaultCaption;
                var kname = new string(caption.Where(ch => !InvalidFileNameChars.Contains(ch)).ToArray());
                kname = kname.Replace(" ", "");
                var key = "kMap_" + kname;
                var nMap = _projectManager.CreateNewMap(key, mapBGColorPanel.BackColor);

                // create new dockable panel to hold the map
                var dp = new DockablePanel(key, caption, nMap, DockStyle.Fill);
                cmbActiveMapTab.Items.Add(dp.Caption);  // add this map to the map view selections combo
                cmbActiveMapTab.SelectedIndex = 0;

                _dockingControl.Add(dp);
                _dockingControl.SelectPanel(key);  // select the map now to activate plugin bindings
            }
            else
            {
                if (_appManager.Map.FunctionMode == FunctionMode.None)
                {
                    _appManager.Map.Cursor = Cursors.Arrow;
                }
                PopulateMapViews();  // add all the map views to the pull down menu
                // set the active map index on the pull down menu
                cmbActiveMapTab.SelectedIndex = cmbActiveMapTab.Items.IndexOf(SdrConfig.Project.Go2ItProjectSettings.Instance.ActiveMapViewCaption);
                _layerLookup = _projectManager.GetLayerLookup;  // get the layer lookup for type assignment
                AttachLegend((Map)_appManager.Map);  // assign the admin legend to the active map
                PopulateIndexesToForm();
            }

            PopulateSettingsToForm();
            PopulateUsersToForm();
            PopulateHotKeysToForm();
            PopulateGraphicsToForm();
            PopulateNetworkFleetLabels();
            PopulateMapTipsToForm();

            // watch for changes of index on the pull down map tab change
            cmbActiveMapTab.SelectedIndexChanged += CmbActiveMapTabOnSelectedIndexChanged;

            FormClosing += AdminForm_Closing; // check for isdirty changes to project file
            FormClosed += AdminFormClosed;

            _projectManager.Serializing += ProjectManagerOnSerializing;

            // setup a background worker for update progress bar on indexing tab
            _idxWorker.DoWork += idx_DoWork;
            _idxWorker.RunWorkerCompleted += idx_RunWorkerCompleted;

            _projectManager.IsDirty = false;

            SdrConfig.User.Go2ItUserSettings.Instance.AdminModeActive = true;
            _appManager.DockManager.HidePanel(SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel);
        }

        /// <summary>
        /// Modify the user interface at runtime depending on the current mode of the application
        /// </summary>
        private void HandleApplicationModeDiffs()
        {
            if (SdrConfig.Settings.Instance.ApplicationMode != SdrConfig.AppMode.Dispatch) return;
            // in dispatch mode these options have no function
            tblNetworkfleetAvlColors.ColumnStyles[2].Width = 0;
            tblEnterpolAvlSymbology.ColumnStyles[4].Width = 0;
            pnlAliEnterpolAvl.RowStyles[4].SizeType = SizeType.Absolute;
            pnlAliEnterpolAvl.RowStyles[4].Height = 0;
            pnlAliEnterpolAvl.RowStyles[5].SizeType = SizeType.Absolute;
            pnlAliEnterpolAvl.RowStyles[5].Height = 0;
            pnlAliEnterpolAvl.RowStyles[6].SizeType = SizeType.Absolute;
            pnlAliEnterpolAvl.RowStyles[6].Height = 0;
        }

        private void ProjectManagerOnSerializing(object sender, SerializingEventArgs serializingEventArgs)
        {
            // at this point all the projectManager settings have been saved
            // ie we have a path to move/delete any possible indexes created/deleted this session
            string projectName = Path.GetFileNameWithoutExtension(_projectManager.GetProjectShortName());
            string conn = SQLiteHelper.GetSQLiteConnectionString(_projectManager.CurrentProjectFile);
            // first check if there are any deleted indexes to handle
            if (_deleteIndexes.Count > 0)
            {
                foreach (string lyrName in _deleteIndexes)
                {
                    string idxType = GetLayerIndexTableType(lyrName);
                    // check if the table exists and drop it if so
                    if (SQLiteHelper.TableExists(conn, idxType + "_" + lyrName))
                    {
                        SQLiteHelper.DropTable(conn, idxType + "_" + lyrName);
                    }
                    // check if this index type exists in the temp dir
                    var tdir = Path.Combine(TempIndexDir, "_indexes", idxType);
                    if (Directory.Exists(tdir))
                    {
                        Directory.Delete(tdir, true);
                    }
                    // check if it exists in the project storage dir
                    var pdir = Path.Combine(_projectManager.CurrentProjectDirectory, projectName + "_indexes", idxType);
                    if (Directory.Exists(pdir))
                    {
                        Directory.Delete(pdir, true);
                        // remove the whole index dir if there are no indexes present
                        var intdir = Path.Combine(_projectManager.CurrentProjectDirectory, projectName + "_indexes");
                        var dirarr = Directory.GetDirectories(intdir);
                        if (dirarr.Length == 0)
                        {
                            Directory.Delete(intdir);
                        }
                    }
                }
            }
            // now save all new indexes created
            foreach (KeyValuePair<string, List<Dictionary<string, string>>> sKeyVal in _savedIndexes)
            {
                string lyrName = sKeyVal.Key;
                string idxType = GetLayerIndexTableType(lyrName);
                string tableName = idxType + "_" + lyrName;
                if (SQLiteHelper.TableExists(conn, tableName))
                {
                    SQLiteHelper.ClearTable(conn, tableName);
                }
                else
                {
                    var lookupDict = new Dictionary<string, string>
                    {
                        {"key", "INTEGER PRIMARY KEY"},
                        {"lookup", "TEXT"},
                        {"fieldname", "TEXT"}
                    };
                    SQLiteHelper.CreateTable(conn, tableName, lookupDict);
                }
                // iterate through all the field indexes
                List<Dictionary<string, string>> indexList = sKeyVal.Value;
                for (int i = 0; i <= indexList.Count - 1; i++)
                {
                    Dictionary<string, string> d = indexList[i];
                    SQLiteHelper.Insert(conn, idxType + "_" + lyrName, d);
                }
                // and finally be sure to copy all index directories over from the temp dir to the project save dir
                var src = Path.Combine(TempIndexDir, "_indexes", idxType);
                var dst = Path.Combine(_projectManager.CurrentProjectDirectory, projectName + "_indexes", idxType);
                // if the idxType is in the temp dir, move it now
                if (Directory.Exists(src))
                {
                    if (Directory.Exists(dst))
                    {
                        Directory.Delete(dst, true);
                    }
                    DirectoryCopy(src, dst, true);
                }
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory could not be located: "
                    + sourceDirName);
            }
            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }
            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }
            // If copying subdirectories, copy them and their contents to new location. 
            if (!copySubDirs) return;
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, true);
            }
        }

        private void InitializeSaveSplitButton()
        {
            // set the button up as splitter button for both "Save" and "Save As"
            btnSplitSave.ContextMenuStrip = new ContextMenuStrip();
            btnSplitSave.ContextMenuStrip.Items.Add("Save");
            btnSplitSave.ContextMenuStrip.Items.Add("Save As");
            btnSplitSave.AutoSize = true;
            btnSplitSave.ContextMenuStrip.ItemClicked += delegate(object sender, ToolStripItemClickedEventArgs args)
            {
                btnSplitSave.Text = args.ClickedItem.Text;
            };
        }

        private void CmbActiveMapTabOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            if (_isCreatingNewView) return;  // if creating a new tab view, no need to select again

            var txt = cmbActiveMapTab.SelectedItem.ToString();
            // clean the filename for a key value
            var fname = new string(txt.Where(ch => !InvalidFileNameChars.Contains(ch)).ToArray());
            fname = fname.Replace(" ", "");
            var key = "kMap_" + fname;
            if (cmbActiveMapTab.Items.Contains(txt))
            {
                _dockingControl.SelectPanel(key);
            }
        }

        /// <summary>
        /// Process any hotkeys that have been setup
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // update the hotkey datagridview as needed
            if (dgvHotKeys.Focused)
            {
                var selRow = dgvHotKeys.SelectedRows[0];
                var selKey = selRow.Cells[0];
                selKey.Value = keyData.ToString();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void AdminFormClosed(object sender, FormClosedEventArgs formClosedEventArgs)
        {
            // unbind all our events now
            adminLayerSplitter.Paint -= Splitter_Paint;
            _appManager.DockManager.ActivePanelChanged -= DockManager_ActivePanelChanged;
            _appManager.DockManager.PanelHidden -= DockManagerOnPanelHidden;
            _projectManager.Serializing -= ProjectManagerOnSerializing;
            _idxWorker.DoWork -= idx_DoWork;
            _idxWorker.RunWorkerCompleted -= idx_RunWorkerCompleted;
            FormClosing -= AdminForm_Closing;
            FormClosed -= AdminFormClosed;
            cmbActiveMapTab.SelectedIndexChanged -= CmbActiveMapTabOnSelectedIndexChanged;
            UnbindGraphicElementEvents();

            legendSplitter.Panel1.Controls.Remove(_adminLegend);
            SdrConfig.User.Go2ItUserSettings.Instance.AdminModeActive = false;

            _dockingControl.SelectPanel(SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel);
        }

        private void UnbindGraphicElementEvents()
        {
            lineSymbolStyle.SelectedIndexChanged -= LineSymbolStyleOnSelectedIndexChanged;
            lineSymbolCap.SelectedIndexChanged -= LineSymbolCapOnSelectedIndexChanged;
            lineSymbolSize.ValueChanged -= LineSymbolSizeOnValueChanged;
            lineSymbolColor.Click -= LineSymbolColorOnClick;
            lineSymbolColorSlider.ValueChanged -= LineSymbolColorSliderOnValueChanged;
            lineSymbolBorderColor.Click -= LineSymbolBorderColorOnClick;
            ptSymbolStyle.SelectedIndexChanged -= PtSymbolStyleOnSelectedIndexChanged;
            ptSymbolSize.ValueChanged -= PtSymbolSizeOnValueChanged;
            ptSymbolColor.Click -= PtSymbolColorOnClick;
            ptSymbolColorSlider.ValueChanged -= PtSymbolColorSliderOnValueChanged;
            txtAliEnterpolAVLPdChars.TextChanged -= CharGraphicCharsOnTextChanged;
            pnlAliEnterpolAVLPdColor.Click -= CharGraphicColorPanelOnClick;
            txtAliEnterpolAVLFdChars.TextChanged -= CharGraphicCharsOnTextChanged;
            pnlAliEnterpolAVLFdColor.Click -= CharGraphicColorPanelOnClick;
            txtAliEnterpolAVLEmsChars.TextChanged -= CharGraphicCharsOnTextChanged;
            pnlAliEnterpolAVLEmsColor.Click -= CharGraphicColorPanelOnClick;
            pnlAliEnterpolAVLMyVehicleColor.Click -= CharGraphicColorPanelOnClick;
            ptAliNetworkfleetColor.Click -= CharGraphicColorPanelOnClick;
            txtAliNetworkfleetChar.TextChanged -= CharGraphicCharsOnTextChanged;
            pnlAliNetworkfleetAVLMyVehicleColor.Click -= CharGraphicColorPanelOnClick;
            pnlAliAVLInactiveColor.Click -= CharGraphicColorPanelOnClick;
            pnlAliNetworkfleetAVLInactiveColor.Click -= CharGraphicColorPanelOnClick;
        }

        private void LayersOnLayerAdded(object sender, LayerEventArgs layerEventArgs)
        {
            var layer = (IMapLayer) layerEventArgs.Layer;
            if (layer == null) return;

            // add this layer to our '_layerLookup' Dictionary for easy access on indexing
            string fileName;
            if (layer.GetType().Name == "MapImageLayer")
            {
                var mImage = (IMapImageLayer) layer;
                if (String.IsNullOrEmpty(Path.GetFileNameWithoutExtension(mImage.Image.Filename))) return;
                fileName = Path.GetFileNameWithoutExtension(mImage.Image.Filename);
                if (fileName != null) _layerLookup.Add(fileName, mImage);
                _projectManager.IsDirty = true;
            }
            else
            {
                var mLayer = (IMapFeatureLayer)layer;
                if (String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((mLayer.DataSet.Filename)))) return;
                fileName = Path.GetFileNameWithoutExtension(mLayer.DataSet.Filename);
                if (fileName != null) _layerLookup.Add(fileName, mLayer);
                _projectManager.IsDirty = true;
            }
            if (layer.GetType().Name != "MapPointLayer" && layer.GetType().Name != "MapPolygonLayer" &&
                layer.GetType().Name != "MapLineLayer") return;
            
            var mMapLayer = (IMapFeatureLayer)layer;
            if (mMapLayer.DataSet.Filename == null) return;

            var fs = FeatureSet.Open(mMapLayer.DataSet.Filename);
            AddLayer(fs); // perform all form specific operations based on the add event
        }

        private void LayersOnLayerRemoved(object sender, LayerEventArgs layerEventArgs)
        {
            var layer = (IMapLayer)layerEventArgs.Layer;
            if (layer == null) return;

            // before full removal check that this layer doesnt exist on another map tab already
            foreach (KeyValuePair<string, DockPanelInfo> dockPanelInfo in _dockingControl.DockPanelLookup)
            {
                if (!dockPanelInfo.Key.Trim().StartsWith("kMap")) continue;
                var map = (Map)dockPanelInfo.Value.DotSpatialDockPanel.InnerControl;
                if (map.Layers.Contains(layer))
                {
                    return;
                }
            }
            // it does not appear to be on any other map tabs, remove it from the alllookup dictionary
            string fileName;
            if (layer.GetType().Name == "MapImageLayer")
            {
                var mImage = (IMapImageLayer)layer;
                if (String.IsNullOrEmpty(Path.GetFileNameWithoutExtension(mImage.Image.Filename))) return;
                fileName = Path.GetFileNameWithoutExtension(mImage.Image.Filename);
                if (fileName != null) _layerLookup.Remove(fileName);
                _projectManager.IsDirty = true;
            }
            else
            {
                var mLayer = (IMapFeatureLayer)layer;
                if (String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((mLayer.DataSet.Filename)))) return;
                fileName = Path.GetFileNameWithoutExtension(mLayer.DataSet.Filename);
                if (fileName != null) _layerLookup.Remove(fileName);
                _projectManager.IsDirty = true;

                // remove any layers that could be part of active config now
                if ((mLayer.DataSet.FeatureType.ToString() != "Point" &&
                     mLayer.DataSet.FeatureType.ToString() != "Line" &&
                     mLayer.DataSet.FeatureType.ToString() != "Polygon")) return;
                var fs = FeatureSet.Open(mLayer.DataSet.Filename);
                RemoveLayer(fs); // perform all form specific operations based on the layer removed event
            }
        }

        private static void Splitter_Paint(object sender, PaintEventArgs e)
        {
            PaintSplitterDots((SplitContainer) sender, e);
        }

        private static void PaintSplitterDots(SplitContainer sc, PaintEventArgs e)
        {
            var control = sc;
            // paint the three dots
            var points = new Point[3];
            var w = control.Width;
            var h = control.Height;
            var d = control.SplitterDistance;
            var sW = control.SplitterWidth;
            // calculate the position of the points
            if (control.Orientation == Orientation.Horizontal)
            {
                points[0] = new Point((w/2), d + (sW/2));
                points[1] = new Point(points[0].X - 10, points[0].Y);
                points[2] = new Point(points[0].X + 10, points[0].Y);
            }
            else
            {
                points[0] = new Point(d + (sW/2), (h/2));
                points[1] = new Point(points[0].X, points[0].Y - 10);
                points[2] = new Point(points[0].X, points[0].Y + 10);
            }
            foreach (Point p in points)
            {
                p.Offset(-2, -2);
                e.Graphics.FillEllipse(SystemBrushes.ControlDark,
                    new Rectangle(p, new Size(3, 3)));
                p.Offset(1, 1);
                e.Graphics.FillEllipse(SystemBrushes.ControlLight,
                    new Rectangle(p, new Size(3, 3)));
            }
        }

        private void DockManagerOnPanelHidden(object sender, DockablePanelEventArgs e)
        {
            var dockControl = (DockingControl) sender;
            var key = e.ActivePanelKey;
            DockPanelInfo dockInfo;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;
            if (dockInfo.DotSpatialDockPanel.Key.StartsWith("kMap_"))
            {
                var map = (Map)dockInfo.DotSpatialDockPanel.InnerControl;
                if (map == null) return;
                // unbind any map specific events
                map.Layers.LayerAdded -= LayersOnLayerAdded;
                map.Layers.LayerRemoved -= LayersOnLayerRemoved;
            }
        }

        private void DockManager_ActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            var dockControl = (DockingControl) sender;
            var key = e.ActivePanelKey;
            DockPanelInfo dockInfo;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;
            if (dockInfo.DotSpatialDockPanel.Key.StartsWith("kMap_"))
            {
                var caption = dockInfo.DotSpatialDockPanel.Caption;
                var map = (Map)dockInfo.DotSpatialDockPanel.InnerControl;
                if (map == null) return;

                map.Layers.LayerAdded += LayersOnLayerAdded;
                map.Layers.LayerRemoved += LayersOnLayerRemoved;

                _appManager.Map = map;  // assign as the active map of the application
                AttachLegend(map);
                
                var idx = cmbActiveMapTab.Items.IndexOf(caption);
                if (idx >= 0)
                {
                    cmbActiveMapTab.SelectedIndex = idx;
                }
            }
        }

        private void PopulateUsersToForm()
        {
            string conn = SdrConfig.Settings.Instance.ApplicationRepoConnectionString;
            const string query = "SELECT username FROM logins";
            DataTable table = SQLiteHelper.GetDataTable(conn, query);
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DataRow r = table.Rows[i];
                var username = r["username"].ToString();
                lstUsers.Items.Add(username);
            }
        }

        private void UpdateLineGraphics(Map map)
        {
            // parse out line shape styles
            LineCap lineCap;
            Enum.TryParse(lineSymbolCap.SelectedItem.ToString(), true, out lineCap);
            DashStyle lineStyle;
            Enum.TryParse(lineSymbolStyle.SelectedItem.ToString(), true, out lineStyle);
            var lLyr = map.MapFrame.DrawingLayers[0] as MapLineLayer;
            if (lLyr != null)
            {
                lLyr.Symbolizer = new LineSymbolizer(lineSymbolColor.BackColor,
                    lineSymbolBorderColor.BackColor, Convert.ToInt32(lineSymbolSize.Text),
                    lineStyle, lineCap);
            }
            map.BackColor = mapBGColorPanel.BackColor;
            map.MapFrame.Invalidate();
        }

        private void UpdatePointGraphics(Map map)
        {
            PointShape ptShape;  // parse out point shape style
            Enum.TryParse(ptSymbolStyle.SelectedItem.ToString(), true, out ptShape);
            var pLyr = map.MapFrame.DrawingLayers[0] as MapPointLayer;
            if (pLyr != null)
            {
                pLyr.Symbolizer = new PointSymbolizer(ptSymbolColor.BackColor,
                    ptShape, Convert.ToInt32(ptSymbolSize.Text));
            }
            map.BackColor = mapBGColorPanel.BackColor;
            map.MapFrame.Invalidate();
        }

        private void DrawPointGraphics()
        {
            Map ptMap;  // check for a map first
            if (ptSymbolGraphic.Controls.Count != 0)
            {
                ptMap = ptSymbolGraphic.Controls[0] as Map;
                UpdatePointGraphics(ptMap);
            } 
            else
            {
                ptMap = new Map
                {
                    ViewExtents = new Envelope(-130, -60, 10, 55).ToExtent(),
                    FunctionMode = FunctionMode.None,
                };
                ptMap.MapFunctions.Clear(); // clear all built in map functions (nav/zoom/etc)
                ptSymbolGraphic.Controls.Add(ptMap);

                var ftSet = new FeatureSet(FeatureType.Point);
                var ftLyr = new MapPointLayer(ftSet);
                ptMap.MapFrame.DrawingLayers.Add(ftLyr);

                // get the center of the control panel (location to render point)
                var y = ((ptSymbolGraphic.Bottom - ptSymbolGraphic.Top) / 2) - 1;
                var x = ((ptSymbolGraphic.Right - ptSymbolGraphic.Left) / 2) - 1;
                var c = ptMap.PixelToProj(new Point(x, y));
                ftSet.AddFeature(new DotSpatial.Topology.Point(c));
            }
            UpdatePointGraphics(ptMap);
        }

        private void DrawLineGraphics()
        {
            Map lineMap;  // check for a map first
            if (lineSymbolGraphic.Controls.Count != 0)
            {
                lineMap = lineSymbolGraphic.Controls[0] as Map;
            }
            else
            {
                lineMap = new Map
                {
                    ViewExtents = new Envelope(-130, -60, 10, 55).ToExtent(),
                    FunctionMode = FunctionMode.None
                };
                lineMap.MapFunctions.Clear(); // clear all built in map functions (nav/zoom/etc)
                lineSymbolGraphic.Controls.Add(lineMap);

                var ftSet = new FeatureSet(FeatureType.Line);
                var ftLyr = new MapLineLayer(ftSet);
                lineMap.MapFrame.DrawingLayers.Add(ftLyr);

                // create a new line geometry for the feature
                var coords = new List<Coordinate>();
                var geo = new LineString(coords);
                var lineFt = ftSet.AddFeature(geo);
                var sx = ((Convert.ToInt32(lineSymbolSize.Text) - 1) / 2 + 1) * -1;
                var sy = lineSymbolGraphic.Bottom - lineSymbolGraphic.Top;
                var sc = lineMap.PixelToProj(new Point(sx, sy));
                var ex = lineSymbolGraphic.Right - lineSymbolGraphic.Left;
                var ey = ((Convert.ToInt32(lineSymbolSize.Text) - 1) / 2 + 1) * -1;
                var ec = lineMap.PixelToProj(new Point(ex, ey));
                lineFt.Coordinates.Add(sc);
                lineFt.Coordinates.Add(ec);
            }
            UpdateLineGraphics(lineMap);
        }

        private void PopulateGraphicsToForm()
        {
            // networkfleet char graphics
            Color nfColor = SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetColor;
            ptAliNetworkfleetColor.BackColor = nfColor;
            ptAliNetworkfleetColor.Click += CharGraphicColorPanelOnClick;
            txtAliNetworkfleetChar.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetChar.ToString(CultureInfo.InvariantCulture);
            txtAliNetworkfleetChar.TextChanged += CharGraphicCharsOnTextChanged;
            _networkFleetSymbolFont = SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetFont;
            lblAliNetworkfleetFont.Text = _networkFleetSymbolFont.Name;
            lblAliNetworkfleetFont.Font = _networkFleetSymbolFont;
            txtAliNetworkfleetSize.Text = _networkFleetSymbolFont.Size.ToString(CultureInfo.InvariantCulture);
            Color avlNfInactive = SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetAvlInactiveColor;
            pnlAliNetworkfleetAVLInactiveColor.BackColor = avlNfInactive;
            pnlAliNetworkfleetAVLInactiveColor.Click += CharGraphicColorPanelOnClick;
            Color avlNfMyColor = SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetAvlMyColor;
            pnlAliNetworkfleetAVLMyVehicleColor.BackColor = avlNfMyColor;
            pnlAliNetworkfleetAVLMyVehicleColor.Click += CharGraphicColorPanelOnClick;
            _networkFleetLabelFont = SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetLabelFont;
            lblAliNetworkfleetLabelFont.Text = _networkFleetLabelFont.Name;
            lblAliNetworkfleetLabelFont.Font = _networkFleetLabelFont;
            txtAliNetworkfleetLabelFontSize.Text = _networkFleetLabelFont.Size.ToString(CultureInfo.InvariantCulture);
            cmbAliNetworkfleetLabelAlignment.SelectedIndex = cmbAliNetworkfleetLabelAlignment.Items.IndexOf(SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetLabelAlignment);
            // set after the alignment to replace any defaults set by the selection changed event on the combobox
            numAliNetworkfleetLabelXOffset.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetLabelXOffset;
            numAliNetworkfleetLabelYOffset.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetLabelYOffset;

            // enterpol avl char graphics
            Color avlInactive = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlInactiveColor;
            pnlAliAVLInactiveColor.BackColor = avlInactive;
            pnlAliAVLInactiveColor.Click += CharGraphicColorPanelOnClick;
            Color avlMyColor = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlMyColor;
            pnlAliEnterpolAVLMyVehicleColor.BackColor = avlMyColor;
            pnlAliEnterpolAVLMyVehicleColor.Click += CharGraphicColorPanelOnClick;
            Color avlEmsColor = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlEmsColor;
            pnlAliEnterpolAVLEmsColor.BackColor = avlEmsColor;
            pnlAliEnterpolAVLEmsColor.Click += CharGraphicColorPanelOnClick;
            txtAliEnterpolAVLEmsChars.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlEmsChar.ToString(CultureInfo.InvariantCulture);
            txtAliEnterpolAVLEmsChars.TextChanged += CharGraphicCharsOnTextChanged;
            Color avlFdColor = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlFdColor;
            pnlAliEnterpolAVLFdColor.BackColor = avlFdColor;
            pnlAliEnterpolAVLFdColor.Click += CharGraphicColorPanelOnClick;
            txtAliEnterpolAVLFdChars.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlFdChar.ToString(CultureInfo.InvariantCulture);
            txtAliEnterpolAVLFdChars.TextChanged += CharGraphicCharsOnTextChanged;
            Color avlLeColor = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLeColor;
            pnlAliEnterpolAVLPdColor.BackColor = avlLeColor;
            pnlAliEnterpolAVLPdColor.Click += CharGraphicColorPanelOnClick;
            txtAliEnterpolAVLPdChars.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLeChar.ToString(CultureInfo.InvariantCulture);
            txtAliEnterpolAVLPdChars.TextChanged += CharGraphicCharsOnTextChanged;
            _enterpolAvlSymbolFont = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlSymbolFont;
            lblAliEnterpolAVLSymbolFontName.Text = _enterpolAvlSymbolFont.Name;
            lblAliEnterpolAVLSymbolFontName.Font = _enterpolAvlSymbolFont;
            txtAliEnterpolAVLSymbolFontSize.Text = _enterpolAvlSymbolFont.Size.ToString(CultureInfo.InvariantCulture);
            _enterpolAvlLabelFont = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLabelFont;
            lblAliEnterpolAVLLabelFontName.Text = _enterpolAvlLabelFont.Name;
            lblAliEnterpolAVLLabelFontName.Font = _enterpolAvlLabelFont;
            txtAliEnterpolAVLLabelFontSize.Text = _enterpolAvlLabelFont.Size.ToString(CultureInfo.InvariantCulture);
            cmbAliEnterpolAVLLabelAlignment.SelectedIndex = cmbAliEnterpolAVLLabelAlignment.Items.IndexOf(SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLabelAlignment);
            // set after the alignment to replace any defaults set by the selection changed event on the combobox
            numAliEnterpolAVLLabelXOffset.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLabelXOffset;
            numAliEnterpolAVLLabelYOffset.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLabelYOffset;
            DrawAllCharGraphics(); // draw all char based graphics ie: networkfleet, fd_avl, ems_avl, and le_avl

            // point symbology for graphics rendering
            Color pColor = SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicPointColor;
            ptSymbolColorSlider.Value = pColor.GetOpacity();
            ptSymbolColorSlider.MaximumColor = Color.FromArgb(255, pColor.R, pColor.G, pColor.B);
            ptSymbolColorSlider.ValueChanged += PtSymbolColorSliderOnValueChanged;
            ptSymbolColor.BackColor = pColor;
            ptSymbolColor.Click += PtSymbolColorOnClick;
            ptSymbolSize.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicPointSize;
            ptSymbolSize.ValueChanged += PtSymbolSizeOnValueChanged;
            foreach (PointShape ptShape in Enum.GetValues(typeof(PointShape)))
            {
                if (ptShape.ToString().ToUpper() != "UNDEFINED")
                {
                    ptSymbolStyle.Items.Add(ptShape.ToString());
                }
            }
            var idx = ptSymbolStyle.Items.IndexOf(SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicPointStyle);
            ptSymbolStyle.SelectedIndex = idx;
            ptSymbolStyle.SelectedIndexChanged += PtSymbolStyleOnSelectedIndexChanged;
            DrawPointGraphics();

            // line symbology for graphics rendering
            lineSymbolBorderColor.BackColor = SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineBorderColor;
            lineSymbolBorderColor.Click += LineSymbolBorderColorOnClick;
            Color lColor = SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineColor;
            lineSymbolColorSlider.Value = lColor.GetOpacity();
            lineSymbolColorSlider.MaximumColor = Color.FromArgb(255, lColor.R, lColor.G, lColor.B);
            lineSymbolColorSlider.ValueChanged += LineSymbolColorSliderOnValueChanged;
            lineSymbolColor.BackColor = lColor;
            lineSymbolColor.Click += LineSymbolColorOnClick;
            lineSymbolSize.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineSize;
            lineSymbolSize.ValueChanged += LineSymbolSizeOnValueChanged;
            foreach (LineCap lineCap in Enum.GetValues(typeof(LineCap)))
            {
                if (lineCap.ToString().ToUpper() != "CUSTOM")
                {
                    lineSymbolCap.Items.Add(lineCap.ToString());
                }
            }
            idx = lineSymbolCap.Items.IndexOf(SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineCap);
            lineSymbolCap.SelectedIndex = idx;
            lineSymbolCap.SelectedIndexChanged += LineSymbolCapOnSelectedIndexChanged;
            foreach (DashStyle lineStyle in Enum.GetValues(typeof(DashStyle)))
            {
                if (lineStyle.ToString().ToUpper() != "CUSTOM")
                {
                    lineSymbolStyle.Items.Add(lineStyle.ToString());
                }
            }
            idx = lineSymbolStyle.Items.IndexOf(SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineStyle);
            lineSymbolStyle.SelectedIndex = idx;
            lineSymbolStyle.SelectedIndexChanged += LineSymbolStyleOnSelectedIndexChanged;
            DrawLineGraphics();
        }

        private void DrawAllCharGraphics()
        {
            DrawCharacterGraphic(ptAliNetworkfleetGraphic, ptAliNetworkfleetColor.BackColor, txtAliNetworkfleetChar.Text);
            DrawCharacterGraphic(pnlAliEnterpolAVLEmsGraphic, pnlAliEnterpolAVLEmsColor.BackColor, txtAliEnterpolAVLEmsChars.Text);
            DrawCharacterGraphic(pnlAliEnterpolAVLPdGraphic, pnlAliEnterpolAVLPdColor.BackColor, txtAliEnterpolAVLPdChars.Text);
            DrawCharacterGraphic(pnlAliEnterpolAVLFdGraphic, pnlAliEnterpolAVLFdColor.BackColor, txtAliEnterpolAVLFdChars.Text);
        }

        private void DrawCharacterGraphic(Panel panel, Color color, String chars)
        {
            Map ptMap;  // check for a map first
            if (panel.Controls.Count != 0)
            {
                ptMap = panel.Controls[0] as Map;
            }
            else
            {
                ptMap = new Map
                {
                    ViewExtents = new Envelope(-130, -60, 10, 55).ToExtent(),
                    FunctionMode = FunctionMode.None,
                };
                ptMap.MapFunctions.Clear(); // clear all built in map functions (nav/zoom/etc)
                panel.Controls.Add(ptMap);

                var ftSet = new FeatureSet(FeatureType.Point);
                var ftLyr = new MapPointLayer(ftSet);
                ptMap.MapFrame.DrawingLayers.Add(ftLyr);

                // get the center of the control panel (location to render point)
                var y = ((panel.Bottom - panel.Top) / 2) - 1;
                var x = ((panel.Right - panel.Left) / 2) - 1;
                var c = ptMap.PixelToProj(new Point(x, y));
                ftSet.AddFeature(new DotSpatial.Topology.Point(c));
            }
            UpdateCharacterGraphic(ptMap, panel == ptAliNetworkfleetGraphic ? _networkFleetSymbolFont : _enterpolAvlSymbolFont, color, chars);
        }

        private void UpdateCharacterGraphic(Map map, Font font, Color color, String chars)
        {
            var pLyr = map.MapFrame.DrawingLayers[0] as MapPointLayer;
            if (pLyr != null)
            {
                var c = char.Parse(chars);
                pLyr.Symbolizer = new PointSymbolizer(c, font.Name, color, font.Size);

            }
            map.BackColor = mapBGColorPanel.BackColor;
            map.MapFrame.Invalidate();
        }

        private void CharGraphicColorPanelOnClick(object sender, EventArgs eventArgs)
        {
            var pnl = (Panel)sender;
            var oColor = pnl.BackColor;
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;
            var nColor = Color.FromArgb(255, dlg.Color.R, dlg.Color.G, dlg.Color.B);
            if (oColor != nColor)
            {
                _projectManager.IsDirty = true;
            }
            if (pnl == pnlAliEnterpolAVLEmsColor)
            {
                pnlAliEnterpolAVLEmsColor.BackColor = nColor;
                DrawCharacterGraphic(pnlAliEnterpolAVLEmsGraphic, pnlAliEnterpolAVLEmsColor.BackColor, txtAliEnterpolAVLEmsChars.Text);
            } 
            else if (pnl == pnlAliEnterpolAVLFdColor)
            {
                pnlAliEnterpolAVLFdColor.BackColor = nColor;
                DrawCharacterGraphic(pnlAliEnterpolAVLFdGraphic, pnlAliEnterpolAVLFdColor.BackColor, txtAliEnterpolAVLFdChars.Text);
            }
            else if (pnl == pnlAliEnterpolAVLPdColor)
            {
                pnlAliEnterpolAVLPdColor.BackColor = nColor;
                DrawCharacterGraphic(pnlAliEnterpolAVLPdGraphic, pnlAliEnterpolAVLPdColor.BackColor, txtAliEnterpolAVLPdChars.Text);
            }
            else if (pnl == pnlAliEnterpolAVLMyVehicleColor)
            {
                pnlAliEnterpolAVLMyVehicleColor.BackColor = nColor;
                // no character to update on this color selection
            }
            else if (pnl == pnlAliAVLInactiveColor)
            {
                pnlAliAVLInactiveColor.BackColor = nColor;
                // no character to update on this color selection
            }
            else if (pnl == pnlAliNetworkfleetAVLMyVehicleColor)
            {
                pnlAliNetworkfleetAVLMyVehicleColor.BackColor = nColor;
                // no character to update on this color selection
            }
            else if (pnl == pnlAliNetworkfleetAVLInactiveColor)
            {
                pnlAliNetworkfleetAVLInactiveColor.BackColor = nColor;
                // no character to update on this color selection
            }
            else  // networkfleet panel backcolor
            {
                ptAliNetworkfleetColor.BackColor = nColor;
                DrawCharacterGraphic(ptAliNetworkfleetGraphic, ptAliNetworkfleetColor.BackColor, txtAliNetworkfleetChar.Text);
            }
        }

        private void CharGraphicCharsOnTextChanged(object sender, EventArgs eventArgs)
        {
            var txt = (TextBox) sender;
            string oText;
            if (txt == txtAliEnterpolAVLFdChars)
            {
                oText = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlFdChar.ToString(CultureInfo.InvariantCulture);
                DrawCharacterGraphic(pnlAliEnterpolAVLFdGraphic, pnlAliEnterpolAVLFdColor.BackColor, txtAliEnterpolAVLFdChars.Text);
            }
            else if (txt == txtAliEnterpolAVLPdChars)
            {
                oText = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLeChar.ToString(CultureInfo.InvariantCulture);
                DrawCharacterGraphic(pnlAliEnterpolAVLPdGraphic, pnlAliEnterpolAVLPdColor.BackColor, txtAliEnterpolAVLPdChars.Text);
            } 
            else if (txt == txtAliEnterpolAVLEmsChars)
            {
                oText = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlEmsChar.ToString(CultureInfo.InvariantCulture);
                DrawCharacterGraphic(pnlAliEnterpolAVLEmsGraphic, pnlAliEnterpolAVLEmsColor.BackColor, txtAliEnterpolAVLEmsChars.Text);
            }
            else  // networkfleet textbox char
            {
                oText = SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetChar.ToString(CultureInfo.InvariantCulture);
                DrawCharacterGraphic(ptAliNetworkfleetGraphic, ptAliNetworkfleetColor.BackColor, txtAliNetworkfleetChar.Text);
            }
            if (oText != txt.Text)
            {
                _projectManager.IsDirty = true;
            }
        }

        private void LineSymbolColorSliderOnValueChanged(object sender, EventArgs eventArgs)
        {
            int alpha = Convert.ToInt32(lineSymbolColorSlider.Value * 255);
            lineSymbolColor.BackColor = Color.FromArgb(alpha, lineSymbolColor.BackColor.R, lineSymbolColor.BackColor.G, lineSymbolColor.BackColor.B);
            DrawLineGraphics();
            _projectManager.IsDirty = true;
        }

        private void PtSymbolColorSliderOnValueChanged(object sender, EventArgs eventArgs)
        {
            int alpha = Convert.ToInt32(ptSymbolColorSlider.Value*255);
            ptSymbolColor.BackColor = Color.FromArgb(alpha, ptSymbolColor.BackColor.R, ptSymbolColor.BackColor.G, ptSymbolColor.BackColor.B);
            DrawPointGraphics();
            _projectManager.IsDirty = true;
        }

        private void LineSymbolCapOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            DrawLineGraphics();
            _projectManager.IsDirty = true;
        }

        private void LineSymbolStyleOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            DrawLineGraphics();
            _projectManager.IsDirty = true;
        }

        private void LineSymbolSizeOnValueChanged(object sender, EventArgs eventArgs)
        {
            DrawLineGraphics();
            _projectManager.IsDirty = true;
        }

        private void LineSymbolColorOnClick(object sender, EventArgs eventArgs)
        {
            var oColor = lineSymbolColor.BackColor;
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;

            // update the slider max color value for display
            lineSymbolColorSlider.MaximumColor = Color.FromArgb(255, dlg.Color.R, dlg.Color.G, dlg.Color.B);
            // update the color and map display with new color accounting for alpha
            int alpha = Convert.ToInt32(lineSymbolColorSlider.Value * 255);
            lineSymbolColor.BackColor = Color.FromArgb(alpha, dlg.Color.R, dlg.Color.G, dlg.Color.B);
            if (oColor != lineSymbolColor.BackColor)
            {
                _projectManager.IsDirty = true;
            }
            DrawLineGraphics();
        }

        private void LineSymbolBorderColorOnClick(object sender, EventArgs eventArgs)
        {
            var oColor = lineSymbolBorderColor.BackColor;
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;

            lineSymbolBorderColor.BackColor = dlg.Color;
            if (oColor != lineSymbolBorderColor.BackColor)
            {
                _projectManager.IsDirty = true;
            }
            DrawLineGraphics();
        }

        private void PtSymbolStyleOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            DrawPointGraphics();
            _projectManager.IsDirty = true;
        }

        private void PtSymbolSizeOnValueChanged(object sender, EventArgs eventArgs)
        {
            DrawPointGraphics();
            _projectManager.IsDirty = true;
        }

        private void PtSymbolColorOnClick(object sender, EventArgs eventArgs)
        {
            var oColor = ptSymbolColor.BackColor;
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;

            // update the slider max color value for display
            ptSymbolColorSlider.MaximumColor = Color.FromArgb(255, dlg.Color.R, dlg.Color.G, dlg.Color.B);
            // update the color and map display with new color accounting for alpha
            int alpha = Convert.ToInt32(ptSymbolColorSlider.Value * 255);
            ptSymbolColor.BackColor = Color.FromArgb(alpha, dlg.Color.R, dlg.Color.G, dlg.Color.B);
            if (oColor != ptSymbolColor.BackColor)
            {
                _projectManager.IsDirty = true;
            }
            DrawPointGraphics();
        }

        private void PopulateHotKeysToForm()
        {
            // create the columns on the datagridview
            dgvHotKeys.Rows.Clear();
            dgvHotKeys.Columns.Clear();

            var keyCol = new DataGridViewTextBoxColumn
            {
                HeaderText = @"Keys",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
            };
            dgvHotKeys.Columns.Add(keyCol);

            var desCol = new DataGridViewTextBoxColumn
            {
                HeaderText = @"Command",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true
            };
            dgvHotKeys.Columns.Add(desCol);

            Dictionary<HotKey, string> hotKeys = HotKeyManager.HotKeysDictionary();
            for (int i = 0; i < hotKeys.Count; i++)
            {
                var kvPair = hotKeys.ElementAt(i);
                HotKey hKey = kvPair.Key;
                var dgvRow = new DataGridViewRow();
                var txtKey = new DataGridViewTextBoxCell { Value = hKey.Key.ToString() };
                var txtAction = new DataGridViewTextBoxCell
                {
                    Value = hKey.Description,
                    Tag = kvPair.Value
                };
                dgvRow.Cells.Add(txtKey);
                dgvRow.Cells.Add(txtAction);
                dgvHotKeys.Rows.Add(dgvRow);
            }
        }

        private void PopulateSettingsToForm()
        {
            txtAliEnterpolDataSource.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolDataSource;
            txtAliEnterpolInitialCatalog.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolInitialCatalog;
            txtAliEnterpolTableName.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolTableName;

            txtAliGlobalCadLogPath.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadLogPath;
            txtAliGlobalCadArchivePath.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadArchivePath;
            txtAliGlobalCadConfigIni.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadConfigPath;

            txtAliInterfaceDbPath.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerDbPath;
            txtAliInterfaceUdpHost.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpHost;
            numAliInterfaceUdpPort.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpPort;

            txtAliNetworkfleetUdpHost.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetUdpHost;
            numAliNetworkfleetUdpPort.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetUdpPort;
            numAliNetworkfleetAVLAge1Freq.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetAvlAge1Freq;
            numAliNetworkfleetAVLAge2Freq.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetAvlAge2Freq;
            numAliNetworkfleetAVLAge3Freq.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetAvlAge3Freq;
            chkAutoHideInactiveUnitsNetworkfleet.Checked = SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetAvlAutoHideInactiveUnits;
            chkNetworkfleet.Checked = SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseNetworkfleet;
            chkEnterpolAvl.Checked = SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseEnterpolAvl;
            txtAliEnterpolAVLTableName.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlTableName;
            txtAliEnterpolAVLInitialCatalog.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlInitialCatalog;
            numAliEnterpolAVLReadFreq.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlReadFreq;
            txtAliEnterpolAVLSetLocProc.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlSetMyLocProc;
            numAliEnterpolAVLUpdateFreq.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlUpdateFreq;
            txtAliEnterpolAVLWhoAmIProc.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlWhoAmIProc;
            numAliEnterpolAVLAge1Freq.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlAge1Freq;
            numAliEnterpolAVLAge2Freq.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlAge2Freq;
            numAliEnterpolAVLAge3Freq.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlAge3Freq;
            chkAutoHideInactiveUnits.Checked = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlAutoHideInactiveUnits;

            // populate all the ali interfaces to the combobox
            foreach (var aliInterface in _aliInterfaces)
            {
                cmbAliMode.Items.Add(aliInterface.Value);
            }
            // use the alimode lookup dict to display a user friendly name
            var aliMode = SdrConfig.Project.Go2ItProjectSettings.Instance.AliMode;
            string aliOut;
            _aliInterfaces.TryGetValue(aliMode, out aliOut);
            cmbAliMode.SelectedIndex = cmbAliMode.Items.IndexOf(aliOut ?? "Disabled");  // if we fail then just disable the damn thing
            chkPretypes.Checked = SdrConfig.Project.Go2ItProjectSettings.Instance.SearchUsePretypes;
            chkEnableQueryParserLog.Checked = SdrConfig.Project.Go2ItProjectSettings.Instance.SearchQueryParserLogging;
            searchBufferDistance.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.SearchBufferDistance;
            searchHydrantCount.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.HydrantSearchCount;
            searchHydrantDistance.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.HydrantSearchDistance;
            searchZoomFactor.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.SearchZoomFactor;
            mapBGColorPanel.BackColor = SdrConfig.Project.Go2ItProjectSettings.Instance.MapBgColor;
            // set default settings on admin load
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.AddressesProjectType == "POINT")
            {
                radAddressPoints.Checked = true;
            }
            else
            {
                radAddressPolygons.Checked = true;
            }
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.KeyLocationsProjectType == "POINT")
            {
                radKeyLocationsPoints.Checked = true;
            }
            else
            {
                radKeyLocationsPolygons.Checked = true;
            }
            // populate all the layers from the lookup to their respective array types for form display
            foreach (KeyValuePair<string, IMapLayer> kvp in _layerLookup)
            {
                var ftLayer = (IMapFeatureLayer)kvp.Value;
                if (ftLayer.DataSet.Filename == null) continue;
                var fs = FeatureSet.Open(ftLayer.DataSet.Filename);
                AddLayer(fs); // perform all form specific add operations
            }
            SetupLayerSelectionSwitchers();
            SetActiveLayerSelections();
        }

        private void PopulateMapViews()
        {
            foreach (KeyValuePair<string, DockPanelInfo> dpi in _dockingControl.DockPanelLookup)
            {
                if (!dpi.Key.Trim().StartsWith("kMap")) continue;
                if (!cmbActiveMapTab.Items.Contains(dpi.Value.DotSpatialDockPanel.Caption))
                {
                    cmbActiveMapTab.Items.Add(dpi.Value.DotSpatialDockPanel.Caption);
                }
            }
        }

        private void SetActiveLayerSelections()
        {
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.AddressLayers != null)
            {
                foreach (string lyr in SdrConfig.Project.Go2ItProjectSettings.Instance.AddressLayers)
                {
                    chkAddressLayers.SetItemChecked(chkAddressLayers.Items.IndexOf(lyr), true);
                }
            }
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.RoadLayers != null)
            {
                foreach (string lyr in SdrConfig.Project.Go2ItProjectSettings.Instance.RoadLayers)
                {
                    chkRoadLayers.SetItemChecked(chkRoadLayers.Items.IndexOf(lyr), true);
                }
            }
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.KeyLocationLayers != null)
            {
                foreach (string t in SdrConfig.Project.Go2ItProjectSettings.Instance.KeyLocationLayers)
                {
                    chkKeyLocationsLayers.SetItemChecked(chkKeyLocationsLayers.Items.IndexOf(t), true);
                }
            }
            cmbNotesLayer.SelectedItem = SdrConfig.Project.Go2ItProjectSettings.Instance.NotesLayer;
            cmbCityLimitLayer.SelectedItem = SdrConfig.Project.Go2ItProjectSettings.Instance.CityLimitsLayer;
            cmbCellSectorLayer.SelectedItem = SdrConfig.Project.Go2ItProjectSettings.Instance.CellSectorsLayer;
            cmbESNLayer.SelectedItem = SdrConfig.Project.Go2ItProjectSettings.Instance.EsnsLayer;
            cmbParcelsLayer.SelectedItem = SdrConfig.Project.Go2ItProjectSettings.Instance.ParcelsLayer;
            cmbHydrantsLayer.SelectedItem = SdrConfig.Project.Go2ItProjectSettings.Instance.HydrantsLayer;
        }

        private void SetupLayerSelectionSwitchers()
        {
            _lineLayerSwitcher.Add(chkRoadLayers);
            _pointLayerSwitcher.Add(cmbNotesLayer);
            _pointLayerSwitcher.Add(cmbHydrantsLayer);
            if (radAddressPoints.Checked)
            {
                _pointLayerSwitcher.Add(chkAddressLayers);
            }
            if (radKeyLocationsPoints.Checked)
            {
                _pointLayerSwitcher.Add(chkKeyLocationsLayers);
            }
            _polygonLayerSwitcher.Add(cmbCityLimitLayer);
            _polygonLayerSwitcher.Add(cmbCellSectorLayer);
            _polygonLayerSwitcher.Add(cmbESNLayer);
            _polygonLayerSwitcher.Add(cmbParcelsLayer);
            if (radAddressPolygons.Checked)
            {
                _polygonLayerSwitcher.Add(chkAddressLayers);
            }
            if (radKeyLocationsPolygons.Checked)
            {
                _polygonLayerSwitcher.Add(chkKeyLocationsLayers);
            }
        }

        private void AddLayerToMapTipsComboBox(string lyr)
        {
            // check if the row template layer combo box contains this layer
            var cmb = (DataGridViewComboBoxCell)_mapTipsDgvRowTemplate.Cells[0];
            if (!cmb.Items.Contains(lyr))
            {
                cmb.Items.Add(lyr);
                foreach (DataGridViewRow row in dgvMapTips.Rows)
                {
                    cmb = (DataGridViewComboBoxCell)row.Cells[0];
                    cmb.Items.Add(lyr);
                }
            }
        }

        private void RemoveLayerFromMapTipsComboBox(string lyr)
        {
            // make sure the row template layer combo box contains the layer
            var cmb = (DataGridViewComboBoxCell)_mapTipsDgvRowTemplate.Cells[0];
            if (cmb.Items.Contains(lyr))
            {
                cmb.Items.Remove(lyr);
                var remove = new List<int>();
                foreach (DataGridViewRow row in dgvMapTips.Rows)
                {
                    cmb = (DataGridViewComboBoxCell)row.Cells[0];
                    if (cmb.Value.ToString() == lyr)
                    {
                        remove.Add(row.Index);  // store this index for complete removal after the loop completes
                    }
                    else
                    {
                        cmb.Items.Remove(lyr);
                    }
                }
                // remove any rows that had the removed layer set as their active layer
                if (remove.Count > 0)
                {
                    foreach (var i in remove)
                    {
                        dgvMapTips.Rows.RemoveAt(i);
                    }
                    if (dgvMapTips.RowCount == 0)  // the only row available was removed, need to insert a new blank one
                    {
                        dgvMapTips.Rows.Add(CloneDataGridViewRowTemplate(_mapTipsDgvRowTemplate));
                    }
                }
            }
        }

        /// <summary>
        /// Handle all form elements when a new layer is added to the project
        /// </summary>
        /// <param name="mapLayer">The layer added</param>
        private void AddLayer(IFeatureSet mapLayer)
        {
            if (String.IsNullOrEmpty(mapLayer.Filename)) return;
            var f = Path.GetFileNameWithoutExtension(mapLayer.Filename);
            if (f == null) return;

            if (mapLayer.FeatureType.Equals(FeatureType.Line))
            {
                _lineLayers.Add(mapLayer); // add to line layer list
                chkRoadLayers.Items.Add(f);
                AddLayerToMapTipsComboBox(f);
            }
            if (mapLayer.FeatureType.Equals(FeatureType.Point))
            {
                _pointLayers.Add(mapLayer); // add to point layer list
                if (cmbNotesLayer.Items.Count == 0)
                {
                    cmbNotesLayer.Items.Add(string.Empty);
                }
                cmbNotesLayer.Items.Add(f);
                if (cmbHydrantsLayer.Items.Count == 0)
                {
                    cmbHydrantsLayer.Items.Add(string.Empty);
                }
                cmbHydrantsLayer.Items.Add(f);
                if (radAddressPoints.Checked)
                {
                    chkAddressLayers.Items.Add(f);
                }
                if (radKeyLocationsPoints.Checked)
                {
                    chkKeyLocationsLayers.Items.Add(f);
                }
                AddLayerToMapTipsComboBox(f);
            }
            if (mapLayer.FeatureType.Equals(FeatureType.Polygon))
            {
                _polygonLayers.Add(mapLayer); // add to polygon layer list
                if (cmbCityLimitLayer.Items.Count == 0)
                {
                    cmbCityLimitLayer.Items.Add(string.Empty);
                }
                cmbCityLimitLayer.Items.Add(f);
                if (cmbCellSectorLayer.Items.Count == 0)
                {
                    cmbCellSectorLayer.Items.Add(string.Empty);
                }
                cmbCellSectorLayer.Items.Add(f);
                if (cmbESNLayer.Items.Count == 0)
                {
                    cmbESNLayer.Items.Add(string.Empty);
                }
                cmbESNLayer.Items.Add(f);
                if (cmbParcelsLayer.Items.Count == 0)
                {
                    cmbParcelsLayer.Items.Add(string.Empty);
                }
                cmbParcelsLayer.Items.Add(f);
                if (radAddressPolygons.Checked)
                {
                    chkAddressLayers.Items.Add(f);
                }
                if (radKeyLocationsPolygons.Checked)
                {
                    chkKeyLocationsLayers.Items.Add(f);
                }
                AddLayerToMapTipsComboBox(f);
            }
        }

        /// <summary>
        /// Handle all form elements when a layer is removed
        /// </summary>
        /// <param name="mapLayer">layer removed</param>
        private void RemoveLayer(IFeatureSet mapLayer)
        {
            if (String.IsNullOrEmpty(mapLayer.Filename)) return;
            var f = Path.GetFileNameWithoutExtension(mapLayer.Filename);
            if (f == null) return;

            if (mapLayer.FeatureType.Equals(FeatureType.Line))
            {
                _lineLayers.Remove(mapLayer); // remove from layer list
                chkRoadLayers.Items.Remove(f);
                RemoveLayerFromMapTipsComboBox(f);
            }
            if (mapLayer.FeatureType.Equals(FeatureType.Point))
            {
                _pointLayers.Remove(mapLayer); // remove from point layer list
                if (cmbNotesLayer.Text == f)
                {
                    cmbNotesLayer.Text = string.Empty;
                }
                cmbNotesLayer.Items.Remove(f);
                if (cmbHydrantsLayer.Text == f)
                {
                    cmbHydrantsLayer.Text = string.Empty;
                }
                cmbHydrantsLayer.Items.Remove(f);
                if (radAddressPoints.Checked)
                {
                    chkAddressLayers.Items.Remove(f);
                }
                if (radKeyLocationsPoints.Checked)
                {
                    chkKeyLocationsLayers.Items.Remove(f);
                }
                RemoveLayerFromMapTipsComboBox(f);
            }
            if (mapLayer.FeatureType.Equals(FeatureType.Polygon))
            {
                _polygonLayers.Remove(mapLayer); // remove from polygon list
                if (cmbCityLimitLayer.Text == f)
                {
                    cmbCityLimitLayer.Text = string.Empty;
                }
                cmbCityLimitLayer.Items.Remove(f);
                if (cmbCellSectorLayer.Text == f)
                {
                    cmbCellSectorLayer.Text = string.Empty;
                }
                cmbCellSectorLayer.Items.Remove(f);
                if (cmbESNLayer.Text == f)
                {
                    cmbESNLayer.Text = string.Empty;
                }
                cmbESNLayer.Items.Remove(f);
                if (cmbParcelsLayer.Text == f)
                {
                    cmbParcelsLayer.Text = string.Empty;
                }
                cmbParcelsLayer.Items.Remove(f);
                if (radAddressPolygons.Checked)
                {
                    chkAddressLayers.Items.Remove(f);
                }
                if (radKeyLocationsPolygons.Checked)
                {
                    chkKeyLocationsLayers.Items.Remove(f);
                }
                RemoveLayerFromMapTipsComboBox(f);
            }
        }

        private bool ShowSaveProjectDialog()
        {
            using (
                var dlg = new SaveFileDialog
                {
                    Filter = _appManager.SerializationManager.SaveDialogFilterText,
                    SupportMultiDottedExtensions = true
                })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    return SaveProject(dlg.FileName);
                }
                return false;
            }
        }

        /// <summary>
        /// Used when a user has chosen not to load or save a map and closes the admin panel
        /// </summary>
        private void SetNoProject()
        {
            _dockingControl.ResetLayout();  // remove all maptabs now
            SdrConfig.Project.Go2ItProjectSettings.Instance.ResetProjectSettingsToDefaults();  // set all project settings to defaults
            // SdrConfig.Settings.Instance.ProjectRepoConnectionString = null;  // clear any repo connection string available
            _appManager.Map = null;  // remove the appmanager map
            Cursor = Cursors.Default;  
        }

        private void AdminForm_Closing(object sender, FormClosingEventArgs e)
        {
            if (_idxWorker.IsBusy)
            {
                e.Cancel = true;
            }
            else
            {
                // check if this project file has ever been saved
                if (String.IsNullOrEmpty(_projectManager.CurrentProjectFile))
                {
                    // this project file has never been saved before lets ask the user what they want to do?
                    var res = MessageBox.Show(string.Format("Save current project before exiting?"),
                        Resources.AppName,
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button3);
                    switch (res)
                    {
                        case DialogResult.Cancel:
                            e.Cancel = true; // cancel the closing of the admin form
                            break;
                        case DialogResult.No:
                            e.Cancel = false;  // allow this dialog to continue closing
                            SetNoProject();
                            break;
                        case DialogResult.Yes:
                            e.Cancel = false;  // continue to allow the form to close
                            if (!ShowSaveProjectDialog())
                            {
                                SetNoProject();  // user decided not to save, reset main app map back to defaults
                            } // else the user did a proper save and all things should be synced at this point
                            break;
                    }
                }
                else // project file exists, check for any changes the user made and account for them
                {
                    if (_projectManager.IsDirty)
                    {
                        // user has made changes lets see if they want to save them
                        var res =
                            MessageBox.Show(string.Format("Save changes to current project [{0}]?", Path.GetFileName(_projectManager.CurrentProjectFile)),
                                Resources.AppName,
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button3);
                        switch (res)
                        {
                            case DialogResult.Cancel:
                                e.Cancel = true; // cancel the closing of the admin form
                                break;
                            case DialogResult.No:
                                e.Cancel = false;  // allow form to finish closing
                                // user did not save the new changes, so reload the original project file now
                                _projectManager.OpenProject(_projectManager.CurrentProjectFile);
                                break;
                            case DialogResult.Yes:
                                e.Cancel = false; // allow form to finish closing
                                if (!SaveProject(_projectManager.CurrentProjectFile))
                                {
                                    // user canceled the save, so reload the original project file now
                                    _projectManager.OpenProject(_projectManager.CurrentProjectFile);
                                } // else the save was successful and thus everything is in sync now
                                break;
                        }
                    }
                    else // no changes have been made, finish closing the form
                    {
                        e.Cancel = false;
                    }
                }
            }
        }

        private void AttachLegend(Map map)
        {
            if (_adminLegend == null)
            {
                _adminLegend = new Legend
                {
                    BackColor = Color.White,
                    ControlRectangle = new Rectangle(0, 0, 176, 128),
                    DocumentRectangle = new Rectangle(0, 0, 34, 114),
                    HorizontalScrollEnabled = true,
                    Indentation = 20,
                    IsInitialized = false,
                    Location = new Point(217, 12),
                    MinimumSize = new Size(5, 5),
                    Name = "adminLegend",
                    ProgressHandler = null,
                    ResetOnResize = false,
                    SelectionFontColor = Color.Black,
                    SelectionHighlight = Color.FromArgb(215, 238, 252),
                    Size = new Size(176, 128),
                    TabIndex = 0,
                    Text = @"Legend",
                    Dock = DockStyle.Fill,
                    VerticalScrollEnabled = true
                };
                legendSplitter.Panel1.Controls.Add(_adminLegend);
            }
            else
            {
                _adminLegend.RootNodes.Clear();
            }
            // set the active map for the admin legend now
            if (map == null) return;  // verify the map even exists first tho
            map.Legend = _adminLegend;
            _appManager.Legend = _adminLegend;
        }

        private void btnAddLayer_Click(object sender, EventArgs e)
        {
            // add layers to the currently active map
            _appManager.Map.AddLayers();
        }

        private void btnRemoveLayer_Click(object sender, EventArgs e)
        {
            var layer = _appManager.Map.Layers.SelectedLayer;
            if (layer != null)
            {
                _appManager.Map.Layers.Remove(layer);
            }
        }

        private void btnSplitSave_Click(object sender, EventArgs e)
        {
            if (_idxWorker.IsBusy != true)
                if (btnSplitSave.Text == @"Save")
                {
                    bool result = String.IsNullOrEmpty(_projectManager.CurrentProjectFile) ? SaveProjectAs() : SaveProject(_projectManager.CurrentProjectFile);
                    if (result)
                    {
                        Close();
                    }
                }
                else
                {
                    if (SaveProjectAs())
                    {
                        Close();
                    }
                }
            else
            {
                MessageBox.Show(@"Layer Indexing Operation is running, please wait or cancel to continue");
            }
        }

        // TODO: readd the validation back in
        private string VerifyRequiredSettings()
        {
            if (chkAddressLayers.CheckedItems.Count == 0)
            {
                return "Address layer is missing and required";
            }
            return chkRoadLayers.CheckedItems.Count == 0 ? "Road layer is missing and required" : string.Empty;
        }

        private static string SetPointOrPolygonType(RadioButton point)
        {
            return point.Checked ? "POINT" : "POLYGON";
        }

        private void ApplyProjectSettings()
        {
            // setup all project level type lookups
            SdrConfig.Project.Go2ItProjectSettings.Instance.AddressesProjectType = SetPointOrPolygonType(radAddressPoints);
            SdrConfig.Project.Go2ItProjectSettings.Instance.KeyLocationsProjectType = SetPointOrPolygonType(radKeyLocationsPoints);
            // setup layers inside checkboxes
            SdrConfig.Project.Go2ItProjectSettings.Instance.AddressLayers = ApplyCheckBoxSetting(chkAddressLayers);
            SdrConfig.Project.Go2ItProjectSettings.Instance.RoadLayers = ApplyCheckBoxSetting(chkRoadLayers);
            SdrConfig.Project.Go2ItProjectSettings.Instance.KeyLocationLayers = ApplyCheckBoxSetting(chkKeyLocationsLayers);
            // setup layers inside combo boxes
            SdrConfig.Project.Go2ItProjectSettings.Instance.NotesLayer = ApplyComboBoxSetting(cmbNotesLayer);
            SdrConfig.Project.Go2ItProjectSettings.Instance.CityLimitsLayer = ApplyComboBoxSetting(cmbCityLimitLayer);
            SdrConfig.Project.Go2ItProjectSettings.Instance.CellSectorsLayer = ApplyComboBoxSetting(cmbCellSectorLayer);
            SdrConfig.Project.Go2ItProjectSettings.Instance.EsnsLayer = ApplyComboBoxSetting(cmbESNLayer);
            SdrConfig.Project.Go2ItProjectSettings.Instance.ParcelsLayer = ApplyComboBoxSetting(cmbParcelsLayer);
            SdrConfig.Project.Go2ItProjectSettings.Instance.HydrantsLayer = ApplyComboBoxSetting(cmbHydrantsLayer);
            // set the map background color
            SdrConfig.Project.Go2ItProjectSettings.Instance.MapBgColor = mapBGColorPanel.BackColor;
            SdrConfig.Project.Go2ItProjectSettings.Instance.SearchUsePretypes = chkPretypes.Checked;
            SdrConfig.Project.Go2ItProjectSettings.Instance.SearchQueryParserLogging = chkEnableQueryParserLog.Checked;
            SdrConfig.Project.Go2ItProjectSettings.Instance.HydrantSearchCount = (int)searchHydrantCount.Value;
            SdrConfig.Project.Go2ItProjectSettings.Instance.HydrantSearchDistance = (int) searchHydrantDistance.Value;
            SdrConfig.Project.Go2ItProjectSettings.Instance.SearchBufferDistance = (int)searchBufferDistance.Value;
            SdrConfig.Project.Go2ItProjectSettings.Instance.SearchZoomFactor = searchZoomFactor.Value;
            // set the various graphic symbolization
            SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicPointColor = ptSymbolColor.BackColor;
            SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicPointStyle = ApplyComboBoxSetting(ptSymbolStyle);
            SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicPointSize = Convert.ToInt32(ptSymbolSize.Text);
            SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineBorderColor = lineSymbolBorderColor.BackColor;
            SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineColor = lineSymbolColor.BackColor;
            SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineSize = Convert.ToInt32(lineSymbolSize.Text);
            SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineStyle = ApplyComboBoxSetting(lineSymbolStyle);
            SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineCap = ApplyComboBoxSetting(lineSymbolCap);
            // set networkfleet options
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseNetworkfleet = chkNetworkfleet.Checked;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetUdpHost = txtAliNetworkfleetUdpHost.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetUdpPort = (int)numAliNetworkfleetUdpPort.Value;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetFont = _networkFleetSymbolFont;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetChar = char.Parse(txtAliNetworkfleetChar.Text.ToString(CultureInfo.InvariantCulture));
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetColor = ptAliNetworkfleetColor.BackColor;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetLabelFont = _networkFleetLabelFont;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetLabelAlignment = cmbAliNetworkfleetLabelAlignment.SelectedItem.ToString();
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetLabelXOffset = (int)numAliNetworkfleetLabelXOffset.Value;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetLabelYOffset = (int)numAliNetworkfleetLabelYOffset.Value;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetAvlAge1Freq = (int)numAliNetworkfleetAVLAge1Freq.Value;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetAvlAge2Freq = (int)numAliNetworkfleetAVLAge2Freq.Value;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetAvlAge3Freq = (int)numAliNetworkfleetAVLAge3Freq.Value;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetAvlAutoHideInactiveUnits = chkAutoHideInactiveUnitsNetworkfleet.Checked;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetAvlMyColor = pnlAliNetworkfleetAVLMyVehicleColor.BackColor;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetAvlInactiveColor = pnlAliNetworkfleetAVLInactiveColor.BackColor;
            // convert networkfleet labels back to list for storage
            foreach (DataGridViewRow row in dgvNetworkfleetLabelLookup.Rows)
            {
                if (row.Cells[0].Value == null || row.Cells[1].Value == null) continue;
                var record = row.Cells[0].Value + "=" + row.Cells[1].Value;
                if (record.Length > 1)
                {
                    SdrConfig.Project.Go2ItProjectSettings.Instance.NetworkfleetLabels.Add(record);
                }
            }
            // convert maptips lookup to list for storage to sqlite row
            foreach (DataGridViewRow row in dgvMapTips.Rows)
            {
                if (row.Cells[0].Value == null || row.Cells[1].Value == null) continue;
                var record = row.Cells[0].Value + "," + row.Cells[1].Value;
                if (record.Length > 1)
                {
                    SdrConfig.Project.Go2ItProjectSettings.Instance.MapTips.Add(record);
                }
            }
            // setup ali interface configuration
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolInitialCatalog = txtAliEnterpolInitialCatalog.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolTableName = txtAliEnterpolTableName.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolDataSource = txtAliEnterpolDataSource.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadLogPath = txtAliGlobalCadLogPath.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadArchivePath = txtAliGlobalCadArchivePath.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadConfigPath = txtAliGlobalCadConfigIni.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerDbPath = txtAliInterfaceDbPath.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpHost = txtAliInterfaceUdpHost.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpPort = Convert.ToInt32(numAliInterfaceUdpPort.Value);
            // set ali enterpol avl options
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseEnterpolAvl = chkEnterpolAvl.Checked;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlTableName = txtAliEnterpolAVLTableName.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlInitialCatalog = txtAliEnterpolAVLInitialCatalog.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlReadFreq = (int)numAliEnterpolAVLReadFreq.Value;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlSetMyLocProc = txtAliEnterpolAVLSetLocProc.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlUpdateFreq = (int)numAliEnterpolAVLUpdateFreq.Value;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlWhoAmIProc = txtAliEnterpolAVLWhoAmIProc.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlAge1Freq = (int)numAliEnterpolAVLAge1Freq.Value;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlAge2Freq = (int)numAliEnterpolAVLAge2Freq.Value;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlAge3Freq = (int)numAliEnterpolAVLAge3Freq.Value;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlAutoHideInactiveUnits = chkAutoHideInactiveUnits.Checked;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlSymbolFont = _enterpolAvlSymbolFont;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLabelFont = _enterpolAvlLabelFont;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLabelAlignment = cmbAliEnterpolAVLLabelAlignment.SelectedItem.ToString();
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLabelXOffset = (int)numAliEnterpolAVLLabelXOffset.Value;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLabelYOffset = (int)numAliEnterpolAVLLabelYOffset.Value;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlFdChar = char.Parse(txtAliEnterpolAVLFdChars.Text.ToString(CultureInfo.InvariantCulture));
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlFdColor = pnlAliEnterpolAVLFdColor.BackColor;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLeChar = char.Parse(txtAliEnterpolAVLPdChars.Text.ToString(CultureInfo.InvariantCulture));
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLeColor = pnlAliEnterpolAVLPdColor.BackColor;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlEmsChar = char.Parse(txtAliEnterpolAVLEmsChars.Text.ToString(CultureInfo.InvariantCulture));
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlEmsColor = pnlAliEnterpolAVLEmsColor.BackColor;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlMyColor = pnlAliEnterpolAVLMyVehicleColor.BackColor;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlInactiveColor = pnlAliAVLInactiveColor.BackColor;

            string aliValue;  // swap key and value positions to use the friendly label as a lookup key for the enum string value
            var swapDict = _aliInterfaces.ToDictionary(e => e.Value, e => e.Key);
            swapDict.TryGetValue(cmbAliMode.SelectedItem.ToString(), out aliValue);

            // must be last ali value set - it fires off a change event
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliMode = aliValue;
        }

        private static StringCollection ApplyCheckBoxSetting(CheckedListBox chk)
        {
            var strCollection = new StringCollection();
            foreach (string item in chk.CheckedItems)
            {
                strCollection.Add(item);
            }
            return strCollection.Count > 0 ? strCollection : null;
        }

        private static string ApplyComboBoxSetting(ComboBox cmb)
        {
            if (cmb.SelectedItem == null) return string.Empty;
            return cmb.SelectedItem.ToString().Length > 0 ? cmb.SelectedItem.ToString() : string.Empty;
        }

        private bool SaveProject(string fileName)
        {
            try
            {
                // TODO: add validation back in, perhaps improve them as well
                // TODO: also validate the ali interface before closing as well
                // validate all required fields are set
                /*var msg = VerifyRequiredSettings();
                if (msg.Length > 0)
                {
                    ShowSaveSettingsError(msg);
                    return false;
                }*/
                // this is saved to dbase by project manager on serialization event, which is fired just below
                ApplyProjectSettings();

                _projectManager.SaveProject(fileName);
                return true;
            }
            catch (XmlException)
            {
                ShowSaveAsError(fileName);
            }
            catch (IOException)
            {
                ShowSaveAsError(fileName);
            }
            return false;
        }

        private static void ShowSaveSettingsError(string message)
        {
            MessageBox.Show(String.Format(message), Resources.CouldNotWriteMapFile, MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static void ShowSaveAsError(string fileName)
        {
            MessageBox.Show(String.Format(Resources.CouldNotWriteMapFile, fileName), Resources.CouldNotWriteMapFile,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool SaveProjectAs()
        {
            using (
                var dlg = new SaveFileDialog
                {
                    Filter = _projectManager.SaveDialogFilterText,
                    SupportMultiDottedExtensions = true
                })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    return SaveProject(dlg.FileName);
                }
                return false;
            }
        }

        private bool VerifyPolygonLayerNotSelected(string fileName, CheckedListBox checkBox)
        {
            if (cmbCityLimitLayer.SelectedItem != null && cmbCityLimitLayer.SelectedItem.ToString() == fileName) return false;
            if (cmbCellSectorLayer.SelectedItem != null && cmbCellSectorLayer.SelectedItem.ToString() == fileName) return false;
            if (cmbESNLayer.SelectedItem != null && cmbESNLayer.SelectedItem.ToString() == fileName) return false;
            if (cmbParcelsLayer.SelectedItem != null && cmbParcelsLayer.SelectedItem.ToString() == fileName) return false;
            if (radAddressPolygons.Checked)
            {
                if (checkBox != chkAddressLayers)
                {
                    if (
                        chkAddressLayers.CheckedItems.Cast<object>()
                            .Any(checkedItem => checkedItem.ToString() == fileName))
                    {
                        return false;
                    }
                }
            }
            if (radKeyLocationsPolygons.Checked)
            {
                if (checkBox != chkKeyLocationsLayers)
                {
                    return
                        chkKeyLocationsLayers.CheckedItems.Cast<object>()
                            .All(checkedItem => checkedItem.ToString() != fileName);
                }
            }
            return true;
        }

        private bool VerifyPointLayerNotSelected(string fileName, CheckedListBox checkBox)
        {
            if (cmbNotesLayer.SelectedItem != null && cmbNotesLayer.SelectedItem.ToString() == fileName) return false;
            if (cmbHydrantsLayer.SelectedItem != null && cmbHydrantsLayer.SelectedItem.ToString() == fileName) return false;
            if (radAddressPoints.Checked)
            {
                if (checkBox != chkAddressLayers)
                {
                    if (
                        chkAddressLayers.CheckedItems.Cast<object>()
                            .Any(checkedItem => checkedItem.ToString() == fileName))
                    {
                        return false;
                    }
                }
            }
            if (radKeyLocationsPoints.Checked)
            {
                if (checkBox != chkKeyLocationsLayers)
                {
                    return
                        chkKeyLocationsLayers.CheckedItems.Cast<object>()
                            .All(checkedItem => checkedItem.ToString() != fileName);
                }
            }
            return true;
        }

        private void PolygonCheckChange(CheckedListBox checkBox)
        {
            _pointLayerSwitcher.Remove(checkBox);
            IFeatureSet[] layers = _polygonLayers.ToArray();
            foreach (string f in layers.Select(layer => Path.GetFileNameWithoutExtension(layer.Filename)))
            {
                if (VerifyPolygonLayerNotSelected(f, checkBox))
                {
                    checkBox.Items.Add(f);
                }
            }
            _polygonLayerSwitcher.Add(checkBox);
            _projectManager.IsDirty = true;
        }

        private void PointCheckChange(CheckedListBox checkBox)
        {
            _polygonLayerSwitcher.Remove(checkBox);
            IFeatureSet[] layers = _pointLayers.ToArray();
            foreach (string f in layers.Select(layer => Path.GetFileNameWithoutExtension(layer.Filename)))
            {
                if (VerifyPointLayerNotSelected(f, checkBox))
                {
                    checkBox.Items.Add(f);
                }
            }
            _pointLayerSwitcher.Add(checkBox);
            _projectManager.IsDirty = true;
        }

        private void radAddressPolygons_CheckedChanged(object sender, EventArgs e)
        {
            var btn = (RadioButton) sender;
            if (btn == null) return;
            if (!btn.Checked) return;
            PolygonCheckChange(chkAddressLayers);
        }

        private void radAddressPoints_CheckedChanged(object sender, EventArgs e)
        {
            var btn = (RadioButton) sender;
            if (btn == null) return;
            if (!btn.Checked) return;
            PointCheckChange(chkAddressLayers);
        }

        private void radKeyLocationsPoints_CheckedChanged(object sender, EventArgs e)
        {
            var btn = (RadioButton) sender;
            if (btn == null) return;
            if (!btn.Checked) return;
            PointCheckChange(chkKeyLocationsLayers);
        }

        private void radKeyLocationsPolygons_CheckedChanged(object sender, EventArgs e)
        {
            var btn = (RadioButton) sender;
            if (btn == null) return;
            if (!btn.Checked) return;
            PolygonCheckChange(chkKeyLocationsLayers);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void adminLayerSplitter_SplitterMoved(object sender, SplitterEventArgs e)
        {
            // TODO: investigate the effect this has on map rendering/invalidations
        }

        private void mapBGColorPanel_Click(object sender, EventArgs e)
        {
            var oColor = mapBGColorPanel.BackColor;
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;

            mapBGColorPanel.BackColor = dlg.Color;
            if (oColor != mapBGColorPanel.BackColor)
            {
                _projectManager.IsDirty = true;
            }
            var m = (Map) _appManager.Map;
            m.BackColor = mapBGColorPanel.BackColor;
            foreach (KeyValuePair<string, DockPanelInfo> dpi in _dockingControl.DockPanelLookup)
            {
                if (!dpi.Key.Trim().StartsWith("kMap")) continue;
                dpi.Value.DotSpatialDockPanel.InnerControl.BackColor = mapBGColorPanel.BackColor;
                dpi.Value.DotSpatialDockPanel.InnerControl.Refresh();
            }
            _appManager.Map.Invalidate();
            // update the graphic render display to show new map bg color
            DrawLineGraphics();
            DrawPointGraphics();
        }

        private void btnUsersAddUpdate_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text.Length <= 0 || txtVerifyPassword.Text.Length <= 0)
            {
                MessageBox.Show(@"Please add a password and verify it");
                return;
            }
            if (txtPassword.Text != txtVerifyPassword.Text)
            {
                MessageBox.Show(@"Passwords do not match");
                return;
            }
            if (txtUsername.Text.Length <= 0)
            {
                MessageBox.Show(@"Please enter a username");
                return;
            }

            string conn = SdrConfig.Settings.Instance.ApplicationRepoConnectionString;
            SaltedHash sh = SaltedHash.Create(txtPassword.Text);
            var d = new Dictionary<string, string>
            {
                {"username", txtUsername.Text},
                {"salt", sh.Salt},
                {"hash", sh.Hash}
            };

            try
            {
                if (lstUsers.Items.Contains(txtUsername.Text))
                {
                    SQLiteHelper.Update(conn, "logins", d, "username ='" + txtUsername.Text + "'");
                }
                else
                {
                    // add a new user to the database
                    lstUsers.Items.Add(txtUsername.Text);
                    SQLiteHelper.Insert(conn, "logins", d);
                }
                txtPassword.Text = string.Empty;
                txtVerifyPassword.Text = string.Empty;
                txtUsername.Text = string.Empty;
            }
            catch (Exception ex)
            {
                var msg = AppContext.Instance.Get<IUserMessage>();
                msg.Error(ex.Message, ex);
            }
        }

        private void btnUsersDelete_Click(object sender, EventArgs e)
        {
            if (lstUsers.Items.Count == 1)
            {
                MessageBox.Show(@"Delete not available, there is only one user");
                return;
            }
            if (txtUsername.Text.Length <= 0)
            {
                MessageBox.Show(@"Select a user to delete");
                return;
            }
            if (!lstUsers.Items.Contains(txtUsername.Text))
            {
                MessageBox.Show(@"User has already been deleted");
                return;
            }
            lstUsers.Items.Remove(txtUsername.Text); // remove the user from the list 
            string conn = SdrConfig.Settings.Instance.ApplicationRepoConnectionString;

            try
            {
                SQLiteHelper.Delete(conn, "logins", "username ='" + txtUsername.Text + "'");
                txtPassword.Text = string.Empty;
                txtVerifyPassword.Text = string.Empty;
                txtUsername.Text = string.Empty;
            }
            catch (Exception ex)
            {
                var msg = AppContext.Instance.Get<IUserMessage>();
                msg.Error(ex.Message, ex);
            }
        }

        private void lstUsers_DoubleClick(object sender, EventArgs e)
        {
            txtUsername.Text = lstUsers.Items[lstUsers.SelectedIndex].ToString();
        }

        private static IEnumerable<string> ReadIndexLines(string filePath)
        {
            using (StreamReader reader = File.OpenText(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        private DataTable GetDefaultIndexLookupTable(string layName)
        {
            var table = new DataTable("defaults");
            table.Columns.Add("lookup");
            table.Columns.Add("fieldname");

            StringCollection sc = ApplyCheckBoxSetting(chkAddressLayers);
            if (sc != null && sc.Contains(layName))
            {
                var file = ReadIndexLines(SdrConfig.Settings.Instance.ApplicationDataDirectory + @"\Config\address_indexes.txt");
                foreach (string key in file)
                {
                    if (key == "Pre Type")
                    {
                        if (chkPretypes.Checked)
                        {
                            table.Rows.Add(key, "");
                        }
                    }
                    else
                    {
                        table.Rows.Add(key, "");
                    }
                }
                return table;
            }
            sc = ApplyCheckBoxSetting(chkRoadLayers);
            if (sc != null && sc.Contains(layName))
            {
                var file = ReadIndexLines(SdrConfig.Settings.Instance.ApplicationDataDirectory + @"\Config\road_indexes.txt");
                foreach (string key in file)
                {
                    if (key == "Pre Type")
                    {
                        if (chkPretypes.Checked)
                        {
                            table.Rows.Add(key, "");
                        }
                    }
                    else
                    {
                        table.Rows.Add(key, "");
                    }
                }
                return table;
            }
            sc = ApplyCheckBoxSetting(chkKeyLocationsLayers);
            if (sc != null && sc.Contains(layName))
            {
                var file = ReadIndexLines(SdrConfig.Settings.Instance.ApplicationDataDirectory + @"\Config\keyLocation_indexes.txt");
                foreach (string key in file)
                {
                    table.Rows.Add(key, "");
                }
                return table;
            }
            if (layName == ApplyComboBoxSetting(cmbCityLimitLayer))
            {
                var file = ReadIndexLines(SdrConfig.Settings.Instance.ApplicationDataDirectory + @"\Config\cityLimit_indexes.txt");
                foreach (string key in file)
                {
                    table.Rows.Add(key, "");
                }
                return table;
            }
            if (layName == ApplyComboBoxSetting(cmbCellSectorLayer))
            {
                var file = ReadIndexLines(SdrConfig.Settings.Instance.ApplicationDataDirectory + @"\Config\cellSector_indexes.txt");
                foreach (string key in file)
                {
                    table.Rows.Add(key, "");
                }
                return table;
            }
            if (layName == ApplyComboBoxSetting(cmbESNLayer))
            {
                var file = ReadIndexLines(SdrConfig.Settings.Instance.ApplicationDataDirectory + @"\Config\esn_indexes.txt");
                foreach (string key in file)
                {
                    table.Rows.Add(key, "");
                }
                return table;
            }
            if (layName == ApplyComboBoxSetting(cmbParcelsLayer))
            {
                var file = ReadIndexLines(SdrConfig.Settings.Instance.ApplicationDataDirectory + @"\Config\parcel_indexes.txt");
                foreach (string key in file)
                {
                    table.Rows.Add(key, "");
                }
                return table;
            }
            if (layName == ApplyComboBoxSetting(cmbHydrantsLayer))
            {
                var file = ReadIndexLines(SdrConfig.Settings.Instance.ApplicationDataDirectory + @"\Config\hydrant_indexes.txt");
                foreach (string key in file)
                {
                    table.Rows.Add(key, "");
                }
                return table;
            }
            if (layName == ApplyComboBoxSetting(cmbNotesLayer))
            {
                var file = ReadIndexLines(SdrConfig.Settings.Instance.ApplicationDataDirectory + @"\Config\note_indexes.txt");
                foreach (string key in file)
                {
                    table.Rows.Add(key, "");
                }
                return table;
            }
            return null;
        }

        private void PopulateIndexesToForm()
        {
            if (_projectManager.CurrentProjectFile.Length == 0) return;
            string conn = SQLiteHelper.GetSQLiteConnectionString(_projectManager.CurrentProjectFile);
            if (conn == null) return;

            string[] tblNames = SQLiteHelper.GetAllTableNames(conn);
            foreach (string tblName in tblNames)
            {
                string[] split = tblName.Split('_');
                if (split.Length >= 2)
                {
                    switch (split[0])
                    {
                        case "AddressIndex":
                            lstExistingIndexes.Items.Add(tblName.Substring(13));
                            break;
                        case "RoadIndex":
                            lstExistingIndexes.Items.Add(tblName.Substring(10));
                            break;
                        case "KeyLocationIndex":
                            lstExistingIndexes.Items.Add(tblName.Substring(17));
                            break;
                        case "CityLimitIndex":
                            lstExistingIndexes.Items.Add(tblName.Substring(15));
                            break;
                        case "CellSectorIndex":
                            lstExistingIndexes.Items.Add(tblName.Substring(16));
                            break;
                        case "ParcelIndex":
                            lstExistingIndexes.Items.Add(tblName.Substring(12));
                            break;
                        case "EsnIndex":
                            lstExistingIndexes.Items.Add(tblName.Substring(9));
                            break;
                        case "HydrantIndex":
                            lstExistingIndexes.Items.Add(tblName.Substring(13));
                            break;
                        case "NoteIndex":
                            lstExistingIndexes.Items.Add(tblName.Substring(10));
                            break;
                    }
                }
            }
        }

        private String GetLayerIndexTableType(string layName)
        {
            StringCollection sc = ApplyCheckBoxSetting(chkAddressLayers);
            if (sc != null && sc.Contains(layName))
            {
                return "AddressIndex";
            }
            sc = ApplyCheckBoxSetting(chkRoadLayers);
            if (sc != null && sc.Contains(layName))
            {
                return "RoadIndex";
            }
            sc = ApplyCheckBoxSetting(chkKeyLocationsLayers);
            if (sc != null && sc.Contains(layName))
            {
                return "KeyLocationIndex";
            }
            if (layName == ApplyComboBoxSetting(cmbCityLimitLayer))
            {
                return "CityLimitIndex";
            }
            if (layName == ApplyComboBoxSetting(cmbCellSectorLayer))
            {
                return "CellSectorIndex";
            }
            if (layName == ApplyComboBoxSetting(cmbESNLayer))
            {
                return "EsnIndex";
            }
            if (layName == ApplyComboBoxSetting(cmbParcelsLayer))
            {
                return "ParcelIndex";
            }
            if (layName == ApplyComboBoxSetting(cmbHydrantsLayer))
            {
                return "HydrantIndex";
            }
            if (layName == ApplyComboBoxSetting(cmbNotesLayer))
            {
                return "NoteIndex";
            }
            return null;
        }

        private void ResetIndexPanel()
        {
            chkLayerIndex.Items.Clear();
            dgvLayerIndex.Rows.Clear();
            dgvLayerIndex.Columns.Clear();

            var txtIndexColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = @"Field Index",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            dgvLayerIndex.Columns.Add(txtIndexColumn);
            dgvLayerIndex.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

            var txtValueColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = @"Field Value",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            dgvLayerIndex.Columns.Add(txtValueColumn);
            dgvLayerIndex.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void PopulateIndexLookupTable(string lyrName)
        {
            var saved = new DataTable();  // table we will use to populate all the datagridview values (field key, value pairs)
            saved.Columns.Add("lookup");
            saved.Columns.Add("fieldname");

            // newList will be initialized if the index exists in the db but not yet in saved lookup dict
            List<Dictionary<string, string>> newList = null;

            // check if this layer exists in the queueed indexes
            if (chkLayersToIndex.Items.Contains(lyrName))
            {
                // in this case we will use the temp set values from the queued source
                List<Dictionary<string, string>> idx;
                _indexQueue.TryGetValue(lyrName, out idx);
                if (idx != null)
                {
                    foreach (Dictionary<string, string> d in idx)
                    {
                        DataRow dr = saved.NewRow();
                        string lookup;
                        d.TryGetValue("lookup", out lookup);
                        dr["lookup"] = lookup;
                        string fieldname;
                        d.TryGetValue("fieldname", out fieldname);
                        dr["fieldname"] = fieldname;
                        saved.Rows.Add(dr);
                    }
                }
            }
            else // no active queue, check for previous saved index instead
            {
                // lets see if it already exists in the saved indexes
                if (_savedIndexes.ContainsKey(lyrName))
                {
                    List<Dictionary<string, string>> idx;
                    _savedIndexes.TryGetValue(lyrName, out idx);
                    if (idx != null)
                    {
                        foreach (Dictionary<string, string> d in idx)
                        {
                            DataRow dr = saved.NewRow();
                            string lookup;
                            d.TryGetValue("lookup", out lookup);
                            dr["lookup"] = lookup;
                            string fieldname;
                            d.TryGetValue("fieldname", out fieldname);
                            dr["fieldname"] = fieldname;
                            saved.Rows.Add(dr);
                        }
                    }
                }
                else  // no queued index, no saved index. check the table itself -> if it exists then populate to _savedIndexes as well for speed next time around
                {
                    if (_projectManager.CurrentProjectFile.Length > 0) // check if there is a saved project file
                    {
                        string conn = SQLiteHelper.GetSQLiteConnectionString(_projectManager.CurrentProjectFile);
                        string lyrType = GetLayerIndexTableType(lyrName);
                        if (SQLiteHelper.TableExists(conn, lyrType + "_" + lyrName))
                        {
                            string query = "SELECT * FROM " + lyrType + "_" + lyrName;
                            saved = SQLiteHelper.GetDataTable(conn, query);
                            newList = new List<Dictionary<string, string>>();
                        }
                    }
                }
            }
            // actually populate the DatagridView with the lookup values now
            SetDgvRowIndexLookups(lyrName, saved, newList);
        }

        private void SetDgvRowIndexLookups(string lyrName, DataTable table, List<Dictionary<string, string>> newList)
        {
            // default DataTable with all the default lookup keys based on layer name, used to lookup type
            DataTable defaults = GetDefaultIndexLookupTable(lyrName);

            // cycle through all the default rows and populate any values that are set on the saved table
            for (int i = 0; i < defaults.Rows.Count; i++)
            {
                // use the default key value to do a lookup on the saved table
                DataRow r = defaults.Rows[i];
                var key = r["lookup"].ToString();
                var val = string.Empty;
                string exp = "lookup = " + "'" + key + "'";
                DataRow[] foundRows = table.Select(exp);
                if (foundRows.Length > 0)
                {
                    DataRow dr = foundRows[0];
                    val = dr["fieldname"].ToString();
                }
                var row = new DataGridViewRow();
                var txtKey = new DataGridViewTextBoxCell { Value = key };
                var txtValue = new DataGridViewTextBoxCell { Value = val };
                row.Cells.Add(txtKey);
                row.Cells.Add(txtValue);
                dgvLayerIndex.Rows.Add(row);

                if (newList != null)
                {
                    if (dgvLayerIndex.Rows[i].Cells[1].Value.ToString().Length > 0)
                    {
                        var d = new Dictionary<string, string>
                        {
                            {"lookup", dgvLayerIndex.Rows[i].Cells[0].Value.ToString()},
                            {"fieldname", dgvLayerIndex.Rows[i].Cells[1].Value.ToString()}
                        };
                        newList.Add(d);
                    }
                }

                if (val.Length <= 0) continue;  // if value is set then set the checked state to true
                for (int j = 0; j < chkLayerIndex.Items.Count; j++)
                {
                    if (chkLayerIndex.Items[j].ToString() == val)
                    {
                        chkLayerIndex.SetItemCheckState(j, CheckState.Checked);
                        break;
                    }
                }
            }
            if (newList != null) // the index exists from a save, store it to the saved index lookup
            {
                _savedIndexes.Add(lyrName, newList);
            }
        }

        private void cmbLayerIndex_SelectedValueChanged(object sender, EventArgs e)
        {
            ResetIndexPanel();
            string lyrName = cmbLayerIndex.SelectedItem.ToString();
            if (lyrName.Length == 0) return;

            // add all the data field columns to the checkbox list
            IMapLayer mapLyr;
            _layerLookup.TryGetValue(lyrName, out mapLyr);
            var mfl = mapLyr as IMapFeatureLayer;
            if (mfl != null && mfl.DataSet != null)
            {
                IFeatureSet fl = mfl.DataSet;
                foreach (DataColumn dc in fl.DataTable.Columns)
                {
                    chkLayerIndex.Items.Add(dc.ColumnName);
                }
            }
            PopulateIndexLookupTable(lyrName);
        }

        private void chkLayerIndex_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (dgvLayerIndex.SelectedCells.Count == 0) return;
            var cb = (CheckedListBox) sender;
            if (e.NewValue.ToString() != "Checked")
            {
                DataGridViewRowCollection rows = dgvLayerIndex.Rows;
                foreach (DataGridViewRow row in rows)
                {
                    if (row.Cells[1].Value == cb.SelectedItem)
                    {
                        row.Cells[1].Value = string.Empty;
                        break;
                    }
                }
            }
            else
            {
                if (cb.SelectedItem == null) return;
                if (dgvLayerIndex.Rows[dgvLayerIndex.CurrentCell.RowIndex].Cells[1].Value == null) return;
                if (dgvLayerIndex.Rows[dgvLayerIndex.CurrentCell.RowIndex].Cells[1].Value.ToString().Length > 0)
                {
                    // uncheck the value of the currently set state
                    for (int i = 0; i < cb.Items.Count; i++)
                    {
                        if (cb.Items[i].ToString() !=
                            dgvLayerIndex.Rows[dgvLayerIndex.CurrentCell.RowIndex].Cells[1].Value.ToString()) continue;
                        cb.SetItemCheckState(i, CheckState.Unchecked);
                        break;
                    }
                }
                dgvLayerIndex.Rows[dgvLayerIndex.CurrentCell.RowIndex].Cells[1].Value = cb.SelectedItem;
            }
        }

        private void btnAddIndex_Click(object sender, EventArgs e)
        {
            if (cmbLayerIndex.SelectedItem.ToString().Length > 0)
            {
                string lyrName = cmbLayerIndex.SelectedItem.ToString();
                if (!chkLayersToIndex.Items.Contains(lyrName))
                {
                    chkLayersToIndex.Items.Add(lyrName);
                    chkLayersToIndex.SetItemChecked(chkLayersToIndex.Items.Count - 1, true);
                }
                else // if its already on the list make sure it is checked
                {
                    for (int i = 0; i <= chkLayersToIndex.Items.Count - 1; i++)
                    {
                        if (chkLayersToIndex.Items[i].ToString().Equals(lyrName))
                        {
                            chkLayersToIndex.SetItemChecked(i, true);
                        }
                    }
                }
                var indexList = new List<Dictionary<string, string>>();
                for (int i = 0; i < dgvLayerIndex.Rows.Count; i++)
                {   
                    if (dgvLayerIndex.Rows[i].Cells[1].Value.ToString().Length > 0)
                    {
                        var d = new Dictionary<string, string>
                        {
                            {"lookup", dgvLayerIndex.Rows[i].Cells[0].Value.ToString()},
                            {"fieldname", dgvLayerIndex.Rows[i].Cells[1].Value.ToString()}
                        };
                        indexList.Add(d);
                    }
                }
                // check if its already in our queue, remove and readd if so
                if (_indexQueue.ContainsKey(lyrName))
                {
                    _indexQueue.Remove(lyrName);
                }
                _indexQueue.Add(lyrName, indexList);
            }
        }

        private void btnCreateIndex_Click(object sender, EventArgs e)
        {
            if (_idxWorker.IsBusy != true)
            {
                // TODO: investigate a better progress panel, so that it parents properly to the form
                _progressPanel = new ProgressPanel();
                _progressPanel.StartProgress("Creating Indexes...");

                var iobjects = new IndexObject[_indexQueue.Count];
                int count = 0;

                foreach (KeyValuePair<string, List<Dictionary<string, string>>> qKeyVal in _indexQueue)
                {
                    string lyrName = qKeyVal.Key;
                    // make sure a featureset exists and assign FID values if not present already
                    IMapLayer mapLyr;
                    _layerLookup.TryGetValue(lyrName, out mapLyr);
                    var mfl = mapLyr as IMapFeatureLayer;
                    IFeatureSet fs;
                    if (mfl != null && mfl.DataSet != null)
                    {
                        fs = mfl.DataSet;
                        fs.AddFid();  // make sure FID values exist for use as lookup key
                        fs.Save();
                    }
                    else
                    {
                        var msg = AppContext.Instance.Get<IUserMessage>();
                        msg.Warn("Error on Create Index, FeatureDataset is null", new Exception());
                        return;
                    }

                    List<Dictionary<string, string>> indexList = qKeyVal.Value;
                    var list = new List<KeyValuePair<string, string>>();

                    // iterate through all the field indexes
                    for (int i = 0; i <= indexList.Count - 1; i++)
                    {
                        Dictionary<string, string> d = indexList[i];
                        var kvPair = new KeyValuePair<string, string>(d["lookup"], d["fieldname"]);
                        list.Add(kvPair);
                    }
                    string idxType = GetLayerIndexTableType(lyrName);
                    var io = new IndexObject(fs, list, lyrName, mapLyr.Projection, idxType);
                    // add the indexobject to our array for creation
                    iobjects.SetValue(io, count);
                    count++;
                }
                _idxWorker.RunWorkerAsync(iobjects);
            }
        }

        private static void LogGeometryIndexError(string file, string ftId, Shape shape, string wkt, Exception ex)
        {
            var p = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SDR\\" +SdrConfig.Settings.Instance.ApplicationName;
            var d = new DirectoryInfo(p);
            if (!d.Exists)
            {
                d.Create();
            }
            var f = p + "\\" + file + "_geoindexing_errors.txt";  // log file name
            using (var sw = File.AppendText(f))
            {
                sw.WriteLine("FeatureID: " + ftId);
                sw.WriteLine("-- Exception -------------------------------------------");
                sw.WriteLine(ex);
                sw.WriteLine("-- Geometry --------------------------------------------");
                sw.WriteLine("FeatureType: " + shape.ToGeometry().FeatureType);
                sw.WriteLine("GeometryType: " + shape.ToGeometry().GeometryType);
                sw.WriteLine("IsEmpty: " + shape.ToGeometry().IsEmpty);
                sw.WriteLine("IsRectangle: " + shape.ToGeometry().IsRectangle);
                sw.WriteLine("IsSimple: " + shape.ToGeometry().IsSimple);
                sw.WriteLine("IsValid: " + shape.ToGeometry().IsValid);
                sw.WriteLine("NumGeometries: " + shape.ToGeometry().NumGeometries);
                sw.WriteLine("NumPoints: " + shape.ToGeometry().NumPoints);
                sw.WriteLine("Centroid: " + shape.ToGeometry().Centroid.Coordinate.ToString());
                sw.WriteLine("PrecisionModel: " + shape.ToGeometry().PrecisionModel);
                sw.WriteLine("SRID: " + shape.ToGeometry().Srid);
                sw.WriteLine("-- WKT -------------------------------------------------");
                sw.WriteLine(wkt);
                sw.WriteLine("========================================================");
                sw.Close();
            }
        }

        private Dictionary<string, List<Document>> GetDocuments(IndexObject[] iobjects)
        {
            var docs = new Dictionary<string, List<Document>>();
            if (iobjects.Length <= 0) return docs;

            foreach (IndexObject o in iobjects)
            {
                var docList = new List<Document>();
                IFeatureSet fs = o.FeatureSet;

                if (o.LayerProjection.ToEsriString() != KnownCoordinateSystems.Geographic.World.WGS1984.ToEsriString())
                {
                    // reproject the in-memory representation of our featureset (actual file remains unchanged)
                    fs.Reproject(KnownCoordinateSystems.Geographic.World.WGS1984);
                }
                foreach (ShapeRange shapeRange in fs.ShapeIndices)  // cycle through each shape/record
                {
                    var doc = new Document();  // new index doc for this record
                    // snatch the row affiliated with this shape-range
                    DataRow dr = fs.DataTable.Rows[shapeRange.RecordNumber - 1];
                    // add standardized lookup fields for each record
                    doc.Add(new Field(Fid, dr[Fid].ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    doc.Add(new Field(Lyrname, o.LayerName, Field.Store.YES, Field.Index.NOT_ANALYZED));

                    // TODO: currently the polygon indexing seems to have issues (diff between esri and opengeo defs)
                    // TODO: investigate and come up with a solution for self-intersecting bullshit
                    if (fs.FeatureType.ToString() != "Polygon")
                    {
                        // snatch the shape affiliated with the shape-range
                        Shape shp = fs.GetShape(shapeRange.RecordNumber - 1, false);
                        IGeometry geom = shp.ToGeometry(); // cast shape to geometry for wkt serialization

                        // serialize the geometry into wkt (which will be read by spatial4n for lucene indexing)
                        var wktWriter = new WktWriter();
                        var wkt = wktWriter.Write((Geometry) geom);

                        // create our strategy for spatial indexing using NTS context
                        var ctx = NtsSpatialContext.GEO; // using NTS (provides polygon/line/point models)
                        SpatialStrategy strategy = new RecursivePrefixTreeStrategy(new GeohashPrefixTree(ctx, 24), Geoshape);
                        try // the esri and ogc defs regarding polygons seem to differ and cause issues here
                        {
                            Spatial4n.Core.Shapes.Shape wktShp = ctx.ReadShape(wkt);
                            foreach (var f in strategy.CreateIndexableFields(wktShp))
                            {
                                doc.Add(f); // add the geometry to the index for queries
                            }
                            // store the actual shape for later use on intersections and such
                            doc.Add(new Field(strategy.GetFieldName(), ctx.ToString(wktShp), Field.Store.YES, Field.Index.NOT_ANALYZED));
                        }
                        catch (Exception ex)
                        {
                            LogGeometryIndexError(o.LayerName, dr[Fid].ToString(), shp, wkt, ex);
                            var msg = AppContext.Instance.Get<IUserMessage>();
                            msg.Error(
                                "Error creating index :: FeatureClass: " + o.LayerName + " FeatureID: " + dr[Fid], ex);
                        }
                    }
                    // handle all other non-spatial field lookups
                    // TODO: refactor so we dont need 2 loops use the shp and grab attributes at same time as above
                    var list = o.FieldLookup;
                    foreach (KeyValuePair<string, string> kv in list)
                    {
                        // TODO: include the field type with the lookup? (more dynamic) explore this idea
                        if (kv.Key == "Phone" || kv.Key == "Aux. Phone" || kv.Key == "Structure Number")
                        {
                            doc.Add(new Field(kv.Key, dr[kv.Value].ToString(), Field.Store.YES,
                                Field.Index.NOT_ANALYZED));
                        }
                        else // run analyzer on all remaining field types
                        {
                            doc.Add(new Field(kv.Key, dr[kv.Value].ToString(), Field.Store.YES, Field.Index.ANALYZED));
                        }
                    }
                    docList.Add(doc);  // add the new document to the documents list
                }
                if (docs.ContainsKey(o.IndexType))
                {
                    // if this index is already in existence, just append our new docs
                    // TODO: do some sort of check that removes duplicates?
                    List<Document> oldList;
                    docs.TryGetValue(o.IndexType, out oldList);
                    if (oldList != null) oldList.AddRange(docList);
                }
                else
                {
                    docs.Add(o.IndexType, docList);
                }
                // reproject the in-memory representation of our featureset back to original projection if needed
                if (o.LayerProjection.ToEsriString() != KnownCoordinateSystems.Geographic.World.WGS1984.ToEsriString())
                {
                    fs.Reproject(o.LayerProjection);
                }
            }
            return docs;
        }

        private void idx_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var iobjects = e.Argument as IndexObject[];
                Dictionary<string, List<Document>> docs = GetDocuments(iobjects);
                if (docs.Count > 0)
                {
                    var path = string.Empty;
                    Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);
                    foreach (KeyValuePair<string, List<Document>> keyValuePair in docs)
                    {
                        path = Path.Combine(TempIndexDir, "_indexes", keyValuePair.Key);

                        DirectoryInfo di = System.IO.Directory.CreateDirectory(path);
                        LDirectory dir = FSDirectory.Open(di);
                        FileInfo[] fi = di.GetFiles();

                        // single indexwriter is thread safe so lets use it in parallel
                        IndexWriter writer = fi.Length == 0 ?
                            new IndexWriter(dir, analyzer, true, IndexWriter.MaxFieldLength.LIMITED) :
                            new IndexWriter(dir, analyzer, false, IndexWriter.MaxFieldLength.LIMITED);

                        // iterate all our documents and add them
                        var documents = keyValuePair.Value;
                        Parallel.ForEach(documents, delegate(Document document, ParallelLoopState state)
                        {
                            // check if this document already exists in the index
                            var fid = document.GetField(Fid).StringValue;
                            var lyr = document.GetField(Lyrname).StringValue;

                            Query qfid = new TermQuery(new Term(Fid, fid));
                            Query qlyr = new TermQuery(new Term(Lyrname, lyr));

                            var query = new BooleanQuery { { qfid, Occur.MUST }, { qlyr, Occur.MUST } };

                            writer.DeleteDocuments(query);

                            // clean the numeric fields
                            if (document.GetField("Phone") != null)
                            {
                                document.GetField("Phone").SetValue(DigitsOnly.Replace(document.GetField("Phone").StringValue, ""));
                            }
                            if (document.GetField("Aux. Phone") != null)
                            {
                                document.GetField("Aux. Phone").SetValue(DigitsOnly.Replace(document.GetField("Aux. Phone").StringValue, ""));
                            }
                            if (document.GetField("Structure Number") != null)
                            {
                                document.GetField("Structure Number").SetValue(DigitsOnly.Replace(document.GetField("Structure Number").StringValue, ""));
                            }

                            writer.AddDocument(document);
                        });
                        writer.Optimize();
                        writer.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = AppContext.Instance.Get<IUserMessage>();
                msg.Error("Error on creating index :: BackgroundWorker Failed", ex);
            }
        }

        private void idx_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // kill the progress indicator
            _progressPanel.StopProgress();
            _progressPanel = null;
            // make sure we set the project as dirty
            _projectManager.IsDirty = true;

            var cleanQueue = new List<string>();
            for (int i = 0; i <= chkLayersToIndex.Items.Count - 1; i++)
            {
                if (chkLayersToIndex.GetItemChecked(i))
                {
                    var item = chkLayersToIndex.Items[i].ToString();
                    // remove this from our overall queue and move it to the saved indexes dict (for OnSave event)
                    List<Dictionary<string, string>> idx;
                    _indexQueue.TryGetValue(item, out idx);
                    if (idx != null)
                    {
                        if (_savedIndexes.ContainsKey(item))
                        {
                            _savedIndexes.Remove(item);
                        }
                        _savedIndexes.Add(item, idx);
                    }
                    _indexQueue.Remove(item);
                    // check if its already on completed indexes, if not then add it to the list
                    if (!lstExistingIndexes.Items.Contains(item))
                    {
                        lstExistingIndexes.Items.Add(item);
                    }
                }
                else
                {
                    cleanQueue.Add(chkLayersToIndex.Items[i].ToString());
                }
            }
            // wipe the queue checkbox and repopulate any items that were left behind
            chkLayersToIndex.Items.Clear();
            foreach (var item in cleanQueue)
            {
                chkLayersToIndex.Items.Add(item, false);
            }
            if (e.Error == null) return;
            var msg = AppContext.Instance.Get<IUserMessage>();
            msg.Error("Error on creating index :: BackgroundWorker Completed with Error", e.Error);
        }

        private void btnDeleteIndex_Click(object sender, EventArgs e)
        {
            if (_idxWorker.IsBusy != true)
            {
                DeleteIndex();   
            }
        }

        private void DeleteIndex()
        {
            string lyrName = lstExistingIndexes.SelectedItem.ToString();
            if (lyrName.Length > 0)
            {
                _projectManager.IsDirty = true;
                _deleteIndexes.Add(lyrName);
                lstExistingIndexes.Items.Remove(lyrName);
                if (_savedIndexes.ContainsKey(lyrName))
                {
                    _savedIndexes.Remove(lyrName);
                }
            }
        }

        private void btnAddView_Click(object sender, EventArgs e)
        {
            if (txtViewName.Text.Length <= 0)
            {
                // TODO; allow the messagebox to send a name back
                MessageBox.Show(@"Please assign a name to the new map view");
                return;
            }
            _isCreatingNewView = true;  // prevent the cmbBox pulldown from performing circular panel selection

            var txt = txtViewName.Text;
            txtViewName.Text = string.Empty;
            // clean the filename for a key value
            var fname = new string(txt.Where(ch => !InvalidFileNameChars.Contains(ch)).ToArray());
            fname = fname.Replace(" ", "");
            var key = "kMap_" + fname;

            if (cmbActiveMapTab.Items.Contains(txt))
            {
                // this map tab already exists select it and move along
                _appManager.DockManager.SelectPanel(key);
                return;
            }
            cmbActiveMapTab.Items.Add(txt);

            // create a new map to stick to the tab
            var nMap = _projectManager.CreateNewMap(key);
            // create new dockable panel and stow that shit yo!
            var dp = new DockablePanel(key, txt, nMap, DockStyle.Fill);
            _appManager.DockManager.Add(dp);
            _appManager.DockManager.SelectPanel(key);

            _projectManager.IsDirty = true;
            _isCreatingNewView = false;  // reset the new view creation flag back to false
        }

        private void btnRemoveView_Click(object sender, EventArgs e)
        {
            if (cmbActiveMapTab.SelectedItem == null) return;
            if (cmbActiveMapTab.SelectedItem.ToString().Length <= 0) return;
            var txt = cmbActiveMapTab.SelectedItem.ToString();
            // clean the filename for a key value
            var fname = new string(txt.Where(ch => !InvalidFileNameChars.Contains(ch)).ToArray());
            fname = fname.Replace(" ", "");
            var key = "kMap_" + fname;
            // remove the dockpanel now (which will also destroy the map and tab)
            _appManager.DockManager.Remove(key);
            // also remove it from the combo box selections
            cmbActiveMapTab.Items.Remove(txt);
            _projectManager.IsDirty = true;
        }

        private List<string> AddLayersToIndex(CheckedListBox clb)
        {
            return (from object item in clb.CheckedItems select item.ToString()).ToList();
        }

        private List<string> AddLayersToIndex(ComboBox cmbBox)
        {
            var l = new List<string>();
            if (cmbBox.Text.Length > 0)
            {
                l.Add(cmbBox.Text);
            }
            return l;
        }

        private void UpdateLayerIndexCombo(List<string> ad, List<string> rd, List<string> kl, List<string> cs, List<string> cl, List<string> es, List<string> pl, List<string> hy, List<string> nl)
        {
            cmbLayerIndex.Items.Clear();
            var sels = new List<object>(ad.Count + rd.Count + kl.Count + cs.Count + cl.Count + es.Count + pl.Count + hy.Count + nl.Count);
            sels.AddRange(ad);
            sels.AddRange(rd);
            sels.AddRange(kl);
            sels.AddRange(cs);
            sels.AddRange(cl);
            sels.AddRange(es);
            sels.AddRange(pl);
            sels.AddRange(hy);
            sels.AddRange(nl);
            cmbLayerIndex.Items.AddRange(sels.ToArray());
        }

        private void chkRoadLayers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var clb = (CheckedListBox)sender;
            var rd = (from object item in clb.CheckedItems where item.ToString() != clb.Items[e.Index].ToString() select item.ToString()).ToList();
            if (e.NewValue == CheckState.Checked)
            {
                rd.Add(clb.Items[e.Index].ToString());
            }
            var ad = AddLayersToIndex(chkAddressLayers);
            var kl = AddLayersToIndex(chkKeyLocationsLayers);
            var cs = AddLayersToIndex(cmbCellSectorLayer);
            var cl = AddLayersToIndex(cmbCityLimitLayer);
            var es = AddLayersToIndex(cmbESNLayer);
            var pl = AddLayersToIndex(cmbParcelsLayer);
            var hy = AddLayersToIndex(cmbHydrantsLayer);
            var nl = AddLayersToIndex(cmbNotesLayer);
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl, hy, nl);
            _projectManager.IsDirty = true;
        }

        private void chkAddressLayers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var clb = (CheckedListBox)sender;
            var ad = (from object item in clb.CheckedItems where item.ToString() != clb.Items[e.Index].ToString() select item.ToString()).ToList();
            if (e.NewValue == CheckState.Checked)
            {
                ad.Add(clb.Items[e.Index].ToString());
            }
            var rd = AddLayersToIndex(chkRoadLayers);
            var kl = AddLayersToIndex(chkKeyLocationsLayers);
            var cs = AddLayersToIndex(cmbCellSectorLayer);
            var cl = AddLayersToIndex(cmbCityLimitLayer);
            var es = AddLayersToIndex(cmbESNLayer);
            var pl = AddLayersToIndex(cmbParcelsLayer);
            var hy = AddLayersToIndex(cmbHydrantsLayer);
            var nl = AddLayersToIndex(cmbNotesLayer);
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl, hy, nl);
            _projectManager.IsDirty = true;
        }

        private void chkKeyLocationsLayers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var clb = (CheckedListBox)sender;
            var kl = (from object item in clb.CheckedItems where item.ToString() != clb.Items[e.Index].ToString() select item.ToString()).ToList();
            if (e.NewValue == CheckState.Checked)
            {
                kl.Add(clb.Items[e.Index].ToString());
            }
            var rd = AddLayersToIndex(chkRoadLayers);
            var ad = AddLayersToIndex(chkAddressLayers);
            var cs = AddLayersToIndex(cmbCellSectorLayer);
            var cl = AddLayersToIndex(cmbCityLimitLayer);
            var es = AddLayersToIndex(cmbESNLayer);
            var pl = AddLayersToIndex(cmbParcelsLayer);
            var hy = AddLayersToIndex(cmbHydrantsLayer);
            var nl = AddLayersToIndex(cmbNotesLayer);
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl, hy, nl);
            _projectManager.IsDirty = true;
        }

        private void cmbCellSectorLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cmb = (ComboBox)sender;
            var cs = new List<string>();
            if (cmb.SelectedItem.ToString().Length > 0)
            {
                cs.Add(cmb.SelectedItem.ToString());
            }
            var ad = AddLayersToIndex(chkAddressLayers);
            var rd = AddLayersToIndex(chkRoadLayers);
            var kl = AddLayersToIndex(chkKeyLocationsLayers);
            var cl = AddLayersToIndex(cmbCityLimitLayer);
            var es = AddLayersToIndex(cmbESNLayer);
            var pl = AddLayersToIndex(cmbParcelsLayer);
            var hy = AddLayersToIndex(cmbHydrantsLayer);
            var nl = AddLayersToIndex(cmbNotesLayer);
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl, hy, nl);
            _projectManager.IsDirty = true;
        }

        private void cmbNotesLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cmb = (ComboBox)sender;
            var nl = new List<string>();
            if (cmb.SelectedItem.ToString().Length > 0)
            {
                nl.Add(cmb.SelectedItem.ToString());
            }
            var ad = AddLayersToIndex(chkAddressLayers);
            var rd = AddLayersToIndex(chkRoadLayers);
            var kl = AddLayersToIndex(chkKeyLocationsLayers);
            var cl = AddLayersToIndex(cmbCityLimitLayer);
            var cs = AddLayersToIndex(cmbCellSectorLayer);
            var es = AddLayersToIndex(cmbESNLayer);
            var pl = AddLayersToIndex(cmbParcelsLayer);
            var hy = AddLayersToIndex(cmbHydrantsLayer);
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl, hy, nl);
            _projectManager.IsDirty = true;
        }

        private void cmbCityLimitLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cmb = (ComboBox)sender;
            var cl = new List<string>();
            if (cmb.SelectedItem.ToString().Length > 0)
            {
                cl.Add(cmb.SelectedItem.ToString());
            }
            var ad = AddLayersToIndex(chkAddressLayers);
            var rd = AddLayersToIndex(chkRoadLayers);
            var kl = AddLayersToIndex(chkKeyLocationsLayers);
            var cs = AddLayersToIndex(cmbCellSectorLayer);
            var es = AddLayersToIndex(cmbESNLayer);
            var pl = AddLayersToIndex(cmbParcelsLayer);
            var hy = AddLayersToIndex(cmbHydrantsLayer);
            var nl = AddLayersToIndex(cmbNotesLayer);
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl, hy, nl);
            _projectManager.IsDirty = true;
        }

        private void cmbESNLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cmb = (ComboBox)sender;
            var es = new List<string>();
            if (cmb.SelectedItem.ToString().Length > 0)
            {
                es.Add(cmb.SelectedItem.ToString());
            }
            var ad = AddLayersToIndex(chkAddressLayers);
            var rd = AddLayersToIndex(chkRoadLayers);
            var kl = AddLayersToIndex(chkKeyLocationsLayers);
            var cs = AddLayersToIndex(cmbCellSectorLayer);
            var cl = AddLayersToIndex(cmbCityLimitLayer);
            var pl = AddLayersToIndex(cmbParcelsLayer);
            var hy = AddLayersToIndex(cmbHydrantsLayer);
            var nl = AddLayersToIndex(cmbNotesLayer);
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl, hy, nl);
            _projectManager.IsDirty = true;
        }

        private void cmbParcelsLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cmb = (ComboBox)sender;
            var pl = new List<string>();
            if (cmb.SelectedItem.ToString().Length > 0)
            {
                pl.Add(cmb.SelectedItem.ToString());
            }
            var ad = AddLayersToIndex(chkAddressLayers);
            var rd = AddLayersToIndex(chkRoadLayers);
            var kl = AddLayersToIndex(chkKeyLocationsLayers);
            var cs = AddLayersToIndex(cmbCellSectorLayer);
            var cl = AddLayersToIndex(cmbCityLimitLayer);
            var es = AddLayersToIndex(cmbESNLayer);
            var hy = AddLayersToIndex(cmbHydrantsLayer);
            var nl = AddLayersToIndex(cmbNotesLayer);
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl, hy, nl);
            _projectManager.IsDirty = true;
        }

        private void cmbHydrantsLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cmb = (ComboBox)sender;
            var hy = new List<string>();
            if (cmb.SelectedItem.ToString().Length > 0)
            {
                hy.Add(cmb.SelectedItem.ToString());
            }
            var ad = AddLayersToIndex(chkAddressLayers);
            var rd = AddLayersToIndex(chkRoadLayers);
            var kl = AddLayersToIndex(chkKeyLocationsLayers);
            var cs = AddLayersToIndex(cmbCellSectorLayer);
            var cl = AddLayersToIndex(cmbCityLimitLayer);
            var es = AddLayersToIndex(cmbESNLayer);
            var pl = AddLayersToIndex(cmbParcelsLayer);
            var nl = AddLayersToIndex(cmbNotesLayer);
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl, hy, nl);
            _projectManager.IsDirty = true;
        }

        private void btnRemoveIndex_Click(object sender, EventArgs e)
        {
            string lyrName = chkLayersToIndex.SelectedItem.ToString();
            if (lyrName.Length > 0)
            {
                chkLayersToIndex.Items.Remove(lyrName);
                _indexQueue.Remove(lyrName);
            }
        }

        private void btnSaveHotKeys_Click(object sender, EventArgs e)
        {
            // clear the existing hotkeys dictionary from the manager
            HotKeyManager.ClearHotKeys();
            foreach (DataGridViewRow row in dgvHotKeys.Rows)
            {
                var cellKeys = row.Cells[0];
                var cellCmd = row.Cells[1];
                // parse into key enum
                var keys = (Keys)Enum.Parse(typeof(Keys), cellKeys.Value.ToString());
                HotKeyManager.AddHotKey(new HotKey(keys, cellCmd.Value.ToString()), cellCmd.Tag.ToString());
            }
            HotKeyManager.SaveHotKeys();
        }

        private void cmbAliMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlAliEnterpol.Visible = false;
            pnlAliGlobalCad.Visible = false;
            pnlAliSdrServer.Visible = false;

            switch (cmbAliMode.Text)
            {
                case "SDR AliServer":
                    pnlAliSdrServer.Visible = true;
                    aliPanelTableLayout.RowStyles[1].SizeType = SizeType.AutoSize;
                    return;
                case "GlobalCAD Log":
                    pnlAliGlobalCad.Visible = true;
                    aliPanelTableLayout.RowStyles[2].SizeType = SizeType.AutoSize;
                    aliPanelTableLayout.RowStyles[1].SizeType = SizeType.Absolute;
                    aliPanelTableLayout.RowStyles[1].Height = 0;
                    return;
                case "Enterpol Database":
                    pnlAliEnterpol.Visible = true;
                    aliPanelTableLayout.RowStyles[2].SizeType = SizeType.Absolute;
                    aliPanelTableLayout.RowStyles[2].Height = 0;
                    aliPanelTableLayout.RowStyles[1].SizeType = SizeType.Absolute;
                    aliPanelTableLayout.RowStyles[1].Height = 0;
                    return;
                default: // disabled
                    return;
            }
        }

        private void btnAliInterfaceDbPathBrowse_Click(object sender, EventArgs e)
        {
            FileDialog fd = new OpenFileDialog();
            fd.Filter = @"MDB Databases|*.mdb";
            fd.CheckFileExists = true;
            DialogResult r = fd.ShowDialog();
            if (r == DialogResult.OK)
            {
                txtAliInterfaceDbPath.Text = fd.FileName;
            }
        }

        private void btnAliGlobalCadLogPathBrowse_Click(object sender, EventArgs e)
        {
            FileDialog fd = new OpenFileDialog();
            fd.Filter = @"Log Files|*.log";
            fd.CheckFileExists = true;
            DialogResult r = fd.ShowDialog();
            if (r == DialogResult.OK)
            {
                txtAliGlobalCadLogPath.Text = fd.FileName;
                // if the archive path has not yet been set, then auto generate a path now
                if (txtAliGlobalCadArchivePath.Text.Length == 0)
                {
                    txtAliGlobalCadArchivePath.Text = Path.GetDirectoryName(txtAliGlobalCadLogPath.Text) + @"\Archive";
                }
            }
        }

        private void btnAliValidate_Click(object sender, EventArgs e)
        {
            string stat = string.Empty;
            if (chkNetworkfleet.Checked)
            {
                if (!ValidateNetworkfleetInput(
                    txtAliNetworkfleetUdpHost.Text,
                    Convert.ToInt32(numAliNetworkfleetUdpPort.Value)))
                {
                    return;
                }
                stat = "Networkfleet/";
            }
            switch (cmbAliMode.Text)
            {
                case "GlobalCAD Log":
                    if (ValidateGlobalCadInput(
                        txtAliGlobalCadLogPath.Text,
                        txtAliGlobalCadArchivePath.Text,
                        txtAliGlobalCadConfigIni.Text))
                    {
                        MessageBox.Show(stat + @"GlobalCAD settings Validated");
                    }
                    return;
                case "SDR AliServer":
                    if (ValidateSdrServerInput(
                        txtAliInterfaceDbPath.Text,
                        txtAliInterfaceUdpHost.Text,
                        Convert.ToInt32(numAliInterfaceUdpPort.Value)))
                    {
                        MessageBox.Show(stat + @"SDR AliServer settings Validated");
                    }
                    return;
                case "Enterpol Database":
                    if (ValidateEnterpolInput(
                        txtAliEnterpolDataSource.Text,
                        txtAliEnterpolTableName.Text,
                        txtAliEnterpolInitialCatalog.Text))
                    {
                        // if avl is active validate it now
                        if (chkEnterpolAvl.Checked)
                        {
                            if (!ValidateEnterpolAvlInput(
                                txtAliEnterpolDataSource.Text,
                                txtAliEnterpolAVLTableName.Text,
                                txtAliEnterpolAVLInitialCatalog.Text,
                                txtAliEnterpolAVLSetLocProc.Text,
                                txtAliEnterpolAVLWhoAmIProc.Text
                                ))
                            {
                                return;
                            }
                            MessageBox.Show(@"Enterpol Database/AVL settings Validated");
                        }
                        else
                        {
                            MessageBox.Show(stat + @"Enterpol Database settings Validated");
                        }
                    }
                    return;
                default: // disabled or if stat is > 0 then its network fleet validation
                    if (stat.Length != 0)
                    {
                        MessageBox.Show(stat.TrimEnd('/') + @" settings Validated");
                    }
                    return;
            }
        }

        private static bool ValidateEnterpolAvlInput(string server, string database, string table, string setLocProc, string whoAmIProc)
        {
            var msg = AppContext.Instance.Get<IUserMessage>();
            if (database.Length == 0)
            {
                MessageBox.Show(@"Database name value is null");
                return false;
            }
            if (table.Length == 0)
            {
                MessageBox.Show(@"Table/View value is null");
                return false;
            }
            if (SdrConfig.Settings.Instance.ApplicationMode == SdrConfig.AppMode.Responder)
            {
                if (setLocProc.Length == 0)
                {
                    MessageBox.Show(@"StoredProcedure 'SetMyLocation' value is null");
                    return false;
                }
                if (whoAmIProc.Length == 0)
                {
                    MessageBox.Show(@"StoredProcedure 'WhoAmI' value is null");
                    return false;
                }
            }
            try
            {
                // generate proper conn string and validate it
                var conn = SqlServerHelper.GetSqlServerConnectionString(
                    "Server=" + server + ";" +
                    "Database=" + database + ";" +
                    "Integrated Security=SSPI;" +
                    "connection timeout=15");

                if (!SqlServerHelper.TableExists(conn, table))
                {
                    msg.Warn(@"Table/View: " + table + " does not exist in the database: " + database);
                    return false;
                }
                if (SdrConfig.Settings.Instance.ApplicationMode == SdrConfig.AppMode.Responder)
                {
                    if (!SqlServerHelper.StoredProcedureExists(conn, setLocProc))
                    {
                        msg.Warn(@"StoredProcedure: " + setLocProc + " does not exist in the database: " + database);
                        return false;
                    }
                    if (!SqlServerHelper.StoredProcedureExists(conn, whoAmIProc))
                    {
                        msg.Warn(@"StoredProcedure: " + whoAmIProc + " does not exist in the database: " + database);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                msg.Error(ex.Message, ex);
                return false;
            }
            return true;
        }

        private static bool ValidateEnterpolInput(string server,  string database,  string table)
        {
            var msg = AppContext.Instance.Get<IUserMessage>();
            if (server.Length == 0)
            {
                MessageBox.Show(@"Database server location value is null");
                return false;
            }
            if (database.Length == 0)
            {
                MessageBox.Show(@"Database name value is null");
                return false;
            }
            if (table.Length == 0)
            {
                MessageBox.Show(@"Table/View value is null");
                return false;
            }
            try
            {
                // generate proper conn string and validate it
                var conn = SqlServerHelper.GetSqlServerConnectionString(
                    "Server=" + server + ";" +
                    "Database=" + database + ";" +
                    "Integrated Security=SSPI;" +
                    "connection timeout=15");

                if (!SqlServerHelper.TableExists(conn, table))
                {
                    msg.Warn(@"Table/View: " + table + " does not exist in the database: " + database);
                    return false;
                }
            }
            catch (Exception ex)
            {
                msg.Error(ex.Message, ex);
                return false;
            }
            return true;
        }

        private static bool ValidateNetworkfleetInput(string udpHost, int udpPort)
        {
            var msg = AppContext.Instance.Get<IUserMessage>();
            if (udpHost.Length == 0)
            {
                MessageBox.Show(@"Networkfleet UDP host value is null");
                return false;
            }
            // validate the networkfleet is at this location and listening
            var client = new AliServerClient(udpHost, udpPort);
            if (!client.Ping())
            {
                msg.Warn(@"Networkfleet is not responding at host: " + udpHost + @" port: " + udpPort);
                client.Close();
                return false;
            }
            client.Close();
            return true;
        }

        private static bool ValidateGlobalCadInput(string logPath, string archivePath, string configPath)
        {
            var msg = AppContext.Instance.Get<IUserMessage>();
            if (archivePath.Length == 0)
            {
                MessageBox.Show(@"Log Archive path value is null");
                return false;
            }
            if (logPath.Length == 0)
            {
                MessageBox.Show(@"Active Log path value is null");
                return false;
            }
            if (configPath.Length == 0)
            {
                MessageBox.Show(@"Parser Config path value is null");
                return false;
            }
            if (!Directory.Exists(archivePath))
            {
                var dr = MessageBox.Show(
                    @"Would you like to create the Archive Directory?",
                    @"Archive Directory Does not Exist", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    try
                    {
                        Directory.CreateDirectory(archivePath);
                    }
                    catch (Exception e)
                    {
                        msg.Error("Failed to create Archive Directory", e);
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show(@"Please create or select the existing Archive directory");
                    return false;
                }
            }
            if (!File.Exists(logPath))
            {
                msg.Warn(@"Log file does not exist");
                return false;
            }
            if (File.Exists(configPath))
            {
                var iniSection = SdrConfig.Plugins.GetPluginApplicationConfigValue(AliPlugin, "GlobalCadConfig", "IniKeyLookup");
                string[] allKeys = SDR.Data.Files.IniHelper.IniGetAllSectionKeys(iniSection, configPath);
                if (allKeys.Length == 1 && allKeys[0].Length == 0)
                {
                    msg.Warn("Config file is missing required section: [" + iniSection + "] values");
                    return false;
                }
            }
            else
            {
                msg.Warn(@"Parser Config file does not exist");
                return false;
            }
            return true;
        }

        private static bool ValidateSdrServerInput(string mdbPath, string udpHost, int udpPort)
        {
            var msg = AppContext.Instance.Get<IUserMessage>();
            if (mdbPath.Length == 0)
            {
                MessageBox.Show(@"AliServer .MDB path value is null");
                return false;
            }
            if (!MdbHelper.DatabaseFileExists(mdbPath))
            {
                MessageBox.Show(@"AliServer database does not exist at location: " + mdbPath);
                return false;
            }
            try
            {
                var conn = MdbHelper.GetMdbConnectionString(mdbPath);
                var tableName = SdrConfig.Plugins.GetPluginApplicationConfigValue(AliPlugin, "SdrAliServerConfig", "TableName");
                if (!MdbHelper.TableExists(conn, tableName))
                {
                    msg.Warn(@"AliServer database is missing table '" + tableName + "'");
                    return false;
                }
                if (udpHost.Length == 0)
                {
                    MessageBox.Show(@"AliServer UDP host value is null");
                    return false;
                }
                // validate the aliserver is at this location and listening
                var client = new AliServerClient(udpHost, udpPort);
                if (!client.Ping())
                {
                    msg.Warn(@"AliServer is not responding at host: " + udpHost + @" port: " + udpPort);
                    client.Close();
                    return false;
                }
                client.Close();
                return true;
            }
            catch (Exception ex)
            {
                msg.Error(ex.Message, ex);
                throw;
            }
        }

        private void btnAliGlobalCadArchivePathBrowse_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog {Description = @"Select GlobalCAD Archive Directory"};
            DialogResult r = fbd.ShowDialog();
            if (r == DialogResult.OK)
            {
                txtAliGlobalCadArchivePath.Text = fbd.SelectedPath;
            }
        }

        private void btnAliGlobalCadConfigIniBrowse_Click(object sender, EventArgs e)
        {
            FileDialog fd = new OpenFileDialog();
            fd.Filter = @"Ini Files|*.ini";
            fd.CheckFileExists = true;
            DialogResult r = fd.ShowDialog();
            if (r == DialogResult.OK)
            {
                txtAliGlobalCadConfigIni.Text = fd.FileName;
            }
        }

        private void btnAliNetworkfleetFont_Click(object sender, EventArgs e)
        {
            var fd = new FontDialog { Font = _networkFleetSymbolFont };
            if (fd.ShowDialog() != DialogResult.OK) return;

            lblAliNetworkfleetFont.Text = fd.Font.Name;
            lblAliNetworkfleetFont.Font = fd.Font;
            txtAliNetworkfleetSize.Text = fd.Font.Size.ToString(CultureInfo.InvariantCulture);
            if (!Equals(fd.Font, _networkFleetSymbolFont))
            {
                _projectManager.IsDirty = true;
            }
            _networkFleetSymbolFont = fd.Font;
            DrawCharacterGraphic(ptAliNetworkfleetGraphic, ptAliNetworkfleetColor.BackColor, txtAliNetworkfleetChar.Text);
        }

        private void chkNetworkfleet_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;
            if (chk.Checked)
            {
                if (chkEnterpolAvl.Checked)
                {
                    chkEnterpolAvl.Checked = false;
                }
            }
            // display/hide the column and set the networkfleet panels visibility
            if (chk.Checked)
            {
                aliPanelTableLayout.ColumnStyles[0].Width = 40;
                tblAliNetworkfleetSettings.Visible = true;
                tblAliNetworkfleetSymbology.Visible = true;
                tblAliNetworkfleetLabeling.Visible = true;
            }
            else
            {
                aliPanelTableLayout.ColumnStyles[0].Width = 0;
                tblAliNetworkfleetSettings.Visible = false;
                tblAliNetworkfleetSymbology.Visible = false;
                tblAliNetworkfleetLabeling.Visible = false;
            }
        }

        private void chkAvl_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;
            if (chk.Checked)
            {
                if (chkNetworkfleet.Checked)
                {
                    chkNetworkfleet.Checked = false;
                }
            }
            pnlAliEnterpolAvl.Visible = chk.Checked;
        }

        private void btnAliEnterpolAVLFont_Click(object sender, EventArgs e)
        {
            var fd = new FontDialog {Font = _enterpolAvlSymbolFont};
            if (fd.ShowDialog() != DialogResult.OK) return;

            lblAliEnterpolAVLSymbolFontName.Text = fd.Font.Name;
            lblAliEnterpolAVLSymbolFontName.Font = fd.Font;
            txtAliEnterpolAVLSymbolFontSize.Text = fd.Font.Size.ToString(CultureInfo.InvariantCulture);
            if (!Equals(fd.Font, _enterpolAvlSymbolFont))
            {
                _projectManager.IsDirty = true;
            }
            _enterpolAvlSymbolFont = fd.Font;
            DrawCharacterGraphic(pnlAliEnterpolAVLEmsGraphic, pnlAliEnterpolAVLEmsColor.BackColor, txtAliEnterpolAVLEmsChars.Text);
            DrawCharacterGraphic(pnlAliEnterpolAVLPdGraphic, pnlAliEnterpolAVLPdColor.BackColor, txtAliEnterpolAVLPdChars.Text);
            DrawCharacterGraphic(pnlAliEnterpolAVLFdGraphic, pnlAliEnterpolAVLFdColor.BackColor, txtAliEnterpolAVLFdChars.Text);
        }

        private void btnAliEnterpolAVLLabelFont_Click(object sender, EventArgs e)
        {
            var fd = new FontDialog {Font = _enterpolAvlLabelFont};
            if (fd.ShowDialog() != DialogResult.OK) return;

            lblAliEnterpolAVLLabelFontName.Text = fd.Font.Name;
            lblAliEnterpolAVLLabelFontName.Font = fd.Font;
            txtAliEnterpolAVLLabelFontSize.Text = fd.Font.Size.ToString(CultureInfo.InvariantCulture);
            if (!Equals(fd.Font, _enterpolAvlLabelFont))
            {
                _projectManager.IsDirty = true;
            }
            _enterpolAvlLabelFont = fd.Font;
        }

        private void cmbAliEnterpolAVLLabelAlignment_SelectedIndexChanged(object sender, EventArgs e)
        {
            // auto-set the default "best" offsets for each label alignment type
            var cmb = (ComboBox)sender;
            switch (cmb.SelectedItem.ToString())
            {
                case "Above":
                    numAliEnterpolAVLLabelXOffset.Value = 0;
                    numAliEnterpolAVLLabelYOffset.Value = -5;
                    break;
                default:
                    numAliEnterpolAVLLabelXOffset.Value = 0;
                    numAliEnterpolAVLLabelYOffset.Value = 0;
                    break;
            }
            _projectManager.IsDirty = true;
        }

        private void cmbAliNetworkfleetLabelAlignment_SelectedIndexChanged(object sender, EventArgs e)
        {
            // auto-set the default "best" offsets for each label alignment type
            var cmb = (ComboBox)sender;
            switch (cmb.SelectedItem.ToString())
            {
                case "Above":
                    numAliNetworkfleetLabelXOffset.Value = 0;
                    numAliNetworkfleetLabelYOffset.Value = -5;
                    break;
                default:
                    numAliNetworkfleetLabelXOffset.Value = 0;
                    numAliNetworkfleetLabelYOffset.Value = 0;
                    break;
            }
            _projectManager.IsDirty = true;
        }

        private void btnAliNetworkfleetLabelFont_Click(object sender, EventArgs e)
        {
            var fd = new FontDialog { Font = _networkFleetLabelFont };
            if (fd.ShowDialog() != DialogResult.OK) return;

            lblAliNetworkfleetLabelFont.Text = fd.Font.Name;
            lblAliNetworkfleetLabelFont.Font = fd.Font;
            txtAliNetworkfleetLabelFontSize.Text = fd.Font.Size.ToString(CultureInfo.InvariantCulture);

            if (!Equals(fd.Font, _networkFleetLabelFont))
            {
                _projectManager.IsDirty = true;
            }
            _networkFleetLabelFont = fd.Font;
        }

        private void btnMapTipsAddLayer_Click(object sender, EventArgs e)
        {
            //tblData.Controls.Clear();
            ////Set the new count
            //tblData.RowCount = tblData.RowCount += 1;
            //for (int i = 0; i < tblData.RowCount; i++)
            //{
            //    tblData.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            //    Label lblLabel = new Label();
            //    //This is just to have text that is smaller then the rest.
            //    //Just to see how the table reacts.
            //    if ((i > 3) & (i < 6))
            //        lblLabel.Text = "Label: " + i.ToString();
            //    else
            //        lblLabel.Text = "Label control: " + i.ToString();
            //    lblLabel.Font = new Font("Verdana", 8, FontStyle.Bold | FontStyle.Regular);
            //    lblLabel.AutoSize = true;
            //    Label lblData = new Label();
            //    lblData.Text = "Data for entity (" + i.ToString() + ")";
            //    lblData.Font = new Font("Verdana", 8, FontStyle.Regular);
            //    lblData.AutoSize = true;
            //    tblData.Controls.Add(lblLabel, 0, i);
            //    tblData.Controls.Add(lblData, 1, i);
            //}
        }
    }
}