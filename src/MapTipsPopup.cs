using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Data;
using DotSpatial.SDR.Controls;
using DotSpatial.Topology;
using Point = System.Drawing.Point;
using SdrConfig = SDR.Configuration;

namespace Go2It
{
    public class MapTipsPopup
    {
        public bool ShowMapTips { get; set; }  // turn the display of maptips off and on from main plugin

        private Map _map;  // currently active map
        private ToolTip MapTip { get; set; }  // maptip to display information for features hovered over
        private Timer HoverTimer { get; set; }  // timer tracks if user has hovered long enough to display a maptip
        private Point MousePosition { get; set; }  // current mouse position to display the maptip
        private bool _isMapActive;  // make sure we are hovering over an actual map
        // dict that holds all the maptip layers and features to display on
        private readonly Dictionary<string, HashSet<string>> _mapTipsDict = new Dictionary<string, HashSet<string>>();
        // simple layer lookup to get a maplayer using the layer name from any existing maptab
        private Dictionary<string, IMapLayer> _layerLookup = new Dictionary<string, IMapLayer>();

        // populated by the main plugin on project deserialization/serialization
        // using ProjectManager.GetLayerLookup() -> gives access to all availble layers
        public Dictionary<string, IMapLayer> LayerLookup
        {
            set { _layerLookup = value; }
        }

        public MapTipsPopup(AppManager app)
        {
            AppManager appManager = app;
            appManager.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
            appManager.DockManager.PanelHidden += DockManagerOnPanelHidden;

            // convert the maptips lookup collection to 
            MapTip = new ToolTip {IsBalloon = false};  // setting balloon to true results in visible redraws on the hoverTimer tick show event
            MapTipsLookup();  // convert the maptip settings list into a proper lookup for speed on maptip display

            SdrConfig.User.Go2ItUserSettings.Instance.AdminModeChanged += InstanceOnAdminModeChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.MapTipsChanged += InstanceOnMapTipsChanged;

            HoverTimer = new Timer {Interval = 1000};  // delay on tooltip display in ms
            HoverTimer.Tick += TimerOnTick;
            HoverTimer.Enabled = false;
        }

        private void InstanceOnMapTipsChanged(object sender, EventArgs eventArgs)
        {
            MapTipsLookup();  // repopulate our maptips lookup dict on maptip changes
        }

        private void InstanceOnAdminModeChanged(object sender, EventArgs eventArgs)
        {
            // turn maptips off when admin mode is active regardless of value of ShowMapTips
            HoverTimer.Enabled = !SdrConfig.User.Go2ItUserSettings.Instance.AdminModeActive;
        }

        public void MapTipsLookup()
        {
            _mapTipsDict.Clear();
            for (var i = 0; i <= SdrConfig.Project.Go2ItProjectSettings.Instance.MapTips.Count - 1; i++)
            {
                var arr = SdrConfig.Project.Go2ItProjectSettings.Instance.MapTips[i].Split(',');
                if (_mapTipsDict.ContainsKey(arr[0]))
                {
                    _mapTipsDict[arr[0]].Add(arr[1]);
                }
                else
                {
                    _mapTipsDict.Add(arr[0], new HashSet<string>{ arr[1]});
                }
            }
        }

        // simulation of an OnHoverEvent()
        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            HoverTimer.Stop();
            DisplayMapTip();
        }

        private void DisplayMapTip()
        {
            var pCoord = _map.PixelToProj(MousePosition); // turn pixels into coordinates
            IEnvelope env = new Envelope(pCoord);
            env.ExpandBy(5);  // arbitrary unit size expansion (to generate an extent)
            var msgList = new List<string>();  // list of all maptips to display

            foreach (KeyValuePair<string, HashSet<string>> kv in _mapTipsDict)
            {
                IMapLayer mapLyr;
                _layerLookup.TryGetValue(kv.Key, out mapLyr);
                // check if this layer is present on the active map
                if (!_map.Layers.Contains(mapLyr)) continue;

                // validate the dataset is available
                var mfl = mapLyr as IMapFeatureLayer;
                if (mfl == null || mfl.DataSet == null) continue;

                // select all features within the extent of our mouse position
                IFeatureSet fs = mfl.DataSet;
                var fl = fs.Select(env.ToExtent());
                if (fl.Count <= 0) continue;

                msgList.Add(kv.Key);  // add the name of the layer to maptip list
                foreach (IFeature ft in fl)
                {
                    // cycle through all the maptips columns for display
                    foreach (var s in kv.Value)
                    {
                        var v = ft.DataRow[s];
                        msgList.Add(s + ": " + v);  // add the feature type and value to maptip list
                    }
                }
            }
            // convert the list of maptips into a string for display
            var msg = string.Join("\n", msgList.ToArray());
            MapTip.Show(msg, _map, MousePosition);
        }

        private void MapOnMouseMove(object sender, MouseEventArgs e)
        {
            if (!ShowMapTips) return;  // check if maptips are displayed, no need to continue if not
            if (!_isMapActive) return;  // check the mouse is hovering over an active map

            MousePosition = new Point {X = e.X, Y = e.Y};
            // if HoverTimer is active reset it now (do not display the maptip on timer.tick)
            if (HoverTimer.Enabled)
            {
                HoverTimer.Enabled = false;
            }
            // reactivate the HoverTimer and wait for either timer.tick or this event again
            HoverTimer.Enabled = true;
        }

        private void DockManagerOnPanelHidden(object sender, DockablePanelEventArgs e)
        {
            DockPanelInfo dockInfo;
            var dockControl = (TabDockingControl)sender;
            var key = e.ActivePanelKey;

            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;
            if (!key.StartsWith("kMap_")) return;
            if (_map == null) return;

            _map.MouseEnter -= MapOnMouseEnter;
            _map.MouseLeave -= MapOnMouseLeave;
            _map.MouseMove -= MapOnMouseMove;
        }

        private void DockManagerOnActivePanelChanged(object sender, DockablePanelEventArgs dockablePanelEventArgs)
        {
            DockPanelInfo dockInfo;
            var dockControl = (DockingControl)sender;
            var key = dockablePanelEventArgs.ActivePanelKey;

            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;
            if (!key.StartsWith("kMap_")) return;

            _map = (Map)dockInfo.DotSpatialDockPanel.InnerControl;
            if (_map == null) return;

            _map.MouseEnter += MapOnMouseEnter;
            _map.MouseLeave += MapOnMouseLeave;
            _map.MouseMove += MapOnMouseMove;
        }

        private void MapOnMouseEnter(object sender, EventArgs e)
        {
            _isMapActive = true;  // mouse is currently within the bounds of the map
        }

        private void MapOnMouseLeave(object sender, EventArgs e)
        {
            _isMapActive = false;  // the mouse is no longer within the bounds of the map
            HoverTimer.Enabled = false;  // no need to track hover time anymore

            if (_map == null) return;

            MapTip.Hide(_map);  // hide any maptip that may currently exist
        }
    }
}
