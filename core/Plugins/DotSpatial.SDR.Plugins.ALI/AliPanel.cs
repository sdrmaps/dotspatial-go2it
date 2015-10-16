using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.SDR.Plugins.ALI.Properties;
using SdrConfig = SDR.Configuration;

namespace DotSpatial.SDR.Plugins.ALI
{
    public sealed partial class AliPanel : UserControl
    {
        #region Constructors
        public AliPanel()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the datagridview for record display
        /// </summary>
        public DataGridView DataGridDisplay
        {
            get { return aliDGV; }
        }

        #endregion

        #region Methods 

        public void ShowGlobalCadInterface()
        {
            aliDGV.Columns[1].Visible = true;  // display the combo dropdown box
        }

        public void ShowStandardInterface()
        {
            aliDGV.Columns[1].Visible = false; // show just the datagridview
        }
        #endregion

        #region Events

        #endregion

        #region FormEvents

        // TODO: handle all form events

        #endregion

        #region Event Handlers

        // TODO: add all event handlers

        #endregion
    }
}
