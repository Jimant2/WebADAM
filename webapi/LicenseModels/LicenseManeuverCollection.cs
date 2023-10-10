using System.Xml.Serialization;

namespace webapi.LicenseModels
{
    public class LicenseManeuverCollection
    {
        [XmlElement(ElementName = "Maneuver")]
        public List<LicenseManeuver> Maneuver { get; set; }
    }
}
