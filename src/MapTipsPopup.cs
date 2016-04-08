using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.SDR.Controls;
using SdrConfig = SDR.Configuration;

namespace Go2It
{
    public class MapTipsPopup
    {
        private Map _map;
        private readonly AppManager _appManager;
        private string _toolTipStr;
        private ToolTip _mapTip { get; set; }
        private Point _currentPosition { get; set; }

        public bool ShowMapTips { get; set; }

        public MapTipsPopup(AppManager app)
        {
            _appManager = app;
            _appManager.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
            _appManager.DockManager.PanelHidden += DockManagerOnPanelHidden;

            _mapTip = new ToolTip();
        }

        private void DockManagerOnPanelHidden(object sender, DockablePanelEventArgs e)
        {
            DockPanelInfo dockInfo;
            var dockControl = (TabDockingControl)sender;
            var key = e.ActivePanelKey;

            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;
            if (!key.StartsWith("kMap_")) return;
            if (_map == null) return;

            _map.MouseHover -= MapOnMouseHover;
            _map.MouseMove -= MapOnMouseMove;
            _map.MouseLeave -= MapOnMouseLeave;
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

            _map.MouseHover += MapOnMouseHover;
            _map.MouseMove += MapOnMouseMove;
            _map.MouseLeave += MapOnMouseLeave;
        }

        private void MapOnMouseLeave(object sender, EventArgs e)
        {
            // _mapTip.Hide(_map);
            // _currentPosition = new Point { X = e.X, Y = e.Y };
            // Debug.WriteLine("OnMouseMove");
        }

        private void MapOnMouseMove(object sender, MouseEventArgs e)
        {
            _currentPosition = new Point { X = e.X, Y = e.Y };
            // Debug.WriteLine("OnMouseMove");
        }

        private void MapOnMouseHover(object sender, EventArgs e)
        {
            // _mapTip.Show("maptip", _map, _currentPosition);
            Debug.WriteLine("OnMouseHover");
            // var mapTip = new ToolTip();
            /* mapTip.Active = true;
            Debug.WriteLine(mapTip.AutoPopDelay);
            Debug.WriteLine(mapTip.ReshowDelay);
            Debug.WriteLine(mapTip.InitialDelay);
            Debug.WriteLine(mapTip.AutomaticDelay);
            Debug.WriteLine(mapTip.ReshowDelay);
            Debug.WriteLine(mapTip.ShowAlways); */

            // mapTip.Show("test", _map, _currentPosition);

            //var x = sender;
            // var z = eventArgs;

            // tT.Show("Why So Many Times?", _map);

            //ToolTip mapTip = new ToolTip();
            //// mapTip.AutoPopDelay = 5000;
            //mapTip.InitialDelay = 1000;
            //mapTip.ReshowDelay = 500;
            //mapTip.ShowAlways = true;

            //mapTip.SetToolTip(_map, "test");

            //mapTip.IsBalloon = true;
        }
    }
}
