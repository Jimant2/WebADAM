using System.Xml.Serialization;
using webapi.LicenseModels;

namespace webapi.LicenseModels
{
    [XmlRoot("License")]
    public class License
    {
        [XmlAttribute("Author")]
        public string Author { get; set; }

        [XmlAttribute("Machine")]
        public string Machine { get; set; }

        [XmlAttribute("Group")]
        public string Group { get; set; }

        public LicenseUser User { get; set; }

        public LicenseRoles Roles { get; set; }

        public LicenseFeatures Features { get; set; }

        [XmlElement("ChannelRestrictions")]
        public LicenseDeviceCollection ChannelRestrictions { get; set; }
    }
}