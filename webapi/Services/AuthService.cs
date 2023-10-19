using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Xml.Serialization;
using webapi.DataRepos;
using webapi.LicenseModels;

namespace webapi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IWebADAMRepo _repository;

        public AuthService(IWebADAMRepo repository)
        {
            _repository = repository;
        }

        public async Task<ClaimsPrincipal> AuthenticateAsync(string username, string password)
        {
            var user = await _repository.FindByUsernameAsync(username);

            if (user != null && PasswordHasher.Verify(password, user.Password))
            {
                var license = user.LicenseXml;

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, license.User.UserName)
        };

                if (license.Roles != null)
                {
                    foreach (var role in license.Roles.Role) 
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                }

                var identity = new ClaimsIdentity(claims, "custom", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                return new ClaimsPrincipal(identity);
            }

            return null;
        }

     
    }

}
