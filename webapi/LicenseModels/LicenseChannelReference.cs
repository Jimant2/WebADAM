using System.Xml.Serialization;

namespace webapi.LicenseModels
{
    public class LicenseChannelReference
    {
        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "Title")]
        public string Title { get; set; }

        [XmlAttribute(AttributeName = "Protection")]
        public LicenseProtectionEnum Protection { get; set; }
    }
}
