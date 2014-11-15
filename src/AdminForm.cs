using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
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
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Spatial;
using Lucene.Net.Spatial.Prefix;
using Lucene.Net.Spatial.Prefix.Tree;
using Lucene.Net.Store;
using SDR.Common;
using SDR.Common.UserMessage;
using Spatial4n.Core.Context.Nts;
using Version = Lucene.Net.Util.Version;
using Directory = Lucene.Net.Store.Directory;
using Field = Lucene.Net.Documents.Field;
using DotSpatial.Data;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls;
using SDR.Authentication;
using SDR.Data.Database;
using Go2It.Properties;
using IGeometry = DotSpatial.Topology.IGeometry;
using ILog = SDR.Common.logging.ILog;
using Point = System.Drawing.Point;
using PointShape = DotSpatial.Symbology.PointShape;
using SdrConfig = SDR.Configuration;

namespace Go2It
{
    public partial class AdminForm : Form
    {
        [Import("Shell")]
        internal ContainerControl Shell { get; set; }

        private const string MapTabDefaultCaption = "My Map";  // name of the initial map tab (default)
        // internal lookup names used by lucene to get feature from the dataset also stores ft shape (normalized)
        private const string FID = "FID";
        private const string LYRNAME = "LYRNAME";
        private const string GEOSHAPE = "GEOSHAPE";

        private bool _dirtyProject;  // change tracking flag for project changes
        private bool _isCreatingNewView;  // prevent circular tab selection when adding a new map view tab

        // default sql row creation for an indexing row in the db (key, lookup, fieldname)
        private readonly Dictionary<string, string> _indexLookupFields = new Dictionary<string, string>();
        // background worker handles the indexing process
        private readonly BackgroundWorker _idxWorker = new BackgroundWorker();
        private ProgressPanel _progressPanel;  // indicate progress of index worker
        // invalid file name chars array for validation
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

        // admin form controls
        private Legend _adminLegend;
        private readonly AppManager _appManager;
        private readonly DockingControl _dockingControl;
        private readonly Map _baseMap;

        // store all the layers by type
        private readonly List<IFeatureSet> _pointLayers = new List<IFeatureSet>();
        private readonly List<IFeatureSet> _polygonLayers = new List<IFeatureSet>();
        private readonly List<IFeatureSet> _lineLayers = new List<IFeatureSet>();

        // switch handlers to control state of check/uncheck controls on admin form
        private readonly SelectionsHandler _pointLayerSwitcher = new SelectionsHandler();
        private readonly SelectionsHandler _lineLayerSwitcher = new SelectionsHandler();
        private readonly SelectionsHandler _polygonLayerSwitcher = new SelectionsHandler();

        // temp dict holds all base map layers for selection/removal on map tabs
        // on save this is passed to the dockingcontrol baselayerlookup dict (basically a lookup for all available layers)
        private readonly Dictionary<string, IMapLayer> _localBaseMapLayerLookup = new Dictionary<string, IMapLayer>();
        
        // temp storage of layers to index until the "create" button is activated (all queued indexes)
        // inner dict hold type/lookups per row
        // the list stores all rows
        // the outer dict holds layer name and list with all dicts
        private readonly Dictionary<string, List<Dictionary<string, string>>> _indexQueue = new Dictionary<string, List<Dictionary<string, string>>>();

        private static readonly Regex DigitsOnly = new Regex(@"[^\d]");

        public AdminForm(AppManager app)
        {
            InitializeComponent();
            InitializeSaveSplitButton();

            // setup the default indexing field names
            _indexLookupFields.Add("key", "INTEGER PRIMARY KEY");
            _indexLookupFields.Add("lookup", "TEXT");
            _indexLookupFields.Add("fieldname", "TEXT");

            _appManager = app;
            _dockingControl = (DockingControl) app.DockManager;

            _baseMap = CreateNewMap("_baseMap");
            _baseMap.MapFunctions.Clear(); // remove all map functions from basemap

            // set options on our indexing bgworker
            _idxWorker.WorkerReportsProgress = false;
            _idxWorker.WorkerSupportsCancellation = false;

            // splitter stuff
            adminLayerSplitter.SplitterWidth = 10;
            adminLayerSplitter.Paint += Splitter_Paint;

            // overall events tied to main application
            _appManager.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;

            // map tracking events for removal and addition of layers
            _baseMap.Layers.LayerRemoved += LayersOnLayerRemoved;
            _baseMap.Layers.LayerAdded += LayersOnLayerAdded;

            // populate all the settings, layers, and maps to the form and attach a legend
            AttachLegend();
            _adminLegend.OrderChanged += AdminLegendOnOrderChanged;

            PopulateMapViews();
            PopulateSettingsToForm();
            PopulateUsersToForm();
            PopulateHotKeysToForm();
            PopulateIndexesToForm();
            PopulateGraphicsToForm();

            // check if there is a valid map loaded to the application
            if (_appManager.Map == null)
            {
                // create the default map view and assign to app.map
                const string caption = MapTabDefaultCaption;
                var kname = new string(caption.Where(ch => !InvalidFileNameChars.Contains(ch)).ToArray());
                kname = kname.Replace(" ", "");
                var key = "kMap_" + kname;
                var nMap = CreateNewMap(key);
                app.Map = nMap;  // assign this as the active map to the application

                // create new dockable panel to hold the map
                var dp = new DockablePanel(key, caption, nMap, DockStyle.Fill);
                cmbActiveMapTab.Items.Add(dp.Caption);  // add this map to the map view selections combo
                cmbActiveMapTab.SelectedIndex = 0;

                if (string.IsNullOrEmpty(_appManager.SerializationManager.CurrentProjectFile))
                {
                    _appManager.SerializationManager.New();  // create the actual serialized map info now
                }
                // add the new tab view to the main form
                _appManager.DockManager.Add(dp);
                // select the map now to activate plugin bindings
                _appManager.DockManager.SelectPanel(key);
            }

            // setup all interface events now
            cmbActiveMapTab.SelectedIndexChanged += CmbActiveMapTabOnSelectedIndexChanged;
            FormClosing += AdminForm_Closing; // check for isdirty changes to project file
            FormClosed += AdminFormClosed;
            chkViewLayers.ItemCheck += chkViewLayers_ItemCheck; // add or remove item to specific map tab view

            // setup a background worker for update progress bar on indexing tab
            _idxWorker.DoWork += idx_DoWork;
            _idxWorker.RunWorkerCompleted += idx_RunWorkerCompleted;

            _dirtyProject = false; // reset dirty flag after populating form on startup

            SdrConfig.User.Go2ItUserSettings.Instance.AdminModeActive = true;
            _appManager.DockManager.HidePanel(SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel);
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
                _appManager.DockManager.SelectPanel(key);
            }
        }

        // TODO: we can safely remove our map loggers
        private void LogMapEvents(IMap map, string name)
        {
            //map.FinishedRefresh += (sender, args) => Debug.WriteLine(name + " Map.FinishedRefresh::AdminForm");
            //map.FunctionModeChanged += (sender, args) => Debug.WriteLine(name + " Map.FunctionModeChanged::AdminForm");
            map.LayerAdded += (sender, args) => Debug.WriteLine(name + " Map.LayerAdded::AdminForm");
            //map.SelectionChanged += (sender, args) => Debug.WriteLine(name + " Map.SelectionChanged::AdminForm");
            //map.Resized += (sender, args) => Debug.WriteLine(name + " Map.Resized::AdminForm");
        }

        private void LogMapFrameEvents(IMapFrame mapframe, string name)
        {
            //mapframe.BufferChanged += (sender, args) => Debug.WriteLine(name + " MapFrame.BufferChanged::AdminForm");
            //mapframe.EnvelopeChanged += (sender, args) => Debug.WriteLine(name + " MapFrame.EnvelopeChanged::AdminForm");
            //mapframe.FinishedLoading += (sender, args) => Debug.WriteLine(name + " MapFrame.FinishedLoading::AdminForm");
            //mapframe.FinishedRefresh += (sender, args) => Debug.WriteLine(name + " MapFrame.FinishedRefresh::AdminForm");
            //mapframe.Invalidated += (sender, args) => Debug.WriteLine(name + " MapFrame.Invalidated::AdminForm");
            //mapframe.ItemChanged += (sender, args) => Debug.WriteLine(name + " MapFrame.ItemChanged::AdminForm");
            mapframe.LayerAdded += (sender, args) => Debug.WriteLine(name + " MapFrame.LayerAdded::AdminForm");
            mapframe.LayerRemoved += (sender, args) => Debug.WriteLine(name + " MapFrame.LayerRemoved::AdminForm");
            //mapframe.LayerSelected += (sender, args) => Debug.WriteLine(name + " MapFrame.LayerSelected::AdminForm");
            //mapframe.RemoveItem += (sender, args) => Debug.WriteLine(name + " MapFrame.RemoveItem::AdminForm");
            //mapframe.ScreenUpdated += (sender, args) => Debug.WriteLine(name + " MapFrame.ScreenUpdated::AdminForm");
            //mapframe.SelectionChanged += (sender, args) => Debug.WriteLine(name + " MapFrame.SelectionChanged::AdminForm");
            //mapframe.ShowProperties += (sender, args) => Debug.WriteLine(name + " MapFrame.ShowProperties::AdminForm");
            mapframe.UpdateMap += (sender, args) => Debug.WriteLine(name + " MapFrame.UpdateMap::AdminForm");
            //mapframe.ViewChanged += (sender, args) => Debug.WriteLine(name + " MapFrame.ViewChanged::AdminForm");
            mapframe.ViewExtentsChanged += (sender, args) => Debug.WriteLine(name + " MapFrame.ViewExtentsChanged::AdminForm");
            //mapframe.VisibleChanged += (sender, args) => Debug.WriteLine(name + " MapFrame.VisibleChanged::AdminForm");
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

        private Map CreateNewMap(String mapName)
        {
            var map = new Map(); // new map 
            var mapframe = new EventMapFrame(); // evented map frame so we can disable visualextent events
            mapframe.SuspendViewExtentChanged();  // suspend view-extents while map is not active

            map.MapFrame = mapframe;  // set the new evented mapframe to the map mapframe
            map.BackColor = mapBGColorPanel.BackColor;
            map.Visible = mapName != "_baseMap";  // set visibility to false if it is the _basemap
            map.Dock = DockStyle.Fill;

            return map;
        }

        private void AdminFormClosed(object sender, FormClosedEventArgs formClosedEventArgs)
        {
            // unbind all our events now
            adminLayerSplitter.Paint -= Splitter_Paint;
            _appManager.DockManager.ActivePanelChanged -= DockManager_ActivePanelChanged;
            _baseMap.Layers.LayerRemoved -= LayersOnLayerRemoved;
            _baseMap.Layers.LayerAdded -= LayersOnLayerAdded;
            _adminLegend.OrderChanged -= AdminLegendOnOrderChanged;
            // remove the legend from the control, otherwise leaves disposed object behind
            legendSplitter.Panel1.Controls.Remove(_adminLegend);
            FormClosing -= AdminForm_Closing;
            chkViewLayers.ItemCheck -= chkViewLayers_ItemCheck;
            _idxWorker.DoWork -= idx_DoWork;
            _idxWorker.RunWorkerCompleted -= idx_RunWorkerCompleted;
            FormClosed -= AdminFormClosed;
            
            SdrConfig.User.Go2ItUserSettings.Instance.AdminModeActive = false;
            _appManager.DockManager.SelectPanel(SdrConfig.User.Go2ItUserSettings.Instance.ActiveFunctionPanel);
        }

        private void LayersOnLayerAdded(object sender, LayerEventArgs layerEventArgs)
        {
            var layer = (IMapLayer) layerEventArgs.Layer;
            if (layer == null) return;

            string fileName;
            if (layer.GetType().Name == "MapImageLayer")
            {
                var mImage = (IMapImageLayer) layer;
                if (String.IsNullOrEmpty(Path.GetFileNameWithoutExtension(mImage.Image.Filename))) return;
                fileName = Path.GetFileNameWithoutExtension(mImage.Image.Filename);
                if (fileName != null) _localBaseMapLayerLookup.Add(fileName, mImage);
                _dirtyProject = true;
            }
            else
            {
                var mLayer = (IMapFeatureLayer) layer;
                if (String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((mLayer.DataSet.Filename)))) return;
                fileName = Path.GetFileNameWithoutExtension(mLayer.DataSet.Filename);
                if (fileName != null) _localBaseMapLayerLookup.Add(fileName, mLayer);
                _dirtyProject = true;
            }
            if (layer.GetType().Name != "MapPointLayer" && layer.GetType().Name != "MapPolygonLayer" &&
                layer.GetType().Name != "MapLineLayer") return;
            if (fileName == null) return;

            var mMapLayer = (IMapFeatureLayer) layer;
            var fs = FeatureSet.Open(mMapLayer.DataSet.Filename);
            AddLayer(fs); // perform all form specific add operations
        }

        private void LayersOnLayerRemoved(object sender, LayerEventArgs layerEventArgs)
        {
            var layer = (IMapLayer) layerEventArgs.Layer;
            if (layer == null) return;
            string fileName;
            if (layer.GetType().Name == "MapImageLayer")
            {
                var mImage = (IMapImageLayer) layer;
                if (String.IsNullOrEmpty(Path.GetFileNameWithoutExtension(mImage.Image.Filename))) return;
                fileName = Path.GetFileNameWithoutExtension(mImage.Image.Filename);
                if (fileName != null) _localBaseMapLayerLookup.Remove(fileName);
                _dirtyProject = true;
            }
            else
            {
                var mLayer = (IMapFeatureLayer) layer;
                if (String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((mLayer.DataSet.Filename)))) return;
                fileName = Path.GetFileNameWithoutExtension(mLayer.DataSet.Filename);
                if (fileName != null) _localBaseMapLayerLookup.Remove(fileName);
                _dirtyProject = true;
                // remove any layers that could be part of active config now
                if ((mLayer.DataSet.FeatureType.ToString() != "Point" &&
                     mLayer.DataSet.FeatureType.ToString() != "Line" &&
                     mLayer.DataSet.FeatureType.ToString() != "Polygon")) return;
                var fs = FeatureSet.Open(mLayer.DataSet.Filename);
                RemoveLayer(fs); // perform all form specific remove operations
            }
            // we need to cycle through all the available dockpanels and check if the layer has been set on that map
            foreach (KeyValuePair<string, DockPanelInfo> dockPanelInfo in _dockingControl.DockPanelLookup)
            {
                if (!dockPanelInfo.Key.Trim().StartsWith("kMap")) continue;
                var map = (Map)dockPanelInfo.Value.DotSpatialDockPanel.InnerControl;
                map.Layers.Remove(layer);
                // now set the map back to itself
                dockPanelInfo.Value.DotSpatialDockPanel.InnerControl = map;
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

        private void DockManager_ActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            var dockControl = (DockingControl) sender;
            var key = e.ActivePanelKey;
            DockPanelInfo dockInfo;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;
            if (dockInfo.DotSpatialDockPanel.Key.StartsWith("kMap_"))
            {
                var caption = dockInfo.DotSpatialDockPanel.Caption;
                var idx = cmbActiveMapTab.Items.IndexOf(caption);
                if (idx >= 0)
                {
                    cmbActiveMapTab.SelectedIndex = idx;
                }
                chkViewLayers.Items.Clear();
                // populate all the layers available to the checkbox
                foreach (ILayer layer in _baseMap.MapFrame.GetAllLayers())
                {
                    if (layer == null) continue;
                    string fileName;
                    if (layer.GetType().Name == "MapImageLayer")
                    {
                        var mImage = (IMapImageLayer) layer;
                        if (String.IsNullOrEmpty(Path.GetFileNameWithoutExtension(mImage.Image.Filename))) return;
                        fileName = Path.GetFileNameWithoutExtension(mImage.Image.Filename);
                    }
                    else
                    {
                        var mLayer = (IMapFeatureLayer) layer;
                        if (String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((mLayer.DataSet.Filename)))) return;
                        fileName = Path.GetFileNameWithoutExtension(mLayer.DataSet.Filename);
                    }
                    if (fileName != null) chkViewLayers.Items.Add(fileName);
                }
                // now set the selections of active layers on active map tab view
                foreach (var lyr in _appManager.Map.MapFrame.GetAllLayers())
                {
                    var layer = (IMapLayer) lyr;
                    if (layer == null) continue;
                    string fileName;
                    if (layer.GetType().Name == "MapImageLayer")
                    {
                        var mImage = (IMapImageLayer) layer;
                        if (String.IsNullOrEmpty(Path.GetFileNameWithoutExtension(mImage.Image.Filename))) return;
                        fileName = Path.GetFileNameWithoutExtension(mImage.Image.Filename);
                    }
                    else
                    {
                        var mLayer = (IMapFeatureLayer) layer;
                        if (String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((mLayer.DataSet.Filename)))) return;
                        fileName = Path.GetFileNameWithoutExtension(mLayer.DataSet.Filename);
                    }
                    if (fileName == null) continue;
                    var ix = chkViewLayers.Items.IndexOf(fileName);
                    chkViewLayers.SetItemChecked(ix, true);
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

        private void UpdateGraphics(Map map)
        {
            switch (map.MapFrame.DrawingLayers[0].GetType().Name)
            {
                case "MapPointLayer":
                    PointShape ptShape;  // parse out point shape style
                    Enum.TryParse(ptSymbolStyle.SelectedItem.ToString(), true, out ptShape);
                    var pLyr = map.MapFrame.DrawingLayers[0] as MapPointLayer;
                    if (pLyr != null)
                    {
                        pLyr.Symbolizer = new PointSymbolizer(ptSymbolColor.BackColor,
                            ptShape, Convert.ToInt32(ptSymbolSize.Text));
                    }
                    break;
                case "MapLineLayer":
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
                    break;
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
                UpdateGraphics(ptMap);
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
            UpdateGraphics(ptMap);
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
            UpdateGraphics(lineMap);
        }

        private void PopulateGraphicsToForm()
        {
            // point symbology for graphics rendering
            ptSymbolColor.BackColor = SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicPointColor;
            ptSymbolColor.Click += PtSymbolColorOnClick;
            ptSymbolSize.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicPointSize;
            ptSymbolSize.ValueChanged += PtSymbolSizeOnValueChanged;
            foreach (PointShape ptShape in Enum.GetValues(typeof(PointShape)))
            {
                ptSymbolStyle.Items.Add(ptShape.ToString());
            }
            var idx = ptSymbolStyle.Items.IndexOf(SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicPointStyle);
            ptSymbolStyle.SelectedIndex = idx;
            ptSymbolStyle.SelectedIndexChanged += PtSymbolStyleOnSelectedIndexChanged;
            DrawPointGraphics();

            // line symbology for graphics rendering
            lineSymbolBorderColor.BackColor = SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineBorderColor;
            lineSymbolBorderColor.Click += LineSymbolBorderColorOnClick;
            lineSymbolColor.BackColor = SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineColor;
            lineSymbolColor.Click += LineSymbolColorOnClick;
            lineSymbolSize.Value = SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineSize;
            lineSymbolSize.ValueChanged += LineSymbolSizeOnValueChanged;
            foreach (LineCap lineCap in Enum.GetValues(typeof(LineCap)))
            {
                lineSymbolCap.Items.Add(lineCap.ToString());
            }
            idx = lineSymbolCap.Items.IndexOf(SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineCap);
            lineSymbolCap.SelectedIndex = idx;
            lineSymbolCap.SelectedIndexChanged += LineSymbolCapOnSelectedIndexChanged;
            foreach (DashStyle lineStyle in Enum.GetValues(typeof(DashStyle)))
            {
                lineSymbolStyle.Items.Add(lineStyle.ToString());
            }
            idx = lineSymbolStyle.Items.IndexOf(SdrConfig.Project.Go2ItProjectSettings.Instance.GraphicLineStyle);
            lineSymbolStyle.SelectedIndex = idx;
            lineSymbolStyle.SelectedIndexChanged += LineSymbolStyleOnSelectedIndexChanged;
            DrawLineGraphics();
        }

        private void LineSymbolCapOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            DrawLineGraphics();
            _dirtyProject = true;
        }

        private void LineSymbolStyleOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            DrawLineGraphics();
            _dirtyProject = true;
        }

        private void LineSymbolSizeOnValueChanged(object sender, EventArgs eventArgs)
        {
            DrawLineGraphics();
            _dirtyProject = true;
        }

        private void LineSymbolColorOnClick(object sender, EventArgs eventArgs)
        {
            var oColor = lineSymbolColor.BackColor;
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;

            lineSymbolColor.BackColor = dlg.Color;
            if (oColor != lineSymbolColor.BackColor)
            {
                _dirtyProject = true;
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
                _dirtyProject = true;
            }
            DrawLineGraphics();
        }

        private void PtSymbolStyleOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            DrawPointGraphics();
            _dirtyProject = true;
        }

        private void PtSymbolSizeOnValueChanged(object sender, EventArgs eventArgs)
        {
            DrawPointGraphics();
            _dirtyProject = true;
        }

        private void PtSymbolColorOnClick(object sender, EventArgs eventArgs)
        {
            var oColor = ptSymbolColor.BackColor;
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;

            ptSymbolColor.BackColor = dlg.Color;
            if (oColor != ptSymbolColor.BackColor)
            {
                _dirtyProject = true;
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
            chkPretypes.Checked = SdrConfig.Project.Go2ItProjectSettings.Instance.UsePretypes;
            chkEnableQuesryParserLog.Checked = SdrConfig.Project.Go2ItProjectSettings.Instance.EnableQueryParserLogging;
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
            // cycle through all our layers and store them to arrays for tracking
            foreach (KeyValuePair<string, IMapLayer> keyValuePair in _dockingControl.BaseLayerLookup)
            {
                var layer = keyValuePair.Value;
                if (layer == null) return;
                if (layer.GetType().Name == "MapImageLayer")
                {
                    var mImage = (IMapImageLayer) layer;
                    if (String.IsNullOrEmpty(Path.GetFileNameWithoutExtension(mImage.Image.Filename))) return;
                    _baseMap.Layers.Add(mImage);
                }
                else
                {
                    var mLayer = (IMapFeatureLayer) layer;
                    if (String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((mLayer.DataSet.Filename)))) return;
                    _baseMap.Layers.Add(mLayer);
                }
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
            chkViewLayers.Items.Add(f);
            int idx = chkViewLayers.Items.IndexOf(f);
            chkViewLayers.SelectedIndex = idx;
            chkViewLayers.SetItemCheckState(idx, CheckState.Checked);
        }

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
            chkViewLayers.Items.Remove(f);
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

        private void SetNoProject()
        {
            _dockingControl.ResetLayout();  // remove all maptabs now
            SdrConfig.Project.Go2ItProjectSettings.Instance.ResetProjectSettings();  // set all project settings to defaults
            SdrConfig.Settings.Instance.ProjectRepoConnectionString = null;  // clear any repo connection string available
            _appManager.Map = null;  // remove the appmanager map
            Cursor = Cursors.Default;  
        }

        private void AdminForm_Closing(object sender, FormClosingEventArgs e)
        {
            // check if this project file has ever been saved
            if (String.IsNullOrEmpty(_appManager.SerializationManager.CurrentProjectFile))
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
                var hasProjectChanges = _appManager.SerializationManager.IsDirty;
                if (hasProjectChanges || _dirtyProject)
                {
                    // user has made changes lets see if they want to save them
                    var res =
                        MessageBox.Show(string.Format("Save changes to current project [{0}]?", Path.GetFileName(_appManager.SerializationManager.CurrentProjectFile)),
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
                            _appManager.SerializationManager.OpenProject(_appManager.SerializationManager.CurrentProjectFile);
                            break;
                        case DialogResult.Yes:
                            e.Cancel = false; // allow form to finish closing
                            if (!SaveProject(_appManager.SerializationManager.CurrentProjectFile))
                            {
                                // user canceled the save, so reload the original project file now
                                _appManager.SerializationManager.OpenProject(_appManager.SerializationManager.CurrentProjectFile);
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

        private void AttachLegend()
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
            _baseMap.Legend = _adminLegend;
            legendSplitter.Panel1.Controls.Add(_adminLegend);
        }

        private void AdminLegendOnOrderChanged(object sender, EventArgs eventArgs)
        {
            var log = AppContext.Instance.Get<ILog>();
            log.Info("AdminLegendOnOrderChanged");
            // todo: some event here to update all map tabs/invalidations/etc.
        }

        private void btnAddLayer_Click(object sender, EventArgs e)
        {
            // add layers to base map (fires event watcher of the basemap add event)
            _baseMap.AddLayers();
        }

        private void btnRemoveLayer_Click(object sender, EventArgs e)
        {
            var layer = _baseMap.Layers.SelectedLayer;
            if (layer != null)
            {
                _baseMap.Layers.Remove(layer);
            }
        }

        private void btnSplitSave_Click(object sender, EventArgs e)
        {
            if (_idxWorker.IsBusy != true)
                if (btnSplitSave.Text == @"Save")
                {
                    bool result;
                    if (String.IsNullOrEmpty(_appManager.SerializationManager.CurrentProjectFile))
                    {
                        result = SaveProjectAs();
                    }
                    else
                    {
                        result = SaveProject(_appManager.SerializationManager.CurrentProjectFile);
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
            // set all the settings variables, these are then serialized into the database via the project manager [SaveProjectSettings()]
            // set the baselayer lookup to the localized lookup used by the admin form
            _dockingControl.BaseLayerLookup.Clear();
            foreach (KeyValuePair<string, IMapLayer> keyValuePair in _localBaseMapLayerLookup)
            {
                _dockingControl.BaseLayerLookup.Add(keyValuePair.Key, keyValuePair.Value);
            }
            // setup all project level type lookups
            SdrConfig.Project.Go2ItProjectSettings.Instance.AddressesProjectType =
                SetPointOrPolygonType(radAddressPoints);
            SdrConfig.Project.Go2ItProjectSettings.Instance.KeyLocationsProjectType =
                SetPointOrPolygonType(radKeyLocationsPoints);
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
            SdrConfig.Project.Go2ItProjectSettings.Instance.UsePretypes = chkPretypes.Checked;
            SdrConfig.Project.Go2ItProjectSettings.Instance.EnableQueryParserLogging = chkEnableQuesryParserLog.Checked;
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
                // validate all required fields are set
                /*var msg = VerifyRequiredSettings();
                if (msg.Length > 0)
                {
                    ShowSaveSettingsError(msg);
                    return false;
                }*/
                // this is saved to dbase by project manager on serialization event, which is fired just below
                ApplyProjectSettings();
                // swap the active map out with our base map now -> all layers will be serialized (not just layers in current active tab)
                var tMap = _appManager.Map;
                _appManager.Map = _baseMap;
                _appManager.SerializationManager.SaveProject(fileName);
                // reset our orginal map view back to the active map
                _appManager.Map = tMap;
                _dirtyProject = false;
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
            _dirtyProject = true;
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
            _dirtyProject = true;
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
                _dirtyProject = true;
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

        private IEnumerable<string> ReadIndexLines(string filePath)
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

        private DataTable GetLayerTypeIndexes(string layName)
        {
            var table = new DataTable();
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
            if (sc !=null && sc.Contains(layName))
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
            return null;
        }

        private void PopulateIndexesToForm()
        {
            string conn = SdrConfig.Settings.Instance.ProjectRepoConnectionString;
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
            return null;
        }

        private void cmbLayerIndex_SelectedValueChanged(object sender, EventArgs e)
        {
            chkLayerIndex.Items.Clear();
            dgvLayerIndex.Rows.Clear();
            dgvLayerIndex.Columns.Clear();

            string lyrName = cmbLayerIndex.SelectedItem.ToString();
            if (lyrName.Length == 0) return;

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

            // add all the data field columns to the checkbox list
            IMapLayer mapLyr;
            _localBaseMapLayerLookup.TryGetValue(lyrName, out mapLyr);
            var mfl = mapLyr as IMapFeatureLayer;
            if (mfl != null && mfl.DataSet != null)
            {
                IFeatureSet fl = mfl.DataSet;
                foreach (DataColumn dc in fl.DataTable.Columns)
                {
                    chkLayerIndex.Items.Add(dc.ColumnName);
                }
            }

            // determine what type of layer we have and set lookup indexes
            string lyrType = GetLayerIndexTableType(lyrName);
            string conn = SdrConfig.Settings.Instance.ProjectRepoConnectionString;

            DataTable defaults = GetLayerTypeIndexes(lyrName);
            var table = new DataTable();

            // check if this layer has already been added to queue
            if (chkLayersToIndex.Items.Contains(lyrName))
            {
                // in this case we will use the temp set values
                List<Dictionary<string, string>> idx;
                _indexQueue.TryGetValue(lyrName, out idx);
                // generate the table for population
                table.Columns.Add("lookup");
                table.Columns.Add("fieldname");
                if (idx != null)
                {
                    foreach (Dictionary<string, string> d in idx)
                    {
                        DataRow dr = table.NewRow();
                        string lookup;
                        d.TryGetValue("lookup", out lookup);
                        dr["lookup"] = lookup;
                        string fieldname;
                        d.TryGetValue("fieldname", out fieldname);
                        dr["fieldname"] = fieldname;
                        table.Rows.Add(dr);
                    }
                }
            }
            else // no active queue lets check the table for one instead
            {
                if (SQLiteHelper.TableExists(conn, lyrType + "_" + lyrName))
                {
                    string query = "SELECT * FROM " + lyrType + "_" + lyrName;
                    table = SQLiteHelper.GetDataTable(conn, query);
                    if (table.Rows.Count == 0)
                    {
                        table = defaults;
                    }
                }
                else // none saved to table, set as the defaults
                {
                    table = defaults;
                }
            }
            // go ahead and populate our data grid view with our table data
            for (int i = 0; i < defaults.Rows.Count; i++)
            {
                DataRow r = defaults.Rows[i];
                var key = r["lookup"].ToString();
                var val = string.Empty;
                string exp = "lookup = " + "'" + key + "'";
                DataRow[] foundRows = table.Select(exp);
                if (foundRows.Length > 0)
                {
                    DataRow d = foundRows[0];
                    val = d["fieldname"].ToString();
                }
                var row = new DataGridViewRow();
                var txtKey = new DataGridViewTextBoxCell {Value = key};
                var txtValue = new DataGridViewTextBoxCell {Value = val};
                row.Cells.Add(txtKey);
                row.Cells.Add(txtValue);
                dgvLayerIndex.Rows.Add(row);
                if (val.Length <= 0) continue;
                for (int j = 0; j < chkLayerIndex.Items.Count; j++)
                {
                    if (chkLayerIndex.Items[j].ToString() == val)
                    {
                        chkLayerIndex.SetItemCheckState(j, CheckState.Checked);
                        break;
                    }
                }
            }
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

                string conn = SdrConfig.Settings.Instance.ProjectRepoConnectionString;
                var iobjects = new IndexObject[_indexQueue.Count];
                int count = 0;

                foreach (KeyValuePair<string, List<Dictionary<string, string>>> keyValuePair in _indexQueue)
                {
                    string lyrName = keyValuePair.Key;
                    string idxType = GetLayerIndexTableType(lyrName);
                    // check if the table exists - then clear and clean it or create a new one
                    if (SQLiteHelper.TableExists(conn, idxType + "_" + lyrName))
                    {
                        SQLiteHelper.ClearTable(conn, idxType + "_" + lyrName);
                    }
                    else
                    {
                        SQLiteHelper.CreateTable(conn, idxType + "_" + lyrName, _indexLookupFields);
                    }
                    // setup everything else we need to generate our lucene index      
                    IMapLayer mapLyr;
                    _localBaseMapLayerLookup.TryGetValue(lyrName, out mapLyr);
                    var mfl = mapLyr as IMapFeatureLayer;
                    IFeatureSet fs;
                    if (mfl != null && mfl.DataSet != null)
                    {
                        fs = mfl.DataSet;
                    }
                    else
                    {
                        var msg = AppContext.Instance.Get<IUserMessage>();
                        msg.Warn("Error on Create Index, FeatureDataset is null", new Exception());
                        return;
                    }
                    // make sure this featureset has FID values for lookup
                    fs.AddFid(); 
                    fs.Save();
                    // array of indexobjects we will add to the index
                    List<Dictionary<string, string>> indexList = keyValuePair.Value;
                    var list = new List<KeyValuePair<string, string>>();
                    // iterate through all the field indexes
                    for (int i = 0; i <= indexList.Count - 1; i++)
                    {
                        Dictionary<string, string> d = indexList[i];
                        SQLiteHelper.Insert(conn, idxType + "_" + lyrName, d);
                        var kvPair = new KeyValuePair<string, string>(d["lookup"], d["fieldname"]);
                        list.Add(kvPair);
                    }
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
                    Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);
                    var db = SQLiteHelper.GetSQLiteFileName(SdrConfig.Settings.Instance.ProjectRepoConnectionString);
                    var d = Path.GetDirectoryName(db);
                    if (d == null) return;

                    foreach (KeyValuePair<string, List<Document>> keyValuePair in docs)
                    {
                        var path = Path.Combine(d, "indexes", keyValuePair.Key);
                        DirectoryInfo di = System.IO.Directory.CreateDirectory(path);
                        Directory dir = FSDirectory.Open(di);
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

                            var query = new BooleanQuery {{qfid, Occur.MUST}, {qlyr, Occur.MUST}};

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

            var cleanQueue = new List<string>();
            for (int i = 0; i <= chkLayersToIndex.Items.Count - 1; i++)
            {
                if (chkLayersToIndex.GetItemChecked(i))
                {
                    var item = chkLayersToIndex.Items[i].ToString();
                    // remove this from our overall queue
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
                try
                {
                    // remove the layer name from the existing listbox
                    lstExistingIndexes.Items.Remove(lyrName);
                    // now remove the keyvalue lookups from the config database
                    string conn = SdrConfig.Settings.Instance.ProjectRepoConnectionString;
                    string idxType = GetLayerIndexTableType(lyrName);
                    // check if the table exists and drop it if so
                    if (SQLiteHelper.TableExists(conn, idxType + "_" + lyrName))
                    {
                        SQLiteHelper.DropTable(conn, idxType + "_" + lyrName);
                    }
                    // time to update our lucene indexes
                    var db = SQLiteHelper.GetSQLiteFileName(SdrConfig.Settings.Instance.ProjectRepoConnectionString);
                    var d = Path.GetDirectoryName(db);
                    if (d == null) return;

                    var path = Path.Combine(d, "indexes", idxType);
                    Directory dir = FSDirectory.Open(new DirectoryInfo(path));
                    var writer = new IndexWriter(dir, new KeywordAnalyzer(), IndexWriter.MaxFieldLength.LIMITED);
                    Query q = new QueryParser(Version.LUCENE_30, "LYRNAME", new KeywordAnalyzer()).Parse(lyrName);
                    writer.DeleteDocuments(q);
                    writer.Optimize();
                    writer.Commit();
                    writer.Dispose();
                }
                catch (Exception ex)
                {
                    var msg = AppContext.Instance.Get<IUserMessage>();
                    msg.Error("Error on deleting index", ex);
                }
            }
        }

        private void chkViewLayers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var clb = (CheckedListBox)sender;
            if (clb.SelectedItem == null) return;

            var lyr = _localBaseMapLayerLookup[clb.SelectedItem.ToString()];
            if (e.NewValue.ToString() == "Checked")
            {
                _appManager.Map.Layers.Add(lyr);  // add the checked layer to the active map tab
            }
            else
            {
                _appManager.Map.Layers.Remove(lyr);  // remove the checked layer from the active map
            }
            _appManager.Map.Invalidate();
            _dirtyProject = true;
        }

        private void btnAddView_Click(object sender, EventArgs e)
        {
            if (txtViewName.Text.Length <= 0) return;

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
            var nMap = CreateNewMap(key);
            // create new dockable panel and stow that shit yo!
            var dp = new DockablePanel(key, txt, nMap, DockStyle.Fill);
            _appManager.DockManager.Add(dp);
            _appManager.DockManager.SelectPanel(key);

            _dirtyProject = true;
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
            if (_dockingControl.DockPanelLookup.Count == 0)
            {
                for (int i = 0; i < chkViewLayers.Items.Count; i++)
                {
                    chkViewLayers.SetItemCheckState(i, (CheckState.Unchecked));
                }
            }
            _dirtyProject = true;
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

        private void UpdateLayerIndexCombo(List<string> ad, List<string> rd, List<string> kl, List<string> cs, List<string> cl, List<string> es, List<string> pl, List<string> hy)
        {
            cmbLayerIndex.Items.Clear();
            var sels = new List<object>(ad.Count + rd.Count + kl.Count + cs.Count + cl.Count + es.Count + pl.Count + hy.Count);
            sels.AddRange(ad);
            sels.AddRange(rd);
            sels.AddRange(kl);
            sels.AddRange(cs);
            sels.AddRange(cl);
            sels.AddRange(es);
            sels.AddRange(pl);
            sels.AddRange(hy);
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
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl, hy);
            _dirtyProject = true;
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
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl, hy);
            _dirtyProject = true;
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
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl, hy);
            _dirtyProject = true;
        }

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
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl, hy);
            _dirtyProject = true;
        }

        private void cmbNotesLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            _dirtyProject = true;
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
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl, hy);
            _dirtyProject = true;
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
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl, hy);
            _dirtyProject = true;
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
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl, hy);
            _dirtyProject = true;
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
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl, hy);
            _dirtyProject = true;
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
    }
}