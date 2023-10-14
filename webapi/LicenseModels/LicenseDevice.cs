using System.Xml.Serialization;

namespace webapi.LicenseModels
{
    public class LicenseDevice
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }

        [XmlAttribute(AttributeName = "Protection")]
        public LicenseProtectionEnum Protection { get; set; }

        [XmlElement(ElementName = "ExperimentBuilds")]
        public LicenseExperimentBuildsCollection ExperimentBuilds { get; set; }
    }
}
