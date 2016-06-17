using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using DotSpatial.SDR.Controls;
using SdrConfig = SDR.Configuration;

namespace Go2It
{
    /// <summary>
    /// Create a SplitContainer that hosts a TabDockingControl
    /// </summary>
    public class DockingControl : TabDockingControl, IPartImportsSatisfiedNotification
    {
        [Import("Shell", typeof (ContainerControl))]
        private ContainerControl Shell { get; set; }

        private SplitContainer _toolSplitContainer;

        public void CollapseToolPanel()
        {
            _toolSplitContainer.Panel1Collapsed = true;
        }

        public void ExtendToolPanel(int height)
        {
            if (!_toolSplitContainer.Panel1Collapsed) return;

            _toolSplitContainer.Panel1Collapsed = false;
            if (height > 0)
            {
                _toolSplitContainer.SplitterDistance = height;
            }
        }

        #region IPartImportsSatisfiedNotification

        public void OnImportsSatisfied()
        {
            _toolSplitContainer = new SplitContainer
            {
                Name = "innerContainer",
                Orientation = Orientation.Horizontal,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                SplitterWidth = 10,
                FixedPanel = FixedPanel.Panel1,
                Panel1MinSize = 0,
                Panel1Collapsed = true,
                TabStop = false
            };

            // grab or create the split container as needed
            var container = (SplitContainer) Shell.Controls.Find("outerContainer", false).FirstOrDefault();
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
                    TabStop = false,
                };
                Shell.Controls.Add(container);
            }
            container.Panel2.Controls.Add(_toolSplitContainer);
            Initialize(_toolSplitContainer);
        }

        #endregion
    }
}
