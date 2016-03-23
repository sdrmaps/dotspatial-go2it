using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        private bool _showMapTips;
        public ToolTip _mapTip { get; set; }

        public MapTipsPopup(AppManager app)
        {
            _appManager = app;
            _appManager.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
            _appManager.DockManager.PanelHidden += DockManagerOnPanelHidden;

            _mapTip = new ToolTip();
            // todo: function to apply maptips dictionary/list for query against
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

            // todo: do i need to capture mouse move to get the coordinate?
            _map.MouseHover += MapOnMouseHover;
            _map.MouseMove += MapOnMouseMove;

        }

        private void MapOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
           //  Debug.WriteLine("map tip hide");
           //  _mapTip.Hide(_map);
        }

        private void MapOnMouseHover(object sender, EventArgs eventArgs)
        {
            // Debug.WriteLine("map tip show");
            //_mapTip.Show("test", _map);

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

        public bool ShowMapTips
        {
            get
            {
                return _showMapTips;
            }
            set
            {
                _showMapTips = value;
                //if (_showMapTips == false)
                //{
                //    _appManager.ProgressHandler.Remove(_latLonStatusPanel);
                //}
                //else
                //{
                //    _appManager.ProgressHandler.Add(_latLonStatusPanel);
                //}
                //_latLonStatusPanel.Caption = String.Empty;
            }
        }
    }
}
