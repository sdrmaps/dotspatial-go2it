using System;
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
        public ComboBox ComLogsComboBox
        {
            get { return cmbAliCommLog; }
        }
        public CheckedListBox VehicleFleetListBox
        {
            get { return chkFleetList;  }
        }
        #endregion

        #region Methods
        public delegate void SetAvlVehicleCheckStateCallback(int id, bool status);
        public void SetAvlVehicleCheckState(int id, bool status)
        {
            if (chkFleetList.InvokeRequired)
            {
                var cb = new SetAvlVehicleCheckStateCallback(SetAvlVehicleCheckState);
                Invoke(cb, new object[] { id, status });
            }
            else
            {
                chkFleetList.SetItemChecked(id, status);
            }
        }

        public delegate void SelectAvlVehicleRowCallback(int id);
        public void SelectAvlVehicleRow(int id)
        {
            if (chkFleetList.InvokeRequired)
            {
                var cb = new SelectAvlVehicleRowCallback(SelectAvlVehicleRow);
                Invoke(cb, new object[] { id });
            }
            else
            {
                chkFleetList.SelectedIndex = id;
            }
        }

        public delegate int GetSelectedAvlVehicleRowCallback();
        public int GetSelectedAvlVehicleRow()
        {
            if (chkFleetList.InvokeRequired)
            {
                return (int)chkFleetList.Invoke(new GetSelectedAvlVehicleRowCallback(GetSelectedAvlVehicleRow));
            }
            return chkFleetList.SelectedIndex;
        }

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

        public delegate void SetCheckedListBoxBindingSourceCallback(BindingSource source);
        public void SetCheckedListBoxBindingSource(BindingSource source)
        {
            if (chkFleetList.InvokeRequired)
            {
                var cb = new SetCheckedListBoxBindingSourceCallback(SetCheckedListBoxBindingSource);
                Invoke(cb, new object[] { source });
            }
            else
            {
                chkFleetList.DataSource = source;
                chkFleetList.DisplayMember = "UnitLabel";
                chkFleetList.ValueMember = "UnitId";
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
            tsbAliLocate.Enabled = true;
            tsbAliUpdate.Enabled = true;
            aliTableLayoutPanel.ColumnStyles[0].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[0].Width = 100;
            aliTableLayoutPanel.ColumnStyles[1].SizeType = SizeType.Absolute;
            aliTableLayoutPanel.ColumnStyles[1].Width = 0;
            aliTableLayoutPanel.ColumnStyles[2].SizeType = SizeType.Absolute;
            aliTableLayoutPanel.ColumnStyles[2].Width = 0;
        }

        public void DisplayAvlListInterface(string listTitle, bool enableFunctions)
        {
            tsbAliLocate.Enabled = enableFunctions;
            tsbAliUpdate.Enabled = enableFunctions;
            lblFleetList.Text = listTitle;
            aliTableLayoutPanel.ColumnStyles[0].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[0].Width = 75;
            aliTableLayoutPanel.ColumnStyles[1].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[1].Width = 25;
            aliTableLayoutPanel.ColumnStyles[2].SizeType = SizeType.Absolute;
            aliTableLayoutPanel.ColumnStyles[2].Width = 0;
        }

        public void DisplayAvlListInterfaceWithMyUnit(string listTitle, string unitTitle, bool enableFunctions)
        {
            tsbAliLocate.Enabled = enableFunctions;
            tsbAliUpdate.Enabled = enableFunctions;
            lblFleetList.Text = listTitle;
            lblCommLog.Text = unitTitle;  // because collapsing the top row would complicate things
            aliTableLayoutPanel.ColumnStyles[0].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[0].Width = 60;
            aliTableLayoutPanel.ColumnStyles[1].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[1].Width = 20;
            aliTableLayoutPanel.ColumnStyles[2].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[2].Width = 20;

            aliTableLayoutPanel.RowStyles[1].SizeType = SizeType.Absolute;
            aliTableLayoutPanel.RowStyles[1].Height = 0;
            aliTableLayoutPanel.RowStyles[2].SizeType = SizeType.Absolute;
            aliTableLayoutPanel.RowStyles[2].Height = 0;
        }

        public void DisplayAvlListWithMyUnitAndCommLogInterface(string listTitle, string unitTitle, string logTitle)
        {
            tsbAliLocate.Enabled = true;
            tsbAliUpdate.Enabled = true;
            lblFleetList.Text = listTitle;
            lblCommLog.Text = logTitle;
            lblMyUnit.Text = unitTitle;
            aliTableLayoutPanel.ColumnStyles[0].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[0].Width = 60;
            aliTableLayoutPanel.ColumnStyles[1].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[1].Width = 20;
            aliTableLayoutPanel.ColumnStyles[2].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[2].Width = 20;

            aliTableLayoutPanel.RowStyles[1].SizeType = SizeType.AutoSize;
            aliTableLayoutPanel.RowStyles[2].SizeType = SizeType.Absolute;
            aliTableLayoutPanel.RowStyles[2].Height = 20;
        }

        public void DisplayAvlListAndCommLogInterface(string listTitle, string logTitle)
        {
            tsbAliLocate.Enabled = true;
            tsbAliUpdate.Enabled = true;
            lblFleetList.Text = listTitle;
            lblCommLog.Text = logTitle;
            aliTableLayoutPanel.ColumnStyles[0].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[0].Width = 60;
            aliTableLayoutPanel.ColumnStyles[1].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[1].Width = 20;
            aliTableLayoutPanel.ColumnStyles[2].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[2].Width = 20;

            aliTableLayoutPanel.RowStyles[1].SizeType = SizeType.AutoSize;
            aliTableLayoutPanel.RowStyles[2].SizeType = SizeType.Absolute;
            aliTableLayoutPanel.RowStyles[2].Height = 0;
            aliTableLayoutPanel.RowStyles[3].SizeType = SizeType.Absolute;
            aliTableLayoutPanel.RowStyles[3].Height = 0;
        }

        public void DisplayCommLogInterface(string logTitle)
        {
            tsbAliLocate.Enabled = true;
            tsbAliUpdate.Enabled = true;
            lblCommLog.Text = logTitle;
            aliTableLayoutPanel.ColumnStyles[0].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[0].Width = 75;
            aliTableLayoutPanel.ColumnStyles[1].SizeType = SizeType.Absolute;
            aliTableLayoutPanel.ColumnStyles[1].Width = 0;
            aliTableLayoutPanel.ColumnStyles[2].SizeType = SizeType.Percent;
            aliTableLayoutPanel.ColumnStyles[2].Width = 25;

            aliTableLayoutPanel.RowStyles[1].SizeType = SizeType.AutoSize;
            aliTableLayoutPanel.RowStyles[2].SizeType = SizeType.Absolute;
            aliTableLayoutPanel.RowStyles[2].Height = 0;
            aliTableLayoutPanel.RowStyles[3].SizeType = SizeType.Absolute;
            aliTableLayoutPanel.RowStyles[3].Height = 0;
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

        private void tsbAliUpdate_Click(object sender, EventArgs e)
        {

        }

        private void tsbAliLocate_Click(object sender, EventArgs e)
        {

        }
    }
}
