using System.Xml.Serialization;

namespace webapi.LicenseModels
{
    public class LicenseFeatures
    {
        [XmlElement(ElementName = "Feature")]
        public List<string> Feature { get; set; }
    }
}
