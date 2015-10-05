using DotSpatial.Controls;

namespace DotSpatial.SDR.Plugins.AliServer
{
    /// <summary>
    /// A MapFunction that connects with AliServer feed
    /// </summary>
    public class MapFunctionAliServer : MapFunction
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of MapFunction, with panel
        /// </summary>
        /// <param name="mp"></param>
        public MapFunctionAliServer()
        {
            Configure();
        }

        private void Configure()
        {

        }
        #endregion

        #region Methods

        /// <summary>
        ///  Allows for new behavior during activation
        /// </summary>
        protected override void OnActivate()
        {
            base.OnActivate();
        }

        /// <summary>
        /// Allows for new behavior during deactivation.
        /// </summary>
        protected override void OnDeactivate()
        {

        }

        /// <summary>
        /// Occurs when this function is removed.
        /// </summary>
        protected override void OnUnload()
        {

        }

        #endregion
    }
}