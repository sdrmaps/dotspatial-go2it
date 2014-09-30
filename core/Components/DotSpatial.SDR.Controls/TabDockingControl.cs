using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;

namespace DotSpatial.SDR.Controls
{
    /// <summary>
    /// Implementation of tab docking control
    /// </summary>
    public class TabDockingControl : IDockManager
    {
        private TabControl _mapTabs;
        private TablessControl _toolTabs;
        private readonly ImageList _tabImages = new ImageList();

        // lookup list of all available tool and map dockpanels
        public readonly Dictionary<string, DockPanelInfo> DockPanelLookup = new Dictionary<string, DockPanelInfo>();
        public readonly Dictionary<string, IMapLayer> BaseLayerLookup = new Dictionary<string, IMapLayer>();

        private SplitContainer _splitContainer;
        private int _splitterDistance;  // hold the size of the splitter for invalidation

        #region Constructor
        public void Initialize(SplitContainer sc)
        {
            _splitContainer = sc;
            _splitContainer.DoubleClick += Splitter_DoubleClick;
            _splitContainer.Paint += Splitter_Paint;
            _splitterDistance = _splitContainer.SplitterDistance;

            SplitterPanel toolPanel = sc.Panel1;
            SplitterPanel mapPanel = sc.Panel2;
            
            // create the tab and tabless panel controls
            _mapTabs = new TabControl {Name = "contentTabs", Dock = DockStyle.Fill};

            _mapTabs.Selected += MapTabsOnSelected;
            _mapTabs.Deselected += MapTabsOnDeselected;

            _toolTabs = new TablessControl {Name = "toolTabs", Dock = DockStyle.Fill};
            _toolTabs.Selected += ToolTabsOnSelected;
            _toolTabs.Deselected += ToolTabsOnDeselected;

            toolPanel.Controls.Add(_toolTabs);
            mapPanel.Controls.Add(_mapTabs);
        }
        #endregion

        #region Public Methods
        public void Add(DockablePanel panel)
        {
            if (panel == null) return;
            var key = panel.Key;
            if (DockPanelLookup.ContainsKey(key))
            {
                throw new ArgumentOutOfRangeException("panel", string.Format("Unable to add panel with Key: {0}, because it already added.", key));
            }

            var caption = panel.Caption;
            var innerControl = panel.InnerControl; // the map or tool panel
            var dockStyle = panel.Dock;
            var zorder = panel.DefaultSortOrder;
            // set the dock style of inner control to fill (a map or a tool)
            innerControl.Dock = DockStyle.Fill;

            Image img = null;
            if (panel.SmallImage != null) img = panel.SmallImage;

            // create tabpage to hold the control
            var tabPage = new TabPage { Padding = new Padding(0), Margin = new Padding(0) };
 
            // add the actual control panel to the tabpage
            tabPage.Controls.Add(innerControl);
            tabPage.Dock = dockStyle;
            tabPage.Text = caption;
            tabPage.Name = key;
            tabPage.Tag = key;
            innerControl.Tag = key;

            if (img != null)
            {
                _tabImages.Images.Add(key, ImageToIcon(img));
                tabPage.ImageKey = key;
                tabPage.ImageIndex = _tabImages.Images.IndexOfKey(key);
            }

            // add the tab panel to the dictionary for easy lookup as needed
            DockPanelLookup.Add(key, new DockPanelInfo(panel, tabPage, zorder));

            if (key.Trim().StartsWith("kMap_"))
            {
                // kMaps are map tabs
                _mapTabs.Controls.Add(tabPage);
                if (_mapTabs.TabCount > 1)
                {
                    int sortIndex = ConvertSortOrderToIndex(_mapTabs, zorder);
                    _mapTabs.TabIndex = sortIndex;
                }
            }
            else if ((panel.Key.Trim().StartsWith("kPanel_")))
            {
                // kPanels are tool panels
                _toolTabs.Controls.Add(tabPage);
                if (_toolTabs.TabCount > 1)
                {
                    int sortIndex = ConvertSortOrderToIndex(_toolTabs, zorder);
                    _toolTabs.TabIndex = sortIndex;
                }
            }

            // trigger the panel added event
            OnPanelAdded(key);
        }

        public void Remove(string key)
        {
            if (_mapTabs.Controls.ContainsKey(key))
            {
                _mapTabs.Controls.RemoveByKey(key);
            }
            if (_toolTabs.Controls.ContainsKey(key))
            {
                _toolTabs.Controls.RemoveByKey(key);
            }
            // finally lets remove the actual dockpanel now
            DockPanelInfo dockInfo;
            if (!DockPanelLookup.TryGetValue(key, out dockInfo)) return;
            DockPanelLookup.Remove(key);

            // trigger the panel removed event
            OnPanelRemoved(key);
        }

        public void ResetLayout()
        {
            // remove all the current map tab panels but leave the tool panels
            foreach (TabPage mapTab in _mapTabs.TabPages)
            {
                Remove(mapTab.Name);  // same as the tab key (could also use mapTab.Tag)
            }
            BaseLayerLookup.Clear();  // clear base layer lookup (wipe all layers)
        }

        private void MapTabsOnDeselected(object sender, TabControlEventArgs tabControlEventArgs)
        {
            if (tabControlEventArgs.TabPage != null)
            {
                HidePanel(tabControlEventArgs.TabPage.Name);
            }       
        }

        private void MapTabsOnSelected(object sender, TabControlEventArgs tabControlEventArgs)
        {
            if (tabControlEventArgs.TabPage != null)
            {
                SelectPanel(tabControlEventArgs.TabPage.Name);
            }
        }

        private void ToolTabsOnDeselected(object sender, TabControlEventArgs tabControlEventArgs)
        {
            if (tabControlEventArgs.TabPage != null)
            {
                HidePanel(tabControlEventArgs.TabPage.Name);
            }       
        }

        private void ToolTabsOnSelected(object sender, TabControlEventArgs tabControlEventArgs)
        {
            if (tabControlEventArgs.TabPage != null)
            {
                SelectPanel(tabControlEventArgs.TabPage.Name);
            }
        }

        public void SelectPanel(string key)
        {
            DockPanelInfo info;
            if (!DockPanelLookup.TryGetValue(key, out info)) return;

            if (key.StartsWith("kMap_"))
            {
                if (_mapTabs.SelectedTab == info.DockPanelTab)
                {
                    OnActivePanelChanged(key);
                }
                else
                {
                    _mapTabs.SelectTab(info.DockPanelTab);
                }
            }
            else if (key.StartsWith("kPanel_"))
            {
                if (_toolTabs.SelectedTab == info.DockPanelTab)
                {
                    OnActivePanelChanged(key);
                }
                else
                {
                    _toolTabs.SelectTab(info.DockPanelTab);
                }
            }
        }

        public void HidePanel(string key)
        {
            DockPanelInfo info;
            if (!DockPanelLookup.TryGetValue(key, out info)) return;

            OnPanelDeactivated(key);
        }

        public void ShowPanel(string key)
        {
            throw new NotImplementedException();
        }

        public IMapFeatureLayer GetMapFeatureLayerByName(string lyrName)
        {
            IMapLayer layer;
            BaseLayerLookup.TryGetValue(lyrName, out layer);
            return layer as IMapFeatureLayer;
        }

        #endregion

        public event EventHandler<DockablePanelEventArgs> ActivePanelChanged;
        public event EventHandler<DockablePanelEventArgs> PanelClosed;
        public event EventHandler<DockablePanelEventArgs> PanelAdded;
        public event EventHandler<DockablePanelEventArgs> PanelRemoved;
        public event EventHandler<DockablePanelEventArgs> PanelHidden;

        #region Private Methods
        private int ConvertSortOrderToIndex(TabControl tc, int sortOrder)
        {
            var sortOrderList = new List<int>();
            foreach (TabPage tabPage in tc.TabPages)
            {
                var key = tabPage.Tag.ToString();
                DockPanelInfo info;
                if (DockPanelLookup.TryGetValue(key, out info))
                {
                    sortOrderList.Add(info.SortOrder);
                }
            }
            sortOrderList.Sort();
            var index = sortOrderList.IndexOf(sortOrder);
            return index;
        }

        #region OnPanelDeactivated
        /// <summary>
        /// Triggers the OnPanelDeactivated event.
        /// </summary>
        public virtual void OnPanelDeactivated(string panelKey)
        {
            var handler = PanelHidden;
            if (handler != null)
            {
                handler(this, new DockablePanelEventArgs(panelKey));
            }
        }
        #endregion

        #region OnActivePanelChanged
        /// <summary>
        /// Triggers the ActivePanelChanged event.
        /// </summary>
        public virtual void OnActivePanelChanged(string panelKey)
        {
            var handler = ActivePanelChanged;
            if (handler != null)
            {
                handler(this, new DockablePanelEventArgs(panelKey));
            }
        }
        #endregion

        #region OnPanelClosed
        /// <summary>
        /// Triggers the PanelClosed event.
        /// </summary>
        public virtual void OnPanelClosed(string panelKey)
        {
            var handler = PanelClosed;
            if (handler != null)
            {
                handler(this, new DockablePanelEventArgs(panelKey));
            }
        }
        #endregion

        #region OnPanelAdded
        /// <summary>
        /// Triggers the PanelAdded event.
        /// </summary>
        public virtual void OnPanelAdded(string panelKey)
        {
            var handler = PanelAdded;
            if (handler != null)
            {
                handler(this, new DockablePanelEventArgs(panelKey));
            }
        }
        #endregion

        #region OnPanelRemoved
        /// <summary>
        /// Triggers the PanelRemoved event.
        /// </summary>
        public virtual void OnPanelRemoved(string panelKey)
        {
            var handler = PanelRemoved;
            if (handler != null)
            {
                handler(this, new DockablePanelEventArgs(panelKey));
            }
        }
        #endregion

        private static Icon ImageToIcon(Image img)
        {
            var bm = img as Bitmap;
            return bm != null ? Icon.FromHandle(bm.GetHicon()) : null;
        }

        private static void PaintSplitterDots(SplitContainer sc, PaintEventArgs e)
        {
            var control = sc;
            // paint the three dots'
            var points = new Point[3];
            var w = control.Width;
            var h = control.Height;
            var d = control.SplitterDistance;
            var sW = control.SplitterWidth;

            //calculate the position of the points'
            if (control.Orientation == Orientation.Horizontal)
            {
                points[0] = new Point((w / 2), d + (sW / 2));
                points[1] = new Point(points[0].X - 10, points[0].Y);
                points[2] = new Point(points[0].X + 10, points[0].Y);
            }
            else
            {
                points[0] = new Point(d + (sW / 2), (h / 2));
                points[1] = new Point(points[0].X, points[0].Y - 10);
                points[2] = new Point(points[0].X, points[0].Y + 10);
            }

            foreach (Point p in points)
            {
                p.Offset(-2, -2);
                e.Graphics.FillEllipse(SystemBrushes.ControlDark,
                    new Rectangle(p, new Size(3, 3)));

                p.Offset(1, 1);
                e.Graphics.FillEllipse(SystemBrushes.ControlLight,
                    new Rectangle(p, new Size(3, 3)));
            }
        }
        #endregion

        #region Event Handlers
        private static void Splitter_Paint(object sender, PaintEventArgs e)
        {
            PaintSplitterDots((SplitContainer)sender, e);
        }

        private void Splitter_DoubleClick(object sender, EventArgs e)
        {
            var sc = (SplitContainer)sender;
            var dist = sc.SplitterDistance;
            if (dist != 0)
            {
                _splitterDistance = dist;
                sc.SplitterDistance = 0;
            }
            else
            {
                sc.SplitterDistance = _splitterDistance;
            }
        }
        #endregion
    }
}
