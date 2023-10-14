using System.Xml.Serialization;

namespace webapi.LicenseModels
{
    public class LicenseDeviceCollection
    {
        [XmlElement(ElementName = "Device")]
        public List<LicenseDevice> Device { get; set; }
    }
}
