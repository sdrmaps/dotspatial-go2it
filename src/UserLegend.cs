//using System;
//using System.ComponentModel.Composition;
//using System.IO;
//using System.Windows.Forms;
//using DotSpatial.Controls;
//using DotSpatial.Controls.Docking;
//using DotSpatial.Controls.Header;
//using DotSpatial.Data;
//using DotSpatial.SDR.Controls;
//using DotSpatial.Symbology;
//using Go2It.Properties;

//namespace Go2It
//{
//    public class UserLegend : Extension
//    {
//        [Import("Shell")]
//        internal ContainerControl Shell { get; set; }

//        private Map _map;
//        private const string HomeMenuKey = DotSpatial.Controls.Header.HeaderControl.HomeRootItemKey;

//        private RestrictedLegend _userLegend;
//        private UserLegendForm _userLegendForm;

//        public override void Activate()
//        {
//            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "Legend", ToggleLegendTool_Click)
//            {
//                GroupCaption = "Legend_Toggle",
//                ToolTipText = "Toggle Legend Visibility",
//                SmallImage = Resources.legend_16,
//                LargeImage = Resources.legend_32
//            });

//            // hotkeys for toggling various layers
//            // HotKeyManager.AddHotKey(new HotKey(Keys.F3, "Toggle Address Layers"), "Legend_Toggle_Address_Layers");

//            App.DockManager.ActivePanelChanged += DockManagerOnActivePanelChanged;

//            // watch for hotkeys activated via the mainform plugin
//            // HotKeyManager.HotKeyEvent += HotKeyManagerOnHotKeyEvent;

//            base.Activate();
//        }

//        //private void HotKeyManagerOnHotKeyEvent(string action)
//        //{
//        //    switch (action)
//        //    {
//        //        case "Legend_Toggle_Address_Layers":
//        //            ToggleAddressLayers();
//        //            break;
//        //        default:
//        //            break;
//        //    }
//        //}

//        //private void ToggleAddressLayers()
//        //{
//        //    if (_map == null) return;
//        //    // get all layer names set as address layers
//        //    var layers = SDR.Configuration.Project.Go2ItProjectSettings.Instance.AddressLayers;
//        //    // locate and toggle required layers visibility
//        //    foreach (IMapLayer ml in App.Map.Layers)
//        //    {
//        //        if (ml.GetType().Name != "MapImageLayer")
//        //        {
//        //            var mFeatureLyr = ml as IMapFeatureLayer;
//        //            // make sure this is a valid map feature layer
//        //            if (mFeatureLyr != null && String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((mFeatureLyr.DataSet.Filename)))) return;
//        //            // get the name of this layer for comparison
//        //            var lyrName = Path.GetFileNameWithoutExtension(mFeatureLyr.DataSet.Filename);
//        //            // find the required layers and toggle that shit
//        //            foreach (string layer in layers)
//        //            {
//        //                if (lyrName == layer)
//        //                {
//        //                    if (mFeatureLyr.IsVisible)
//        //                    {
//        //                        mFeatureLyr.
//        //                    }
//        //                }
//        //            }
//        //        }
//        //    }
//        //}

//        private void DockManagerOnActivePanelChanged(object sender, DockablePanelEventArgs e)
//        {
//            DockPanelInfo dockInfo;
//            var dockControl = (DockingControl)sender;
//            var key = e.ActivePanelKey;
//            if (!dockControl.DockPanelLookup.TryGetValue(key, out dockInfo)) return;

//            if (!key.StartsWith("kMap_")) return;

//            if (_userLegend != null)
//            {
//                _userLegend.RootNodes.Clear();
//            }

//            // update the active _map for the legend now
//            _map = (Map)dockInfo.DotSpatialDockPanel.InnerControl;

//            if (_map == null) return;
//            _map.Legend = _userLegend;
//        }

//        public override void Deactivate()
//        {
//            App.HeaderControl.RemoveAll();
//            App.DockManager.ActivePanelChanged -= DockManagerOnActivePanelChanged;
//            _userLegendForm.FormClosing -= UserLegend_Closed;
//            base.Deactivate();
//        }

//        private void ToggleLegendTool_Click(object sender, EventArgs e)
//        {
//            if (_userLegendForm == null)
//                _userLegendForm = new UserLegendForm();

//            if (_userLegend == null)
//                AttachLegend();

//            if (_userLegendForm.Visible == false)
//            {
//                _userLegendForm.Show(Shell);
//                // _userLegendForm.Focus();
//            }
//            else
//            {
//                _userLegendForm.Close();
//            }
//        }

//        private void UserLegend_Closed(object sender, FormClosingEventArgs e)
//        {
//            _userLegendForm.Hide();
//            e.Cancel = true; // cancel the event from closing completion (disposing of legend)
//        }

//        private void AttachLegend()
//        {
//            _userLegend = new RestrictedLegend
//            {
//                BackColor = System.Drawing.Color.White,
//                ControlRectangle = new System.Drawing.Rectangle(0, 0, 176, 128),
//                DocumentRectangle = new System.Drawing.Rectangle(0, 0, 34, 114),
//                HorizontalScrollEnabled = true,
//                Indentation = 30,
//                IsInitialized = false,
//                Location = new System.Drawing.Point(217, 12),
//                MinimumSize = new System.Drawing.Size(5, 5),
//                Name = "User Legend",
//                ProgressHandler = null,
//                ResetOnResize = false,
//                SelectionFontColor = System.Drawing.Color.Black,
//                SelectionHighlight = System.Drawing.Color.FromArgb(215, 238, 252),
//                Size = new System.Drawing.Size(176, 128),
//                TabIndex = 0,
//                Text = @"Legend",
//                Dock = DockStyle.Fill,
//                VerticalScrollEnabled = true
//            };

//            _userLegendForm.Controls.Add(_userLegend);
//            _userLegendForm.FormClosing += UserLegend_Closed;
//            // assign the legend to the active _map now
//            if (_map != null)
//            {
//                _map.Legend = _userLegend;
//            }
//        }
//    }
//}
