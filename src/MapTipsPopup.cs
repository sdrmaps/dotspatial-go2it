using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private Map _map;  // currently active map
        private readonly AppManager _appManager;
        private ToolTip MapTip { get; set; }  // maptip to display information for features hovered over
        private Timer HoverTimer { get; set; }  // timer tracks if user has hovered long enough to display a maptip
        private Point MousePosition { get; set; }  // current mouse position to display the maptip
        private bool _isMapActive;  // make sure we are hovering over the actual map, and not anything else

        private readonly Dictionary<string, HashSet<string>> _mapTipsDict = new Dictionary<string, HashSet<string>>();

        private Dictionary<string, IMapLayer> _layerLookup = new Dictionary<string, IMapLayer>();


        // TODO: we need to finish integrating the option to activate and deactivate maptips (see main plugin)
        // also should we disable maptips while in admin mode.. cause it could get to be confusing
        // public bool ShowMapTips { get; set; }

        // populated by the main plugin on project deserialization/serialization using ProjectManager.GetLayerLookup()
        public Dictionary<string, IMapLayer> LayerLookup
        {
            set { _layerLookup = value; }
        }

        public MapTipsPopup(AppManager app)
        {
            _appManager = app;
            _appManager.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
            _appManager.DockManager.PanelHidden += DockManagerOnPanelHidden;

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
            // turn maptips off when admin mode is active
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

        // simulated hover event
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
            var msgList = new List<string>();

            foreach (KeyValuePair<string, HashSet<string>> kv in _mapTipsDict)
            {
                IMapLayer mapLyr;
                _layerLookup.TryGetValue(kv.Key, out mapLyr);
                // check if this layer is present on the active map
                if (!_map.Layers.Contains(mapLyr)) continue;

                var mfl = mapLyr as IMapFeatureLayer;
                // validate the dataset is available
                if (mfl == null || mfl.DataSet == null) continue;

                IFeatureSet fs = mfl.DataSet;
                // select all features to list under the mouse
                // TODO: doesnt seem to work with points?
                var fl = fs.Select(env.ToExtent());
                if (fl.Count <= 0) continue;

                msgList.Add(kv.Key);
                foreach (IFeature ft in fl)
                {
                    // cycle through all the maptips columns to display
                    foreach (string s in kv.Value)
                    {
                        var v = ft.DataRow[s];
                        msgList.Add(s + ": " + v);
                    }
                }
            }
            // convert the list of maptips into a string for display
            var msg = string.Join("\n", msgList.ToArray());
            MapTip.Show(msg, _map, MousePosition);
        }

        private void MapOnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMapActive) return;

            MousePosition = new Point { X = e.X, Y = e.Y };
            // reset the timer on a move event, we are simulating a hover event
            if (HoverTimer.Enabled)
            {
                HoverTimer.Enabled = false;
            }
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
            HoverTimer.Enabled = false;  // no need to track hover time 

            if (_map == null) return;

            MapTip.Hide(_map);  // hide any maptip that may currently exist
        }
    }
}
