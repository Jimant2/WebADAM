using System.Security.Claims;
using webapi.DataRepos;
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
                if (license.User.Expires > DateTime.UtcNow)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, license.User.UserName)
            };

                    if (license.Roles != null)
                    {
                        foreach (var role in license.Roles.Role)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));

                            if (role == "Administrator")
                            {
                                claims.Add(new Claim("CanAccessEverything", "true"));
                            }
                            else if (role == "DataAnalyzer")
                            {
                                // Add additional claims for DataAnalyzer role
                                claims.Add(new Claim("CanAccessLimitedData", "true"));
                            }
                        }
                    }
                    var identity = new ClaimsIdentity(claims, "custom", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                    return new ClaimsPrincipal(identity);
                }
            }

            return null;
        }


    }

}
