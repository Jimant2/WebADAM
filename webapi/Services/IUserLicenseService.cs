using webapi.LicenseModels;

namespace webapi.Services
{
    public interface IUserLicenseService
    {
        bool IsValidLicense(License license, string username, string password, string requiredRole, List<string> allowedDevices);
    }
}
