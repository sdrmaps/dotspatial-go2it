using System;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.SDR.Controls;

namespace Go2It
{
    /// <summary>
    /// This class displays info about active layer and number of selected features
    /// </summary>
    public class SelectionsDisplay
    {
        private Map _map;
        private readonly AppManager _appManager;
        private readonly StatusPanel _selectionStatusPanel;
        private bool _showSelectionStatus;

        public SelectionsDisplay(AppManager app)
        {
            _selectionStatusPanel = new StatusPanel
            {
                Width = 400
            };
            // set the application manager and the panel changed events
            _appManager = app;
            _appManager.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
            _appManager.DockManager.PanelHidden += DockManagerOnPanelHidden;
        }

        private void DockManagerOnPanelHidden(object sender, DockablePanelEventArgs e)
        {
            DockPanelInfo dockInfo;
            var dockControl = (TabDockingControl)sender;
            var key = e.ActivePanelKey;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

            if (!key.StartsWith("kMap_")) return;

            if (_map == null) return;
            _map.SelectionChanged -= MapOnSelectionChanged;
        }

        private void DockManagerOnActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            DockPanelInfo dockInfo;
            var dockControl = (DockingControl)sender;
            var key = e.ActivePanelKey;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

            if (!key.StartsWith("kMap_")) return;

            _map = (Map)dockInfo.DotSpatialDockPanel.InnerControl;
            if (_map == null) return;
            _map.SelectionChanged += MapOnSelectionChanged;
        }

        private void MapOnSelectionChanged(object sender, EventArgs eventArgs)
        {
            if (!ShowSelectionStatus)
            {
                return;
            }

            if (_map.MapFrame.IsSelected)
            {
                _selectionStatusPanel.Caption = "All Layers Selected";
            }
            else
            {
                if (_map.Layers.SelectedLayer == null) return;
                string layName = _map.Layers.SelectedLayer.LegendText;
                var mfl = _map.Layers.SelectedLayer as IMapFeatureLayer;
                if (mfl != null)
                {
                    _selectionStatusPanel.Caption = String.Format("layer: {0} Selected: {1}", layName, mfl.Selection.Count);
                }
            }
                
        }

        public bool ShowSelectionStatus
        {
            get { return _showSelectionStatus; }
            set
            {
                _showSelectionStatus = value;
                if (_showSelectionStatus == false)
                {
                    _appManager.ProgressHandler.Remove(_selectionStatusPanel);
                }
                else
                {
                    _appManager.ProgressHandler.Add(_selectionStatusPanel);
                }
                _selectionStatusPanel.Caption = String.Empty;
            }
        }
    }
}
