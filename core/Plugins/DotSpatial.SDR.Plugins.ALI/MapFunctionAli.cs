using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.SDR.Plugins.ALI.Properties;
using SDR.Common;
using ILog = SDR.Common.logging.ILog;
using SDR.Common.UserMessage;
using SDR.Data.Database;
using SDR.Data.Files;
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
        // private NetworkFleet _networkFleet = NetworkFleet.Null;  // set to null on startup only

        // specific interface variables
        private AliServerClient _aliServerClient;  // handles SDR AliServer Interface
        private FileSystemWatcher _globalCadArkWatch;  // archive watcher for global cad
        private FileSystemWatcher _globalCadLogWatch;  // active log watcher for global cad

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

        //private void StartSqlServerDependencyWatcher()
        //{
        //    var conn = GetEnterpolConnString();
        //    SqlConnection sqlConn = new SqlConnection(conn);
        //    SqlDependency.Start(conn);
        //    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM " + SdrAliServerConfig.Default.TableName, sqlConn))
        //    {
        //        // create dependency associated with sql command
        //        var dependency = new SqlDependency(cmd);
        //        // subscribe to the dependency event.
        //        dependency.OnChange += new OnChangeEventHandler(OnDependencyChange);
        //        cmd.ExecuteReader();
        //        //using (SqlDataReader reader = command.ExecuteReader())
        //        //{
        //            // Process the DataReader.
        //        //}
        //    }
        //}

        //void OnDependencyChange(object sender, SqlNotificationEventArgs e)
        //{
        //    MessageBox.Show(e.Info.ToString());
        //}

        //private void EndSqlServerDependencyWatcher()
        //{
        //    var conn = GetEnterpolConnString();
        //    SqlDependency.Stop(conn);
        //}

        private void DisableCurrentAliMode()
        {
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
                if (_globalCadArkWatch != null)
                {
                    _globalCadArkWatch.EnableRaisingEvents = false;
                    _globalCadArkWatch = null;
                }
                if (_globalCadLogWatch != null)
                {
                    _globalCadLogWatch.EnableRaisingEvents = false;
                    _globalCadLogWatch = null;
                }
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadArchivePathChanged -= InstanceOnAliGlobalCadArchivePathChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadLogPathChanged -= InstanceOnAliGlobalCadLogPathChanged;
                _aliPanel.GlobalCadComLogs.SelectedIndexChanged -= GlobalCadComLogsOnSelectedIndexChanged;
            }
            if (_currentAliMode == AliMode.Enterpol)
            {
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolDataSourceChanged -= InstanceOnAliEnterpolDataSourceChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolInitialCatalogChanged -= InstanceOnAliEnterpolInitialCatalogChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolTableNameChanged -= InstanceOnAliEnterpolTableNameChanged;
            }
        }

        private void GlobalCadComLogsOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            try
            {
                var cmb = (ComboBox)sender;
                var itm = cmb.SelectedItem.ToString();
                PluginSettings.Instance.ActiveGlobalCadCommLog = itm;
                string log;
                if (itm.EndsWith(DateTime.Now.ToShortDateString().Replace("/", "")))
                {
                    log = SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadLogPath;
                }
                else
                {
                    log = SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadArchivePath + "\\" + itm + ".log";
                }
                // populate the collection to the datagridview
                _aliPanel.DataGridDisplay.DataSource = GlobalCadLogToCollection(log).ToArray();
            }
            catch (Exception ex)
            {
                var log = AppContext.Instance.Get<ILog>();
                log.Warn("Failed to Populate GlobalCAD File Records to DataView", ex);
            }
        }

        private void PopulateEnterpolDbDgv(string  conn)
        {
            // lets update the dgv column order if needed
            if (_aliPanel.DataGridDisplay.ColumnCount == PluginSettings.Instance.EnterpolDgvSortOrder.Count)
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
                PluginSettings.Instance.EnterpolDgvSortOrder = dgvOrder;
            }
            var sql = ConstructEnterpolSqlQuery();
            BindDataGridViewToSqlServer(conn, sql);
            HumanizeCamelCasedDgvHeaders();
        }

        private void PopulateSdrAliServerDgv(string conn)
        {
            // lets update the dgv column order if needed
            if (_aliPanel.DataGridDisplay.ColumnCount == PluginSettings.Instance.SdrAliServerDgvSortOrder.Count)
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
                PluginSettings.Instance.SdrAliServerDgvSortOrder = dgvOrder;
            }
            var sql = ConstructSdrServerSqlQuery();
            BindDataGridViewToOleDb(conn, sql);
            HumanizeCamelCasedDgvHeaders();
        }

        private void BindDataGridViewToOleDb(string connString, string sqlString)
        {
            try
            {
                var cnn = new OleDbConnection(connString);
                cnn.Open();
                var dbTable = new DataTable();
                var dbAdapter = new OleDbDataAdapter(sqlString, cnn);
                dbAdapter.Fill(dbTable);
                var bindingSource = new BindingSource { DataSource = dbTable };
                _aliPanel.SetDgvBindingSource(bindingSource);
                cnn.Close();
            }
            catch (Exception ex)
            {
                var msg = AppContext.Instance.Get<IUserMessage>();
                msg.Error("Database table binding failed", ex);
            }
        }

        private void BindDataGridViewToSqlServer(string connString, string sqlString)
        {
            try
            {
                var dbAdapter = new SqlDataAdapter(sqlString, connString);
                var dbTable = new DataTable {Locale = System.Globalization.CultureInfo.InvariantCulture};
                dbAdapter.Fill(dbTable);
                var bindingSource = new BindingSource { DataSource = dbTable };
                _aliPanel.SetDgvBindingSource(bindingSource);
            }
            catch (Exception ex)
            {
                var msg = AppContext.Instance.Get<IUserMessage>();
                msg.Error("Database table binding failed", ex);
            }
        }

        private static string ConstructEnterpolSqlQuery()
        {
            const string callDate = "CONVERT(varchar(10), [DateTime], 120) As CallDate";
            const string callTime = "CONVERT(varchar(8), [DateTime], 108) As CallTime";
            const string phone = "LTRIM(RTRIM([RPPhone])) As Phone";
            const string address = "LTRIM(RTRIM(CAST(ISNULL([HouseNumber], '') AS VARCHAR) + ' ' + ISNULL([HouseNumberSuffix], ''))) As Address";
            const string street = "LTRIM(RTRIM(LTRIM(RTRIM(ISNULL([PreDirection], '') + SPACE(1) + ISNULL([StreetName1], ''))) + SPACE(1) + LTRIM(RTRIM(ISNULL([StType], '') + SPACE(1) + ISNULL([PostDirection], ''))))) As Street";
            const string srvClass = "ISNULL([RPvia], '') As ServiceClass";
            const string state = "ISNULL([State], '') As State";
            const string city = "ISNULL([CityName], '') As City";
            const string customer = "ISNULL([CustomerName], '') As Customer";
            const string lat = "ISNULL([Latitude], '') As Y";
            const string lng = "ISNULL([Longitude], '') As X";

            string sql = "SELECT ";
            for (int i = 0; i <= PluginSettings.Instance.EnterpolDgvSortOrder.Count - 1; i++)
            {
                var col = PluginSettings.Instance.EnterpolDgvSortOrder[i];
                switch (col)
                {
                    case "Address":
                        sql = sql + address + ",";
                        break;
                    case "CallDate":
                        sql = sql + callDate + ",";
                        break;
                    case "CallTime":
                        sql = sql + callTime + ",";
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
                }
                if (i != PluginSettings.Instance.EnterpolDgvSortOrder.Count - 1) continue;
                char[] chr = { ',' };  // strip off the final comma if last item
                sql = sql.TrimEnd(chr);
            }
            sql = sql + " FROM [" + SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolTableName + "].[dbo].[" + SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolInitialCatalog + "]";
            return sql;
        }

        private void HandleAliServerClient()
        {
            var msg = AppContext.Instance.Get<IUserMessage>();
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerDbPathChanged += InstanceOnAliSdrServerDbPathChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpHostChanged += InstanceOnAliSdrServerUdpHostChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpPortChanged += InstanceOnAliSdrServerUdpPortChanged;

            _aliServerClient = new AliServerClient(SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpHost, SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpPort);
            _aliServerClient.Login();
            _aliServerClient.PacketReceieved += AliServerClientOnPacketReceieved;
            if (_aliServerClient.Ping())  // if the server is not responding notify the user
            {
                msg.Warn(@"AliServer is not responding at host: " + SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpHost + @" port: " + SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpPort);
            }

            try
            {
                var aliServerDbConnString = MdbHelper.GetMdbConnectionString(SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerDbPath);
                PopulateSdrAliServerDgv(aliServerDbConnString);
            }
            catch (Exception ex)
            {
                msg.Error(ex.Message, ex);
            }
        }

        private void HandleEnterpolDbView()
        {
            // validate our connection, table, and init catalog are valid
            var msg = AppContext.Instance.Get<IUserMessage>();
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolDataSourceChanged += InstanceOnAliEnterpolDataSourceChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolInitialCatalogChanged += InstanceOnAliEnterpolInitialCatalogChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolTableNameChanged += InstanceOnAliEnterpolTableNameChanged;

            try
            {
                var conn = GetEnterpolConnString();
                PopulateEnterpolDbDgv(conn);
                // StartSqlServerDependencyWatcher();
            }
            catch (Exception ex)
            {
                msg.Error(ex.Message, ex);
            }
        }

        private string GetEnterpolConnString()
        {
            var connString = SqlServerHelper.GetSqlServerConnectionString(
                "Server=" + SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolDataSource + ";" +
                "Database=" + SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolTableName + ";" +
                "Integrated Security=SSPI;" +
                "connection timeout=15");

            if (!SqlServerHelper.TableExists(connString, SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolInitialCatalog))
            {
                var m = @"Initial catalog " +
                        SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolInitialCatalog +
                        " does not exist in the database " +
                        SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolTableName;

                var ex = new ArgumentException(m, "initial_catalog", null);
                throw ex;
            }
            return connString;
        }

        private void InstanceOnAliEnterpolTableNameChanged(object sender, EventArgs eventArgs)
        {
            try
            {
                var conn = GetEnterpolConnString();
                PopulateEnterpolDbDgv(conn);
            }
            catch (Exception ex)
            {
                var msg = AppContext.Instance.Get<IUserMessage>();
                msg.Error(ex.Message, ex);
            }
        }

        private void InstanceOnAliEnterpolInitialCatalogChanged(object sender, EventArgs eventArgs)
        {
            try
            {
                var conn = GetEnterpolConnString();
                PopulateEnterpolDbDgv(conn);
            }
            catch (Exception ex)
            {
                var msg = AppContext.Instance.Get<IUserMessage>();
                msg.Error(ex.Message, ex);
            }
        }

        private void InstanceOnAliEnterpolDataSourceChanged(object sender, EventArgs eventArgs)
        {
            try
            {
                var conn = GetEnterpolConnString();
                PopulateEnterpolDbDgv(conn);
            }
            catch (Exception ex)
            {
                var msg = AppContext.Instance.Get<IUserMessage>();
                msg.Error(ex.Message, ex);
            }
        }

        private void HandleGlobalCadFiles()
        {
            //  initiate file watchers for changes
            _globalCadArkWatch = new FileSystemWatcher
            {
                Path = SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadArchivePath,
                NotifyFilter = NotifyFilters.FileName,
                Filter = "*.log"
            };
            _globalCadArkWatch.Created += OnGlobalCadArchiveChanged;
            _globalCadArkWatch.Deleted += OnGlobalCadArchiveChanged;
            _globalCadArkWatch.Renamed += OnGlobalCadArchiveChanged;
            _globalCadArkWatch.Changed += OnGlobalCadArchiveChanged;
            _globalCadArkWatch.EnableRaisingEvents = true;

            _globalCadLogWatch = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadLogPath),
                Filter = "*.log",
                NotifyFilter = NotifyFilters.LastWrite
            };
            _globalCadLogWatch.Changed += OnGlobalCadFileChanged;
            _globalCadLogWatch.EnableRaisingEvents = true;

            SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadArchivePathChanged += InstanceOnAliGlobalCadArchivePathChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadLogPathChanged += InstanceOnAliGlobalCadLogPathChanged;

            PopulateGlobalCadInterface();
        }

        public void PopulateGlobalCadInterface()
        {
            if (PluginSettings.Instance.ActiveGlobalCadCommLog.Length == 0)
            {
                PluginSettings.Instance.ActiveGlobalCadCommLog = Path.GetFileNameWithoutExtension(
                    SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadLogPath) +
                    DateTime.Now.ToShortDateString().Replace("/", "");
            }
            // populate the combobox with files now
            _aliPanel.PopulateComboBox();
            // set up the event handler for the combobox index change
            _aliPanel.GlobalCadComLogs.SelectedIndexChanged += GlobalCadComLogsOnSelectedIndexChanged;
            // set the selection initiating the datagridview columns
            _aliPanel.GlobalCadComLogs.SelectedIndex = _aliPanel.GlobalCadComLogs.FindStringExact(PluginSettings.Instance.ActiveGlobalCadCommLog);
        }

        private void OnGlobalCadArchiveChanged(object source, FileSystemEventArgs e)
        {
            _aliPanel.GlobalCadComLogs.SelectedIndexChanged -= GlobalCadComLogsOnSelectedIndexChanged;
            _aliPanel.PopulateComboBox();
            _aliPanel.GlobalCadComLogs.SelectedIndexChanged += GlobalCadComLogsOnSelectedIndexChanged;
        }

        private void OnGlobalCadFileChanged(object source, FileSystemEventArgs e)
        {
            var curLog = Path.GetFileNameWithoutExtension(
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadLogPath) +
                DateTime.Now.ToShortDateString().Replace("/", "");

            if (PluginSettings.Instance.ActiveGlobalCadCommLog == curLog)
            {
                try
                {
                    var log = SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadLogPath;
                    var array = GlobalCadLogToCollection(log).ToArray();
                    _aliPanel.SetDgvDataSource(array);
                }
                catch (Exception ex)
                {
                    var msg = AppContext.Instance.Get<IUserMessage>();
                    msg.Warn(ex.Message, ex);
                }
            }
        }

        private List<GlobalCadRecord> GlobalCadLogToCollection(string logPath)
        {
            var bindingList = new List<GlobalCadRecord>();
            var newRec = false;
            var rec = new List<string>();

            using (var f = File.OpenText(logPath))
            {
                while (!f.EndOfStream)
                {
                    var l = f.ReadLine();
                    // do basic start end cleanup
                    if (l != null && l.Contains("<Start Data>"))
                    {
                        l = "<Start Data>";
                    }
                    if (l != null && l.Contains("<End Data>"))
                    {
                        l = "<End Data>";
                    }
                    // determine line type and add to our list for processing
                    switch (l)
                    {
                        case "<Start Data>":
                            rec.Clear();
                            newRec = true;
                            rec.Add(l);
                            break;
                        case "<End Data>":
                            rec.Add(l);
                            var obj = ProcessGlobalCadRecord(rec);
                            if (obj != null)
                            {
                                bindingList.Add(obj);
                            }
                            rec.Clear();
                            newRec = false;
                            break;
                        default:
                            if (newRec)
                            {
                                rec.Add(l);
                            }
                            break;
                    }
                }
            }
            return bindingList;
        }

        private static GlobalCadRecord ProcessGlobalCadRecord(List<string> recList)
        {
            // fetch the settings for parsing the values from the list
            var pFile = SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadConfigPath;
            var timeLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "TimeLine", pFile)) - 1;
            var timeStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "TimeStart", pFile)) - 1;
            var timeLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "TimeLength", pFile));
            var phoneLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "PhoneLine", pFile)) - 1;
            var phoneStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "PhoneStart", pFile)) - 1;
            var phoneLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "PhoneLength", pFile));
            var houseNumLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "HouseNumberLine", pFile)) - 1;
            var houseNumStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "HouseNumberStart", pFile)) - 1;
            var houseNumLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "HouseNumberLength", pFile));
            var alphaLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "AlphaLine", pFile)) - 1;
            var alphaStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "AlphaStart", pFile)) - 1;
            var alphaLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "AlphaLength", pFile));
            var strPreLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "StreetPrefixLine", pFile)) - 1;
            var strPreStartDelta = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "StreetPrefixStartDelta", pFile));
            var strPreLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "StreetPrefixLength", pFile));
            var strLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "StreetLine", pFile)) - 1;
            var strStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "StreetStart", pFile)) - 1;
            var strLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "StreetLength", pFile));
            var servClsLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "ClassOfServiceLine", pFile)) - 1;
            var servClsStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "ClassOfServiceStart", pFile)) - 1;
            var servClsLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "ClassOfServiceLength", pFile));
            var xLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "XLine", pFile)) - 1;
            var xStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "XStart", pFile)) - 1;
            var xLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "XLength", pFile));
            var yLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "YLine", pFile)) - 1;
            var yStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "YStart", pFile)) - 1;
            var yLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "YLength", pFile));
            var uncLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "UNCLine", pFile)) - 1;
            var uncStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "UNCStart", pFile)) - 1;
            var uncLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "UNCLength", pFile));
            var cityLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "CityLine", pFile)) - 1;
            var cityStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "CityStart", pFile)) - 1;
            var cityLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "CityLength", pFile));
            var stateLine = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "StateLine", pFile)) - 1;
            var stateStart = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "StateStart", pFile)) - 1;
            var stateLen = Convert.ToInt32(IniHelper.IniReadValue(GlobalCadConfig.Default.IniKeyLookup, "StateLength", pFile));

            var gcr = new GlobalCadRecord();
            try
            {
                gcr.Time = recList[timeLine].Substring(timeStart, timeLen).Trim();
                gcr.Phone = recList[phoneLine].Substring(phoneStart, phoneLen).Trim();
                // assemble the address
                gcr.Address = (recList[houseNumLine].Substring(houseNumStart, houseNumLen).Trim() + " " +
                                recList[alphaLine].Substring(alphaStart, alphaLen).Trim()).Trim();
                // check for a street prefix
                var tPrefix = string.Empty;
                if (recList[houseNumLine].Length > houseNumLen)
                {
                    tPrefix = recList[strPreLine].Substring(
                        recList[strPreLine].Length - strPreStartDelta, strPreLen).Trim();
                }
                gcr.Street = (tPrefix + " " + recList[strLine].Substring(strStart, strLen).Trim()).Trim();
                gcr.ServiceClass = recList[servClsLine].Substring(servClsStart, servClsLen).Trim();
                if (gcr.ServiceClass.ToUpper() == "MOBL" ||
                    gcr.ServiceClass.ToUpper() == "WRLS" ||
                    gcr.ServiceClass.ToUpper() == "WPH1" ||
                    gcr.ServiceClass.ToUpper() == "WPH2")
                {
                    gcr.X = recList[xLine].Substring(xStart, xLen).Trim();
                    gcr.Y = recList[yLine].Substring(yStart, yLen).Trim();
                    gcr.Unc = recList[uncLine].Substring(uncStart, uncLen).Trim();
                }
                else
                {
                    gcr.X = string.Empty;
                    gcr.Y = string.Empty;
                    gcr.Unc = string.Empty;
                }
                gcr.State = recList[stateLine].Substring(stateStart, stateLen).Trim();
                gcr.City = recList[cityLine].Substring(cityStart, cityLen).Trim();
            }
            catch (Exception ex)
            {
                var msg = AppContext.Instance.Get<IUserMessage>();
                msg.Error("Failed to Parse GlobalCAD Record", ex);
                return null;
            }
            return gcr;
        }

        private void HandleNetworkFleetClient()
        {
            //_aliServerClient = new AliServerClient(SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpHost, SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpPort);
            //_aliServerClient.Login();
            //_aliServerClient.PacketReceieved += AliServerClientOnPacketReceieved;
            //SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerDbPathChanged += InstanceOnAliSdrServerDbPathChanged;
            //SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpHostChanged += InstanceOnAliSdrServerUdpHostChanged;
            //SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpPortChanged += InstanceOnAliSdrServerUdpPortChanged;
            //_aliServerDbConnString = MdbHelper.GetMdbConnectionString(SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerDbPath);
            //PopulateSdrAliServerDgv();
        }

        private void HumanizeCamelCasedDgvHeaders()
        {
            for (var j = 0; j <= _aliPanel.DataGridDisplay.ColumnCount - 1; j++)
            {
                var n = _aliPanel.DataGridDisplay.Columns[j].HeaderText;
                _aliPanel.DataGridDisplay.Columns[j].HeaderText = Regex.Replace(n, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
            }
        }

        private static string ConstructSdrServerSqlQuery()
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
            for (int i = 0; i <= PluginSettings.Instance.SdrAliServerDgvSortOrder.Count - 1; i++)
            {
                var col = PluginSettings.Instance.SdrAliServerDgvSortOrder[i];
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
                if (i != PluginSettings.Instance.SdrAliServerDgvSortOrder.Count - 1) continue;
                char[] chr = { ',' };  // strip off the final comma if last item
                sql = sql.TrimEnd(chr);
            }
            sql = sql + " FROM " + SdrAliServerConfig.Default.TableName + " ORDER BY IDField DESC";
            return sql;
        }

        private void InstanceOnAliSdrServerUdpPortChanged(object sender, EventArgs eventArgs)
        {
            var s = (SdrConfig.Project.Go2ItProjectSettings) sender;
            _aliServerClient.UdpPort = s.AliSdrServerUdpPort;
            if (!_aliServerClient.Ping())
            {
                var msg = AppContext.Instance.Get<IUserMessage>();
                msg.Warn(@"AliServer is not responding at host: " + SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpHost + @" port: " + SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpPort);
            }
        }

        private void InstanceOnAliSdrServerUdpHostChanged(object sender, EventArgs eventArgs)
        {
            var s = (SdrConfig.Project.Go2ItProjectSettings)sender;
            _aliServerClient.UdpHost = s.AliSdrServerUdpHost;
            if (!_aliServerClient.Ping())
            {
                var msg = AppContext.Instance.Get<IUserMessage>();
                msg.Warn(@"AliServer is not responding at host: " + SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpHost + @" port: " + SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpPort);
            }
        }

        private void InstanceOnAliSdrServerDbPathChanged(object sender, EventArgs eventArgs)
        {
            try
            {
                var s = (SdrConfig.Project.Go2ItProjectSettings)sender;
                var aliServerDbConnString = MdbHelper.GetMdbConnectionString(s.AliSdrServerDbPath);
                PopulateSdrAliServerDgv(aliServerDbConnString);
            }
            catch (Exception ex)
            {
                var msg = AppContext.Instance.Get<IUserMessage>();
                msg.Error(ex.Message, ex);
            }
        }

        private void AliServerClientOnPacketReceieved(object sender, AliServerDataPacket packet)
        {
            try
            {
                var aliServerDbConnString = MdbHelper.GetMdbConnectionString(SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerDbPath);
                PopulateSdrAliServerDgv(aliServerDbConnString);
            }
            catch (Exception ex)
            {
                var msg = AppContext.Instance.Get<IUserMessage>();
                msg.Error(ex.Message, ex);
            }
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
                    HandleEnterpolDbView();
                    break;
            }
        }

        public void ConfigureInterface(bool nf)
        {
            if (nf)
            {
                // if (_networkFleet == NetworkFleet.Active) return;  // it has already been activated no need to repeat
                // _networkFleet = NetworkFleet.Active;
                switch (_currentAliMode)
                {
                    case AliMode.Sdraliserver:
                        _aliPanel.DisplayNetworkfleetInterface();
                        break;
                    case AliMode.Globalcad:
                        _aliPanel.DisplayNetworkfleetAndGlobalInterface();
                        break;
                    case AliMode.Enterpol:
                        _aliPanel.DisplayNetworkfleetInterface();
                        break;
                    default: // disabled alimode use networkfleet independantly
                        HandleNetworkFleetClient();
                        _aliPanel.DisplayNetworkfleetInterface();
                        break;
                }
            }
            else
            {
                // TODO: validate these are actually causing issues (not needed)
                // if (_networkFleet == NetworkFleet.Disabled) return;
                // _networkFleet = NetworkFleet.Disabled;
                switch (_currentAliMode)
                {
                    case AliMode.Sdraliserver:
                        _aliPanel.DisplayStandardInterface();
                        break;
                    case AliMode.Globalcad:
                        _aliPanel.DisplayGlobalInterface();
                        break;
                    case AliMode.Enterpol:
                        _aliPanel.DisplayStandardInterface();
                        break;
                    default: // disabled
                        // DisableNetworkFleet();
                        break;
                }
            }
        }

        private void InstanceOnAliGlobalCadLogPathChanged(object sender, EventArgs eventArgs)
        {
            if (_currentAliMode == AliMode.Globalcad)
            {
                _globalCadLogWatch.EnableRaisingEvents = false;
                _globalCadLogWatch.Path = Path.GetDirectoryName(SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadLogPath);
                _globalCadLogWatch.EnableRaisingEvents = true;
            }
        }

        private void InstanceOnAliGlobalCadArchivePathChanged(object sender, EventArgs eventArgs)
        {
            if (_currentAliMode == AliMode.Globalcad)
            {
                _globalCadArkWatch.EnableRaisingEvents = false;
                _globalCadArkWatch.Path = SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadArchivePath;
                _globalCadArkWatch.EnableRaisingEvents = true;
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

public class GlobalCadRecord
{
    public string Time { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    [DisplayName(@"Service Class")]
    public string ServiceClass { get; set; }
    public string X { get; set; }
    public string Y { get; set; }
    public string Unc { get; set; }
    [DisplayName(@"Full Address")]
    public string FullAddress
    {
        get { return (Address + " " + Street).Trim(); }
    }
}