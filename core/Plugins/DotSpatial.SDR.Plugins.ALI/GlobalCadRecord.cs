using System.ComponentModel;

namespace DotSpatial.SDR.Plugins.ALI
{
    public class GlobalCadRecord
    {
        public string Time { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        [DisplayName(@"Service Class")]
        public string ServiceClass { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public string Unc { get; set; }
        [DisplayName(@"Full Address")]
        public string FullAddress
        {
            get { return (Address + " " + Street).Trim(); }
        }
    }
}