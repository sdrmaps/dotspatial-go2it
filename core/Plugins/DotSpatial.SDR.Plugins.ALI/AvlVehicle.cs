using System;

namespace DotSpatial.SDR.Plugins.ALI
{
    public class AvlVehicle
    {
        public AvlVehicle()
        {
            CurrentInterval = 0;
            Visible = true;
            UnitType = AvlVehicleType.None;
            IgnoreActiveHide = false;
        }
        public bool Visible { get; set; }
        public int CurrentInterval { get; set; }
        public string UnitId { get; set; }
        public string UnitLabel { get; set; }
        public AvlVehicleType UnitType { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime UpdateTime { get; set; }
        public bool IgnoreActiveHide { get; set; }
        public string TimeZone { get; set; }
    }

    public enum AvlVehicleType
    {
        None,
        LawEnforcement,
        FireDepartment,
        EmergencyMedicalService,
    }
}
