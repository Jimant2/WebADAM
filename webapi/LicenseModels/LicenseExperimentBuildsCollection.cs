using System.Xml.Serialization;

namespace webapi.LicenseModels
{
    public class LicenseExperimentBuildsCollection
    {
        [XmlElement(ElementName = "ExperimentBuild")]
        public List<LicenseExperimentBuild> ExperimentBuild { get; set; }
    }
}
