using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.SDR.Plugins.ALI.Properties;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using GeoTimeZone;
using SDR.Common;
using ILog = SDR.Common.logging.ILog;
using SDR.Common.UserMessage;
using SDR.Data.Database;
using SDR.Data.Files;
using SDR.Network;
using SdrConfig = SDR.Configuration;
using System.IO;
using Point = System.Drawing.Point;

namespace DotSpatial.SDR.Plugins.ALI
{
    /// <summary>
    /// A MapFunction that connects with ALI Interfaces
    /// </summary>
    public class MapFunctionAli : MapFunction
    {
        private AliPanel _aliPanel;
        private AliMode _currentAliMode = AliMode.Disabled;
        private AliAvl _currentAliAvl = AliAvl.Disabled;
        private readonly ProjectionInfo _wgs84Projection = KnownCoordinateSystems.Geographic.World.WGS1984;

        // specific interface variables
        private AliServerClient _aliServerClient;  // sdr ali server client
        private AliServerClient _networkFleetClient; // verizon network fleet client
        private FileSystemWatcher _globalCadArkWatch;  // watches archive directory global cad interface updates
        private FileSystemWatcher _globalCadLogWatch;  // watches log file for global cad interface updates
        private System.Timers.Timer _avlReadIntervalTimer;  // reads avl update on user specified interval
        private System.Timers.Timer _avlUpdateIntervalTimer; // in responder mode sends vehicle position to database
        private Dictionary<string, AvlVehicle> _avlVehicles;  // track all avl vehicle states
        private int _myUnitId = -1;  // in responder mode this holds the unit id of the user

        #region Constructors

        /// <summary>
        /// Creates a new instance of MapFunctionAli, with associated panel
        /// </summary>
        /// <param name="ap"></param>
        public MapFunctionAli(AliPanel ap)
        {
            Name = "MapFunctionAli";
            YieldStyle = YieldStyles.AlwaysOn;

            _aliPanel = ap;

            HandleAliPanelEvents();
        }

        private void HandleAliPanelEvents()
        {
            _aliPanel.PerformUpdate += AliPanelOnPerformUpdate;
            _aliPanel.PerformLocate += AliPanelOnPerformLocate;
            _aliPanel.OnRowDoublelicked += AliPanelOnOnRowDoublelicked;
        }

        // BEGIN THE HACK FOR OTTAWA DEMO
        // basically we are gonna fake it till we make it.
        // check if the row contains an X or Y coord, if so then zoom to the location
        // if not then fetch the address info and search the address layer for the record?
        private static int GetColumnDisplayIndex(string name, DataGridView dgv)
        {
            for (int i = 0; i <= dgv.ColumnCount - 1; i++)
            {
                if (dgv.Columns[i].Name == name)
                {
                    return dgv.Columns[i].DisplayIndex;
                }
            }
            return -1;
        }

        private double[] LatLonReproject(double x, double y)
        {
            var xy = new[] { x, y };

            // change y coordinate to be less than 90 degrees to prevent an issue from arising
            if (xy[1] >= 90) xy[1] = 89.9;
            if (xy[1] <= -90) xy[1] = -89.9;

            // convert points to proper projection. by default it describes WGS84 points which may or may not be right
            // TODO: isnt this referenced somewhere else ?? no need to redefine it?
            const string wgs84String = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223562997]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.0174532925199433]]";
            var mapProjEsriString = Map.Projection.ToEsriString();
            var isWgs84 = (mapProjEsriString.Equals(wgs84String));

            if (isWgs84) return xy; // if the projection is anything other than WGS84, convert to points to describe location

            var z = new double[1];
            var wgs84Projection = ProjectionInfo.FromEsriString(wgs84String);
            var currentMapProjection = ProjectionInfo.FromEsriString(mapProjEsriString);
            Reproject.ReprojectPoints(xy, z, wgs84Projection, currentMapProjection, 0, 1);

            return xy;  // return array with a single x and y value
        }

        private void ZoomToCoordinates(string strLat, string strLng)
        {
            double lat;
            double.TryParse(strLat, out lat);
            double lon;
            double.TryParse(strLng, out lon);
            // TODO: look at these two and determine the better method
            // var one = ConvertLatLonToMap(lat, lon);
            var xy = LatLonReproject(lon, lat);

            //  Get extent where center is desired X,Y coordinate.
            var width = Map.ViewExtents.Width;
            var height = Map.ViewExtents.Height;
            Map.ViewExtents.X = (xy[0] - (width / 2));
            Map.ViewExtents.Y = (xy[1] + (height / 2));
            var ex = Map.ViewExtents;

            // set App.Map.ViewExtents to new extent that centers on desired LatLong.
            Map.ViewExtents = ex;
        }

        private void ZoomToAddress(string address)
        {
            // make sure the active map panel has the layer available for display
            // ActivateMapPanelWithLayer(ftLookup.Layer);
            // cycle through layers of the active map panel and find the one we need
            var layers = Map.GetFeatureLayers();
            foreach (IMapFeatureLayer mapLayer in layers)
            {
                if (mapLayer != null &&
                    String.IsNullOrEmpty(Path.GetFileNameWithoutExtension((mapLayer.DataSet.Filename)))) return;
                if (mapLayer == null) continue;

                IFeatureSet fs = FeatureSet.Open(mapLayer.DataSet.Filename);
                // TODO: terrible terrible hack
                if (fs == null || fs.Name != "address") continue;

                string q = "[FULL_ADDRE] = '" + address + "'";
                var sel = fs.SelectByAttribute(q);
                if (sel.Count > 0)
                {
                    IFeature ft = sel[0];
                    var xy = ft.Coordinates[0];

                    //  Get extent where center is desired X,Y coordinate.
                    var width = Map.ViewExtents.Width;
                    var height = Map.ViewExtents.Height;
                    Map.ViewExtents.X = (xy[0] - (width / 2));
                    Map.ViewExtents.Y = (xy[1] + (height / 2));
                    var ex = Map.ViewExtents;

                    // set App.Map.ViewExtents to new extent that centers on desired LatLong.
                    Map.ViewExtents = ex;
                }
            }
        }
        // END HACK FOR OTTAWA DEMO - THE ABOVE WAS ALL RIPPED FROM SEARCH 

        private void AliPanelOnOnRowDoublelicked(object sender, EventArgs eventArgs)
        {
            var evnt = eventArgs as DataGridViewCellEventArgs;
            if (evnt == null) return;

            var xIdx = GetColumnDisplayIndex("X", _aliPanel.DataGridDisplay);
            var xVal = _aliPanel.DataGridDisplay.Rows[evnt.RowIndex].Cells[xIdx].Value.ToString();
            var yIdx = GetColumnDisplayIndex("Y", _aliPanel.DataGridDisplay);
            var yVal = _aliPanel.DataGridDisplay.Rows[evnt.RowIndex].Cells[yIdx].Value.ToString();
            var aIdx = GetColumnDisplayIndex("FullAddress", _aliPanel.DataGridDisplay);
            var aVal = _aliPanel.DataGridDisplay.Rows[evnt.RowIndex].Cells[aIdx].Value.ToString();

            if (xVal != string.Empty && yVal != string.Empty)
            {
                ZoomToCoordinates(yVal, xVal);
            }
            else
            {
                ZoomToAddress(aVal);
            }

            // TODO: when we actually write this correctly
            //switch (_currentAliMode)
            //{
            //    case AliMode.Enterpol:
            //        break;
            //    case AliMode.Globalcad:


            //        break;
            //    case AliMode.Sdraliserver:
            //        break;
            //}
        }

        private void AliPanelOnPerformLocate(object sender, EventArgs eventArgs)
        {
            var x = sender;

            //if (_searchPanel.SearchQuery.Length <= 0) return;
            //var q = _searchPanel.SearchQuery;
            //// if its an intersection and there are two terms do a location zoom, otherwise find intersections
            //if (PluginSettings.Instance.SearchMode == SearchMode.Intersection)
            //{
            //    if (_searchPanel.SearchQuery.Length <= 1) return;  // only the | is present (blank search)
            //    var arr = q.Split('|');
            //    if (arr[1].Length > 0) // zoom to intersection of the two features
            //    {
            //        ZoomToIntersection(arr[0], arr[1]);
            //        return;
            //    } // else look for intersections on this feature and populate combos and dgv
            //    q = arr[0];
            //}
            //else if (PluginSettings.Instance.SearchMode == SearchMode.City || PluginSettings.Instance.SearchMode == SearchMode.Esn)
            //{
            //    _searchPanel.ClearSearches();  // clear any existing searches (fires the event above this one actually)
            //    var hitz = ExecuteLuceneQuery(q);
            //    var scoreDocs = hitz as ScoreDoc[] ?? hitz.ToArray();
            //    if (scoreDocs.Count() != 0)
            //    {
            //        var hit = scoreDocs.First();
            //        var doc = _indexSearcher.Doc(hit.Doc);
            //        var lookup = new FeatureLookup
            //        {
            //            Fid = doc.Get(FID),
            //            Layer = doc.Get(LYRNAME),
            //            Shape = doc.Get(GEOSHAPE)
            //        };
            //        ZoomToFeatureLookup(lookup);
            //        return;
            //    }
            //}
            //else if (PluginSettings.Instance.SearchMode == SearchMode.Coordinate)
            //{
            //    if (_searchPanel.SearchQuery.Length <= 1) return;  // only the | (internal separator) is present (empty search)
            //    var arr = q.Split('|');
            //    if (arr.Length != 2) return; // lat and long have not been passed in
            //    if (arr[0].Length > 0 && arr[1].Length > 0)
            //    {
            //        _searchPanel.ClearSearches();  // clear any existing searches
            //        ZoomToCoordinates(arr[0], arr[1]);
            //        return;
            //    }
            //}
     
            // * all other query types are processed and proceed from this point ....
            // * each requires a lucene query and populates combos and datagridview    
            // */
            //_searchPanel.ClearSearches();  // clear any existing searches (fires the event above this one actually)
            //// setup columns, ordering, etc for results datagridview
            //PrepareDataGridView();
            //// execute our lucene query
            //var hits = ExecuteLuceneQuery(q);
            //FormatQueryResults(hits);
        }

        private void AliPanelOnPerformUpdate(object sender, EventArgs eventArgs)
        {
            var x = sender;

            // TODO
        }

        #endregion

        #region Methods
        // called on startup and everytime the alimode/aliavl changes
        public void ConfigureAliClient(AliMode am, AliAvl avl)
        {
            if (_currentAliMode == am && _currentAliAvl == avl) return;  // no need to update anything no change was made

            if (_currentAliMode != am)
            {
                DisableCurrentAliModeEventHandlers();
                switch (am)
                {
                    case AliMode.Enterpol:
                        HandleEnterpolIncidentsView();
                        break;
                    case AliMode.Globalcad:
                        HandleGlobalCadFiles();
                        break;
                    case AliMode.Sdraliserver:
                        HandleAliServerClient();
                        break;
                }
            }
            if (_currentAliAvl != avl)
            {
                DisableCurrentAliAvlEventHandlers();
                switch (avl)
                {
                    case AliAvl.Enterpolavl:
                        HandleEnterpolAvlView();
                        break;
                    case AliAvl.Networkfleet:
                        HandleNetworkFleetClient();
                        break;
                }
            }
            // update the current avl and mode
            _currentAliMode = am;
            _currentAliAvl = avl;
            // update interface display to alipanel
            ConfigureAliPanelInterface();
        }

        // TODO: Is enterpol going to implemnt this or not?
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

        private void DisableCurrentAliModeEventHandlers()
        {
            if (_currentAliMode == AliMode.Enterpol)
            {
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolDataSourceChanged -= InstanceOnAliEnterpolIncidentsDbParamsChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolInitialCatalogChanged -= InstanceOnAliEnterpolIncidentsDbParamsChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolTableNameChanged -= InstanceOnAliEnterpolIncidentsDbParamsChanged;
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
                _aliPanel.ComLogsComboBox.SelectedIndexChanged -= GlobalCadComLogsOnSelectedIndexChanged;
            }
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
        }

        private void DisableCurrentAliAvlEventHandlers()
        {
            if (_currentAliAvl == AliAvl.Enterpolavl)
            {
                _aliPanel.VehicleFleetListBox.ItemCheck -= VehicleFleetListBoxOnItemCheck;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlReadFreqChanged -= InstanceOnAliEnterpolAvlReadFreqChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolDataSourceChanged -= InstanceOnAliEnterpolAvlDbParamsChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlTableNameChanged -= InstanceOnAliEnterpolAvlDbParamsChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlInitialCatalogChanged -= InstanceOnAliEnterpolAvlDbParamsChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlAge1FreqChanged -= InstanceOnAliEnterpolAvlAgeFreqChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlAge2FreqChanged -= InstanceOnAliEnterpolAvlAgeFreqChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlAge3FreqChanged -= InstanceOnAliEnterpolAvlAgeFreqChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlEmsCharChanged -= InstanceOnAliEnterpolAvlEmsSymbolChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlEmsColorChanged -= InstanceOnAliEnterpolAvlEmsSymbolChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLeCharChanged -= InstanceOnAliEnterpolAvlLeSymbolChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLeColorChanged -= InstanceOnAliEnterpolAvlLeSymbolChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlFdCharChanged -= InstanceOnAliEnterpolAvlFdSymbolChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlFdColorChanged -= InstanceOnAliEnterpolAvlFdSymbolChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlSymbolFontChanged -= InstanceOnAliEnterpolAvlSymbolFontChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLabelFontChanged -= InstanceOnAliEnterpolAvlLabelFontChanged;
                if (_avlReadIntervalTimer != null) _avlReadIntervalTimer.Enabled = false;
                _avlReadIntervalTimer = null;
                // handle all responder based events
                if (SdrConfig.Settings.Instance.ApplicationMode != SdrConfig.AppMode.Responder) return;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlSetMyLocProcChanged -= InstanceOnAliEnterpolAvlSetMyLocProcChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlWhoAmIProcChanged -= InstanceOnAliEnterpolAvlWhoAmIProcChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlUpdateFreqChanged -= InstanceOnAliEnterpolAvlUpdateFreqChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlMyColorChanged -= InstanceOnAliEnterpolAvlMyColorChanged;
                if (_avlUpdateIntervalTimer != null) _avlUpdateIntervalTimer.Enabled = false;
                _avlUpdateIntervalTimer = null;
            }
            if (_currentAliAvl == AliAvl.Networkfleet)
            {
                // TODO:
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
                if (itm.EndsWith(FormatCurrentDate()))
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

        private void PopulateEnterpolIncidentsDgv(string  conn)
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

        private static AvlVehicleType SetAvlVehicleType(string vType)
        {
            if (EnterpolAvlConfig.Default.UnitTypeEmsLookup.Equals(vType))
            {
                return AvlVehicleType.EmergencyMedicalService;
            }
            if (EnterpolAvlConfig.Default.UnitTypePdLookup.Equals(vType))
            {
                return AvlVehicleType.LawEnforcement;
            }
            if (EnterpolAvlConfig.Default.UnitTypeFdLookup.Equals(vType))
            {
                return AvlVehicleType.FireDepartment;
            }
            return AvlVehicleType.None;
        }

        private bool CheckAutoHideStatus()
        {
            switch (_currentAliAvl)
            {
                case AliAvl.Enterpolavl:
                    return SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlAutoHideInactiveUnits;
                case AliAvl.Networkfleet:
                    return SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetAvlAutoHideInactiveUnits;
            }
            return false;
        }

        private AvlVehicle CreateNewAvlVehicle(DataRowView item)
        {
            var avlVehicle = new AvlVehicle
            {
                UnitId = item[1].ToString(),
                UpdateTime = DateTime.Parse(item[3].ToString()),
                Latitude = Convert.ToDouble(item[4].ToString()),
                Longitude = Convert.ToDouble(item[5].ToString())
            };
            // if the current state is inactive and auto-hide function is enabled
            if (CheckAutoHideStatus())
            {
                var diff = DateTime.Now.Subtract(avlVehicle.UpdateTime);
                if (diff.TotalSeconds > SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlAge3Freq)
                {
                    avlVehicle.Visible = false;
                }
            }
            return avlVehicle;
        }

        private void VehicleFleetListBoxOnItemCheck(object sender, ItemCheckEventArgs e)
        {
            AvlVehicle avlVehicle = null; // attempt to get the unit by the unit id
            if (_currentAliAvl == AliAvl.Enterpolavl)
            {
                var item = _aliPanel.VehicleFleetListBox.Items[e.Index] as DataRowView;
                if (item == null) return;
                _avlVehicles.TryGetValue(item[1].ToString(), out avlVehicle);
            }
            if (_currentAliAvl == AliAvl.Networkfleet)
            {
                avlVehicle = _aliPanel.VehicleFleetListBox.Items[e.Index] as AvlVehicle;
            }
            if (avlVehicle == null) return;
            avlVehicle.Visible = e.NewValue == CheckState.Checked;
            if (CheckAutoHideStatus())
            {
                if (avlVehicle.CurrentInterval.Equals(3))
                {
                    avlVehicle.IgnoreActiveHide = CheckState.Checked == e.NewValue;
                }   
            }
            if (_currentAliAvl == AliAvl.Enterpolavl)
            {
                UpdateAvlFleetPositions();
            }
            if (_currentAliAvl == AliAvl.Networkfleet)
            {
                if (Map != null)
                {
                    Map.Invalidate();  // paint the map with updated avl positions (calls MapOnPaint)
                }
            }
        }

        private void BindCheckedListBoxToSqlServer(string connString, string sqlString)
        {
            try
            {
                var dbAdapter = new SqlDataAdapter(sqlString, connString);
                var dbTable = new DataTable { Locale = CultureInfo.InvariantCulture };
                dbAdapter.Fill(dbTable);
                var bindingSource = new BindingSource { DataSource = dbTable };
                _aliPanel.SetCheckedListBoxBindingSource(bindingSource);
            }
            catch (Exception ex)
            {
                var msg = AppContext.Instance.Get<IUserMessage>();
                msg.Error("Database table binding failed", ex);
            }
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
                var dbTable = new DataTable {Locale = CultureInfo.InvariantCulture};
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

        private static string GeneratePartialQuery(string[] columns, bool isNumeric)
        {
            var sql = string.Empty;
            for (int i = 0; i <= columns.Length - 1; i++)
            {
                if (i != columns.Length - 1)
                {
                    if (isNumeric)
                    {
                        sql = sql + " ISNULL([" + columns[i].Trim() + "], 0) + SPACE(1) + ";
                    }
                    else
                    {
                        sql = sql + " ISNULL([" + columns[i].Trim() + "], '') + SPACE(1) + ";
                    }
                }
                else
                {
                    if (isNumeric)
                    {
                        sql = sql + " ISNULL([" + columns[i].Trim() + "], 0)";
                    }
                    else
                    {
                        sql = sql + " ISNULL([" + columns[i].Trim() + "], '')";
                    }
                }
            }
            return sql;
        }

        private static string ConstructEnterpolAvlVehicleListSqlQuery()
        {
            var sql = "SELECT";
            sql = sql + GeneratePartialQuery(EnterpolAvlConfig.Default.UnitLabelQuery.Split(','), false) + " AS UnitLabel, ";
            sql = sql + GeneratePartialQuery(EnterpolAvlConfig.Default.UnitIdQuery.Split(','), false) + " AS UnitId, ";
            sql = sql + GeneratePartialQuery(EnterpolAvlConfig.Default.UnitTypeQuery.Split(','), false) + " AS UnitType, ";
            sql = sql + GeneratePartialQuery(EnterpolAvlConfig.Default.UnitPositionTime.Split(','), false) + " AS UpdateTime, ";
            sql = sql + GeneratePartialQuery(EnterpolAvlConfig.Default.UnitLatitudeQuery.Split(','), true) + " AS Latitude, ";
            sql = sql + GeneratePartialQuery(EnterpolAvlConfig.Default.UnitLongitudeQuery.Split(','), true) + " AS Longitude ";
            sql = sql + "FROM [" + SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlTableName  +  "].[dbo].[" + SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlInitialCatalog + "]";
            return sql;
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
            if (!_aliServerClient.Ping())  // if the server is not responding notify the user
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

        private void HandleEnterpolIncidentsView()
        {
            // validate our connection, table, and init catalog are valid
            var msg = AppContext.Instance.Get<IUserMessage>();
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolDataSourceChanged += InstanceOnAliEnterpolIncidentsDbParamsChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolInitialCatalogChanged += InstanceOnAliEnterpolIncidentsDbParamsChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolTableNameChanged += InstanceOnAliEnterpolIncidentsDbParamsChanged;
            try
            {
                var conn = GetEnterpolIncidentsConnString();
                PopulateEnterpolIncidentsDgv(conn);
                // TODO: validate we can do this?
                // StartSqlServerDependencyWatcher();
            }
            catch (Exception ex)
            {
                msg.Error(ex.Message, ex);
            }
        }

        private void HandleEnterpolAvlView()
        {
            // validate our connection, table, and init catalog are valid
            var msg = AppContext.Instance.Get<IUserMessage>();
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolDataSourceChanged += InstanceOnAliEnterpolAvlDbParamsChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlTableNameChanged += InstanceOnAliEnterpolAvlDbParamsChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlInitialCatalogChanged += InstanceOnAliEnterpolAvlDbParamsChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlAge1FreqChanged += InstanceOnAliEnterpolAvlAgeFreqChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlAge2FreqChanged += InstanceOnAliEnterpolAvlAgeFreqChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlAge3FreqChanged += InstanceOnAliEnterpolAvlAgeFreqChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlEmsCharChanged += InstanceOnAliEnterpolAvlEmsSymbolChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlEmsColorChanged += InstanceOnAliEnterpolAvlEmsSymbolChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLeCharChanged += InstanceOnAliEnterpolAvlLeSymbolChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLeColorChanged += InstanceOnAliEnterpolAvlLeSymbolChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlFdCharChanged += InstanceOnAliEnterpolAvlFdSymbolChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlFdColorChanged += InstanceOnAliEnterpolAvlFdSymbolChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlSymbolFontChanged += InstanceOnAliEnterpolAvlSymbolFontChanged;  // update all symbols
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLabelFontChanged += InstanceOnAliEnterpolAvlLabelFontChanged;
            // check for responder application mode
            if (SdrConfig.Settings.Instance.ApplicationMode == SdrConfig.AppMode.Responder)
            {
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlSetMyLocProcChanged += InstanceOnAliEnterpolAvlSetMyLocProcChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlWhoAmIProcChanged += InstanceOnAliEnterpolAvlWhoAmIProcChanged;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlMyColorChanged += InstanceOnAliEnterpolAvlMyColorChanged;
                try
                {
                    FetchWhoAmI();
                    InitiateAvlUpdateTimer();
                }
                catch (Exception ex)
                {
                    msg.Error(ex.Message, ex);
                }
            }
            try
            {
                InitiateAvlReadTimer();
            }
            catch (Exception ex)
            {
                msg.Error(ex.Message, ex);
            }
        }

        private void InstanceOnAliEnterpolAvlLabelFontChanged(object sender, EventArgs eventArgs)
        {
                
        }

        private void InstanceOnAliEnterpolAvlSymbolFontChanged(object sender, EventArgs eventArgs)
        {
            
        }

        private void FetchWhoAmI()
        {
            using (var conn = new SqlConnection(GetEnterpolAvlConnString()))
            using (var comm = new SqlCommand(SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlWhoAmIProc, conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                conn.Open();
                _myUnitId = Convert.ToInt32(comm.ExecuteScalar());
                conn.Close();
            }
        }

        private void InstanceOnAliEnterpolAvlMyColorChanged(object sender, EventArgs eventArgs)
        {
            // TODO:
        }

        private void InitiateAvlReadTimer()
        {
            if (_avlReadIntervalTimer == null)
            {
                var interval = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlReadFreq;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlReadFreqChanged += InstanceOnAliEnterpolAvlReadFreqChanged;
                _avlReadIntervalTimer = new System.Timers.Timer(interval) { AutoReset = true };
                _avlReadIntervalTimer.Elapsed += delegate
                {
                    UpdateAvlFleetPositions();
                };
            }
            else
            {
                _avlReadIntervalTimer.Enabled = false;
                _avlReadIntervalTimer.Interval = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlReadFreq;
            }
            _avlReadIntervalTimer.Enabled = true;
        }

        private void InitiateAvlUpdateTimer()
        {
            if (_avlUpdateIntervalTimer == null)
            {
                var interval = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlUpdateFreq;
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlUpdateFreqChanged += InstanceOnAliEnterpolAvlUpdateFreqChanged;
                _avlUpdateIntervalTimer = new System.Timers.Timer(interval) { AutoReset = true };
                _avlUpdateIntervalTimer.Elapsed += delegate
                {
                    UpdateMyVehiclePosition();
                };
            }
            else
            {
                _avlUpdateIntervalTimer.Enabled = false;
                _avlUpdateIntervalTimer.Interval = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlUpdateFreq;
            }
            _avlUpdateIntervalTimer.Enabled = true;
        }

        private void UpdateMyVehiclePosition()
        {
            var item = SdrConfig.User.Go2ItUserSettings.Instance.ResponderUnitLocation;
            if (item == null || item.Count <= 0) return;

            string latStr;
            item.TryGetValue("latitude", out latStr);
            if (latStr == null) return;
            var latitude = Double.Parse(latStr);

            string lonStr;
            item.TryGetValue("longitude", out lonStr);
            if (lonStr == null) return;
            var longitude = Double.Parse(lonStr);

            string stampStr;
            item.TryGetValue("timestamp", out stampStr);
            var timestamp = DateTime.Parse(stampStr);

            using (var conn = new SqlConnection(GetEnterpolAvlConnString()))
            {
                using (var cmd = new SqlCommand(SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlSetMyLocProc, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Latitude", latitude));
                    cmd.Parameters.Add(new SqlParameter("@Longitude", longitude));
                    cmd.Parameters.Add(new SqlParameter("@PosTime", timestamp));
                    conn.Open();
                    cmd.BeginExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        private void InstanceOnAliEnterpolAvlUpdateFreqChanged(object sender, EventArgs eventArgs)
        {
            // TODO:
        }

        // overriddeen paint event for rendering avl vehicle positions
        private void MapOnPaint(object sender, PaintEventArgs e)
        {
            foreach (KeyValuePair<string, AvlVehicle> kv in _avlVehicles)
            {
                var cv = kv.Value;
                if (cv.Visible)
                {
                    DrawAvlIcon(cv, e.Graphics);
                }
            }
        }

        public void AddAvlMapPaintEvent()
        {
            if (Map == null) return;
            var map = Map as Map;
            if (map != null) map.Paint += MapOnPaint;
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseEnterpolAvl)
            {
                UpdateAvlFleetPositions();
            }
        }

        public void RemoveAvlMapPaintEvent()
        {
            if (Map == null) return;
            var map = Map as Map;
            if (map != null) map.Paint -= MapOnPaint;
        }

        private Coordinate ConvertLatLonToMap(double lat, double lon)
        {
            if (Map.Projection.Equals(KnownCoordinateSystems.Geographic.World.WGS1984)) return new Coordinate(lon, lat);

            ProjectionInfo mapProjInfo = ProjectionInfo.FromEsriString(Map.Projection.ToEsriString());
            var xy = new double[2];
            xy[0] = lon;
            xy[1] = lat;
            var z = new double[1];
            Reproject.ReprojectPoints(xy, z, _wgs84Projection, mapProjInfo, 0, 1);
            return new Coordinate(xy[0], xy[1]);
        }

        private static int SetAlphaLevel(int cInterval)
        {
            // networkfleet and enterpol share these "application" level values for alpha
            switch (cInterval)
            {
                case 1:
                    return AvlConfig.Default.AvlIntervalAlpha1;
                case 2:
                    return AvlConfig.Default.AvlIntervalAlpha2;
                case 3:
                    return AvlConfig.Default.AvlInactiveAlpha;
                default:
                    return 255;  // no transparency
            }
        }

        private static char SetUnitTypeChar(AvlVehicleType uType)
        {
            switch (uType)
            {
                case AvlVehicleType.EmergencyMedicalService:
                    return SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlEmsChar;
                case AvlVehicleType.FireDepartment:
                    return SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlFdChar;
                case AvlVehicleType.LawEnforcement:
                    return SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLeChar;
                default:  // in this case we are using networkfleet
                    return SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetChar;
            }
        }

        private Coordinate ParseMyGpsCoordinate(AvlVehicle v)
        {
            try
            {
                var gpsCoord = SdrConfig.User.Go2ItUserSettings.Instance.ResponderUnitLocation;
                string stampStr;
                gpsCoord.TryGetValue("timestamp", out stampStr);
                var timestamp = DateTime.Parse(stampStr);
                if (timestamp > v.UpdateTime) // validate the GPS position is newer than the db stored value
                {
                    string latStr;
                    gpsCoord.TryGetValue("latitude", out latStr);
                    string lonStr;
                    gpsCoord.TryGetValue("longitude", out lonStr);
                    var latitude = Double.Parse(latStr);
                    var longitude = Double.Parse(lonStr);
                    return ConvertLatLonToMap(latitude, longitude);
                }
            }
            catch
            {
                return ConvertLatLonToMap(v.Latitude, v.Longitude);
            }
            return ConvertLatLonToMap(v.Latitude, v.Longitude);
        }

        private Point GetLabelOffsetPoint(Point unitPxPos, string unitSymbol, string unitLabel)
        {
            var symFont = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlSymbolFont;
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseNetworkfleet)
            {
                symFont = SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetFont;
            }
            var symSize = TextRenderer.MeasureText(unitSymbol, symFont);  // size of symbol graphic
            var lblFont = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLabelFont;
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseNetworkfleet)
            {
                lblFont = SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetLabelFont;
            }
            var lblSize = TextRenderer.MeasureText(unitLabel, lblFont);  // size of label graphic
            var xOffset = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLabelXOffset;
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseNetworkfleet)
            {
                xOffset = SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetLabelXOffset;
            }
            var yOffset = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLabelYOffset;
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseNetworkfleet)
            {
                yOffset = SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetLabelYOffset;
            }
            var align = SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLabelAlignment;
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseNetworkfleet)
            {
                align = SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetLabelAlignment;
            }

            int startY;
            int startX;
            switch (align)
            {
                case "Left":
                    startY = (lblSize.Height/2) + yOffset;
                    startX = (symSize.Width / 2) + lblSize.Width - xOffset;
                    return new Point(unitPxPos.X - startX, unitPxPos.Y - startY);
                case "Right":
                    startY = (lblSize.Height/2) + yOffset;
                    startX = (symSize.Width / 2) + xOffset;
                    return new Point(unitPxPos.X + startX, unitPxPos.Y - startY);
                case "Below":
                    startY = (symSize.Height/2) - yOffset;
                    startX = (lblSize.Width/2) - xOffset;
                    return new Point(unitPxPos.X - startX, unitPxPos.Y + startY);
                default:  // "Above"
                    startY = (symSize.Height/2) + lblSize.Height + yOffset;
                    startX = (lblSize.Width/2)- xOffset;
                    return new Point(unitPxPos.X - startX, unitPxPos.Y - startY);
            }
        }

        private Color FetchMyVehicleColor(int alpha)
        {
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseEnterpolAvl)
            {
                return Color.FromArgb(alpha, SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlMyColor);
            }
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseNetworkfleet)
            {
                return Color.FromArgb(alpha, SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetAvlMyColor);
            }
            return Color.Fuchsia;
        }

        private Color FetchInactiveVehicleColor(int alpha)
        {
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseEnterpolAvl)
            {
                return Color.FromArgb(alpha, SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetAvlInactiveColor);
            }
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseNetworkfleet)
            {
                return Color.FromArgb(alpha, SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetAvlInactiveColor);
            }
            return Color.Gray;
        }

        private void DrawAvlIcon(AvlVehicle v, Graphics g)
        {
            Coordinate d;
            var c = new Color();
            var a = SetAlphaLevel(v.CurrentInterval);
            // TODO: we need to set the unitvalue for the networkfleet portion of the code
            if (Convert.ToInt32(v.UnitId) == _myUnitId)  // if this is 'my unit' then use myColor and more accurate GPS position (if available)
            {
                c = FetchMyVehicleColor(a);
                d = ParseMyGpsCoordinate(v);
            }
            else
            {
                switch (v.UnitType)
                {
                    case AvlVehicleType.FireDepartment:
                        c = Color.FromArgb(a, SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlFdColor);
                        break;
                    case AvlVehicleType.LawEnforcement:
                        c = Color.FromArgb(a, SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLeColor);
                        break;
                    case AvlVehicleType.EmergencyMedicalService:
                        c = Color.FromArgb(a, SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlEmsColor);
                        break;
                    default:  // this is a networkfleet icon 
                        c = Color.FromArgb(a, SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetColor);
                        break;
                }
                d = ConvertLatLonToMap(v.Latitude, v.Longitude);
            }
            if (v.CurrentInterval == 3)  // inactive unit: use inactive unit color
            {
                c = FetchInactiveVehicleColor(a);
            }
            var sp = Map.ProjToPixel(d);
            var b = new SolidBrush(c);
            var us = SetUnitTypeChar(v.UnitType).ToString(CultureInfo.InvariantCulture);
            // generate the unit graphic centered over the lat/long coordinate
            using (var strformat = new StringFormat())
            {
                strformat.LineAlignment = StringAlignment.Center;
                strformat.Alignment = StringAlignment.Center;
                if (SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseEnterpolAvl)
                {
                    g.DrawString(us, SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlSymbolFont, b, sp.X, sp.Y, strformat);
                }
                if (SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseNetworkfleet)
                {
                    g.DrawString(us, SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetFont, b, sp.X, sp.Y, strformat);
                }
            }
            // draw the label for the unit according to the chosen alignment and offset
            var lp = GetLabelOffsetPoint(sp, us, v.UnitLabel);
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseEnterpolAvl)
            {
                g.DrawString(v.UnitLabel, SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlLabelFont, b, lp.X, lp.Y);
            }
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseNetworkfleet)
            {
                g.DrawString(v.UnitLabel, SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetLabelFont, b, lp.X, lp.Y);
            }   
        }

        private void InstanceOnAliEnterpolAvlWhoAmIProcChanged(object sender, EventArgs eventArgs)
        {
                
        }

        private void InstanceOnAliEnterpolAvlSetMyLocProcChanged(object sender, EventArgs eventArgs)
        {

        }

        private void InstanceOnAliEnterpolAvlFdSymbolChanged(object sender, EventArgs eventArgs)
        {

        }

        private void InstanceOnAliEnterpolAvlLeSymbolChanged(object sender, EventArgs eventArgs)
        {

        }

        private void InstanceOnAliEnterpolAvlEmsSymbolChanged(object sender, EventArgs eventArgs)
        {

        }

        private void InstanceOnAliEnterpolAvlAgeFreqChanged(object sender, EventArgs eventArgs)
        {

        }

        private void InstanceOnAliEnterpolAvlReadFreqChanged(object sender, EventArgs eventArgs)
        {
            // InitiateAvlTimers();
        }

        private void InstanceOnAliEnterpolAvlDbParamsChanged(object sender, EventArgs eventArgs)
        {
            //try
            //{
            //    var conn = GetEnterpolAvlConnString();
            //    PopulateEnterpolAvlCheckedListBox(conn);
            //}
            //catch (Exception ex)
            //{
            //    var msg = AppContext.Instance.Get<IUserMessage>();
            //    msg.Error(ex.Message, ex);
            //}
        }

        private string GetEnterpolAvlConnString()
        {
            var connString = SqlServerHelper.GetSqlServerConnectionString(
                "Server=" + SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolDataSource + ";" +
                "Database=" + SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlTableName + ";" +
                "Integrated Security=SSPI;" +
                "connection timeout=15");

            if (!SqlServerHelper.TableExists(connString, SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlInitialCatalog))
            {
                var m = @"Table/View " + SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlInitialCatalog + " does not exist in the database " + SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlTableName;
                var ex = new ArgumentException(m, "table/view", null);
                throw ex;
            }
            return connString;
        }

        private string GetEnterpolIncidentsConnString()
        {
            var connString = SqlServerHelper.GetSqlServerConnectionString(
                "Server=" + SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolDataSource + ";" +
                "Database=" + SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolTableName + ";" +
                "Integrated Security=SSPI;" +
                "connection timeout=15");

            if (!SqlServerHelper.TableExists(connString, SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolInitialCatalog))
            {
                var m = @"Table/View " + SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolInitialCatalog + " does not exist in the database " + SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolTableName;
                var ex = new ArgumentException(m, "table/view", null);
                throw ex;
            }
            return connString;
        }

        private void InstanceOnAliEnterpolIncidentsDbParamsChanged(object sender, EventArgs eventArgs)
        {
            try
            {
                var conn = GetEnterpolIncidentsConnString();
                PopulateEnterpolIncidentsDgv(conn);
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

        private static string FormatCurrentDate()
        {
            var curDate = DateTime.Now;
            var day = curDate.Day.ToString(CultureInfo.InvariantCulture);
            if (day.Length == 1)
            {
                day = 0 + day;
            }
            var month = curDate.Month.ToString(CultureInfo.InvariantCulture);
            if (month.Length == 1)
            {
                month = 0 + month;
            }
            var year = curDate.Year.ToString(CultureInfo.InvariantCulture);
            return month + day + year;
        }

        public void PopulateGlobalCadInterface()
        {
            if (PluginSettings.Instance.ActiveGlobalCadCommLog.Length == 0)
            {
                PluginSettings.Instance.ActiveGlobalCadCommLog = Path.GetFileNameWithoutExtension(
                    SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadLogPath) + FormatCurrentDate();                  
            }
            // populate the combobox with files now
            _aliPanel.PopulateLogComboBox(FormatCurrentDate());
            // set up the event handler for the combobox index change
            _aliPanel.ComLogsComboBox.SelectedIndexChanged += GlobalCadComLogsOnSelectedIndexChanged;
            // set the selection initiating the datagridview columns
            _aliPanel.ComLogsComboBox.SelectedIndex = _aliPanel.ComLogsComboBox.FindStringExact(PluginSettings.Instance.ActiveGlobalCadCommLog);
        }

        private void OnGlobalCadArchiveChanged(object source, FileSystemEventArgs e)
        {
            _aliPanel.ComLogsComboBox.SelectedIndexChanged -= GlobalCadComLogsOnSelectedIndexChanged;
            _aliPanel.PopulateLogComboBox(FormatCurrentDate());
            _aliPanel.ComLogsComboBox.SelectedIndexChanged += GlobalCadComLogsOnSelectedIndexChanged;
        }

        private void OnGlobalCadFileChanged(object source, FileSystemEventArgs e)
        {
            var curLog = Path.GetFileNameWithoutExtension(
                SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadLogPath) + FormatCurrentDate();

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

        private void SetVehicleVisibility(AvlVehicle avlVehicle)
        {
            // get the timezone of this system and store as iana string
            TimeZone winTz = TimeZone.CurrentTimeZone;
            var locTz = WindowsToIana(winTz.StandardName);
            DateTime curTime = DateTime.Now;

            if (locTz != avlVehicle.TimeZone)
            {
                // get the windows named timezone of the vehicletimezone
                var avlTzStr = IanaToWindows(avlVehicle.TimeZone);
                var avlTz = TimeZoneInfo.FindSystemTimeZoneById(avlTzStr);
                // determine the offsets of both tz's from utc
                var avlOffset = avlTz.GetUtcOffset(avlVehicle.UpdateTime);
                var locOffset = winTz.GetUtcOffset(curTime);
                var totOffset = avlOffset - locOffset;
                curTime = curTime + totOffset;
            }
            var diff = curTime.Subtract(avlVehicle.UpdateTime);
            if (diff.TotalMinutes <= SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetAvlAge1Freq)
            {
                avlVehicle.CurrentInterval = 0;
            }
            else if (diff.TotalMinutes <= SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetAvlAge2Freq)
            {
                avlVehicle.CurrentInterval = 1;
            }
            else if (diff.TotalMinutes <= SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetAvlAge3Freq)
            {
                avlVehicle.CurrentInterval = 2;
            }
            else  // in this case we are setting the vehicle to "inactive" display state
            {
                avlVehicle.CurrentInterval = 3;  // set to inactive mode
                if (CheckAutoHideStatus())
                {
                    avlVehicle.Visible = avlVehicle.IgnoreActiveHide;
                }
            }
        }

        private void NetworkFleetClientOnPacketReceieved(object sender, AliServerDataPacket packet)
        {
            /* ------------------------------
             * VERIZON NETWORK FLEET FORMAT
            ---------------------------------
            messageTime: 2014-04-17T21:56:43Z
            satellite: false
            latitude: 36.420978
            longitude: -94.798436
            accuracy: 0.0
            keyOn: false
            parked: false
            lastSpeed: 0
            avgSpeed: 1
            maxSpeed: 4
            heading:
            vehicleId: 371861
            ------------------------------ */

            if (packet.Command != Command.Message) return;
            if (packet.Name != "SDR_NetworkFleet_Server") return;

            var msgArr = packet.Message.Split(',');
            var msgTime = string.Empty;
            var msgLat = string.Empty;
            var msgLon = string.Empty;
            var msgVid = string.Empty;
            foreach (var msg in msgArr)
            {
                var m = msg.Split('|');
                switch (m[0])
                {
                    case "messageTime":
                        msgTime = m[1];
                        break;
                    case "latitude":
                        msgLat = m[1];
                        break;
                    case "longitude":
                        msgLon = m[1];
                        break;
                    case "vehicleId":
                        msgVid = m[1];
                        break;
                }
            }

            _aliPanel.VehicleFleetListBox.ItemCheck -= VehicleFleetListBoxOnItemCheck;  // disable item-check event handling temporarily
            AvlVehicle avlVehicle;
            _avlVehicles.TryGetValue(msgVid, out avlVehicle);
            if (avlVehicle == null)
            {
                // temp assign vehicle id as id and label
                avlVehicle = new AvlVehicle
                {
                    UnitId = msgVid,
                    UnitLabel = msgVid,
                    Latitude = Convert.ToDouble(msgLat),
                    Longitude = Convert.ToDouble(msgLon),
                    UpdateTime = DateTime.Parse(msgTime),
                    TimeZone = TimeZoneLookup.GetTimeZone(Convert.ToDouble(msgLat), Convert.ToDouble(msgLon)).Result
                };
                // search through available lookups to find the label
                if (SdrConfig.Project.Go2ItProjectSettings.Instance.NetworkfleetLabels.Count > 0)
                {
                    foreach (var nfLabel in SdrConfig.Project.Go2ItProjectSettings.Instance.NetworkfleetLabels)
                    {
                        var arr = nfLabel.Split('=');
                        // found the matched vehicle id for the label
                        if (msgVid == arr[0].ToString(CultureInfo.InvariantCulture))
                        {
                            avlVehicle.UnitLabel = arr[1].ToString(CultureInfo.InvariantCulture);
                        }
                    }
                }
                // determine the visibility of the vehicle
                SetVehicleVisibility(avlVehicle);
                _avlVehicles.Add(avlVehicle.UnitId, avlVehicle);
            }
            else
            {
                if (avlVehicle.Latitude.Equals(Convert.ToDouble(msgLat)) &&
                    avlVehicle.Longitude.Equals(Convert.ToDouble(msgLon)))
                {
                    SetVehicleVisibility(avlVehicle);
                }
                else  // lat/long have changed, so update the time and position and reset display UpdateInterval to shortest color value
                {
                    avlVehicle.UpdateTime = DateTime.Parse(msgTime);
                    avlVehicle.Latitude = Convert.ToDouble(msgLat);
                    avlVehicle.Longitude = Convert.ToDouble(msgLon);
                    avlVehicle.CurrentInterval = 0;
                    avlVehicle.IgnoreActiveHide = false;
                }
            }
            BindCheckedListBoxToAvlList();
            var idx = _aliPanel.VehicleFleetListBox.Items.IndexOf(avlVehicle);
            _aliPanel.SetAvlVehicleCheckState(idx, avlVehicle.Visible);
            _aliPanel.VehicleFleetListBox.ItemCheck += VehicleFleetListBoxOnItemCheck;
            if (Map != null)
            {
                Map.Invalidate();  // paint the map with updated avl positions (calls MapOnPaint)
            }
        }

        public string WindowsToIana(string windowsZoneId)
        {
            if (windowsZoneId.Equals("UTC", StringComparison.Ordinal))
                return "Etc/UTC";

            var tzdbSource = NodaTime.TimeZones.TzdbDateTimeZoneSource.Default;
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(windowsZoneId);
            if (tzi == null) return null;
            var tzid = tzdbSource.MapTimeZoneId(tzi);
            if (tzid == null) return null;
            return tzdbSource.CanonicalIdMap[tzid];
        }

        // return the windows zone that matches the IANA zone, if one exists.
        public string IanaToWindows(string ianaZoneId)
        {
            var utcZones = new[] { "Etc/UTC", "Etc/UCT", "Etc/GMT" };
            if (utcZones.Contains(ianaZoneId, StringComparer.Ordinal))
                return "UTC";

            var tzdbSource = NodaTime.TimeZones.TzdbDateTimeZoneSource.Default;

            // resolve any link, since the CLDR doesn't necessarily use canonical IDs
            var links = tzdbSource.CanonicalIdMap
                .Where(x => x.Value.Equals(ianaZoneId, StringComparison.Ordinal))
                .Select(x => x.Key);

            // resolve canonical zones, and include original zone as well
            var possibleZones = tzdbSource.CanonicalIdMap.ContainsKey(ianaZoneId)
                ? links.Concat(new[] { tzdbSource.CanonicalIdMap[ianaZoneId], ianaZoneId })
                : links;

            // map the windows zone
            var mappings = tzdbSource.WindowsMapping.MapZones;
            var item = mappings.FirstOrDefault(x => x.TzdbIds.Any(possibleZones.Contains));
            if (item == null) return null;
            return item.WindowsId;
        }

        private void UpdateItemStateAndListBox()
        {
            _aliPanel.VehicleFleetListBox.ItemCheck -= VehicleFleetListBoxOnItemCheck;  // disable item-check event handling temporarily
            var removeList = _avlVehicles.Keys.ToList();  // on method completion, remove any vehicles that remain on this list from drawing dictionary
            for (var i = 0; i <= _aliPanel.VehicleFleetListBox.Items.Count - 1; i++)
            {
                var item = _aliPanel.VehicleFleetListBox.Items[i] as DataRowView;
                if (item == null) continue;
                AvlVehicle avlVehicle;  // attempt to get the unit by the unit id
                _avlVehicles.TryGetValue(item[1].ToString(), out avlVehicle);
                if (avlVehicle == null)
                {
                    avlVehicle = CreateNewAvlVehicle(item);
                    _avlVehicles.Add(avlVehicle.UnitId, avlVehicle);
                }
                removeList.Remove(avlVehicle.UnitId);
                // check lat/long: if no movement then adjust UpdateInterval to assign proper color for avl age on map paint
                if (avlVehicle.Latitude.Equals(Convert.ToDouble(item[4].ToString())) && avlVehicle.Longitude.Equals(Convert.ToDouble(item[5].ToString())))
                {
                    var diff = DateTime.Now.Subtract(avlVehicle.UpdateTime);
                    if (diff.TotalMinutes <= SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlAge1Freq)
                    {
                        avlVehicle.CurrentInterval = 0;
                    }
                    else if (diff.TotalMinutes <= SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlAge2Freq)
                    {
                        avlVehicle.CurrentInterval = 1;
                    }
                    else if (diff.TotalMinutes <= SdrConfig.Project.Go2ItProjectSettings.Instance.AliEnterpolAvlAge3Freq)
                    {
                        avlVehicle.CurrentInterval = 2;
                    }
                    else  // in this case we are setting the vehicle to "inactive" display state
                    {
                        avlVehicle.CurrentInterval = 3;  // set to inactive mode
                        if (CheckAutoHideStatus())
                        {
                            avlVehicle.Visible = avlVehicle.IgnoreActiveHide;
                        }
                    }
                }
                else  // lat/long have changed, so update the time and position and reset display UpdateInterval to shortest color value
                {
                    avlVehicle.UpdateTime = DateTime.Parse(item[3].ToString());
                    avlVehicle.Latitude = Convert.ToDouble(item[4].ToString());
                    avlVehicle.Longitude = Convert.ToDouble(item[5].ToString());
                    avlVehicle.CurrentInterval = 0;
                    avlVehicle.IgnoreActiveHide = false;
                }
                avlVehicle.UnitLabel = item[0].ToString();
                avlVehicle.UnitType = SetAvlVehicleType(item[2].ToString());
                _aliPanel.SetAvlVehicleCheckState(i, avlVehicle.Visible);
            }
            if (removeList.Count > 0)  // anything that remains on the list is no longer part of the query and should be removed
            {
                foreach (string item in removeList)
                {
                    _avlVehicles.Remove(item);
                }
            }
            _aliPanel.VehicleFleetListBox.ItemCheck += VehicleFleetListBoxOnItemCheck;
        }

        private void HandleNetworkFleetClient()
        {
            var msg = AppContext.Instance.Get<IUserMessage>();


            //_aliPanel.MyUnitComboBox.SelectedIndexChanged += delegate(object sender, EventArgs args)
            //{
            // TODO: handle my unit changed
            //    Debug.WriteLine("changed");
            //};

            // TODO:
            // SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpHostChanged += InstanceOnAliSdrServerUdpHostChanged;
            // SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpPortChanged += InstanceOnAliSdrServerUdpPortChanged;

            _networkFleetClient = new AliServerClient(SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetUdpHost, SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetUdpPort);
            if (!_networkFleetClient.Ping())  // if the server is not responding notify the user
            {
                msg.Warn(@"NetworkFleet is not responding at host: " + SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetUdpHost + @" port: " + SdrConfig.Project.Go2ItProjectSettings.Instance.AliNetworkfleetUdpPort);
            }
            try
            {
                if (_avlVehicles == null) // check for an existing avl dictionary
                {
                    _avlVehicles = new Dictionary<string, AvlVehicle>();
                    _aliPanel.VehicleFleetListBox.ItemCheck += VehicleFleetListBoxOnItemCheck;
                }
                _networkFleetClient.PacketReceieved += NetworkFleetClientOnPacketReceieved;
                _networkFleetClient.Login();  // on login we will receive a full vehicle list from the client
            }
            catch (Exception ex)
            {
                msg.Error(ex.Message, ex);
            }
        }

        private void BindCheckedListBoxToAvlList()
        {
            try
            {
                int avlRowSelected = 0;
                if (_aliPanel.VehicleFleetListBox.Items.Count >= 0)
                {
                    avlRowSelected = _aliPanel.GetSelectedAvlVehicleRow();
                }
                var avlBindList = _avlVehicles.Values.ToList();
                var bindingSource = new BindingSource { DataSource = avlBindList };
                _aliPanel.SetCheckedListBoxBindingSource(bindingSource);
                // now populate the my_unit combobox and set selection if available
                var myUnitsList = new List<string>();
                foreach (AvlVehicle v in _aliPanel.VehicleFleetListBox.Items)
                {
                    myUnitsList.Add(v.UnitLabel);
                }
                _aliPanel.PopulateMyUnitComboBox(myUnitsList.ToArray());
                // make to set the selected avl vehicle if present
                if (_aliPanel.VehicleFleetListBox.Items.Count >= avlRowSelected)
                {
                    _aliPanel.SelectAvlVehicleRow(avlRowSelected);
                }
            }
            catch (Exception ex)
            {
                var msg = AppContext.Instance.Get<IUserMessage>();
                msg.Error("AVL list table binding failed", ex);
            }
        }

        private void UpdateAvlFleetPositions()
        {
            int avlRowSelected = 0;
            if (_avlVehicles == null) // check for out avlVehicle dict (?first run through)
            {
                _avlVehicles = new Dictionary<string, AvlVehicle>();
                _aliPanel.VehicleFleetListBox.ItemCheck += VehicleFleetListBoxOnItemCheck;
            }
            else
            {
                // get the currently selected row in the listbox (for restoring the selection after rebinding)
                if (_aliPanel.VehicleFleetListBox.Items.Count >= 0)
                {
                    avlRowSelected = _aliPanel.GetSelectedAvlVehicleRow();
                }
            }

            // check for enterpol server binding if running
            if (SdrConfig.Project.Go2ItProjectSettings.Instance.AliUseEnterpolAvl)
            {
                var conn = GetEnterpolAvlConnString();
                var sql = ConstructEnterpolAvlVehicleListSqlQuery();
                BindCheckedListBoxToSqlServer(conn, sql);
            }

            UpdateItemStateAndListBox();

            if (_aliPanel.VehicleFleetListBox.Items.Count >= avlRowSelected)
            {
                _aliPanel.SelectAvlVehicleRow(avlRowSelected);
            }
            if (Map != null)
            {
                Map.Invalidate();  // paint the map with updated avl positions (calls MapOnPaint)
            }
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

        private void ConfigureAliPanelInterface()
        {
            if (_currentAliAvl == AliAvl.Networkfleet)
            {
                switch (_currentAliMode)
                {
                    case AliMode.Enterpol:
                    case AliMode.Sdraliserver:
                        if (SdrConfig.Settings.Instance.ApplicationMode == SdrConfig.AppMode.Responder)
                        {
                            _aliPanel.DisplayAvlListInterfaceWithMyUnit("Networkfleet Vehicles", "My Unit", true);
                            break;
                        }
                        _aliPanel.DisplayAvlListInterface("Networkfleet Vehicles", true);
                        break;
                    case AliMode.Globalcad:
                        if (SdrConfig.Settings.Instance.ApplicationMode == SdrConfig.AppMode.Responder)
                        {
                            _aliPanel.DisplayAvlListWithMyUnitAndCommLogInterface("Networkfleet Vehicles", "My Unit", "Active Comm Log");
                            break;
                        }
                        _aliPanel.DisplayAvlListAndCommLogInterface("Networkfleet Vehicles", "Active Comm Log");
                        break;
                    default: // No active ALI mode
                        if (SdrConfig.Settings.Instance.ApplicationMode == SdrConfig.AppMode.Responder)
                        {
                            _aliPanel.DisplayAvlListInterfaceWithMyUnit("Networkfleet Vehicles", "My Unit", false);
                            break;
                        }
                        _aliPanel.DisplayAvlListInterface("Networkfleet Vehicles", false);
                        break;
                }
            }
            else
            {
                switch (_currentAliMode)
                {
                    case AliMode.Enterpol:
                        if (_currentAliAvl == AliAvl.Enterpolavl) // enterpol avl can only be used enterpol interface
                        {
                            _aliPanel.DisplayAvlListInterface("Enterpol AVL Vehicles", true);
                        }
                        else
                        {
                            _aliPanel.DisplayStandardInterface();
                        }
                        break;
                    case AliMode.Globalcad:
                        _aliPanel.DisplayCommLogInterface("Active Comm Log");
                        break;
                    default:  // No active AVL interface
                        _aliPanel.DisplayStandardInterface();
                        break;
                }
            }
        }

        private void InstanceOnAliGlobalCadLogPathChanged(object sender, EventArgs eventArgs)
        {
            if (_currentAliMode != AliMode.Globalcad) return;
            _globalCadLogWatch.EnableRaisingEvents = false;
            _globalCadLogWatch.Path = Path.GetDirectoryName(SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadLogPath);
            _globalCadLogWatch.EnableRaisingEvents = true;
        }

        private void InstanceOnAliGlobalCadArchivePathChanged(object sender, EventArgs eventArgs)
        {
            if (_currentAliMode != AliMode.Globalcad) return;
            _globalCadArkWatch.EnableRaisingEvents = false;
            _globalCadArkWatch.Path = SdrConfig.Project.Go2ItProjectSettings.Instance.AliGlobalCadArchivePath;
            _globalCadArkWatch.EnableRaisingEvents = true;
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

        ///// <summary>
        ///// Allows for new behavior during deactivation.
        ///// </summary>
        //protected override void OnDeactivate()
        //{
        //    base.OnDeactivate();
        //}

        ///// <summary>
        ///// Occurs when this function is removed.
        ///// </summary>
        //protected override void OnUnload()
        //{
        //    base.OnUnload();
        //}
        #endregion
    }
}