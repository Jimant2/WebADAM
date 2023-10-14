using System.Xml.Serialization;

namespace webapi.LicenseModels
{
    public class LicenseUser
    {
        [XmlAttribute(AttributeName = "RealName")]
        public string RealName { get; set; }

        [XmlAttribute(AttributeName = "UserName")]
        public string UserName { get; set; }

        [XmlAttribute(AttributeName = "Password")]
        public string Password { get; set; }

        [XmlAttribute(AttributeName = "Expires")]
        public DateTime Expires { get; set; }

        [XmlAttribute(AttributeName = "Email")]
        public string Email { get; set; }

        public List<string> AllowedDevices { get; set; }
    }
}
