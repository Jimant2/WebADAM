using System.Xml.Serialization;

namespace webapi.LicenseModels
{
    public class LicenseExperimentBuild
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }

        [XmlAttribute(AttributeName = "Protection")]
        public LicenseProtectionEnum Protection { get; set; }

        [XmlElement(ElementName = "Channels")]
        public LicenseChannelCollection Channels { get; set; }

        [XmlElement(ElementName = "Experiments")]
        public LicenseExperimentCollection Experiments { get; set; }
    }
}
