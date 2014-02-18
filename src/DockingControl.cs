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

        #region IPartImportsSatisfiedNotification

        public void OnImportsSatisfied()
        {
            var innerContainer = new SplitContainer
            {
                Name = "innerContainer",
                Orientation = Orientation.Horizontal,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                SplitterWidth = 10,
                FixedPanel = FixedPanel.Panel1,
                Panel1MinSize = 0
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
                    Panel1MinSize = 0
                };
                Shell.Controls.Add(container);
            }
            container.Panel2.Controls.Add(innerContainer);
            Initialize(innerContainer);
        }
        #endregion
    }
}
