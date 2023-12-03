using System.Security.Claims;
using webapi.DefinitionModels;
using webapi.LicenseModels;

namespace webapi.Services
{
    public interface IAuthService
    {
        public Task<ClaimsPrincipal> AuthenticateAsync(string username, string password);

        public List<ChannelReference> GetAuthorizedChannels(ChannelReference[] allChannels, License userLicense);

    }
}
