using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DotSpatial.Controls.Header;
using Microsoft.SqlServer.Server;

namespace DotSpatial.SDR.Controls
{
    /// <summary>
    /// DropDownPanel class allows user-defined dropdown panels to be added to the status bar
    /// </summary>
    public class DropDownStatusPanel : StatusPanel
    {
        #region Fields

        private string _selectedItem;
        private string[] itemArray;

        #endregion

        public DropDownStatusPanel()
        {
        }

        /// <summary>
        /// Gets or sets the selected item of the DropDownPanel
        /// </summary>
        /// <value>
        /// The Selected Item of the DropDownPanel
        /// </value>
        public string SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (_selectedItem == value) return;
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }
    }
}
