using System.Security.Claims;
using webapi.DataRepos;
using webapi.DefinitionModels;
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
                if (license.User.Expires > DateTime.UtcNow)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, license.User.UserName),
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
        public List<ChannelReference> GetAuthorizedChannels(ChannelReference[] allChannels, License userLicense)
        {
            var authorizedChannels = new List<ChannelReference>();

            foreach (var userDevice in userLicense.ChannelRestrictions.Device)
            {
                foreach (var build in userDevice.ExperimentBuilds.ExperimentBuild)
                {
                    foreach (var channel in build.Channels.Channel)
                    {
                        var existingChannel = allChannels.FirstOrDefault(c => c.Id == channel.Id);

                        if (existingChannel != null && channel.Protection.ToString() == "Allowed")
                        {
                            authorizedChannels.Add(existingChannel);
                        }
                    }
                }
            }

            return authorizedChannels;
        }


    }

}
