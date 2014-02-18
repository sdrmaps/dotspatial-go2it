using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Controls.Header;
using SdrConfig = SDR.Configuration;

namespace DotSpatial.SDR.Controls
{
    /// <summary>
    /// Implementation of grid toolbar header control.
    /// </summary>
    public class GridHeaderControl : HeaderControl
    {
        private const string StrDefaultGroupName = "Default Menu Group";

        private TableLayoutPanel _tablePanel;
        private MenuStrip _menuStrip;
        private List<ToolStrip> _toolStrips;
        private Dictionary<ToolStrip, int> _gridStrips;

        private SplitContainer _splitContainer;
        private int _splitterDistance;  // hold the size of the splitter for invalidation
        private bool _splitterInvalidate;  // only invalidate onmoved if a user moved the splitter

        #region Constructor

        /// <summary>
        /// Initializes the specified container.
        /// </summary>
        /// <param name="panel">TableLayoutPanel for GridControl</param>
        public void Initialize(TableLayoutPanel panel)
        {
            _tablePanel = panel;
            _splitContainer = (SplitContainer)panel.Parent.Controls.Owner.Parent;
            _splitContainer.SplitterMoved += Splitter_Moved;
            _splitContainer.SplitterMoving += Splitter_Moving;
            _splitContainer.DoubleClick += Splitter_DoubleClick;
            _splitContainer.Paint += Splitter_Paint;
            _splitterDistance = _splitContainer.SplitterDistance;

            // create the menu strip
            _menuStrip = new MenuStrip {Name = StrDefaultGroupName, Dock = DockStyle.Top};
            // add the menu to the form so that it appears on top of all the toolbars.
            _splitContainer.Parent.Controls.Add(_menuStrip);
            // dict used for sorting all the grid panel strips
            _gridStrips = new Dictionary<ToolStrip, int>();
            // list for sorting all root level tool strips
            _toolStrips = new List<ToolStrip> {_menuStrip};

            // menu strip events
            _menuStrip.ItemClicked += MenuStrip_ItemClicked;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void Add(MenuContainerItem item)
        {
            var menu = new ToolStripMenuItem(item.Caption) {Name = item.Key};

            var root = _menuStrip.Items[item.RootKey] as ToolStripDropDownButton;
            if (root != null)
            {
                root.DropDownItems.Add(menu);
                root.Visible = true;
            }
        }

        /// <summary>
        /// Adds the separator.
        /// </summary>
        /// <param name="item">The item.</param>
        public override void Add(SeparatorItem item)
        {
            var separator = new ToolStripSeparator();
            ToolStrip strip = GetOrCreateStrip(item.GroupCaption);

            if (strip != null)
            {
                strip.Items.Add(separator);
            }
        }

        /// <summary>
        /// Adds the specified root item.
        /// </summary>
        /// <param name="item">
        /// The root item.
        /// </param>
        /// <remarks>
        /// </remarks>
        public override void Add(RootItem item)
        {
            // The root may have already been created.
            var root = _menuStrip.Items[item.Key] as ToolStripDropDownButton;
            if (root == null)
            {
                // if not we need to create it.
                CreateToolStripDropDownButton(item);
            }
            else
            {
                // We have already created the RootItem in anticipation of it being needed. (As it was specified by some HeaderItem already)
                // Update the caption and sort order of the root.
                root.Text = item.Caption;
                root.MergeIndex = item.SortOrder;
            }
            RefreshRootItemOrder();
        }

        /// <summary>
        /// This will add a new item that will appear on the standard toolbar or ribbon control.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <remarks>
        /// </remarks>
        public void Add(PermissionedActionItem item)
        {
            Add((SimpleActionItem)item);
        }

        /// <summary>
        /// This will add a new item that will appear on the standard toolbar or ribbon control.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <remarks>
        /// </remarks>
        public override void Add(SimpleActionItem item)
        {
            ToolStripItem menu;

            if (IsForMenuStrip(item) || item.GroupCaption == ApplicationMenuKey)
            {
                // add it to the menu bar
                menu = new ToolStripMenuItem(item.Caption) {Image = item.SmallImage};
            }
            else
            {
                // create a tool strip and add it as a button
                menu = new ToolStripButton(item.Caption)
                {
                    DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                    TextImageRelation = TextImageRelation.ImageAboveText,
                    ImageAlign = ContentAlignment.MiddleCenter,
                    Image = item.LargeImage,
                    ImageScaling = ToolStripItemImageScaling.None,
                    Text = item.Caption,
                    TextAlign = ContentAlignment.BottomCenter,
                    AutoSize = false,
                    Height = 52,
                    Width = 60,
                    BackColor = Color.Transparent
                };
                // menu.BackgroundImage = item.LargeImage;
                // menu.BackgroundImageLayout = ImageLayout.Center;
            }

            menu.Name = item.Key;
            menu.Enabled = item.Enabled;
            menu.Visible = item.Visible;
            menu.Click += (sender, e) => item.OnClick(e);

            EnsureNonNullRoot(item);
            var root = _menuStrip.Items[item.RootKey] as ToolStripDropDownButton ??
                       CreateToolStripDropDownButton(new RootItem(item.RootKey, "AddRootItemWithKey " + item.RootKey));

            if (item.MenuContainerKey == null)
            {
                if (IsForMenuStrip(item) || item.GroupCaption == ApplicationMenuKey || item.GroupCaption == null)
                {
                    root.DropDownItems.Add(menu);
                    root.Visible = true;
                }
                else
                {
                    // find or assemble the strip that the item is attached to
                    int position = item.SortOrder;
                    ToolStrip strip = GetOrCreateStrip(item.GroupCaption);
                    if (strip != null)
                    {
                        strip.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
                        strip.GripStyle = ToolStripGripStyle.Hidden;
                        strip.Items.Add(menu);
                        strip.Dock = DockStyle.None;
                        strip.Items.Add(menu);
                    }
                    menu.ToolTipText = String.IsNullOrWhiteSpace(item.ToolTipText) == false ? item.ToolTipText : item.Caption;
                    if (_gridStrips.ContainsValue(position) || position == 0)
                    {
                        position = GetNewGridOrderPosition();
                    }
                    // add the strip to our sort tracking dict
                    if (strip != null) _gridStrips.Add(strip, position);
                    RefreshGridControlOrder();
                }
            }
            else
            {
                var subMenu = root.DropDownItems[item.MenuContainerKey] as ToolStripMenuItem;
                if (subMenu != null)
                {
                    subMenu.DropDownItems.Add(menu);
                }
            }
            item.PropertyChanged += SimpleActionItem_PropertyChanged;
        }

        /// <summary>
        /// Adds a combo box style item
        /// </summary>
        /// <param name="item">The item.</param>
        public override void Add(DropDownActionItem item)
        {
            ToolStrip strip = GetOrCreateStrip(item.GroupCaption);
            var combo = new ToolStripComboBox(item.Key);

            ParseAllowEditingProperty(item, combo);

            combo.ToolTipText = item.Caption;
            if (item.Width != 0)
            {
                combo.Width = item.Width;
            }

            combo.Items.AddRange(item.Items.ToArray());
            combo.SelectedIndexChanged += delegate
            {
                item.PropertyChanged -= DropDownActionItem_PropertyChanged;
                item.SelectedItem = combo.SelectedItem;
                item.PropertyChanged += DropDownActionItem_PropertyChanged;
            };

            if (strip != null)
            {
                strip.Items.Add(combo);
            }

            item.PropertyChanged += DropDownActionItem_PropertyChanged;
        }

        /// <summary>
        /// Adds the specified textbox item.
        /// </summary>
        /// <param name="item">The item.</param>
        public override void Add(TextEntryActionItem item)
        {
            ToolStrip strip = GetOrCreateStrip(item.GroupCaption);
            var textBox = new ToolStripTextBox(item.Key);
            if (item.Width != 0)
            {
                textBox.Width = item.Width;
            }

            textBox.TextChanged += delegate
            {
                item.PropertyChanged -= TextEntryActionItem_PropertyChanged;
                item.Text = textBox.Text;
                item.PropertyChanged += TextEntryActionItem_PropertyChanged;
            };
            if (strip != null)
            {
                strip.Items.Add(textBox);
            }

            item.PropertyChanged += TextEntryActionItem_PropertyChanged;
        }

        /// <summary>
        /// Remove item from the standard toolbar or ribbon control
        /// </summary>
        /// <param name="key">
        /// The string itemName to remove from the standard toolbar or ribbon control
        /// </param>
        /// <remarks>
        /// </remarks>
        public override void Remove(string key)
        {
            var item = GetItem(key);
            if (item != null)
            {
                ToolStrip toolStrip = item.Owner;
                _gridStrips.Remove(toolStrip);
                item.Dispose();
                if (toolStrip.Items.Count == 0)
                {
                    _toolStrips.Remove(toolStrip);
                    toolStrip.Dispose();
                }
            }
            base.Remove(key);
            RefreshGridControlOrder();
        }

        /// <summary>
        /// Selects the root. (Does nothing.)
        /// </summary>
        /// <param name="key">The key.</param>
        public override void SelectRoot(string key)
        {
            // we won't do anything here.
        }

        #endregion

        #region Private Methods

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

        private void RefreshGridControlOrder()
        {
            foreach (KeyValuePair<ToolStrip, int> kvPair in _gridStrips)
            {
                //  now find the proper place on the panel to place the strip/control
                int row = 1;
                if (kvPair.Value > _tablePanel.ColumnCount)
                {
                    // find the proper row to place this control
                    var drow = kvPair.Value / _tablePanel.ColumnCount;
                    row = (int)Math.Ceiling((double)drow);
                }
                var posSub = (row - 1) * _tablePanel.ColumnCount;
                var column = kvPair.Value - posSub;

                _tablePanel.Controls.Add(kvPair.Key, column - 1, row - 1);
            }
            _splitContainer.SplitterDistance = _tablePanel.Width;
        }

        private int GetNewGridOrderPosition()
        {
            // determine the last slot and return it for new control
            int sortOrder = 0;
            var list = _gridStrips.Values.ToList();
            if (list.Count > 0)
            {
                list.Sort();
                sortOrder = list[list.Count - 1];
                sortOrder++;
            }
            return sortOrder;
        }

        /// <summary>
        /// Determines whether [is for tool strip]  being that it has an icon. Otherwise it should go on a menu.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is for tool strip] [the specified item]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsForMenuStrip(SimpleActionItem item)
        {
            return item.SmallImage == null;
        }

        private static void ParseAllowEditingProperty(DropDownActionItem item, ToolStripComboBox combo)
        {
            combo.DropDownStyle = item.AllowEditingText ? ComboBoxStyle.DropDown : ComboBoxStyle.DropDownList;
        }

        private void RefreshRootItemOrder()
        {
            // Get a list of all the menus
            var pages = new List<ToolStripItem>(_menuStrip.Items.Count);
            pages.AddRange(_menuStrip.Items.Cast<ToolStripItem>());

            // order the list by SortOrder
            var sortedPages = (from entry in pages orderby entry.MergeIndex ascending select entry);

            // Re add all of the items in the new order.
            _menuStrip.Items.Clear();
            foreach (var sortedPage in sortedPages)
            {
                _menuStrip.Items.Add(sortedPage);
            }
        }

        private void MenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            OnRootItemSelected(e.ClickedItem.Name);
        }

        private void ActionItem_PropertyChanged(ActionItem item, PropertyChangedEventArgs e)
        {
            var guiItem = GetItem(item.Key);

            switch (e.PropertyName)
            {
                case "Caption":
                    guiItem.Text = item.Caption;
                    break;

                case "Enabled":
                    guiItem.Enabled = item.Enabled;
                    break;

                case "Visible":
                    guiItem.Visible = item.Visible;
                    break;

                case "ToolTipText":
                    guiItem.ToolTipText = item.ToolTipText;
                    break;

                case "GroupCaption":
                    // todo: change group
                    break;

                case "RootKey":
                    // todo: change root
                    // note, this case will also be selected in the case that we set the Root key in our code.
                    break;

                default:
                    throw new NotSupportedException(" This Header Control implementation doesn't have an implemenation for or has banned modifying that property.");
            }
        }

        private ToolStrip AddToolStrip(string groupName)
        {
            var strip = new ToolStrip {Name = groupName};

            _toolStrips.Add(strip);
            _toolStrips.Remove(_menuStrip);
            ToolStrip[] strips = _toolStrips.ToArray();
            _toolStrips.Add(_menuStrip);
            _toolStrips.AddRange(strips);

            return strip;
        }

        private ToolStripDropDownButton CreateToolStripDropDownButton(RootItem item)
        {
            var menu = new ToolStripDropDownButton(item.Caption)
            {
                Name = item.Key,
                ShowDropDownArrow = false,
                Visible = false,
                MergeIndex = item.SortOrder
            };
            _menuStrip.Items.Add(menu);
            return menu;
        }

        private void DropDownActionItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as DropDownActionItem;
            if (item == null) return;
            var guiItem = GetItem(item.Key) as ToolStripComboBox;
            if (guiItem == null) return;

            switch (e.PropertyName)
            {
                case "AllowEditingText":
                    ParseAllowEditingProperty(item, guiItem);
                    break;

                case "Width":
                    guiItem.Width = item.Width;
                    break;

                case "SelectedItem":
                    guiItem.SelectedItem = item.SelectedItem;
                    break;

                case "ToggleGroupKey":
                    break;

                default:
                    ActionItem_PropertyChanged(item, e);
                    break;
            }
        }

        /// <summary>
        /// Ensure the extensions tab exists.
        /// </summary>
        private void EnsureExtensionsTabExists()
        {
            bool exists = _menuStrip.Items.ContainsKey(ExtensionsRootKey);
            if (!exists)
            {
                Add(new RootItem(ExtensionsRootKey, "Extensions"));
            }
        }

        /// <summary>
        /// Make sure the root key is present or use a default.
        /// </summary>
        /// <param name="item">
        /// </param>
        private void EnsureNonNullRoot(ActionItem item)
        {
            if (item.RootKey == null)
            {
                EnsureExtensionsTabExists();
                item.RootKey = ExtensionsRootKey;
            }
        }

        private ToolStripItem GetItem(string key)
        {
            return _toolStrips.Select(strip => strip.Items.Find(key, true).FirstOrDefault()).FirstOrDefault(item => item != null);
        }

        private ToolStrip GetOrCreateStrip(string groupCaption)
        {
            var query = from s in _toolStrips
                        where s.Name == (groupCaption ?? StrDefaultGroupName)
                        select s;

            ToolStrip strip = query.FirstOrDefault() ?? AddToolStrip(groupCaption);

            return strip;
        }

        private void SimpleActionItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as SimpleActionItem;
            if (item == null) return;
            var guiItem = GetItem(item.Key);

            switch (e.PropertyName)
            {
                case "SmallImage":
                    guiItem.Image = item.SmallImage;
                    break;
                case "LargeImage":
                    guiItem.Image = item.LargeImage;
                    break;
                case "MenuContainerKey":
                    break;
                case "ToggleGroupKey":
                    break;
                default:
                    ActionItem_PropertyChanged(item, e);
                    break;
            }
        }

        private void TextEntryActionItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as TextEntryActionItem;
            if (item == null) return;
            var guiItem = GetItem(item.Key) as ToolStripTextBox;
            if (guiItem == null) return;

            switch (e.PropertyName)
            {
                case "Width":
                    guiItem.Width = item.Width;
                    break;
                case "Text":
                    guiItem.Text = item.Text;
                    break;
                default:
                    ActionItem_PropertyChanged(item, e);
                    break;
            }
        }

        /* /// <summary>
        /// Unchecks all toolstrip buttons except the current button
        /// </summary>
        /// <param name="checkedButton">
        /// The toolstrip button which should
        /// stay checked
        /// </param>
        private void UncheckButtonsExcept(ToolStripButton checkedButton)
        {
            foreach (ToolStrip strip in _toolStrips)
            {
                foreach (ToolStripItem item in strip.Items)
                {
                    var buttonItem = item as ToolStripButton;
                    if (buttonItem != null)
                    {
                        if (buttonItem.Name != checkedButton.Name && buttonItem.Checked)
                        {
                            buttonItem.Checked = false;
                        }
                    }
                }
            }
        } */

        #endregion

        #region Event Handlers

        /* private void button_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripButton button = sender as ToolStripButton;
            if (button.Checked)
            {
                this.UncheckButtonsExcept(button);
            }
        } */

        private static void Splitter_Paint(object sender, PaintEventArgs e)
        {
            PaintSplitterDots((SplitContainer)sender, e);
        }

        private void Splitter_Moved(object sender, EventArgs e)
        {
            if (!_splitterInvalidate) return;
            var sc = (SplitContainer)sender;
            var btn = sc.Panel1.Controls[0].Controls[0];  // grab a single control button
            int btnWidth = btn.Width;
            int splitDist = sc.SplitterDistance;
            int dcolCount = splitDist / btnWidth;
            var colCount = (int)Math.Round((double)dcolCount);

            if (splitDist % btnWidth != 0)
            {
                int dist = colCount * btnWidth;
                sc.SplitterDistance = dist;
            }
            else
            {
                _splitterDistance = sc.SplitterDistance;
                _tablePanel.ColumnCount = colCount;
                _splitterInvalidate = false;
                RefreshGridControlOrder();
            }
            SdrConfig.User.Go2ItUserSettings.Instance.GridHeaderColumnCount = colCount;
        }

        private void Splitter_Moving(object sender, EventArgs e)
        {
            // simple flag to only do the splitter invalidation after
            // a user has moved the splitter, ignores onload events etc.
            _splitterInvalidate = true;
        }

        private void Splitter_DoubleClick(object sender, EventArgs e)
        {
            var sc = (SplitContainer)sender;
            int dist = sc.SplitterDistance;

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
