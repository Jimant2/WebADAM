using System.Xml.Serialization;

namespace webapi.LicenseModels
{
    public class LicenseExperimentCollection
    {
        [XmlElement(ElementName = "Experiment")]
        public List<LicenseExperiment> Experiment { get; set; }
    }
}
