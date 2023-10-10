using System.Xml.Serialization;

namespace webapi.LicenseModels
{
    public class LicenseRoles
    {
        [XmlElement(ElementName = "Role")]
        public List<string> Role { get; set; }
    }
}
