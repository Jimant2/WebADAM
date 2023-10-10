using System.ComponentModel;
using webapi.LicenseModels;

namespace webapi.Services
{
    public class UserLicenseService : IUserLicenseService
    {
        public bool IsValidLicense(LicenseModels.License license, string username, string password, string requiredRole, List<string> allowedDevices)
        {
            // Check if license object is null
            if (license == null || license.User == null)
            {
                return false;
            }

            // Check if the license has a valid user with required roles
            if (string.IsNullOrWhiteSpace(license.User.UserName) ||
                string.IsNullOrWhiteSpace(license.User.Password) ||
                license.User.Expires < DateTime.UtcNow ||
                !license.Roles.Role.Contains(requiredRole))
            {
                return false;
            }

            // Check if the provided username and password match the ones in the license
            if (license.User.UserName != username || license.User.Password != password)
            {
                return false;
            }

            // Check if the user has access to at least one allowed device
            if (allowedDevices != null && allowedDevices.Count > 0)
            {
                // Check if the user's allowed devices intersect with the allowed devices list
                if (!license.User.AllowedDevices.Intersect(allowedDevices).Any())
                {
                    return false;
                }
            }

            // If all checks pass, the license is valid, the user has the required role, and has access to allowed devices
            return true;
        }
    }

    }
