using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls.Header;

namespace DotSpatial.SDR.Controls
{
    /// <summary>
    /// ProgressStatusPanel class allows user-defined progress bar panels to be added to the status bar
    /// </summary>
    public class ProgressBarStatusPanel : StatusPanel
    {
        #region Fields

        private int _percent;
        private bool _removeOnComplete;

        #endregion

        public ProgressBarStatusPanel() {}

        /// <summary>
        /// Gets or sets the percentage complete
        /// </summary>
        /// <value>
        /// Percentage complete of the ProgressBar
        /// </value>
        public int Percent
        {
            get
            {
                return _percent;
            }
            set
            {
                if (_percent == value) return;
                _percent = value;
                OnPropertyChanged("Percent");
            }
        }
    }
}
