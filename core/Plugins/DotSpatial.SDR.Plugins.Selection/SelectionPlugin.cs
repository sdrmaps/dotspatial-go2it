using System;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.SDR.Plugins.Selection.Properties;
using GeoAPI.Geometries;

namespace DotSpatial.SDR.Plugins.Selection
{
    public class SelectionPlugin : Extension
    {
        #region Constants and Fields

        private const string HomeMenuKey = HeaderControl.HomeRootItemKey;

        #endregion

        #region Public Methods

        public override void Activate()
        {
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "Select", SelectionTool_Click)
            {
                GroupCaption = "Selection_Select",
                ToolTipText = "Select Features",
                SmallImage = Resources.select_16,
                LargeImage = Resources.select_32
            });
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "Deselect", DeselectAllTool_Click)
            {
                GroupCaption = "Selection_Deselect",
                ToolTipText = "Deselect all Features",
                SmallImage = Resources.unselect_16,
                LargeImage = Resources.unselect_32
            });
            App.HeaderControl.Add(new SimpleActionItem(HomeMenuKey, "Identify", IdentifierTool_Click)
            {
                GroupCaption = "Selection_Identify",
                ToolTipText = "Identify Features",
                SmallImage = Resources.identify_16,
                LargeImage = Resources.identify_32
            });
            base.Activate();
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            base.Deactivate();
        }

        #endregion

        /// <summary>
        /// Identifier Tool
        /// </summary>
        private void IdentifierTool_Click(object sender, EventArgs e)
        {
            App.Map.FunctionMode = FunctionMode.Info;
        }

        /// <summary>
        /// Select or deselect Features
        /// </summary>
        private void SelectionTool_Click(object sender, EventArgs e)
        {
            App.Map.FunctionMode = FunctionMode.Select;
        }

        /// <summary>
        /// Deselect all features in all layers
        /// </summary>
        private void DeselectAllTool_Click(object sender, EventArgs e)
        {
            Envelope env;
            App.Map.MapFrame.ClearSelection(out env);
        }
    }
}