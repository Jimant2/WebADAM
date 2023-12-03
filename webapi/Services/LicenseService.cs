using System.Security.Cryptography;
using System.Text;
using webapi.DataRepos;
using webapi.LicenseModels;
using webapi.Models;

namespace webapi.Services
{
    public class LicenseService : ILicenseService
    {

        private readonly IWebADAMRepo _repository;

        public LicenseService(IWebADAMRepo repository) 
        {
            _repository = repository;

        }

        public async Task SaveLicensedUserFromService(License license)
        {
            string hashedPassword = PasswordHasher.HashPassword(license.User.Password);
            Users user = new Users
            {
                LicenseXml = license,
                Password = hashedPassword,
                Username = license.User.UserName
            };

            await _repository.AddLicenseXmlAsync(user);
        }
        //public async Task<License> GetLicenseFromService()
        //{

        //}
    }
}
