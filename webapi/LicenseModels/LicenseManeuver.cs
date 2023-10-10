using System.Xml.Serialization;

namespace webapi.LicenseModels
{
    public class LicenseManeuver
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }

        [XmlElement(ElementName = "Channels")]
        public LicenseChannelCollection Channels { get; set; }
    }
}
