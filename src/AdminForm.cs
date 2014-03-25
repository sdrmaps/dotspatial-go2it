using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using DotSpatial.SDR.Controls;
using DotSpatial.Symbology;
using DotSpatial.Topology;
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
        [Import("Shell")]
        internal ContainerControl Shell { get; set; }

        // name of the initial map tab
        private const string MapTabDefaultName = "My Map";

        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();
        private readonly BackgroundWorker _idxWorker = new BackgroundWorker();

        // change tracking flags for project changes as well as mapview changes
        private bool _dirtyProject;

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

        // temp dict holds all base map layers for selection/removal on map tabs
        // on save this is passed to the dockingcontrol baselayerlookup dict
        private readonly Dictionary<string, IMapLayer> _localBaseMapLayerLookup = new Dictionary<string, IMapLayer>();

        public AdminForm(AppManager app)
        {
            InitializeComponent();
            _appManager = app;
            _dockingControl = (DockingControl) app.DockManager;
            // a basemap to hold all layers for the adminlegend
            _baseMap = new Map
            {
                Dock = DockStyle.Fill,
                Visible = false,
                Projection = app.Map.Projection,
                ViewExtents = app.Map.ViewExtents
            };
            // set options on our indexing bgworker
            _idxWorker.WorkerReportsProgress = true;
            _idxWorker.WorkerSupportsCancellation = true;
            // splitter stuff
            adminLayerSplitter.SplitterWidth = 10;
            adminLayerSplitter.Paint += Splitter_Paint;
            // overall events tied to main application
            _appManager.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;

            // map tracking events on removal and addition of a layer
            _baseMap.Layers.LayerRemoved += LayersOnLayerRemoved;
            _baseMap.Layers.LayerAdded += LayersOnLayerAdded;

            // populate all the settings, layers, and maps to the form and attach a legend
            AttachLegend();
            PopulateMapViews();
            PopulateSettingsToForm();
            PopulateUsersToForm();

            // check if we have any available map tab views
            if (_dockingControl.DockPanelLookup.Count == 0)
            {
                const string txt = MapTabDefaultName;
                // clean the filename for a key value
                var fname = new string(txt.Where(ch => !InvalidFileNameChars.Contains(ch)).ToArray());
                fname = fname.Replace(" ", "");
                var key = "kMap_" + fname;
                // create a new map to stick to the tab using the settings from _baseMap
                var nMap = new Map
                {
                    BackColor = mapBGColorPanel.BackColor,
                    Dock = DockStyle.Fill,
                    Visible = true,
                    Projection = _baseMap.Projection,
                    ViewExtents = _baseMap.ViewExtents
                };
                // create new dockable panel and stow that shit yo!
                var dp = new DockablePanel(key, txt, nMap, DockStyle.Fill);
                cmbActiveMapTab.Items.Add(dp.Caption);
                // add the new tab view to the main form and select it to activate
                _appManager.DockManager.Add(dp);
                _appManager.DockManager.SelectPanel(key);
            }
            else // if there is then select the active tab now
            {
                _appManager.DockManager.SelectPanel(SdrConfig.Project.Go2ItProjectSettings.Instance.ActiveMapViewKey);
            }

            // setup all interface events now
            FormClosing += AdminForm_Closing; // check for isdirty changes to project file
            FormClosed += AdminFormClosed;
            chkViewLayers.ItemCheck += chkViewLayers_ItemCheck; // add or remove item to specific map tab view

            // setup a background worker for update progress bar on indexing tab
            _idxWorker.DoWork += idx_DoWork;
            _idxWorker.ProgressChanged += idx_ProgressChanged;
            _idxWorker.RunWorkerCompleted += idx_RunWorkerCompleted;

            _dirtyProject = false; // reset dirty flag after populating form on startup
        }

        private void AdminFormClosed(object sender, FormClosedEventArgs formClosedEventArgs)
        {
            // unbind all our events now
            adminLayerSplitter.Paint -= Splitter_Paint;
            _appManager.DockManager.ActivePanelChanged -= DockManager_ActivePanelChanged;
            _baseMap.Layers.LayerRemoved -= LayersOnLayerRemoved;
            _baseMap.Layers.LayerAdded -= LayersOnLayerAdded;
            FormClosing -= AdminForm_Closing;
            chkViewLayers.ItemCheck -= chkViewLayers_ItemCheck;
            _idxWorker.DoWork -= idx_DoWork;
            _idxWorker.ProgressChanged -= idx_ProgressChanged;
            _idxWorker.RunWorkerCompleted -= idx_RunWorkerCompleted;
            FormClosed -= AdminFormClosed;
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
                IFeatureSet fs = FeatureSet.Open(mLayer.DataSet.Filename);
                RemoveLayer(fs); // perform all form specific remove operations
            }
            // we need to cycle through all the available dockpanels check if the layer has been set on that map
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
            string f = Path.GetFileNameWithoutExtension(mapLayer.Filename);
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
                        // the project was not saved, reset the active app.map
                        _appManager.SerializationManager.New();
                        break;
                    case DialogResult.Yes:
                        e.Cancel = false;  // continue to allow the form to close
                        if (!ShowSaveProjectDialog())
                        {
                            // user decided not to save, reset main app map back to defaults
                            _appManager.SerializationManager.New();
                        } // else the user did a proper save and all things should be synced now
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
                        MessageBox.Show(string.Format("Save changes to current project [{0}]?", GetProjectShortName()),
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
                            e.Cancel = false; // finish form closing
                            if (!SaveProject(_appManager.SerializationManager.CurrentProjectFile))
                            {
                                // user canceled the save, so reload the original project file now
                                _appManager.SerializationManager.OpenProject(_appManager.SerializationManager.CurrentProjectFile);
                            } // else the save was successful and thus everything is in sync now
                            break;
                    }
                }
                else // no changes have been made, allow the form to finish closing
                {
                    e.Cancel = false;
                }
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
            var h = "this";
            // todo:probably should do some event here to update all map tabs
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
                    if (String.IsNullOrEmpty(_appManager.SerializationManager.CurrentProjectFile))
                    {
                        SaveProjectAs();
                    }
                    else
                    {
                        SaveProject(_appManager.SerializationManager.CurrentProjectFile);
                    }
                    Close();
                }
                else
                {
                    SaveProjectAs();
                    Close();
                }
            else
            {
                MessageBox.Show(@"Layer Indexing Operation is running, please wait or cancel to process");
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
            // set the baselayer lookup to the localized one used by the admin form
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
                // this is saved to dbase by project manager on serialization event fired just below
                ApplyProjectSettings();
                // swap the active map out with our base map now -> all layers will be serialized (not just active ones)
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
            var m = (Map) _appManager.Map;
            m.BackColor = mapBGColorPanel.BackColor;
            foreach (KeyValuePair<string, DockPanelInfo> dpi in _dockingControl.DockPanelLookup)
            {
                if (!dpi.Key.Trim().StartsWith("kMap")) continue;
                dpi.Value.DotSpatialDockPanel.InnerControl.BackColor = mapBGColorPanel.BackColor;
                dpi.Value.DotSpatialDockPanel.InnerControl.Refresh();
            }
            _appManager.Map.Refresh();
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
                    table.Rows.Add(key, "");
                }
                return table;
            }
            sc = ApplyCheckBoxSetting(chkRoadLayers);
            if (sc != null && sc.Contains(layName))
            {
                var file = ReadIndexLines(SdrConfig.Settings.Instance.ApplicationDataDirectory + @"\Config\road_indexes.txt");
                foreach (string key in file)
                {
                    table.Rows.Add(key, "");
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
            return null;
        }

        private String GetLayerIndexTable(string layName)
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
                        // TODO: account for some sort of logging and inform the user of the issues here
                        return;
                    }
                    // make sure this featureset has FID values for lookup
                    fs.AddFid(); 
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
                    foreach (var kv in io.FieldLookup)
                    {
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
            if (cmbLayerIndex.SelectedItem.ToString().Length <= 0) return;

            var lyrName = cmbLayerIndex.SelectedItem.ToString();
            var lyrType = GetLayerIndexTable(lyrName);
            var db = SQLiteHelper.GetSQLiteFileName(SdrConfig.Settings.Instance.ProjectRepoConnectionString);
            var d = Path.GetDirectoryName(db);
            if (d != null)
            {
                var path = Path.Combine(d, "indexes", lyrType);
                if (System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.Delete(path, true);
                }
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

        private void chkViewLayers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var clb = (CheckedListBox)sender;
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
            _dirtyProject = true;
        }

        private void btnAddView_Click(object sender, EventArgs e)
        {
            if (txtViewName.Text.Length <= 0) return;
            var txt = txtViewName.Text;
            txtViewName.Text = string.Empty;
            // clean the filename for a key value
            var fname = new string(txt.Where(ch => !InvalidFileNameChars.Contains(ch)).ToArray());
            fname = fname.Replace(" ", "");
            var key = "kMap_" + fname;
            if (cmbActiveMapTab.Items.Contains(txt))
            {
                // this map tab already exists
                _appManager.DockManager.SelectPanel(key);
                return;
            }
            cmbActiveMapTab.Items.Add(txt);
            // create a new map to stick to the tab using the settings from _baseMap
            var nMap = new Map
            {
                BackColor = mapBGColorPanel.BackColor,
                Dock = DockStyle.Fill,
                Visible = true,
                Projection = _baseMap.Projection,
                ViewExtents = _baseMap.ViewExtents
            };
            
            // create new dockable panel and stow that shit yo!
            var dp = new DockablePanel(key, txt, nMap, DockStyle.Fill);
            _appManager.DockManager.Add(dp);
            _appManager.DockManager.SelectPanel(key);
            _dirtyProject = true;
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
            // remove the dockpanel now (which will also kill the map and tab)
            _appManager.DockManager.Remove(key);
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
            var l  = new List<string>();
            foreach (var item in clb.CheckedItems)
            {
                l.Add(item.ToString());
            }
            return l;
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

        private void UpdateLayerIndexCombo(List<string> ad, List<string> rd, List<string> kl, List<string> cs, List<string> cl, List<string> es, List<string> pl)
        {
            cmbLayerIndex.Items.Clear();
            var sels = new List<object>(ad.Count + rd.Count + kl.Count + cs.Count + cl.Count + es.Count + pl.Count);
            sels.AddRange(ad);
            sels.AddRange(rd);
            sels.AddRange(kl);
            sels.AddRange(cs);
            sels.AddRange(cl);
            sels.AddRange(es);
            sels.AddRange(pl);
            cmbLayerIndex.Items.AddRange(sels.ToArray());
        }

        private void chkRoadLayers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var clb = (CheckedListBox)sender;
            var rd = new List<string>();
            foreach (var item in clb.CheckedItems)
            {
                if (item != clb.Items[e.Index].ToString())
                {
                    rd.Add(item.ToString());
                }
            }
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
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl);
            _dirtyProject = true;
        }

        private void chkAddressLayers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var clb = (CheckedListBox)sender;
            var ad = new List<string>();
            foreach (var item in clb.CheckedItems)
            {
                if (item != clb.Items[e.Index].ToString())
                {
                    ad.Add(item.ToString());
                }
            }
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
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl);
            _dirtyProject = true;
        }

        private void chkKeyLocationsLayers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var clb = (CheckedListBox)sender;
            var kl = new List<string>();
            foreach (var item in clb.CheckedItems)
            {
                if (item != clb.Items[e.Index].ToString())
                {
                    kl.Add(item.ToString());   
                }
            }
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
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl);
            _dirtyProject = true;
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

        private void cmbActiveMapTab_SelectedIndexChanged(object sender, EventArgs e)
        {
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

        private void idx_DoWork(object sender, DoWorkEventArgs e)
        {
            var io = e.Argument as IndexObject;
            // our worker for running the indexing operation
            var worker = sender as BackgroundWorker;
            // setup everything else we need to generate ourselves a lucene index
            if (io != null)
            {
                IFeatureSet fl = io.FeatureSet;
                var db = SQLiteHelper.GetSQLiteFileName(SdrConfig.Settings.Instance.ProjectRepoConnectionString);
                var d = Path.GetDirectoryName(db);
                if (d == null) return;
                var path = Path.Combine(d, "indexes", io.LayerType);
                DirectoryInfo di = System.IO.Directory.CreateDirectory(path);
                Directory dir = FSDirectory.Open(di);
                
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
                        KeyValuePair<string, string> kv = list[i];
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
                    double ratio = (double)fl.DataTable.Rows.Count / 100;
                    // update the progress bar on the main thread interface
                    int result = Convert.ToInt32((x + 1) / ratio);
                    if (worker != null) worker.ReportProgress((result));
                }
            }
        }

        private void idx_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                idxProgressBar.Hide();
                DeleteIndex();
            }
            else if (e.Error != null)
            {
                // TODO:this.tbProgress.Text = ("Error: " + e.Error.Message);
                MessageBox.Show(@"Error on Indexing Layer");
            }
            idxProgressBar.Hide();
        }

        private void idx_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // idxProgressBar.Text = (e.ProgressPercentage.ToString() + "%");
            idxProgressBar.Value = e.ProgressPercentage;
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
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl);
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
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl);
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
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl);
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
            UpdateLayerIndexCombo(ad, rd, kl, cs, cl, es, pl);
            _dirtyProject = true;
        }

        private void cmbHydrantsLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            _dirtyProject = true;
        }
    }
}