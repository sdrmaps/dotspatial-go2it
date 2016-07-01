using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using DotSpatial.Controls.Header;
using DotSpatial.SDR.Controls;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace Go2It
{
    /// <summary>
    /// Replaces the built in StatusControl of DotSpatial, allows us to customize as needed
    /// </summary>
    public class StatusControl : IStatusControl, IPartImportsSatisfiedNotification
    {
        private StatusPanel _defaultStatusPanel;  // default label status panel to display progress and state info
        private StatusStrip _statusStrip;  // holds the default statuspanel and any/all user additions

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
            // add the status strip control
            Shell.Controls.Add(_statusStrip);
            // displays application level msgs: project opening, progress, etc.
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
        /// <param name="sPanel">the user-specified status panel</param>
        public void Add(StatusPanel sPanel)
        {
            // attempt to cast to a specific StatusPanel type
            var cbPanel = sPanel as ComboBoxStatusPanel;
            if (cbPanel != null)
            {
                AddStatusComboBox(cbPanel);
                return;
            }
            var pbPanel = sPanel as ProgressBarStatusPanel;
            if (pbPanel != null)
            {
                AddStatusLabel(pbPanel);
                return;
            }
            // a basic StatusPanel: add a label and move along...
            AddStatusLabel(sPanel);
        }

        public void Remove(StatusPanel panel)
        {
            _statusStrip.Items.RemoveByKey(panel.Key);
        }

        private void AddStatusComboBox(ComboBoxStatusPanel panel)
        {
            var statusComboBox = new ToolStripComboBox();
            statusComboBox.Items.AddRange(panel.Items);
            statusComboBox.SelectedItem = panel.SelectedItem;
            statusComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            statusComboBox.SelectedIndexChanged += delegate(object sender, EventArgs args)
            {
                var cmbBox = sender as ToolStripComboBox;
                if (cmbBox == null) return;
                // set the panel.SelectedItem to the value of the cmbBox.SelectedItem
                panel.SelectedItem = cmbBox.SelectedItem;
            };

            panel.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                var item = sender as ComboBoxStatusPanel;
                if (item == null) return;

                switch (e.PropertyName)
                {
                    case "Width":
                        statusComboBox.Width = item.Width;
                        break;
                    case "SelectedItem":
                        statusComboBox.SelectedItem = item.SelectedItem;
                        break;
                    case "Items":
                        statusComboBox.Items.Clear();
                        statusComboBox.Items.AddRange(item.Items);
                        break;
                }
            };
            _statusStrip.Items.Add(statusComboBox);
        }

        private void AddStatusLabel(StatusPanel panel)
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

        #endregion
    }
}