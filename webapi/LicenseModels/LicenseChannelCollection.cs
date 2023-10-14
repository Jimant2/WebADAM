using System.Xml.Serialization;

namespace webapi.LicenseModels
{
    public class LicenseChannelCollection
    {
        [XmlElement(ElementName = "Channel")]
        public List<LicenseChannelReference> Channel { get; set; }
    }
}
