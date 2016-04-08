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
        private Timer _timer { get; set; }
        private Point _currentPosition { get; set; }
        private bool _isMapActive;

        public bool ShowMapTips { get; set; }

        public MapTipsPopup(AppManager app)
        {
            _appManager = app;
            _appManager.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
            _appManager.DockManager.PanelHidden += DockManagerOnPanelHidden;

            _mapTip = new ToolTip {IsBalloon = true};
            _timer = new Timer {Interval = 1000};  // delay on tooltip display in ms
            _timer.Tick += TimerOnTick;
            _timer.Enabled = false;
        }

        // this is our simulated hover event
        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            _timer.Stop();
            _mapTip.Show("maptip", _map, _currentPosition);
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
            _isMapActive = true;
        }
        private void MapOnMouseLeave(object sender, EventArgs e)
        {
            _isMapActive = false;
            _timer.Enabled = false;
            if (_map == null) return;
            _mapTip.Hide(_map);
        }

        private void MapOnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMapActive) return;

            _currentPosition = new Point { X = e.X, Y = e.Y };
            // reset the timer on a move event, we are watching for a hover
            if (_timer.Enabled)
            {
                _timer.Enabled = false;
            }
            _timer.Enabled = true;
        }
    }
}
