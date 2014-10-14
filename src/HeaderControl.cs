using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows.Forms;
using DotSpatial.Controls.Header;
using DotSpatial.SDR.Controls;
using SdrConfig = SDR.Configuration;
using System.Linq;

namespace Go2It
{
    /// <summary>
    /// Creates a TableLayoutPanel that hosts a GridHeaderControl.
    /// </summary>
    [Export(typeof(IHeaderControl))]
    public class HeaderControl : GridHeaderControl, IPartImportsSatisfiedNotification
    {
        [Import("Shell", typeof(ContainerControl))]
        private ContainerControl Shell { get; set; }

        #region IPartImportsSatisfiedNotification

        public void OnImportsSatisfied()
        {
            // table layout panel for tool buttons
            var buttonPanel = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = SdrConfig.User.Go2ItUserSettings.Instance.GridHeaderColumnCount
            };
            for (int i = 0; i < SdrConfig.User.Go2ItUserSettings.Instance.GridHeaderColumnCount; i++ )
            {
                buttonPanel.ColumnStyles.Add(new ColumnStyle());
            }
            buttonPanel.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
            buttonPanel.Dock = DockStyle.Left;
            buttonPanel.Name = "buttonPanel";
            buttonPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize, 100F));
            // grab or create the split container as needed
            var container = (SplitContainer)Shell.Controls.Find("outerContainer", false).FirstOrDefault();
            if (container == null)
            {
                container = new SplitContainer
                {
                    FixedPanel = FixedPanel.Panel1,
                    BorderStyle = BorderStyle.FixedSingle,
                    Orientation = Orientation.Vertical,
                    Name = "outerContainer",
                    BackColor = Color.Transparent,
                    Dock = DockStyle.Fill,
                    SplitterWidth = 10,
                    Panel1MinSize = 0,
                };
                Shell.Controls.Add(container);
            }
            container.Panel1.Controls.Add(buttonPanel);
            Initialize(buttonPanel);
        }
        #endregion
    }
}