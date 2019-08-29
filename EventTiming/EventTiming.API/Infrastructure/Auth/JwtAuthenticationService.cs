using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using EventTiming.Logic.Services.Auth;


namespace EventTiming.API.Infrastructure.Auth
{
    public class JwtAuthenticationService : IAuthenticationService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IIdentity _identity;

        private readonly string _login;

        public JwtAuthenticationService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _identity = GetIdentity();


            var claimsPrincipal = GetUser();

            var loginClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == JwtConstants.JwtClaimIdentifiers.Login);

            if (loginClaim == null)
                return;

            _login = loginClaim.Value.ToLower();

        }

        public bool IsAuthenticated
        {
            get
            {
                return _identity.IsAuthenticated;
            }
        }

        public string UserName
        {
            get
            {
                return _login;
            }
        }



        private ClaimsPrincipal GetUser()
        {
            if (_contextAccessor.HttpContext == null)
                throw new Exception("HttpContext не определен!");

            if (_contextAccessor.HttpContext.User == null)
                throw new Exception("HttpContext.User не определен!");

            return _contextAccessor.HttpContext.User;

        }

        private IIdentity GetIdentity()
        {
            if (_contextAccessor.HttpContext == null)
                throw new Exception("HttpContext не определен!");

            if (_contextAccessor.HttpContext.User == null)
                throw new Exception("HttpContext.User не определен!");

            if (_contextAccessor.HttpContext.User.Identity == null)
                throw new Exception("HttpContext.User.Identity не определен!");

            return _contextAccessor.HttpContext.User.Identity;

        }
    }
}
