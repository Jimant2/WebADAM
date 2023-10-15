using System.Security.Claims;

namespace webapi.Services
{
    public interface IAuthService
    {
        public Task<ClaimsPrincipal> AuthenticateAsync(string username, string password);

    }
}
