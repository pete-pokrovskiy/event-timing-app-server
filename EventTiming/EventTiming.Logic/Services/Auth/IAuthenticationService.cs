using System.Security.Principal;

namespace EventTiming.Logic.Services.Auth
{
    public interface IAuthenticationService
    {
        bool IsAuthenticated { get; }
        string UserName { get; }

        IIdentity IdentityUser { get; }
    }
}
