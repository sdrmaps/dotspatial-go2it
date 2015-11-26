﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SDR.Common;
using SDR.Common.UserMessage;
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
        public ComboBox GlobalCadComLogs  // combobox used to select active globalcad comm log // used for index change events on mapfunction
        {
            get { return cmbAliCommLog; }
        }
        #endregion

        #region Methods 
        delegate void SetDgvDataSourceCallback(GlobalCadRecord[]source);
        public void SetDgvDataSource(GlobalCadRecord[] source)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (aliDGV.InvokeRequired)
            {
                var cb = new SetDgvDataSourceCallback(SetDgvDataSource);
                Invoke(cb, new object[] { source });
            }
            else
            {
                aliDGV.DataSource = source;
            }
        }

        public delegate void SetDgvBindingSourceCallback(BindingSource source);
        public void SetDgvBindingSource(BindingSource source)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (aliDGV.InvokeRequired)
            {
                var cb = new SetDgvBindingSourceCallback(SetDgvBindingSource);
                Invoke(cb, new object[] { source });
            }
            else
            {
                aliDGV.DataSource = source;
            }
        }

        delegate void FillComboBoxCallback(string[] array);
        private void FillComboBox(string[] array)
        {
            if (cmbAliCommLog.InvokeRequired)
            {
                var cb = new FillComboBoxCallback(FillComboBox);
                Invoke(cb, new object[] { array });
            }
            else
            {
                cmbAliCommLog.Items.AddRange(array);
            }
        }

        delegate void ClearComboBoxCallback();
        private void ClearComboBox()
        {
            if (cmbAliCommLog.InvokeRequired)
            {
                var cb = new ClearComboBoxCallback(ClearComboBox);
                Invoke(cb, new object[] { });
            }
            else
            {
                cmbAliCommLog.Items.Clear();
            }
        }

        public void DisplayStandardInterface()
        {
            aliTableLayoutPanel.ColumnStyles[0].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[0].Width = 100;
            aliTableLayoutPanel.ColumnStyles[1].SizeType = SizeType.Absolute;
            aliTableLayoutPanel.ColumnStyles[1].Width = 0;
            aliTableLayoutPanel.ColumnStyles[2].SizeType = SizeType.Absolute;
            aliTableLayoutPanel.ColumnStyles[2].Width = 0;
        }

        public void DisplayNetworkfleetInterface()
        {
            aliTableLayoutPanel.ColumnStyles[0].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[0].Width = 75;
            aliTableLayoutPanel.ColumnStyles[1].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[1].Width = 25;
            aliTableLayoutPanel.ColumnStyles[2].SizeType = SizeType.Absolute;
            aliTableLayoutPanel.ColumnStyles[2].Width = 0;
        }

        public void DisplayNetworkfleetAndGlobalInterface()
        {
            aliTableLayoutPanel.ColumnStyles[0].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[0].Width = 60;
            aliTableLayoutPanel.ColumnStyles[1].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[1].Width = 20;
            aliTableLayoutPanel.ColumnStyles[2].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[2].Width = 20;
        }

        public void DisplayGlobalInterface()
        {
            aliTableLayoutPanel.ColumnStyles[0].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[0].Width = 75;
            aliTableLayoutPanel.ColumnStyles[1].SizeType = SizeType.Absolute;
            aliTableLayoutPanel.ColumnStyles[1].Width = 0;
            aliTableLayoutPanel.ColumnStyles[2].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[2].Width = 25;
        }

        public void PopulateComboBox()
        {
            ClearComboBox();
            var files = new List<string>();
            var dir = new DirectoryInfo(SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadArchivePath);
            try
            {
                files.AddRange(dir.GetFiles("*.log").Select(fi => Path.GetFileNameWithoutExtension(fi.Name)));
                // add todays log to the list
                var curLog = Path.GetFileNameWithoutExtension(
                    SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadLogPath) +
                    DateTime.Now.ToShortDateString().Replace("/", "");
                files.Add(curLog);
                files.Sort();
                files.Reverse();
                FillComboBox(files.ToArray());
            }
            catch (Exception ex)
            {
                var msg = AppContext.Instance.Get<IUserMessage>();
                msg.Warn("Failed to populate ComboBox with Log Files", ex);
            }
        }
        #endregion
    }
}
