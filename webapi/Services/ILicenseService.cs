using webapi.LicenseModels;

namespace webapi.Services
{
    public interface ILicenseService
    {
        public Task SaveLicensedUserFromService(License license);

    }
}
