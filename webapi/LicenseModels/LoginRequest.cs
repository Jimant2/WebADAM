namespace webapi.LicenseModels
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string LicenseXml { get; set; }
    }
}
