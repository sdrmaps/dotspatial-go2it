using System;
using System.Windows.Forms;
using DotSpatial.SDR.Plugins.Measure.Properties;

namespace DotSpatial.SDR.Plugins.Measure
{
    public partial class MeasurePanel : UserControl
    {
        private double _areaIntoSquareMeters;
        private double _distIntoMeters;
        private double _distance;
        private readonly MeasureMode _measureMode;
        private double _totalArea;
        private double _totalDistance;

        #region Private Variables

        private readonly double[] _areaUnitFactors =
        {
            1E-6, 0.0001, 1, .01, 3.86102159E-7, 0.000247105381, 10.7639104,
            1.19599005
        };

        private readonly object[] _areaUnitNames =
        {
            "Square Kilometers", "Hectares", "Square Meters", "Ares", "Square Miles",
            "Acres", "Square Feet", "Square Yards" };

        private readonly double[] _distanceUnitFactors =
        {
            .001, 1, 10, 100, 1000,
            0.000621371192, 0.000539956803, 1.0936133, 3.2808399, 39.3700787, 8.983152098E-6
        };

        private readonly object[] _distanceUnitNames =
        {
            "Kilometers", "Meters", "Decimeters", "Centimeters", "Millimeters",
            "Miles", "NauticalMiles", "Yards", "Feet", "Inches", "DecimalDegrees"
        };
        #endregion

        #region Constructors

        public MeasurePanel()
        {
            InitializeComponent();
            // get the measure mode set in user settings or default
            _measureMode = MeasureMode;
            if (_measureMode == MeasureMode.Distance)
            {
                cmbUnits.Items.AddRange(_distanceUnitNames);
                cmbUnits.SelectedIndex = DistanceUnitIndex;
                Text = @"Measure Distance";
                lblMeasure.Text = @"Distance";
            }
            else if (_measureMode == MeasureMode.Area)
            {
                cmbUnits.Items.AddRange(_areaUnitNames);
                cmbUnits.SelectedIndex = AreaUnitIndex;
                Text = @"Measure Area";
                lblMeasure.Text = @"Area";
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the distance in meters of just one segment
        /// </summary>
        public double Distance
        {
            get { return _distance; }
            set
            {
                _distance = value;
                if (_measureMode == MeasureMode.Distance)
                {
                    lblPartialValue.Text = (_distance * _distIntoMeters).ToString("#, ###");
                }
            }
        }

        /// <summary>
        /// The total distance across all segments in meters
        /// </summary>
        public double TotalDistance
        {
            get { return _totalDistance; }
            set
            {
                _totalDistance = value;
                if (_measureMode == MeasureMode.Distance)
                {
                    lblTotalValue.Text = (_totalDistance * _distIntoMeters).ToString("#, ###");
                }
            }
        }

        /// <summary>
        /// Gets or sets the total area in square meters
        /// </summary>
        public double TotalArea
        {
            get { return _totalArea; }
            set
            {
                _totalArea = value;
                lblTotalValue.Text = (_totalArea * _areaIntoSquareMeters).ToString("#, ###");
            }
        }

        public void ClearMeasurements()
        {
            if (MeasurementsCleared != null) MeasurementsCleared(this, EventArgs.Empty);
        }

        #endregion

        /// <summary>
        /// Gets or sets whether to display the distances or areas.
        /// </summary>
        public MeasureMode MeasureMode
        {
            get
            {
                var funcMode = UserSettings.Default.MeasureMode;
                if (funcMode.Length <= 0) return MeasureMode.Distance;
                MeasureMode mm;
                Enum.TryParse(funcMode, true, out mm);
                return mm;
            }
            set
            {
                UserSettings.Default.MeasureMode = value.ToString();
                UserSettings.Default.Save();
            }
        }

        public int AreaUnitIndex
        {
            get { return UserSettings.Default.AreaUnitIndex; }
            set
            {
                UserSettings.Default.AreaUnitIndex = value;
                UserSettings.Default.Save();
            }
        }

        public int DistanceUnitIndex
        {
            get { return UserSettings.Default.DistanceUnitIndex; }
            set
            {
                UserSettings.Default.DistanceUnitIndex = value;
                UserSettings.Default.Save();
            }
        }

        // simple function for activating or deactivation the measurement mode button
        public void ToggleMeasurementModeButton(bool toggle)
        {
            if (_measureMode == MeasureMode.Distance)
            {
                tsbDistance.Checked = toggle;
                tsbArea.Checked = false;
            }
            else if (_measureMode == MeasureMode.Area)
            {
                tsbDistance.Checked = false;
                tsbArea.Checked = toggle;
            }
        }

        /// <summary>
        /// Occurs when the measuring mode has been changed.
        /// </summary>
        public event EventHandler MeasureModeChanged;

        /// <summary>
        /// Occurs when the clear button has been pressed.
        /// </summary>
        public event EventHandler MeasurementsCleared;

        ///// <summary>
        ///// Clean up any resources being used.
        ///// </summary>
        ///// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void AreaButton_Click(object sender, EventArgs e)
        {
            if (_measureMode != MeasureMode.Area)
            {
                MeasureMode = MeasureMode.Area;
                DistanceUnitIndex = cmbUnits.SelectedIndex;
                cmbUnits.SuspendLayout();
                cmbUnits.Items.Clear();
                cmbUnits.Items.AddRange(_areaUnitNames);
                cmbUnits.SelectedIndex = AreaUnitIndex;
                OnMeasureModeChanged();
                cmbUnits.ResumeLayout();
                Text = @"Measure Area";
                lblMeasure.Text = @"Area";
                tsbDistance.Checked = false;
                tsbArea.Checked = true;
            }
        }

        private void OnMeasureModeChanged()
        {
            if (MeasureModeChanged != null) MeasureModeChanged(this, new EventArgs());
        }

        private void DistanceButton_Click(object sender, EventArgs e)
        {
            if (_measureMode != MeasureMode.Distance)
            {
                MeasureMode = MeasureMode.Distance;
                AreaUnitIndex = cmbUnits.SelectedIndex;
                cmbUnits.SuspendLayout();
                cmbUnits.Items.Clear();
                cmbUnits.Items.AddRange(_distanceUnitNames);
                cmbUnits.SelectedIndex = DistanceUnitIndex;
                cmbUnits.ResumeLayout();
                OnMeasureModeChanged();
                Text = @"Measure Distance";
                lblMeasure.Text = @"Distance";
                tsbArea.Checked = false;
                tsbDistance.Checked = true;
            }
        }

        private void cmbUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_measureMode == MeasureMode.Distance)
            {
                _distIntoMeters = _distanceUnitFactors[cmbUnits.SelectedIndex];
            }
            else
            {
                _areaIntoSquareMeters = _areaUnitFactors[cmbUnits.SelectedIndex];
            }
            lblPartialValue.Text = String.Empty;
            lblTotalValue.Text = String.Empty;
        }

        private void tsbClear_Click(object sender, EventArgs e)
        {
            if (MeasurementsCleared != null) MeasurementsCleared(this, EventArgs.Empty);
        }
    }
}
