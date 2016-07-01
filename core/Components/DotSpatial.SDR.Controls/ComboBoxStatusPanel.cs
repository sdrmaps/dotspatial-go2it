using DotSpatial.Controls.Header;

namespace DotSpatial.SDR.Controls
{
    /// <summary>
    /// ComboBoxStatusPanel class allows user-defined combobox panels to be added to the status bar
    /// </summary>
    public class ComboBoxStatusPanel : StatusPanel
    {
        #region Fields

        private object _selectedItem;
        private object[] _itemArray;

        #endregion

        public ComboBoxStatusPanel() {}

        /// <summary>
        /// Gets or sets the selected item of the ComboBox
        /// </summary>
        /// <value>
        /// The Selected Item of the ComboBox
        /// </value>
        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem == value) return;
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        /// <summary>
        /// Gets or sets the items of the ComboBox
        /// </summary>
        /// <value>
        /// The Selected Item of the ComboBox
        /// </value>
        public object[] Items
        {
            get
            {
                return _itemArray;
            }
            set
            {
                if (_itemArray == value) return;
                _itemArray = value;
                OnPropertyChanged("Items");
            }
        }
    }
}
