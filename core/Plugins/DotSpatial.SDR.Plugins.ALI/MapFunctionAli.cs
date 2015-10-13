using System;
using DotSpatial.Controls;
using SDR.Network;
using SdrConfig = SDR.Configuration;

namespace DotSpatial.SDR.Plugins.ALI
{
    /// <summary>
    /// A MapFunction that connects with ALI Interfaces
    /// </summary>
    public class MapFunctionAli : MapFunction
    {
        private AliPanel _aliPanel;
        private AliServerClient _aliServerClient;

        #region Constructors

        /// <summary>
        /// Creates a new instance of MapFunction, with panel
        /// </summary>
        /// <param name="ap"></param>
        public MapFunctionAli(AliPanel ap)
        {
            _aliPanel = ap;

            //if (SdrConfig.Project.Go2ItProjectSettings.Instance.AliMode == "SDR AliServer")
            //{
            //    _aliServerClient = new AliServerClient(
            //        SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpHost,
            //        SdrConfig.Project.Go2ItProjectSettings.Instance.AliSdrServerUdpPort);

            //}
            Configure();
        }

        private void Configure()
        {
            Name = "MapFunctionAli";
            YieldStyle = YieldStyles.AlwaysOn;
            
            // TODO: setup for all ali interface events and configurations
            // HandleDetectionEvents();
            // HandleNmeaEvents();
            // HandleGpsPanelEvents();
            // ConfigureActivateGps();
        }

        private AliMode GetAliMode()
        {
            var aliMode = SdrConfig.Project.Go2ItProjectSettings.Instance.AliMode;
            if (aliMode.Length <= 0) return AliMode.Disabled;
            AliMode am;
            Enum.TryParse(aliMode, true, out am);
            return am;
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Allows for new behavior during activation
        /// </summary>
        protected override void OnActivate()
        {
            if (_aliPanel == null || _aliPanel.IsDisposed)
            {
                _aliPanel = new AliPanel();
                // TODO: setup for all ali interface events and configurations
                // HandleDetectionEvents();
                // HandleNmeaEvents();
                // HandleGpsPanelEvents();
                // ConfigureActivateGps();
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