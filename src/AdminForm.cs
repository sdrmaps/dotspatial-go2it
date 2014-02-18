using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using DotSpatial.Projections;
using DotSpatial.SDR.Controls;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using DotSpatial.Topology.Index.Bintree;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;
using Directory = Lucene.Net.Store.Directory;
using Field = Lucene.Net.Documents.Field;
using DotSpatial.Data;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls;
using SDR.Authentication;
using SDR.Data.Database;
using Go2It.Properties;
using Point = System.Drawing.Point;
using SdrConfig = SDR.Configuration;

namespace Go2It
{
    public partial class AdminForm : Form
    {
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();
        private readonly BackgroundWorker _idxWorker = new BackgroundWorker();

        // change tracking flags for project changes as well as mapview changes
        private bool _dirtyProject;
        private bool _dirtyTabs;

        // admin form level controls
        private Legend _adminLegend;
        private readonly AppManager _appManager;
        private readonly DockingControl _dockingControl;
        private readonly Map _baseMap;

        // store all the layers by type
        private readonly List<IFeatureSet> _pointLayers = new List<IFeatureSet>();
        private readonly List<IFeatureSet> _polygonLayers = new List<IFeatureSet>();
        private readonly List<IFeatureSet> _lineLayers = new List<IFeatureSet>();

        // switch routines to handle options of check/unchek on admin form
        private readonly LayerSelectionSwitcher _pointLayerSwitcher = new LayerSelectionSwitcher();
        private readonly LayerSelectionSwitcher _lineLayerSwitcher = new LayerSelectionSwitcher();
        private readonly LayerSelectionSwitcher _polygonLayerSwitcher = new LayerSelectionSwitcher();

        // temp dict to hold all base map layers for use selection/removal on individual tab maps
        // on save this is passed to the dockingcontrol baselayerlookup dict
        private readonly Dictionary<string, IMapLayer> _localBaseMapLayerLookup = new Dictionary<string, IMapLayer>();

        // string collections for all search indexes
        private readonly StringCollection _addressIndexes = new StringCollection
        {
            "First Name",
            "Last Name",
            "Structure Number",
            "Pre Directional",
            "Pre Type",
            "Street Name",
            "Street Type",
            "Post Directional",
            "Sub Unit Type",
            "Sub Unit Designation",
            "Community",
            "Phone",
            "Aux. Phone",
            "Other 1",
            "Other 2"
        };

        private readonly StringCollection _roadIndexes = new StringCollection
        {
            "Pre Directional",
            "Pre Type",
            "Street Name",
            "Street Type",
            "Post Directional",
            "Right Zip Community",
            "Left Zip Community",
            "Right MSAG Community",
            "Left MSAG Community",
            "Other 1",
            "Other 2"
        };

        private readonly StringCollection _keyLocationsIndexes = new StringCollection
        {
            "Name",
            "Type",
            "Description"
        };

        private readonly StringCollection _cellSectorsIndexes = new StringCollection
        {
            "Sector ID",
            "Tower ID",
            "Company ID"
        };

        private readonly StringCollection _cityLimitsIndexes = new StringCollection
        {
            "Name"
        };

        private readonly StringCollection _esnsIndexes = new StringCollection
        {
            "Name"
        };

        private readonly StringCollection _parcelsIndexes = new StringCollection
        {
            "Parcel ID",
            "Owner Name",
            "Other 1",
            "Other 2"
        };

        public AdminForm(AppManager app)
        {
            InitializeComponent();
            _appManager = app;
            _dockingControl = (DockingControl) app.DockManager;
            // a basemap to hold all layers for the adminlegend
            _baseMap = new Map
            {
                BackColor = SdrConfig.Project.Go2ItProjectSettings.Instance.MapBgColor,
                Dock = DockStyle.Fill,
                Visible = false,
                ViewExtents = app.Map.ViewExtents,
                Projection = app.Map.Projection
            };
            // set options on our indexing bgworker
            _idxWorker.WorkerReportsProgress = true;
            _idxWorker.WorkerSupportsCancellation = true;
            // splitter stuff
            adminLayerSplitter.SplitterWidth = 10;
            adminLayerSplitter.Paint += Splitter_Paint;
            // lets setup the full administrative form
            AttachLegend();
            PopulateSettingsToForm();
            PopulateUsersToForm();
            // setup all events now
            FormClosing += AdminForm_Closed; // check for isdirty changes to project file
            adminTab_Control.Selected += adminTab_Control_Selected;
            // validate that project is saved before allowing indexing and views
            chkViewLayers.ItemCheck += chkViewLayers_ItemCheck; // add or remove item to specific map tab view
            // setup a background worker for update progress bar on indexing tab
            _idxWorker.DoWork += idx_DoWork;
            _idxWorker.ProgressChanged += idx_ProgressChanged;
            _idxWorker.RunWorkerCompleted += idx_RunWorkerCompleted;

            // map tracking events on removal and addition of a layer
            // _baseMap.Layers.LayerRemoved += LayersOnLayerRemoved;
            _baseMap.Layers.LayerAdded += LayersOnLayerAdded;
            
            // overall events tied to main application
            _appManager.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;
            // active map or tool panel has been changed
            _dirtyProject = false; // reset dirty flag after populating form
        }

        private void idx_DoWork(object sender, DoWorkEventArgs e)
        {
            IndexObject io = e.Argument as IndexObject;
            // our worker for running the indexing operation
            var worker = sender as BackgroundWorker;
            // setup everything else we need to generate ouselves a lucene index
            if (io != null)
            {
                IFeatureSet fl = io.FeatureSet;
                Directory dir = FSDirectory.Open(new DirectoryInfo(_appManager.SerializationManager.CurrentProjectDirectory + "\\indexes\\" + io.LayerType));
                for (int x = 0; x < fl.DataTable.Rows.Count; x++)
                {
                    if (worker != null && (worker.CancellationPending))
                    {
                        e.Cancel = true;
                        break;
                    }
                    // grab a row from the datatable and index that shit
                    DataRow dr = fl.DataTable.Rows[x];
                    var doc = new Document();  // generate a document for indexing by lucene
                    var list = io.FieldLookup;
                    for (int i = 0; i < list.Count; i++)
                    {
                        doc.Add(new Field("FID", dr["FID"].ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                        // no need to run an analyzer on numbered values
                        var kv = new KeyValuePair<string, string>();
                        kv = list[i];
                        if (kv.Key == "Phone" || kv.Key == "Aux. Phone" || kv.Key == "Structure Number")
                        {
                            doc.Add(new Field(kv.Key, dr[kv.Value].ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                        }
                        else  // run analyzer on all remaining field types
                        {
                            doc.Add(new Field(kv.Key, dr[kv.Value].ToString(), Field.Store.YES, Field.Index.ANALYZED));
                        }
                    }
                    Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);
                    // either create a new index or update an existing index
                    var writer = x == 0 ? new IndexWriter(dir, analyzer, true, IndexWriter.MaxFieldLength.LIMITED) : new IndexWriter(dir, analyzer, false, IndexWriter.MaxFieldLength.LIMITED);
                    writer.AddDocument(doc);
                    writer.Optimize();
                    writer.Dispose();
                    // get the current ratio of the list we are indexing
                    double ratio = (double)fl.DataTable.Rows.Count/100;
                    // update the progress bar on the main thread interface
                    int result = Convert.ToInt32((x + 1)/ratio);
                    if (worker != null) worker.ReportProgress((result));
                }
            }
        }

        private void idx_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                idxProgressBar.Hide();
                DeleteIndex();
            }
            else if (e.Error != null)
            {
                // TODO:this.tbProgress.Text = ("Error: " + e.Error.Message);
                MessageBox.Show("Error");
            }
            idxProgressBar.Hide();
        }

        private void idx_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // idxProgressBar.Text = (e.ProgressPercentage.ToString() + "%");
            idxProgressBar.Value = e.ProgressPercentage;
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
            /*var layer = (IMapLayer) layerEventArgs.Layer;
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
                IFeatureSet fs = FeatureSet.Open(mLayer.DataSet.Filename);
                RemoveLayer(fs); // perform all form specific remove operations
            }
            // we need to cycle through all the available dockpanels check if the layer has been set on that map
            foreach (KeyValuePair<string, DockPanelInfo> dockPanelInfo in _dockingControl.DockPanelLookup)
            {
                var map = (Map) dockPanelInfo.Value.DotSpatialDockPanel.InnerControl;
                map.Layers.Remove(layer);
                dockPanelInfo.Value.DotSpatialDockPanel.InnerControl = map;
                // remove the old map from the maptab controls now and reset it (not sure if this is even needed)
                dockPanelInfo.Value.DockPanelTab.Controls.Clear();
                dockPanelInfo.Value.DockPanelTab.Controls.Add(dockPanelInfo.Value.DotSpatialDockPanel.InnerControl);
            }*/
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
                var idx = lstViews.Items.IndexOf(caption);
                lstViews.SetSelected(idx, true);
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
                foreach (IMapLayer layer in _appManager.Map.MapFrame.GetAllLayers())
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

        private void PopulateSettingsToForm()
        {
            // set the map background color (default is black)
            mapBGColorPanel.BackColor = SdrConfig.Project.Go2ItProjectSettings.Instance.MapBgColor;
            // set default base settings on admin load
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
                    IFeatureSet fs = FeatureSet.Open(mLayer.DataSet.Filename);
                    if (mLayer.DataSet.FeatureType.Equals(DotSpatial.Topology.FeatureType.Line))
                    {
                        _lineLayers.Add(fs);
                    }
                    if (mLayer.DataSet.FeatureType.Equals(DotSpatial.Topology.FeatureType.Point))
                    {
                        _pointLayers.Add(fs);
                    }
                    if (mLayer.DataSet.FeatureType.Equals(DotSpatial.Topology.FeatureType.Polygon))
                    {
                        _polygonLayers.Add(fs);
                    }
                    _baseMap.Layers.Add(mLayer);
                }
            }
            SetupLayerSelectionSwitchers();
            SetActiveLayerSelections();
            PopulateMapViews();
        }

        private void PopulateMapViews()
        {
            foreach (KeyValuePair<string, DockPanelInfo> dpi in _dockingControl.DockPanelLookup)
            {
                lstViews.Items.Add(dpi.Value.DotSpatialDockPanel.Caption);
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

        private static object[] GetArrayOfAllLayerNames(IEnumerable<IFeatureSet> mapLayers)
        {
            string[] strArr =
                mapLayers.Select(mapFtLayer => Path.GetFileNameWithoutExtension(mapFtLayer.Filename)).ToArray();
            var objArr = new object[strArr.Length];
            for (int i = 0; i < strArr.Length; i++)
            {
                objArr[i] = strArr[i];
            }
            return objArr;
        }

        private void SetupLayerSelectionSwitchers()
        {
            // line layers
            object[] lineNames = GetArrayOfAllLayerNames(_lineLayers);
            chkRoadLayers.Items.AddRange(lineNames);
            _lineLayerSwitcher.Add(chkRoadLayers);
            // point layers
            object[] pointNames = GetArrayOfAllLayerNames(_pointLayers);
            cmbNotesLayer.Items.AddRange(pointNames);
            cmbNotesLayer.Items.Add(string.Empty);
            cmbHydrantsLayer.Items.AddRange(pointNames);
            cmbHydrantsLayer.Items.Add(string.Empty);
            _pointLayerSwitcher.Add(cmbNotesLayer, cmbHydrantsLayer);
            if (radAddressPoints.Checked)
            {
                chkAddressLayers.Items.AddRange(pointNames);
                _pointLayerSwitcher.Add(chkAddressLayers);
            }
            if (radKeyLocationsPoints.Checked)
            {
                chkKeyLocationsLayers.Items.AddRange(pointNames);
                _pointLayerSwitcher.Add(chkKeyLocationsLayers);
            }
            // polygon layers
            object[] polygonNames = GetArrayOfAllLayerNames(_polygonLayers);
            cmbCityLimitLayer.Items.AddRange(polygonNames);
            cmbCityLimitLayer.Items.Add(string.Empty);
            cmbCellSectorLayer.Items.AddRange(polygonNames);
            cmbCellSectorLayer.Items.Add(string.Empty);
            cmbESNLayer.Items.AddRange(polygonNames);
            cmbESNLayer.Items.Add(string.Empty);
            cmbParcelsLayer.Items.AddRange(polygonNames);
            cmbParcelsLayer.Items.Add(string.Empty);
            _polygonLayerSwitcher.Add(cmbCityLimitLayer, cmbCellSectorLayer, cmbESNLayer, cmbParcelsLayer);
            if (radAddressPolygons.Checked)
            {
                chkAddressLayers.Items.AddRange(polygonNames);
                _polygonLayerSwitcher.Add(chkAddressLayers);
            }
            if (radKeyLocationsPolygons.Checked)
            {
                chkKeyLocationsLayers.Items.AddRange(polygonNames);
                _polygonLayerSwitcher.Add(chkKeyLocationsLayers);
            }
        }

        private IFeatureSet GetLayerByName(string layName)
        {
            if (_lineLayers.Count > 0)
            {
                foreach (IFeatureSet fl in _lineLayers)
                {
                    if (Path.GetFileNameWithoutExtension(fl.Filename) == layName) return fl;
                }
            }
            if (_pointLayers.Count > 0)
            {
                foreach (IFeatureSet fl in _pointLayers)
                {
                    if (Path.GetFileNameWithoutExtension(fl.Filename) == layName) return fl;
                }
            }
            if (_polygonLayers.Count > 0)
            {
                return _polygonLayers.FirstOrDefault(fl => Path.GetFileNameWithoutExtension(fl.Filename) == layName);
            }
            return null;
        }

        /// <summary>
        /// Handle all form elements when a new layer is added to the project
        /// </summary>
        /// <param name="mapLayer">The layer added</param>
        private void AddLayer(IFeatureSet mapLayer)
        {
            if (String.IsNullOrEmpty(mapLayer.Filename)) return;
            var f = Path.GetFileNameWithoutExtension(mapLayer.Filename);
            if (mapLayer.FeatureType.Equals(DotSpatial.Topology.FeatureType.Line))
            {
                _lineLayers.Add(mapLayer); // add to line layer list
                chkRoadLayers.Items.Add(f);
            }
            if (mapLayer.FeatureType.Equals(DotSpatial.Topology.FeatureType.Point))
            {
                _pointLayers.Add(mapLayer); // add to point layer list
                cmbNotesLayer.Items.Add(f);
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
            if (mapLayer.FeatureType.Equals(DotSpatial.Topology.FeatureType.Polygon))
            {
                _polygonLayers.Add(mapLayer); // add to polygon layer list
                cmbCityLimitLayer.Items.Add(f);
                cmbCellSectorLayer.Items.Add(f);
                cmbESNLayer.Items.Add(f);
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

        private void RemoveLayer(IFeatureSet mapLayer)
        {
            if (String.IsNullOrEmpty(mapLayer.Filename)) return;
            string f = Path.GetFileNameWithoutExtension(mapLayer.Filename);
            if (mapLayer.FeatureType.Equals(DotSpatial.Topology.FeatureType.Line))
            {
                _lineLayers.Remove(mapLayer); // remove from layer list
                chkRoadLayers.Items.Remove(f);
            }
            if (mapLayer.FeatureType.Equals(DotSpatial.Topology.FeatureType.Point))
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
            if (mapLayer.FeatureType.Equals(DotSpatial.Topology.FeatureType.Polygon))
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

        private bool ProjectExists()
        {
            var projectExists = (!String.IsNullOrEmpty(_appManager.SerializationManager.CurrentProjectFile));
            if (projectExists) return true;

            var res = MessageBox.Show(string.Format("Please save your project to continue."),
                Resources.AppName,
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button3);
            switch (res)
            {
                case DialogResult.Cancel:
                    return false;
                case DialogResult.No:
                    return false;
                case DialogResult.Yes:
                    return ShowSaveProjectDialog();
            }
            return false;
        }

        private bool ProjectIsClean(bool flag)
        {
            var projectExists = (!String.IsNullOrEmpty(_appManager.SerializationManager.CurrentProjectFile));
            if (projectExists)
            {
                var hasProjectChanges = _appManager.SerializationManager.IsDirty;
                if (hasProjectChanges || flag)
                {
                    var res =
                        MessageBox.Show(string.Format("Save changes to current project [{0}]?", GetProjectShortName()),
                            Resources.AppName,
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button3);
                    switch (res)
                    {
                        case DialogResult.Cancel:
                            return false;
                        case DialogResult.No:
                            return false;
                        case DialogResult.Yes:
                            return SaveProject(_appManager.SerializationManager.CurrentProjectFile);
                    }
                }
                return true;
            }
            return false;
        }

        private void AdminForm_Closed(object sender, FormClosingEventArgs e)
        {
            if (ProjectExists())
            {
                if (ProjectIsClean(_dirtyProject))
                {
                    e.Cancel = !ProjectIsClean(_dirtyTabs);
                }
                else
                {
                    e.Cancel = true; // discard changes
                }
            }
            else
            {
                e.Cancel = false; // no project file exists. leave as it is
            }
        }

        private string GetProjectShortName()
        {
            return Path.GetFileName(_appManager.SerializationManager.CurrentProjectFile);
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
            // _adminLegend.OrderChanged += AdminLegendOnOrderChanged;
            _baseMap.Legend = _adminLegend;
            legendSplitter.Panel1.Controls.Add(_adminLegend);
        }

        private void AdminLegendOnOrderChanged(object sender, EventArgs eventArgs)
        {
            // todo:
            // probabley should do some event here to update all map tabs
            // var g = sender;
            // var w = eventArgs;
        }

        private void btnAddLayer_Click(object sender, EventArgs e)
        {
            // add layers to base map (fires event watcher of the basemap add event)
            _baseMap.AddLayers();
        }

        private void btnRemoveLayer_Click(object sender, EventArgs e)
        {
            // remove layer from base map (fires event watcher of the basemap remove event)
            var layer = _baseMap.Layers.SelectedLayer;
            if (layer != null)
            {
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
                    IFeatureSet fs = FeatureSet.Open(mLayer.DataSet.Filename);
                    RemoveLayer(fs); // perform all form specific remove operations
                }
                // we need to cycle through all the available dockpanels check if the layer has been set on that map
                foreach (KeyValuePair<string, DockPanelInfo> dockPanelInfo in _dockingControl.DockPanelLookup)
                {
                    var map = (Map) dockPanelInfo.Value.DotSpatialDockPanel.InnerControl;
                    map.Layers.Remove(layer);
                    dockPanelInfo.Value.DotSpatialDockPanel.InnerControl = map;
                    // remove the old map from the maptab controls now and reset it (not sure if this is even needed)
                    dockPanelInfo.Value.DockPanelTab.Controls.Clear();
                    dockPanelInfo.Value.DockPanelTab.Controls.Add(dockPanelInfo.Value.DotSpatialDockPanel.InnerControl);
                }

                // originally we were using the event but moving layers in the legend add/removes.. fucking stupid shit fucktards
                // _baseMap.Layers.Remove(layer);
            }
        }

        private void btnSplitSave_Click(object sender, EventArgs e)
        {
            if (btnSplitSave.Text == @"Save")
            {
                if (String.IsNullOrEmpty(_appManager.SerializationManager.CurrentProjectFile))
                {
                    SaveProjectAs();
                }
                else
                {
                    SaveProject(_appManager.SerializationManager.CurrentProjectFile);
                }
            }
            else
            {
                SaveProjectAs();
            }
        }

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
            // set the baselayer lookup to the temp one we will be using on the admin form
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
            // active map panel and caption will already be set / no need to fuss with that shit
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
                // validate all required fields are set
                var msg = VerifyRequiredSettings();
                if (msg.Length > 0)
                {
                    ShowSaveSettingsError(msg);
                    return false;
                }
                // apply go2it setting to the app settings  later saved to dbase by project manager on serialization event
                ApplyProjectSettings();
                // swap the active map out with our base map now (so we serialize all layers to project xml file)
                var tMap = _appManager.Map;
                _appManager.Map = _baseMap;
                _appManager.SerializationManager.SaveProject(fileName);
                // reset the orginal map back to the active map
                _appManager.Map = tMap;
                _dirtyProject = false;
                _dirtyTabs = false;
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

        private void SaveProjectAs()
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
                    SaveProject(dlg.FileName);
                }
            }
        }

        private bool VerifyPolygonLayerNotSelected(string fileName, CheckedListBox checkBox)
        {
            if (cmbCityLimitLayer.SelectedItem != null && cmbCityLimitLayer.SelectedItem.ToString() == fileName)
                return false;
            if (cmbCellSectorLayer.SelectedItem != null && cmbCellSectorLayer.SelectedItem.ToString() == fileName)
                return false;
            if (cmbESNLayer.SelectedItem != null && cmbESNLayer.SelectedItem.ToString() == fileName) return false;
            if (cmbParcelsLayer.SelectedItem != null && cmbParcelsLayer.SelectedItem.ToString() == fileName)
                return false;
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
            if (cmbHydrantsLayer.SelectedItem != null && cmbHydrantsLayer.SelectedItem.ToString() == fileName)
                return false;
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
            // we might have to handle map redraws here / not sure yet
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
            if (lstUsers.Items.Count <= 0)
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
                MessageBox.Show(@"This user does not exist. No delete available");
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

        private void adminTab_Control_Selected(object sender, TabControlEventArgs e)
        {
            // only run the check on the view manager and index manager tabs
            if (adminTab_Control.SelectedTab.Name == "adminTab_SearchProperties" ||
                adminTab_Control.SelectedTab.Name == "adminTab_ViewManagement")
            {
                // verify the user has created the project file
                if (ProjectExists())
                {
                    // to use views or indexing the project must be clean
                    if (!ProjectIsClean(_dirtyProject))
                    {
                        adminTab_Control.SelectedIndex = 0;
                    }
                }
                else // no project currently exists
                {
                    adminTab_Control.SelectedIndex = 0;
                }
            }
            // populate form specific attribution now
            if (adminTab_Control.SelectedTab.Name == "adminTab_SearchProperties")
            {
                cmbLayerIndex.Items.Clear();
                AddLayersToIndex(chkAddressLayers);
                AddLayersToIndex(chkRoadLayers);
                AddLayersToIndex(chkKeyLocationsLayers);
                AddLayersToIndex(cmbCellSectorLayer);
                AddLayersToIndex(cmbCityLimitLayer);
                AddLayersToIndex(cmbESNLayer);
                AddLayersToIndex(cmbParcelsLayer);
            }
        }

        private void AddLayersToIndex(CheckedListBox chkBox)
        {
            if (chkBox == null) return;
            foreach (object t in chkBox.CheckedItems)
            {
                cmbLayerIndex.Items.Add(t);
            }
        }

        private void AddLayersToIndex(ComboBox cmbBox)
        {
            if (cmbBox.Text.Length > 0)
            {
                cmbLayerIndex.Items.Add(cmbBox.Text);
            }
        }

        private DataTable GetLayerTypeIndexes(string layName)
        {
            var table = new DataTable();
            table.Columns.Add("lookup");
            table.Columns.Add("fieldname");
            StringCollection sc = ApplyCheckBoxSetting(chkAddressLayers);
            if (sc.Contains(layName))
            {
                foreach (string key in _addressIndexes)
                {
                    table.Rows.Add(key, "");
                }
                return table;
            }
            sc = ApplyCheckBoxSetting(chkRoadLayers);
            if (sc.Contains(layName))
            {
                foreach (string key in _roadIndexes)
                {
                    table.Rows.Add(key, "");
                }
                return table;
            }
            sc = ApplyCheckBoxSetting(chkKeyLocationsLayers);
            if (sc.Contains(layName))
            {
                foreach (string key in _keyLocationsIndexes)
                {
                    table.Rows.Add(key, "");
                }
                return table;
            }
            if (layName == ApplyComboBoxSetting(cmbCityLimitLayer))
            {
                foreach (string key in _cityLimitsIndexes)
                {
                    table.Rows.Add(key, "");
                }
                return table;
            }
            if (layName == ApplyComboBoxSetting(cmbCellSectorLayer))
            {
                foreach (string key in _cellSectorsIndexes)
                {
                    table.Rows.Add(key, "");
                }
                return table;
            }
            if (layName == ApplyComboBoxSetting(cmbESNLayer))
            {
                foreach (string key in _esnsIndexes)
                {
                    table.Rows.Add(key, "");
                }
                return table;
            }
            if (layName == ApplyComboBoxSetting(cmbParcelsLayer))
            {
                foreach (string key in _parcelsIndexes)
                {
                    table.Rows.Add(key, "");
                }
                return table;
            }
            return null;
        }

        private String GetLayerIndexTable(string layName)
        {
            StringCollection sc = ApplyCheckBoxSetting(chkAddressLayers);
            if (sc.Contains(layName))
            {
                return "AddressIndex";
            }
            sc = ApplyCheckBoxSetting(chkRoadLayers);
            if (sc.Contains(layName))
            {
                return "RoadIndex";
            }
            sc = ApplyCheckBoxSetting(chkKeyLocationsLayers);
            if (sc.Contains(layName))
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
            IFeatureSet fl = GetLayerByName(lyrName);
            foreach (DataColumn dc in fl.DataTable.Columns)
            {
                chkLayerIndex.Items.Add(dc.ColumnName);
            }
            // determine what type of layer we have and set lookup indexes
            string lyrIndexTable = GetLayerIndexTable(lyrName);
            string conn = SdrConfig.Settings.Instance.ProjectRepoConnectionString;
            string query = "SELECT * FROM " + lyrIndexTable;
            DataTable defaults = GetLayerTypeIndexes(lyrName);
            DataTable table = SQLiteHelper.GetDataTable(conn, query);
            if (table.Rows.Count == 0)
            {
                table = defaults;
            }
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

        private void btnCreateIndex_Click(object sender, EventArgs e)
        {
            if (_idxWorker.IsBusy != true)
            {
                if (cmbLayerIndex.SelectedItem.ToString().Length > 0)
                {
                    string lyrName = cmbLayerIndex.SelectedItem.ToString();
                    var list = new List<KeyValuePair<string, string>>();
                    for (int i = 0; i < dgvLayerIndex.Rows.Count; i++)
                    {
                        if (dgvLayerIndex.Rows[i].Cells[1].Value.ToString().Length > 0)
                        {
                            // set the value of the lookup for each field to index
                            var kv = new KeyValuePair<string, string>(dgvLayerIndex.Rows[i].Cells[0].Value.ToString(),
                                dgvLayerIndex.Rows[i].Cells[1].Value.ToString());
                            list.Add(kv);
                        }
                    }
                    // setup everything else we need to generate ouselves a lucene index                  
                    IFeatureSet fs = GetLayerByName(lyrName);
                    // make sure we have a fid value for lookup
                    fs.AddFid();  // make sure this featureset has FID values for lookup
                    fs.Save();
                    string lyrType = GetLayerIndexTable(lyrName);
                    var io = new IndexObject(fs, list, lyrType);
                    idxProgressBar.Visible = true;
                    idxProgressBar.Minimum = 0;
                    idxProgressBar.Maximum = 100;
                    idxProgressBar.Value = 0;
                    _idxWorker.RunWorkerAsync(io);
                    // while that runs lets save the settings to the database
                    string conn = SdrConfig.Settings.Instance.ProjectRepoConnectionString;
                    SQLiteHelper.ClearTable(conn, lyrType);
                    for (int i = 0; i < io.FieldLookup.Count; i++)
                    {
                        var kv = new KeyValuePair<string, string>();
                        kv = io.FieldLookup[i];
                        var d = new Dictionary<string, string>
                        {
                            {"lookup", kv.Key},
                            {"fieldname", kv.Value}
                        };
                        SQLiteHelper.Insert(conn, lyrType, d);
                    }
                }
            }
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
            if (cmbLayerIndex.SelectedItem.ToString().Length > 0)
            {
                string lyrName = cmbLayerIndex.SelectedItem.ToString();
                string lyrType = GetLayerIndexTable(lyrName);
                if (System.IO.Directory.Exists(_appManager.SerializationManager.CurrentProjectDirectory + "\\" + "indexes\\" + lyrType))
                {
                    System.IO.Directory.Delete(_appManager.SerializationManager.CurrentProjectDirectory + "\\" + "indexes\\" + lyrType, true);
                }
                string conn = SdrConfig.Settings.Instance.ProjectRepoConnectionString;
                SQLiteHelper.ClearTable(conn, lyrType);
                // reset the existing table
                DataGridViewRowCollection rows = dgvLayerIndex.Rows;
                foreach (DataGridViewRow row in rows)
                {
                    row.Cells[1].Value = string.Empty;
                }
                foreach (int i in chkLayerIndex.CheckedIndices)
                {
                    chkLayerIndex.SetItemCheckState(i, CheckState.Unchecked);
                }
            } 
        }

        private void chkViewLayers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var clb = (CheckedListBox) sender;
            if (clb.SelectedItem == null) return;

            var lyr = _localBaseMapLayerLookup[clb.SelectedItem.ToString()];
            if (e.NewValue.ToString() == "Checked")
            {
                // add the checked layer to the active map tab
                _appManager.Map.Layers.Add(lyr);
            }
            else
            {
                // remove the checked layer from the active map
                _appManager.Map.Layers.Remove(lyr);
            }
            _appManager.Map.Invalidate();
            _dirtyTabs = true;
        }

        //public static ProjectionInfo DefaultProjection { get { return KnownCoordinateSystems.Projected.World.WebMercator; } }

        //public Envelope DefaultMapExtents()
        //{
        //    var defaultMapExtent = new Envelope(-130, -60, 10, 55);
        //    var xy = new double[4];
        //    xy[0] = defaultMapExtent.Minimum.X;
        //    xy[1] = defaultMapExtent.Minimum.Y;
        //    xy[2] = defaultMapExtent.Maximum.X;
        //    xy[3] = defaultMapExtent.Maximum.Y;
        //    var z = new double[] { 0, 0 };
        //    ProjectionInfo wgs84 = KnownCoordinateSystems.Geographic.World.WGS1984;
        //    Reproject.ReprojectPoints(xy, z, wgs84, DefaultProjection, 0, 2);
        //    return new Envelope(xy[0], xy[2], xy[1], xy[3]);
        //}

        private void btnAddView_Click(object sender, EventArgs e)
        {
            if (txtViewName.Text.Length <= 0) return;
            var txt = txtViewName.Text;
            txtViewName.Text = string.Empty;
            // clean the filename for a key value
            var fname = new string(txt.Where(ch => !InvalidFileNameChars.Contains(ch)).ToArray());
            fname = fname.Replace(" ", "");
            var key = "kMap_" + fname;
            if (lstViews.Items.Contains(txt))
            {
                // this map tab already exists
                _appManager.DockManager.SelectPanel(key);
                return;
            }
            lstViews.Items.Add(txt);
            // create a new map to stick to the tab using the settings from _baseMap
            var nMap = new Map
            {
                BackColor = SdrConfig.Project.Go2ItProjectSettings.Instance.MapBgColor,
                Dock = DockStyle.Fill,
                Visible = true,
                // Projection = _baseMap.Projection,
                // ViewExtents = _baseMap.ViewExtents
            };
            // create new dockable panel and stow that shit yo!
            var dp = new DockablePanel(key, txt, nMap, DockStyle.Fill);
            _appManager.DockManager.Add(dp);
            _appManager.DockManager.SelectPanel(key);
            _dirtyTabs = true;
        }

        private void btnRemoveView_Click(object sender, EventArgs e)
        {
            if (lstViews.SelectedItem == null) return;
            if (lstViews.SelectedItem.ToString().Length <= 0) return;
            var txt = lstViews.SelectedItem.ToString();
            // clean the filename for a key value
            var fname = new string(txt.Where(ch => !InvalidFileNameChars.Contains(ch)).ToArray());
            fname = fname.Replace(" ", "");
            var key = "kMap_" + fname;
            // remove the dockpanel now (which will also kill the map and tab)
            _appManager.DockManager.Remove(key);
            lstViews.Items.Remove(txt);
            // auto updates with next selected panel, if no panels then wipe the list
            if (lstViews.Items.Count == 0)
            {
                chkViewLayers.Items.Clear();
            }
            _dirtyTabs = true;
        }

        private void cmbCityLimitLayer_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _dirtyProject = true;
        }

        private void cmbNotesLayer_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _dirtyProject = true;
        }

        private void chkRoadLayers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            _dirtyProject = true;
        }

        private void chkAddressLayers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            _dirtyProject = true;
        }

        private void cmbCellSectorLayer_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _dirtyProject = true;
        }

        private void cmbESNLayer_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _dirtyProject = true;
        }

        private void cmbParcelsLayer_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _dirtyProject = true;
        }

        private void cmbHydrantsLayer_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _dirtyProject = true;
        }

        private void chkKeyLocationsLayers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            _dirtyProject = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Debug.WriteLine(_appManager.Map.ViewExtents.ToString());
            Debug.WriteLine(_baseMap.ViewExtents.ToString());
            Debug.WriteLine(_appManager.Map.Extent.ToString());
            Debug.WriteLine(_baseMap.Extent.ToString());
            Debug.WriteLine(_appManager.Map.Projection.ToEsriString());
            Debug.WriteLine(_baseMap.Projection.ToEsriString());
        }

        private void btnIndexCancel_Click(object sender, EventArgs e)
        {
            if (_idxWorker.WorkerSupportsCancellation)
            {
                _idxWorker.CancelAsync();
            }
        }

        private class IndexObject
        {
            public IFeatureSet FeatureSet { get; private set; }
            public string LayerType { get; private set; }
            public List<KeyValuePair<string, string>> FieldLookup { get; private set; }
            public IndexObject(IFeatureSet featureSet, List<KeyValuePair<string, string>> fieldLookup, string layerType)
            {
                FeatureSet = featureSet;
                FieldLookup = fieldLookup;
                LayerType = layerType;
            }
        }
    }
}