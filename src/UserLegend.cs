using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using DotSpatial.SDR.Controls;
using Go2It.Properties;

namespace Go2It
{
    public class UserLegend : Extension
    {
        [Import("Shell")]
        internal ContainerControl Shell { get; set; }

        private RestrictedLegend _userLegend;
        private UserLegendForm _userLegendForm;

        private const string HomeMenuKey = DotSpatial.Controls.Header.HeaderControl.HomeRootItemKey;

        public override void Activate()
        {
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "Legend", ToggleLegendTool_Click)
            {
                GroupCaption = "Legend_Toggle",
                ToolTipText = "Toggle Legend Visibility",
                SmallImage = Resources.legend_16,
                LargeImage = Resources.legend_32
            });

            // hotkeys for this extension
            HotKeyManager.AddHotKey(new HotKey(Keys.F3, "Toggle Address Layers"), "Legend_Toggle_Address_Layers");

            App.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;

            // watch for hotkeys activated via the mainform plugin
            HotKeyManager.HotKeyEvent += HotKeyManagerOnHotKeyEvent;
            base.Activate();
        }

        private void HotKeyManagerOnHotKeyEvent(string action)
        {
            switch (action)
            {
                case "Legend_Toggle_Address_Layers":
                    ToggleAddressLayers();
                    break;
            }
        }

        private void ToggleAddressLayers()
        {
            if (App.Map == null) return;
            // get all layer names set as address layers
            var layers = SDR.Configuration.Project.Go2ItProjectSettings.Instance.AddressLayers;
            // locate and toggle required layers visibility
            foreach (IMapLayer ml in App.Map.Layers)
            {
                if (ml.GetType().Name != "MapImageLayer")
                {
                    var mFeatureLyr = ml as IMapFeatureLayer;
                    // make sure this is a valid map feature layer
                    if (mFeatureLyr != null && String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((mFeatureLyr.DataSet.Filename)))) return;
                    // get the name of this layer for comparison
                    var lyrName = Path.GetFileNameWithoutExtension(mFeatureLyr.DataSet.Filename);
                    // find the required layers and toggle that shit
                    foreach (string layer in layers)
                    {
                        if (lyrName == layer)
                        {
                            if (mFeatureLyr.IsVisible)
                            {
                                // mFeatureLyr.SetVisible();
                                // mFeatureLyr.
                            }
                        }
                    }
                }
            }
        }

        private void DockManagerOnActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            DockPanelInfo dockInfo;
            var dockControl = (DockingControl)sender;
            var key = e.ActivePanelKey;

            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;
            if (!key.StartsWith("kMap_")) return;

            if (_userLegend != null)
            {
                _userLegend.RootNodes.Clear();
            }

            // update the active map for the legend now
            var map = (Map)dockInfo.DotSpatialDockPanel.InnerControl;

            if (map == null) return;
            map.Legend = _userLegend;
            App.Legend = _userLegend;
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.ActivePanelChanged -= DockManagerOnActivePanelChanged;
            base.Deactivate();
        }

        private void ToggleLegendTool_Click(object sender, EventArgs e)
        {
            if (App.Map == null) return;

            if (_userLegendForm == null || _userLegendForm.IsDisposed)
            {
                _userLegendForm = new UserLegendForm();
                AttachLegend();
            }
            
            if (_userLegendForm.Visible == false)
            {
                _userLegendForm.Show(Shell);
                _userLegendForm.Focus();
            }
            else
            {
                _userLegendForm.Close();
            }
        }

        private void AttachLegend()
        {
            _userLegend = new RestrictedLegend
            {
                BackColor = System.Drawing.Color.White,
                ControlRectangle = new System.Drawing.Rectangle(0, 0, 176, 128),
                DocumentRectangle = new System.Drawing.Rectangle(0, 0, 34, 114),
                HorizontalScrollEnabled = true,
                Indentation = 30,
                IsInitialized = false,
                Location = new System.Drawing.Point(217, 12),
                MinimumSize = new System.Drawing.Size(5, 5),
                Name = "User Legend",
                ProgressHandler = null,
                ResetOnResize = false,
                SelectionFontColor = System.Drawing.Color.Black,
                SelectionHighlight = System.Drawing.Color.FromArgb(215, 238, 252),
                Size = new System.Drawing.Size(176, 128),
                TabIndex = 0,
                AllowDrop = false,
                Text = @"Legend",
                Dock = DockStyle.Fill,
                VerticalScrollEnabled = true
            };
            _userLegendForm.Controls.Add(_userLegend);
            App.Map.Legend = _userLegend;
            App.Legend = _userLegend;
        }
    }
}
