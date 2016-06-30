using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows.Forms;
using DotSpatial.Controls.Header;
using DotSpatial.SDR.Controls;

namespace Go2It
{
    /// <summary>
    /// Replaces the built in StatusControl of DotSpatial, allows us to customize as needed
    /// </summary>
    public class StatusControl : IStatusControl, IPartImportsSatisfiedNotification
    {
        private StatusPanel _defaultStatusPanel;
        private StatusStrip _statusStrip;

        [Import("Shell", typeof(ContainerControl))]
        private ContainerControl Shell { get; set; }

        #region IPartImportsSatisfiedNotification Members

        public void OnImportsSatisfied()
        {
            _statusStrip = new StatusStrip
            {
                Name = "defaultStatusStrip",
                TabIndex = 0,
                Text = String.Empty,
                TabStop = false
            };
            // adding the status strip control
            Shell.Controls.Add(_statusStrip);
            // adding one initial status panel to the status strip control
            // displays application level msgs, such as project opening progress
            _defaultStatusPanel = new StatusPanel();
            Add(_defaultStatusPanel);
        }

        #endregion

        #region IStatusControl Members

        public void Progress(string key, int percent, string message)
        {
            _defaultStatusPanel.Caption = percent == 0 ? message : String.Format("{0}... {1}%", message, percent);

            if (!_statusStrip.InvokeRequired)
            {
                // most actions happen on one thread, as such the status bar never repaints itself until the end of a process
                // to fix this we call Application.DoEvents() or refresh the control.
                _statusStrip.Refresh();
            }
        }

        /// <summary>
        /// Adds a status panel to the status strip
        /// </summary>
        /// <param name="panel">the user-specified status panel</param>
        public void Add(StatusPanel panel)
        {
            var statusLabel = new ToolStripStatusLabel
            {
                Name = panel.Key,
                Text = panel.Caption,
                Width = panel.Width,
                Spring = (panel.Width == 0),
                TextAlign = ContentAlignment.MiddleLeft
            };
            panel.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                var item = sender as StatusPanel;

                if (item == null) return;
                statusLabel.Text = item.Caption;
                statusLabel.Width = item.Width;
            };
            _statusStrip.Items.Add(statusLabel);
        }


        public void Remove(StatusPanel panel)
        {
            _statusStrip.Items.RemoveByKey(panel.Key);
        }

        public void Add(DropDownStatusPanel panel)
        {
            throw new NotImplementedException();
        }

        public void Remove(DropDownStatusPanel panel)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}