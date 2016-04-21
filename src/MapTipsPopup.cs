using System;
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
        private Map _map;  // currently active map
        private readonly AppManager _appManager;
        // private string _toolTipStr;
        private ToolTip MapTip { get; set; }  // maptip to display information for features hovered over
        private Timer HoverTimer { get; set; }  // timer tracks if user has hovered long enough to display a maptip
        private Point CurrentPosition { get; set; }  // current mouse position to display the maptip
        private bool _isMapActive;  // make sure we are hovering over the actual map, and not anything else

        // TODO: we need to finish integrating the option to activate and deactivate maptips (see main plugin)
        // also should we disable maptips while in admin mode.. cause it could get to be confusing
        // public bool ShowMapTips { get; set; }

        public MapTipsPopup(AppManager app)
        {
            _appManager = app;
            _appManager.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
            _appManager.DockManager.PanelHidden += DockManagerOnPanelHidden;

            // convert the maptips lookup collection to 
            MapTip = new ToolTip {IsBalloon = false};  // setting balloon to true results in visible redraws on the hoverTimer tick show event
            MapTipsLookup();  // convert the maptip settings list into a proper lookup for speed on maptip display
            
            
            HoverTimer = new Timer {Interval = 1000};  // delay on tooltip display in ms
            HoverTimer.Tick += TimerOnTick;
            HoverTimer.Enabled = false;
        }

        public void MapTipsLookup()
        {
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.MapTips.Count <= 0) return;
            for (int i = 0; i <= SdrConfig.Project.Go2ItProjectSettings.Instance.MapTips.Count - 1; i++)
            {
                //var arr = SdrConfig.Project.Go2ItProjectSettings.Instance.MapTips[i].Split(',');
                //dgvMapTips.Rows.Add(CloneDataGridViewRowTemplate(_mapTipsDgvRowTemplate));
                //// generate the cells for the maptips datagridview row
                //FillMapTipFieldsComboBox(arr[0], (DataGridViewComboBoxCell)dgvMapTips.Rows[i].Cells[1]);
                //dgvMapTips.Rows[i].Cells[0].Value = arr[0];
                //dgvMapTips.Rows[i].Cells[1].Value = arr[1];
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
            MapTip.Show("maptip", _map, CurrentPosition);
        }

        private void MapOnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMapActive) return;

            CurrentPosition = new Point { X = e.X, Y = e.Y };
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
