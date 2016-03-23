using System;
using System.Collections.Generic;
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

        private readonly Dictionary<string, List<string>> _addressLayersVisible = new Dictionary<string, List<string>>();
        private bool _isAddressToggleActive;

        private readonly Dictionary<string, List<string>> _imageryLayersVisible = new Dictionary<string, List<string>>();
        private bool _isImageryToggleActive;

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
            HotKeyManager.AddHotKey(new HotKey(Keys.F2, "Toggle Imagery Layers"), "Legend_Toggle_Imagery_Layers");

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
                    _isAddressToggleActive = !_isAddressToggleActive;
                    ToggleAddressLayers();
                    break;
                case "Legend_Toggle_Imagery_Layers":
                    _isImageryToggleActive = !_isImageryToggleActive;
                    ToggleImageryLayers();
                    break;
            }
        }

        private void ActivateAddressLayersVisibility(string key, Map map)
        {
            List<string> visibleList;
            _addressLayersVisible.TryGetValue(key, out visibleList);
            // get all layer address layers by name
            var layers = SDR.Configuration.Project.Go2ItProjectSettings.Instance.AddressLayers;
            foreach (IMapLayer ml in map.Layers)
            {
                if (ml.GetType().Name == "MapImageLayer") continue;
                var mFeatureLyr = ml as IMapFeatureLayer;
                // make sure this is a valid map feature layer
                if (mFeatureLyr == null || 
                    String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((mFeatureLyr.DataSet.Filename))))
                    continue;
                var lyrName = Path.GetFileNameWithoutExtension(mFeatureLyr.DataSet.Filename);
                // find the required layers and toggle that shit
                foreach (string layer in layers)
                {
                    if (lyrName != layer) continue;
                    if (visibleList != null && visibleList.Contains(lyrName))
                    {
                        ml.IsVisible = true;
                    }
                }
            }
        }

        private void ActivateImageryLayersVisibility(string key, Map map)
        {
            List<string> visibleList;
            _imageryLayersVisible.TryGetValue(key, out visibleList);
            foreach (IMapLayer ml in map.Layers)
            {
                if (ml.GetType().Name != "MapImageLayer") continue;
                var mImageLyr = ml as IMapImageLayer;
                if (mImageLyr == null ||
                    String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((mImageLyr.DataSet.Filename))))
                    continue;
                var lyrName = Path.GetFileNameWithoutExtension(mImageLyr.DataSet.Filename);
                if (visibleList != null && visibleList.Contains(lyrName))
                {
                    ml.IsVisible = true;
                }
            }
        }

        private static List<string> PopulateAddressVisibilityList(Map map)
        {
            // get all layer address layers by name
            var layers = SDR.Configuration.Project.Go2ItProjectSettings.Instance.AddressLayers;
            var visibleList = new List<string>();
            foreach (IMapLayer ml in map.Layers)
            {
                if (ml.GetType().Name == "MapImageLayer") continue;
                var mFeatureLyr = ml as IMapFeatureLayer;
                // make sure this is a valid map feature layer
                if (mFeatureLyr == null ||
                    String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((mFeatureLyr.DataSet.Filename))))
                    continue;
                // get the name of this layer for comparison
                var lyrName = Path.GetFileNameWithoutExtension(mFeatureLyr.DataSet.Filename);
                // find the required layers and toggle that shit
                foreach (string layer in layers)
                {
                    if (lyrName != layer) continue;
                    if (!mFeatureLyr.IsVisible) continue;
                    visibleList.Add(lyrName);
                    ml.IsVisible = false;
                }
            }
            return visibleList;
        }

        private static List<string> PopulateImageryVisibilityList(Map map)
        {
            var visibleList = new List<string>();
            foreach (IMapLayer ml in map.Layers)
            {
                if (ml.GetType().Name != "MapImageLayer") continue;
                var mImageLyr = ml as IMapImageLayer;
                if (mImageLyr == null ||
                    String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((mImageLyr.DataSet.Filename))))
                    continue;
                var lyrName = Path.GetFileNameWithoutExtension(mImageLyr.DataSet.Filename);
                if (!mImageLyr.IsVisible) continue;
                visibleList.Add(lyrName);
                ml.IsVisible = false;
            }
            return visibleList;
        }

        private void ToggleImageryLayers()
        {
            if (App.Map == null) return;

            var dockControl = (DockingControl)App.DockManager;
            foreach (KeyValuePair<string, DockPanelInfo> kvPair in dockControl.DockPanelLookup)
            {
                if (!kvPair.Key.StartsWith("kMap_")) continue;
                var map = (Map)kvPair.Value.DotSpatialDockPanel.InnerControl;
                if (_isImageryToggleActive)
                {
                    var vList = PopulateImageryVisibilityList(map);
                    _imageryLayersVisible.Add(kvPair.Key, vList);
                }
                else // reactivate all the layers stored in our dict that were previously visible
                {
                    ActivateImageryLayersVisibility(kvPair.Key, map);
                }
            }
            if (!_isImageryToggleActive)
            {
                _imageryLayersVisible.Clear();
            }
            if (_userLegend.Visible)
            {
                _userLegend.Invalidate();
            }
        }

        private void ToggleAddressLayers()
        {
            if (App.Map == null) return;

            var dockControl = (DockingControl) App.DockManager;
            foreach (KeyValuePair<string, DockPanelInfo> kvPair in dockControl.DockPanelLookup)
            {
                if (!kvPair.Key.StartsWith("kMap_")) continue;
                var map = (Map)kvPair.Value.DotSpatialDockPanel.InnerControl;
                if (_isAddressToggleActive)
                {
                    var vList = PopulateAddressVisibilityList(map);
                    _addressLayersVisible.Add(kvPair.Key, vList);
                }
                else // reactivate all the layers stored in our dict that were previously visible
                {
                    ActivateAddressLayersVisibility(kvPair.Key, map);
                }
            }
            if (!_isAddressToggleActive)
            {
                _addressLayersVisible.Clear();
            }
            if (_userLegend.Visible)
            {
                _userLegend.Invalidate();
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
