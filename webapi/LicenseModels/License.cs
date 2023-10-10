using System.Xml.Serialization;

namespace webapi.LicenseModels
{
    [XmlRoot(ElementName = "License", Namespace = "http://ADAM.Model.Licenses.License")]
    public class License
    {
        [XmlElement(ElementName = "User")]
        public LicenseUser User { get; set; }

        [XmlElement(ElementName = "Roles")]
        public LicenseRoles Roles { get; set; }

        [XmlElement(ElementName = "Features")]
        public LicenseFeatures Features { get; set; }

        [XmlElement(ElementName = "ChannelRestrictions")]
        public LicenseDeviceCollection ChannelRestrictions { get; set; }

        [XmlAttribute(AttributeName = "Author")]
        public string Author { get; set; }

        [XmlAttribute(AttributeName = "Machine")]
        public string Machine { get; set; }

        [XmlAttribute(AttributeName = "Group")]
        public string Group { get; set; }
    }
}
