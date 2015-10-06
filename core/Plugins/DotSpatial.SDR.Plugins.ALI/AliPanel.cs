using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.SDR.Plugins.ALI.Properties;

namespace DotSpatial.SDR.Plugins.ALI
{
    public sealed partial class AliPanel : UserControl
    {
        #region Constructors
        public AliPanel()
        {
            InitializeComponent();
            // TODO: setup the basic panel defaults
        }
        #endregion

        #region Properties
        private static AliMode AliMode
        {
            get
            {
                var funcMode = UserSettings.Default.AliMode;
                if (funcMode.Length <= 0) return AliMode.AliInterface;
                AliMode am;
                Enum.TryParse(funcMode, true, out am);
                return am;
            }
            set
            {
                UserSettings.Default.AliMode = value.ToString();
                UserSettings.Default.Save();
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a new location has been received
        /// </summary>
        public event EventHandler LocationReceived;
        #endregion

        #region FormEvents
        // TODO: handle all form events
        #endregion

        #region Event Handlers
        // TODO: add all event handlers
        #endregion
    }
}
