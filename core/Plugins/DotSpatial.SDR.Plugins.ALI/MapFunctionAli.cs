using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.SDR.Plugins.ALI.Properties;
using SDR.Data.Database;
using SDR.Network;
using SdrConfig = SDR.Configuration;

namespace DotSpatial.SDR.Plugins.ALI
{
    /// <summary>
    /// A MapFunction that connects with ALI Interfaces
    /// </summary>
    public class MapFunctionAli : MapFunction
    {
        private string _aliServerDbConnString = string.Empty;

        private AliPanel _aliPanel;
        private readonly DataGridView _dataGridView;
        private BindingSource _bindingSource;
        private AliMode _currentAliMode = AliMode.Disabled;

        // all network communication clients that may be used for ali interface 
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
            _dataGridView = ap.DataGridDisplay;
            _bindingSource = new BindingSource();

            HandleAliPanelEvents();
        }
        #endregion

        #region Methods

        private void HandleAliPanelEvents()
        {
            // two button clicks 
            // a double click
            // also possible pull down menu 
        }

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
            // TODO: handle all other clients
            // set the new current mode
        }

        private void HandleAliServerClient()
        {
            _aliServerClient = new AliServerClient(SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpHost, SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpPort);
            _aliServerClient.PacketReceieved += AliServerClientOnPacketReceieved;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerDbPathChanged += InstanceOnAliSdrServerDbPathChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpHostChanged += InstanceOnAliSdrServerUdpHostChanged;
            SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpPortChanged += InstanceOnAliSdrServerUdpPortChanged;

            _aliServerDbConnString = MdbHelper.GetMdbConnectionString(SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerDbPath);
            // TODO we may need to run the rebind process on db change? trap for bad db connection as well

            
            
            string sql = "SELECT * FROM " + SdrAliServerConfig.Default.TableName;



            BindDataGridView(_aliServerDbConnString, sql);

            _aliPanel.ShowStandardInterface();
        }

        private void BindDataGridView(string connString, string sqlString)
        {
            // bind our db to the dgv now
            var cnn = new OleDbConnection(connString);
            cnn.Open();
            var dbTable = new DataTable();
            var dbAdapter = new OleDbDataAdapter(sqlString, cnn);
            dbAdapter.Fill(dbTable);
            _bindingSource.DataSource = dbTable;
            _dataGridView.DataSource = _bindingSource;
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
            // if we can do binding then here we simply refresh??
            _dataGridView.ResetBindings();
        }

        // this is called on startup as well as everytime the ali mode changes
        public void ConfigureAliClient(AliMode am)
        {
            if (_currentAliMode == am) return;  // no need to update anything no change was made
            DisableCurrentAliMode();
            switch (am)
            {
                case AliMode.Sdraliserver:
                    HandleAliServerClient();
                    break;
                case AliMode.Globalcad:
                    _aliPanel.ShowGlobalCadInterface();
                    break;
                case AliMode.Enterpol:
                    _aliPanel.ShowStandardInterface();
                    break;
            }
            _currentAliMode = am;
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