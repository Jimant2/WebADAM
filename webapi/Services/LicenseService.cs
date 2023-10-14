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
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        public async Task SaveLicensedUserFromService(License license)
        {
            string hashedPassword = HashPassword(license.User.Password);
            Users user = new Users
            {
                LicenseXml = license,
                Password = hashedPassword,
                Username = license.User.UserName
            };

            await _repository.AddLicenseXmlAsync(user);
        }
    }
}
