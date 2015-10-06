using DotSpatial.Controls;

namespace DotSpatial.SDR.Plugins.ALI
{
    /// <summary>
    /// A MapFunction that connects with ALI Interfaces
    /// </summary>
    public class MapFunctionAli : MapFunction
    {
        private AliPanel _aliPanel;

        #region Constructors

        /// <summary>
        /// Creates a new instance of MapFunction, with panel
        /// </summary>
        /// <param name="mp"></param>
        public MapFunctionAli(AliPanel ap)
        {
            _aliPanel = ap;
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