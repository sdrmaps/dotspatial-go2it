using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
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

        private ProjectManager ProjectManager { get; set; }
        private OverviewMapForm _overviewMapForm;
        private readonly Dictionary<string, Map> _thumbMaps = new Dictionary<string, Map>();
 
        private const string HomeMenuKey = DotSpatial.Controls.Header.HeaderControl.HomeRootItemKey;

        public override void Activate()
        {
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "OverviewMap", ToggleOverviewTool_Click)
            {
                GroupCaption = "Overview_Toggle",
                ToolTipText = "Toggle Overview Map Visibility",
                SmallImage = Resources.legend_16,
                LargeImage = Resources.legend_32
            });

            ProjectManager = (ProjectManager)App.SerializationManager;
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

            if (_overviewMapForm == null)
            {
                _overviewMapForm = new OverviewMapForm();
                _overviewMapForm.FormClosing += OverviewMap_Closed;
                // _overviewMapForm.ResizeEnd += OverviewMapFormOnResize;
            }

            Map overViewMap;
            if (!_thumbMaps.TryGetValue(key, out overViewMap))
            {
                overViewMap = ProjectManager.CreateNewMap(key + "_thumb");
                var map  = (Map)dockInfo.DotSpatialDockPanel.InnerControl;
                overViewMap.Layers.AddRange(map.Layers);
                // TODO: remove notes layer?

                _thumbMaps.Add(key, overViewMap);
            }

            overViewMap.Dock = DockStyle.Fill;
            overViewMap.Height = _overviewMapForm.Height;
            overViewMap.Width = _overviewMapForm.Width;

            _overviewMapForm.Controls.Clear();
            _overviewMapForm.Controls.Add(overViewMap);
            overViewMap.Invalidate();
        }

        //private void OverviewMapFormOnResize(object sender, EventArgs eventArgs)
        //{
        // zoom to extent
        //    Map map = (Map)_overviewMapForm.Controls[0];
        //    map.Extent.Height = _overviewMapForm.Height;
        //    map.Extent.Width = _overviewMapForm.Width;
        //    // _overviewMapForm.Controls.Clear();
        //    // _overviewMapForm.Controls.Add(map);
        //}

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.ActivePanelChanged -= DockManagerOnActivePanelChanged;
            _overviewMapForm.FormClosing -= OverviewMap_Closed;
            base.Deactivate();
        }

        private void ToggleOverviewTool_Click(object sender, EventArgs e)
        {
            if (_overviewMapForm.Visible == false)
            {
                _overviewMapForm.Show(Shell);
            }
            else
            {
                _overviewMapForm.Close();
            }
            _overviewMapForm.Focus();
        }

        private void OverviewMap_Closed(object sender, FormClosingEventArgs e)
        {
            _overviewMapForm.Hide();
            e.Cancel = true; // cancel the event from closing completion (disposing of legend)
        }
    }
}
