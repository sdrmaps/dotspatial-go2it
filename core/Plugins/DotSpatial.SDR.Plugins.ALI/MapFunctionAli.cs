using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.SDR.Plugins.ALI.Properties;
using SDR.Data.Database;
using SDR.Network;
using SdrConfig = SDR.Configuration;
using System.IO;

namespace DotSpatial.SDR.Plugins.ALI
{
    /// <summary>
    /// A MapFunction that connects with ALI Interfaces
    /// </summary>
    public class MapFunctionAli : MapFunction
    {
        private AliPanel _aliPanel;
        private AliMode _currentAliMode = AliMode.Disabled;
        private NetworkFleet _networkFleet = NetworkFleet.Null;  // set to null on startup only

        // specific interface variables
        private string _aliServerDbConnString = string.Empty;
        private AliServerClient _aliServerClient;  // handles SDR AliServer Interface

        #region Constructors

        /// <summary>
        /// Creates a new instance of MapFunction, with panel
        /// </summary>
        /// <param name="ap"></param>
        public MapFunctionAli(AliPanel ap)
        {
            Name = "MapFunctionAli";
            YieldStyle = YieldStyles.AlwaysOn;
            _aliPanel = ap;

            HandleAliPanelEvents();
        }
        #endregion

        #region Methods

        private void HandleAliPanelEvents()
        {
            _aliPanel.DataGridDisplay.CellClick += DataGridDisplayOnCellClick;
        }

        private void DataGridDisplayOnCellClick(object sender, DataGridViewCellEventArgs dataGridViewCellEventArgs)
        {
            Debug.WriteLine("datagridrow clicked");
        }

        private void DisableCurrentAliMode()
        {
            _aliPanel.ResetInterface();
            if (_currentAliMode == AliMode.Sdraliserver)
            {
                if (_aliServerClient != null)
                {
                    _aliServerClient.PacketReceieved -= AliServerClientOnPacketReceieved;
                    SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerDbPathChanged -= InstanceOnAliSdrServerDbPathChanged;
                    SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpHostChanged -= InstanceOnAliSdrServerUdpHostChanged;
                    SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpPortChanged -= InstanceOnAliSdrServerUdpPortChanged;
                    _aliServerClient.Close();  // handle logout and close if needed
                    _aliServerClient = null;
                }
            }
            if (_currentAliMode == AliMode.Globalcad)
            {
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadArchivePathChanged -= InstanceOnAliGlobalCadArchivePathChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadLogPathChanged -= InstanceOnAliGlobalCadLogPathChanged;
            }
            // TODO: handle all other clients
        }

        private void HandleAliServerClient()
        {
            _aliServerClient = new AliServerClient(SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpHost, SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpPort);
            _aliServerClient.Login();
            _aliServerClient.PacketReceieved += AliServerClientOnPacketReceieved;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerDbPathChanged += InstanceOnAliSdrServerDbPathChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpHostChanged += InstanceOnAliSdrServerUdpHostChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpPortChanged += InstanceOnAliSdrServerUdpPortChanged;
            _aliServerDbConnString = MdbHelper.GetMdbConnectionString(SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerDbPath);
            PopulateDgv();
        }

        private void HandleNetworkFleetClient()
        {
            // TODO: handle the network fleet client here

        }

        private void HumanizeCamelCasedDgvHeaders()
        {
            for (var j = 0; j <= _aliPanel.DataGridDisplay.ColumnCount - 1; j++)
            {
                var n = _aliPanel.DataGridDisplay.Columns[j].HeaderText;
                _aliPanel.DataGridDisplay.Columns[j].HeaderText = Regex.Replace(n, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
            }
        }

        private void PopulateDgv()
        {
            // lets update the dgv column order if needed
            if (_aliPanel.DataGridDisplay.ColumnCount == PluginSettings.Instance.DgvSortOrder.Count)
            {
                var dgvArr = new string[_aliPanel.DataGridDisplay.ColumnCount];
                for (var j = 0; j <= _aliPanel.DataGridDisplay.ColumnCount - 1; j++)
                {
                    var n = _aliPanel.DataGridDisplay.Columns[j].DataPropertyName;
                    var i = _aliPanel.DataGridDisplay.Columns[j].DisplayIndex;
                    dgvArr[i] = n;
                }
                var dgvOrder = new StringCollection();
                dgvOrder.AddRange(dgvArr);
                PluginSettings.Instance.DgvSortOrder = dgvOrder;
            }
            var sql = ConstructSqlQuery();
            BindDataGridView(_aliServerDbConnString, sql);
            HumanizeCamelCasedDgvHeaders();
        }

        private static string ConstructSqlQuery()
        {
            const string date = "Trim(Trim([mMonth]) + '/' + Trim([mDay]) + '/' + Trim([mYear])) As CallDate";
            const string time = "Trim(Trim([mHour]) + ':' + Trim([mMinute]) + ' ' + Trim([mSecond])) As CallTime";
            const string phone = "Trim(Trim([mAreaCode]) + '-' + Trim([mPhonePrefix]) + '-' + Trim([mPhoneSuffix])) As Phone";
            const string address = "Trim(Trim([mHouseNumber]) + ' ' + Trim([mHouseNumberSuffix])) As Address";
            const string street = "Trim(Trim([mStreetPredirection]) + ' ' + Trim([mStreetName1]) + ' ' + Trim([mStreetSuffix])) As Street";
            const string srvClass = "Trim([mServiceClass]) As  ServiceClass";
            const string customer = "Trim([mCustomerName]) As Customer";
            const string sector = "Trim([mStreetName2]) As Sector";
            const string state = "Trim([mState]) As State";
            const string city = "Trim([mCity]) As City";
            const string lat = "Trim([mLatitude]) As Y";
            const string lng = "Trim([mLongitude]) As X";
            const string unc = "Trim([mUncertainty]) As Uncertainty";

            string sql = "SELECT ";
            for (int i = 0; i <= PluginSettings.Instance.DgvSortOrder.Count - 1; i++)
            {
                var col = PluginSettings.Instance.DgvSortOrder[i];
                switch (col)
                {
                    case "Address":
                        sql = sql + address + ",";
                        break;
                    case "CallDate":
                        sql = sql + date + ",";
                        break;
                    case "CallTime":
                        sql = sql + time + ",";
                        break;
                    case "Phone":
                        sql = sql + phone + ",";
                        break;
                    case "Street":
                        sql = sql + street + ",";
                        break;
                    case "ServiceClass":
                        sql = sql + srvClass + ",";
                        break;
                    case "Customer":
                        sql = sql + customer + ",";
                        break;
                    case "Sector":
                        sql = sql + sector + ",";
                        break;
                    case "State":
                        sql = sql + state + ",";
                        break;
                    case "City":
                        sql = sql + city + ",";
                        break;
                    case "X":
                        sql = sql + lng + ",";
                        break;
                    case "Y":
                        sql = sql + lat + ",";
                        break;
                    case "Uncertainty":
                        sql = sql + unc + ",";
                        break;
                }
                if (i != PluginSettings.Instance.DgvSortOrder.Count - 1) continue;
                // strip off the final comma if last item
                char[] chr = {','};
                sql = sql.TrimEnd(chr);
            }
            sql = sql + " FROM " + SdrAliServerConfig.Default.TableName + " ORDER BY IDField DESC";
            return sql;
        }

        private void BindDataGridView(string connString, string sqlString)
        {
            // bind our db to the dgv now
            var cnn = new OleDbConnection(connString);
            cnn.Open();
            var dbTable = new DataTable();
            var dbAdapter = new OleDbDataAdapter(sqlString, cnn);
            dbAdapter.Fill(dbTable);
            var bindingSource = new BindingSource { DataSource = dbTable };
            _aliPanel.SetDgvBindingSource(bindingSource);
            cnn.Close();
        }

        private void InstanceOnAliSdrServerUdpPortChanged(object sender, EventArgs eventArgs)
        {
            var s = (SdrConfig.Project.Go2ItProjectSettings) sender;
            _aliServerClient.UdpPort = s.AliSdrServerUdpPort;
        }

        private void InstanceOnAliSdrServerUdpHostChanged(object sender, EventArgs eventArgs)
        {
            var s = (SdrConfig.Project.Go2ItProjectSettings)sender;
            _aliServerClient.UdpHost = s.AliSdrServerUdpHost;
        }

        private void InstanceOnAliSdrServerDbPathChanged(object sender, EventArgs eventArgs)
        {
            var s = (SdrConfig.Project.Go2ItProjectSettings)sender;
            _aliServerDbConnString = MdbHelper.GetMdbConnectionString(s.AliSdrServerDbPath);
        }

        private void AliServerClientOnPacketReceieved(object sender, AliServerDataPacket packet)
        {
            PopulateDgv();
        }

        // this is called on startup as well as everytime the ali mode changes
        public void ConfigureAliClient(AliMode am)
        {
            if (_currentAliMode == am) return;  // no need to update anything no change was made
            DisableCurrentAliMode();
            _currentAliMode = am; // update the current alimode
            switch (am)
            {
                case AliMode.Sdraliserver:
                    HandleAliServerClient();
                    break;
                case AliMode.Globalcad:
                    HandleGlobalCadFiles();
                    break;
                case AliMode.Enterpol:
                    // HandleEnterpolDbView();
                    break;
            }
        }

        public void ConfigureInterface(bool nf)
        {
            if (nf)
            {
                if (_networkFleet == NetworkFleet.Active) return;  // it has already been activated no need to repeat
                _networkFleet = NetworkFleet.Active;
                switch (_currentAliMode)
                {
                    case AliMode.Sdraliserver:
                        _aliPanel.DisplayNetworkfleetInterface();
                        break;
                    case AliMode.Globalcad:
                        _aliPanel.DisplayNetworkfleetAndGlobalInterface();
                        break;
                    case AliMode.Enterpol:
                        // HandleEnterpolDbView();
                        break;
                    default: // disabled
                        HandleNetworkFleetClient();
                        _aliPanel.DisplayNetworkfleetInterface();
                        break;
                }
            }
            else
            {
                if (_networkFleet == NetworkFleet.Disabled) return;
                _networkFleet = NetworkFleet.Disabled;
                switch (_currentAliMode)
                {
                    case AliMode.Sdraliserver:
                        _aliPanel.DisplayStandardInterface();
                        break;
                    case AliMode.Globalcad:
                        _aliPanel.DisplayGlobalInterface();
                        break;
                    case AliMode.Enterpol:
                        // _aliPanel.DisplayEnterpolInterface();
                        break;
                    default: // disabled
                        // DisableNetworkFleet();
                        break;
                }
            }
        }

        private void HandleGlobalCadFiles()
        {
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadArchivePathChanged += InstanceOnAliGlobalCadArchivePathChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadLogPathChanged += InstanceOnAliGlobalCadLogPathChanged;
        }

        private void InstanceOnAliGlobalCadLogPathChanged(object sender, EventArgs eventArgs)
        {
            if (_currentAliMode == AliMode.Globalcad)
            {
                _aliPanel.UpdateGlobalCadLogWatcher();
            }
        }

        private void InstanceOnAliGlobalCadArchivePathChanged(object sender, EventArgs eventArgs)
        {
            if (_currentAliMode == AliMode.Globalcad)
            {
                _aliPanel.UpdateGlobalCadArchiveWatcher();
            }
        }

        /// <summary>
        ///  Allows for new behavior during activation
        /// </summary>
        protected override void OnActivate()
        {
            if (_aliPanel == null || _aliPanel.IsDisposed)
            {
                _aliPanel = new AliPanel();
                HandleAliPanelEvents();
            }
            _aliPanel.Show();
            base.OnActivate();
        }

        /// <summary>
        /// Allows for new behavior during deactivation.
        /// </summary>
        protected override void OnDeactivate()
        {
            base.OnDeactivate();
        }

        /// <summary>
        /// Occurs when this function is removed.
        /// </summary>
        protected override void OnUnload()
        {
            base.OnUnload();
        }
        #endregion
    }
}