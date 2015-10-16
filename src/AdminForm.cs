using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
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
using Lucene.Net.QueryParsers;
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
using ILog = SDR.Common.logging.ILog;
using Point = System.Drawing.Point;
using PointShape = DotSpatial.Symbology.PointShape;
using SdrConfig = SDR.Configuration;

namespace Go2It
{
    public partial class AdminForm : Form
    {
        // name of the initial map tab, if no map tabs currently exist
        private const string MapTabDefaultCaption = "My Map";

        // all available ali interfaces, the key is for display and the value matches the alimodes enum in ali plugin
        private readonly Dictionary<string, string> _aliInterfaces = new Dictionary<string, string>
        {
            { "Disabled", "Disabled" },
            { "SDR AliServer", "Sdraliserver" },
            { "GlobalCAD Log", "Globalcad" },
            { "Enterpol Database", "Enterpol" }
        };

        // admin form controls
        private Legend _adminLegend;
        private readonly AppManager _appManager;
        private readonly DockingControl _dockingControl;
        private readonly ProjectManager _projectManager;

        // internal lookup names used by lucene to get feature from the dataset also stores ft shape (normalized)
        private const string FID = "FID";
        private const string LYRNAME = "LYRNAME";
        private const string GEOSHAPE = "GEOSHAPE";

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
                System.Security.AccessControl.DirectorySecurity ds = System.IO.Directory.GetAccessControl(folderPath);
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
            if (System.IO.Directory.Exists(directoryName)) return;
            try
            {
                System.IO.Directory.CreateDirectory(directoryName);
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

        public AdminForm(AppManager app)
        {
            InitializeComponent();
            InitializeSaveSplitButton();

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

            // watch for changes of index on the pull down map tab change
            cmbActiveMapTab.SelectedIndexChanged += CmbActiveMapTabOnSelectedIndexChanged;

            FormClosing += AdminForm_Closing; // check for isdirty changes to project file
            FormClosed += AdminFormClosed;

            _projectManager.Serializing += ProjectManagerOnSerializing;

            // setup a background worker for update progress bar on indexing tab
            _idxWorker.DoWork += idx_DoWork;
            _idxWorker.RunWorkerCompleted += idx_RunWorkerCompleted;

            _projectManager.IsDirty = false;
            // TODO: investigate this flag further
            SdrConfig.User.Go2ItUserSettings.Instance.AdminModeActive = true;
            _appManager.DockManager.HidePanel(SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel);
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
                    if (System.IO.Directory.Exists(tdir))
                    {
                        System.IO.Directory.Delete(tdir, true);
                    }
                    // check if it exists in the project storage dir
                    var pdir = Path.Combine(_projectManager.CurrentProjectDirectory, projectName + "_indexes", idxType);
                    if (System.IO.Directory.Exists(pdir))
                    {
                        System.IO.Directory.Delete(pdir, true);
                        // remove the whole index dir if there are no indexes present
                        var intdir = Path.Combine(_projectManager.CurrentProjectDirectory, projectName + "_indexes");
                        var dirarr = System.IO.Directory.GetDirectories(intdir);
                        if (dirarr.Length == 0)
                        {
                            System.IO.Directory.Delete(intdir);
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
                if (System.IO.Directory.Exists(src))
                {
                    if (System.IO.Directory.Exists(dst))
                    {
                        System.IO.Directory.Delete(dst, true);
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
            if (!System.IO.Directory.Exists(destDirName))
            {
                System.IO.Directory.CreateDirectory(destDirName);
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
            UnbindGraphicElementEvents();

            legendSplitter.Panel1.Controls.Remove(_adminLegend);
            SdrConfig.User.Go2ItUserSettings.Instance.AdminModeActive = false;

            _dockingControl.SelectPanel(SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel);
        }

        private void UnbindGraphicElementEvents()
        {
            ptGpsColorSlider.ValueChanged -= PtGpsColorSliderOnValueChanged;
            ptGpsColor.Click -= PtGpsColorOnClick;
            ptGpsSize.ValueChanged -= PtGpsSizeOnValueChanged;
            ptGpsStyle.SelectedIndexChanged -= PtGpsStyleOnSelectedIndexChanged;
            ptSymbolColorSlider.ValueChanged -= PtSymbolColorSliderOnValueChanged;
            ptSymbolColor.Click -= PtSymbolColorOnClick;
            ptSymbolSize.ValueChanged -= PtSymbolSizeOnValueChanged;
            ptSymbolStyle.SelectedIndexChanged -= PtSymbolStyleOnSelectedIndexChanged;
            lineSymbolBorderColor.Click -= LineSymbolBorderColorOnClick;
            lineSymbolColorSlider.ValueChanged -= LineSymbolColorSliderOnValueChanged;
            lineSymbolColor.Click -= LineSymbolColorOnClick;
            lineSymbolSize.ValueChanged -= LineSymbolSizeOnValueChanged;
            lineSymbolCap.SelectedIndexChanged -= LineSymbolCapOnSelectedIndexChanged;
            lineSymbolStyle.SelectedIndexChanged -= LineSymbolStyleOnSelectedIndexChanged;
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
            AddLayer(fs); // perform all form specific add operations
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
                RemoveLayer(fs); // perform all form specific remove operations
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

        private void UpdateGpsGraphics(Map map)
        {
            PointShape ptShape;  // parse out point shape style
            Enum.TryParse(ptGpsStyle.SelectedItem.ToString(), true, out ptShape);
            var pLyr = map.MapFrame.DrawingLayers[0] as MapPointLayer;
            if (pLyr != null)
            {
                pLyr.Symbolizer = new PointSymbolizer(ptGpsColor.BackColor,
                    ptShape, Convert.ToInt32(ptGpsSize.Text));
            }
            map.BackColor = mapBGColorPanel.BackColor;
            map.MapFrame.Invalidate();
        }


        private void DrawGpsPointGraphics()
        {
            Map gpsMap;
            if (ptGpsGraphic.Controls.Count != 0)
            {
                gpsMap = ptGpsGraphic.Controls[0] as Map;
                UpdateGpsGraphics(gpsMap);
            }
            else
            {
                gpsMap = new Map
                {
                    ViewExtents = new Envelope(-130, -60, 10, 55).ToExtent(),
                    FunctionMode = FunctionMode.None,
                };
                gpsMap.MapFunctions.Clear(); // clear all built in map functions (nav/zoom/etc)
                ptGpsGraphic.Controls.Add(gpsMap);

                var ftSet = new FeatureSet(FeatureType.Point);
                var ftLyr = new MapPointLayer(ftSet);
                gpsMap.MapFrame.DrawingLayers.Add(ftLyr);

                // get the center of the control panel (location to render point)
                var y = ((ptGpsGraphic.Bottom - ptGpsGraphic.Top) / 2) - 1;
                var x = ((ptGpsGraphic.Right - ptGpsGraphic.Left) / 2) - 1;
                var c = gpsMap.PixelToProj(new Point(x, y));
                ftSet.AddFeature(new DotSpatial.Topology.Point(c));
            }
            UpdateGpsGraphics(gpsMap);
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
            // point symbology for graphics rendering
            Color pColor = SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicPointColor;
            Color gpsColor = SdrConfig.Project.Go2ItProjectSettings.Instance.GpsPointColor;
            ptSymbolColorSlider.Value = pColor.GetOpacity();
            ptGpsColorSlider.Value = gpsColor.GetOpacity();
            ptSymbolColorSlider.MaximumColor = Color.FromArgb(255, pColor.R, pColor.G, pColor.B);
            ptGpsColorSlider.MaximumColor = Color.FromArgb(255, gpsColor.R, gpsColor.G, gpsColor.B);
            ptSymbolColorSlider.ValueChanged += PtSymbolColorSliderOnValueChanged;
            ptGpsColorSlider.ValueChanged += PtGpsColorSliderOnValueChanged;
            ptSymbolColor.BackColor = pColor;
            ptGpsColor.BackColor = gpsColor;
            ptSymbolColor.Click += PtSymbolColorOnClick;
            ptGpsColor.Click += PtGpsColorOnClick;
            ptSymbolSize.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicPointSize;
            ptGpsSize.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.GpsPointSize;
            ptSymbolSize.ValueChanged += PtSymbolSizeOnValueChanged;
            ptGpsSize.ValueChanged += PtGpsSizeOnValueChanged;
            foreach (PointShape ptShape in Enum.GetValues(typeof(PointShape)))
            {
                if (ptShape.ToString().ToUpper() != "UNDEFINED")
                {
                    ptSymbolStyle.Items.Add(ptShape.ToString());
                    ptGpsStyle.Items.Add(ptShape.ToString());
                }
            }
            var idx = ptSymbolStyle.Items.IndexOf(SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicPointStyle);
            ptSymbolStyle.SelectedIndex = idx;
            ptSymbolStyle.SelectedIndexChanged += PtSymbolStyleOnSelectedIndexChanged;
            idx = ptGpsStyle.Items.IndexOf(SdrConfig.Project.Go2ItProjectSettings.Instance.GpsPointStyle);
            ptGpsStyle.SelectedIndex = idx;
            ptGpsStyle.SelectedIndexChanged += PtGpsStyleOnSelectedIndexChanged;
            DrawPointGraphics();
            DrawGpsPointGraphics();

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

        private void PtGpsStyleOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            DrawGpsPointGraphics();
            _projectManager.IsDirty = true;
        }

        private void PtGpsSizeOnValueChanged(object sender, EventArgs eventArgs)
        {
            DrawGpsPointGraphics();
            _projectManager.IsDirty = true;
        }

        private void PtGpsColorOnClick(object sender, EventArgs eventArgs)
        {
            var oColor = ptGpsColor.BackColor;
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;

            // update the slider max color value for display
            ptGpsColorSlider.MaximumColor = Color.FromArgb(255, dlg.Color.R, dlg.Color.G, dlg.Color.B);
            // update the color and map display with new color accounting for alpha
            int alpha = Convert.ToInt32(ptGpsColorSlider.Value * 255);
            ptGpsColor.BackColor = Color.FromArgb(alpha, dlg.Color.R, dlg.Color.G, dlg.Color.B);
            if (oColor != ptGpsColor.BackColor)
            {
                _projectManager.IsDirty = true;
            }
            DrawGpsPointGraphics();
        }

        private void PtGpsColorSliderOnValueChanged(object sender, EventArgs eventArgs)
        {
            int alpha = Convert.ToInt32(ptGpsColorSlider.Value * 255);
            ptGpsColor.BackColor = Color.FromArgb(alpha, ptGpsColor.BackColor.R, ptGpsColor.BackColor.G, ptGpsColor.BackColor.B);
            DrawGpsPointGraphics();
            _projectManager.IsDirty = true;
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
            txtAliEnterpolConnString.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolConnectionString;
            txtAliEnterpolDataSource.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolDataSource;
            txtAliEnterpolInitialCatalog.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolInitialCatalog;
            txtAliEnterpolTableName.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolTableName;
            txtAliGlobalCadLogPath.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadLogPath;
            txtAliGlobalCadArchivePath.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadArchivePath;
            txtAliInterfaceDbPath.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerDbPath;
            txtAliInterfaceUdpHost.Text = SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpHost;
            numAliInterfaceUdpPort.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpPort;
            // populate all the ali interfaces to the combobox
            foreach (var aliInterface in _aliInterfaces)
            {
                cmbAliMode.Items.Add(aliInterface.Key);
            }
            var aliMode = SdrConfig.Project.Go2ItProjectSettings.Instance.AliMode;
            cmbAliMode.SelectedIndex = cmbAliMode.Items.IndexOf(aliMode);
            var gpsIntType = SdrConfig.Project.Go2ItProjectSettings.Instance.GpsIntervalType;
            if (gpsIntType == "Time")
            {
                gpsSelectTime.Checked = true;
                gpsIntervalTime.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.GpsIntervalValue;
            }
            else
            {
                gpsSelectCount.Checked = true;
                gpsIntervalCount.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.GpsIntervalValue;
            }
            gpsDisplayPointCount.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.GpsDisplayCount;
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
            SdrConfig.Project.Go2ItProjectSettings.Instance.ResetProjectSettings();  // set all project settings to defaults
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
                    bool result;
                    if (String.IsNullOrEmpty(_projectManager.CurrentProjectFile))
                    {
                        result = SaveProjectAs();
                    }
                    else
                    {
                        result = SaveProject(_projectManager.CurrentProjectFile);
                    }
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
                MessageBox.Show(@"Layer Indexing Operation is running, please wait or cancel to process");
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
            // set the gps symbology styles
            SdrConfig.Project.Go2ItProjectSettings.Instance.GpsPointColor = ptGpsColor.BackColor;
            SdrConfig.Project.Go2ItProjectSettings.Instance.GpsPointStyle = ApplyComboBoxSetting(ptGpsStyle);
            SdrConfig.Project.Go2ItProjectSettings.Instance.GpsPointSize = Convert.ToInt32(ptGpsSize.Text);
            SdrConfig.Project.Go2ItProjectSettings.Instance.GpsDisplayCount = Convert.ToInt32(gpsDisplayPointCount.Text);
            if (gpsSelectCount.Checked)
            {
                SdrConfig.Project.Go2ItProjectSettings.Instance.GpsIntervalType = "Count";
                SdrConfig.Project.Go2ItProjectSettings.Instance.GpsIntervalValue = Convert.ToInt32(gpsIntervalCount.Text);
            }
            else
            {
                SdrConfig.Project.Go2ItProjectSettings.Instance.GpsIntervalType = "Time";
                SdrConfig.Project.Go2ItProjectSettings.Instance.GpsIntervalValue = Convert.ToInt32(gpsIntervalTime.Text);
            }
            // setup ali interface configuration
            string aliValue;
            _aliInterfaces.TryGetValue(cmbAliMode.SelectedItem.ToString(), out aliValue);
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliMode = aliValue;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolInitialCatalog = txtAliEnterpolInitialCatalog.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolTableName = txtAliEnterpolTableName.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolDataSource = txtAliEnterpolDataSource.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolConnectionString = txtAliEnterpolConnString.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadLogPath = txtAliGlobalCadLogPath.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadArchivePath = txtAliGlobalCadArchivePath.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerDbPath = txtAliInterfaceDbPath.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpHost = txtAliInterfaceUdpHost.Text;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpPort =Convert.ToInt32(numAliInterfaceUdpPort.Value);
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
            DrawGpsPointGraphics();
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

            bool success;
            if (lstUsers.Items.Contains(txtUsername.Text))
            {
                // perform an update on an exiting user
                success = SQLiteHelper.Update(conn, "logins", d, "username ='" + txtUsername.Text + "'");
            }
            else
            {
                // add a new user to the database
                lstUsers.Items.Add(txtUsername.Text);
                success = SQLiteHelper.Insert(conn, "logins", d);
            }
            if (!success) return;
            txtPassword.Text = string.Empty;
            txtVerifyPassword.Text = string.Empty;
            txtUsername.Text = string.Empty;
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
            bool success = SQLiteHelper.Delete(conn, "logins", "username ='" + txtUsername.Text + "'");
            if (!success) return;
            txtPassword.Text = string.Empty;
            txtVerifyPassword.Text = string.Empty;
            txtUsername.Text = string.Empty;
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
                    doc.Add(new Field(FID, dr[FID].ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    doc.Add(new Field(LYRNAME, o.LayerName, Field.Store.YES, Field.Index.NOT_ANALYZED));

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
                        SpatialStrategy strategy = new RecursivePrefixTreeStrategy(new GeohashPrefixTree(ctx, 24), GEOSHAPE);
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
                            LogGeometryIndexError(o.LayerName, dr[FID].ToString(), shp, wkt, ex);
                            var msg = AppContext.Instance.Get<IUserMessage>();
                            msg.Error(
                                "Error creating index :: FeatureClass: " + o.LayerName + " FeatureID: " + dr[FID], ex);
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
                            var fid = document.GetField(FID).StringValue;
                            var lyr = document.GetField(LYRNAME).StringValue;

                            Query qfid = new TermQuery(new Term(FID, fid));
                            Query qlyr = new TermQuery(new Term(LYRNAME, lyr));

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

        private void gpsSelectCount_CheckedChanged(object sender, EventArgs e)
        {
            if (gpsSelectCount.Checked)
            {
                gpsIntervalCount.Enabled = true;
                gpsIntervalTime.Enabled = false;
            }
        }

        private void gpsSelectTime_CheckedChanged(object sender, EventArgs e)
        {
            if (gpsSelectTime.Checked)
            {
                gpsIntervalTime.Enabled = true;
                gpsIntervalCount.Enabled = false;
            }
        }

        private void cmbAliMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlAliEnterpol.Visible = false;
            pnlAliGlobalCad.Visible = false;
            pnlAliSdrAliServer.Visible = false;

            switch (cmbAliMode.Text)
            {
                case "GlobalCAD Log":
                    pnlAliGlobalCad.Visible = true;
                    return;
                case "SDR AliServer":
                    pnlAliSdrAliServer.Visible = true;
                    return;
                case "Enterpol Database":
                    pnlAliEnterpol.Visible = true;
                    return;
                default: // disabled
                    return;
            }
        }

        private void btnAliInterfaceDbPathBrowse_Click(object sender, EventArgs e)
        {
            FileDialog fd = new OpenFileDialog();
            fd.Filter = "MDB Databases|*.mdb";
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
            fd.Filter = @"Log Files (*.log)";
            fd.CheckFileExists = true;
            DialogResult r = fd.ShowDialog();
            if (r == DialogResult.OK)
            {
                txtAliGlobalCadLogPath.Text = fd.FileName;
            }
        }

        private void btnAliValidate_Click(object sender, EventArgs e)
        {
            switch (cmbAliMode.Text)
            {
                case "GlobalCAD Log":
                    MessageBox.Show("Not Implemented Yet");
                    return;
                case "SDR AliServer":
                    if (ValidateSdrServerInput(
                        txtAliInterfaceDbPath.Text,
                        txtAliInterfaceUdpHost.Text,
                        Convert.ToInt32(numAliInterfaceUdpPort.Value)))
                    {
                        MessageBox.Show(@"Valid Settings");
                    }
                    return;
                case "Enterpol Database":
                    MessageBox.Show("Not Implemented Yet");
                    return;
                default: // disabled
                    return;
            }
        }

        private bool ValidateSdrServerInput(string mdbPath, string udpHost, int udpPort)
        {
            if (mdbPath.Length == 0)
            {
                MessageBox.Show(@"AliServer .MDB path value is null");
                return false;
            }
            if (!MdbHelper.DatabaseExists(mdbPath))
            {
                MessageBox.Show(@"AliServer database does not exist at location: " + mdbPath);
                return false;
            }
            var conn = MdbHelper.GetMdbConnectionString(mdbPath);
            if (!MdbHelper.TableExists(conn, "IncomingALI"))
            {
                MessageBox.Show(@"AliServer database is missing table 'IncomingALI'");
                return false;
            }
            if (udpHost.Length == 0)
            {
                MessageBox.Show(@"AliServer UDP host value is null");
                return false;
            }
            // validate the aliserver is at this location and listening
            var client = new AliServerClient(txtAliInterfaceUdpHost.Text.ToString(), Convert.ToInt32(numAliInterfaceUdpPort.Value));
            if (!client.Ping())
            {
                MessageBox.Show(@"AliServer is not responding at host: " + udpHost + @" port: " + udpPort.ToString());
                client.Close();
                return false;
            }
            client.Close();
            return true;
        }
    }
}