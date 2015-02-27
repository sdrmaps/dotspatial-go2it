using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.SDR.Controls;
using Go2It.Properties;

namespace Go2It
{
    public class OverviewMap : Extension
    {
        [Import("Shell")]
        internal ContainerControl Shell { get; set; }
       
        private Map _map;
        private const string HomeMenuKey = DotSpatial.Controls.Header.HeaderControl.HomeRootItemKey;

        private Map _overviewMap;
        private OverviewMapForm _overviewMapForm;

        public override void Activate()
        {
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "OverviewMap", ToggleOverviewTool_Click)
            {
                GroupCaption = "Overview_Toggle",
                ToolTipText = "Toggle Overview Map Visibility",
                SmallImage = Resources.legend_16,
                LargeImage = Resources.legend_32
            });

            App.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;
            base.Activate();
        }

        private void DockManagerOnActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            DockPanelInfo dockInfo;
            var dockControl = (DockingControl)sender;
            var key = e.ActivePanelKey;
            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

            if (!key.StartsWith("kMap_")) return;

            // update the active _map for the overview map now
            _map = (Map)dockInfo.DotSpatialDockPanel.InnerControl;
            if (_map == null) return;

            if (_overviewMapForm == null)
                _overviewMapForm = new OverviewMapForm();

            if (_overviewMap == null)
            {
                AttachMap();
            }
            _overviewMap = _map;
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.ActivePanelChanged -= DockManagerOnActivePanelChanged;
            _overviewMapForm.FormClosing -= OverviewMap_Closed;
            base.Deactivate();
        }

        private void ToggleOverviewTool_Click(object sender, EventArgs e)
        {
            if (_overviewMapForm == null)
                _overviewMapForm = new OverviewMapForm();

            if (_overviewMap == null)
                AttachMap();

            if (_overviewMapForm.Visible == false)
            {
                _overviewMapForm.Show(Shell);
                _overviewMapForm.Focus();
                _overviewMap.Invalidate();
            }
            else
            {
                _overviewMapForm.Close();
            }
        }

        private void OverviewMap_Closed(object sender, FormClosingEventArgs e)
        {
            _overviewMapForm.Hide();
            e.Cancel = true; // cancel the event from closing completion (disposing of legend)
        }

        private void AttachMap()
        {
            _overviewMap = new Map();
            _overviewMapForm.Controls.Add(_overviewMap);
            _overviewMapForm.FormClosing += OverviewMap_Closed;
        }
    }
}
