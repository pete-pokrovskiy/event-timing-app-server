using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EventTiming.API.Infrastructure.Auth
{
    public static class JwtConstants
    {
        public static class JwtClaimIdentifiers
        {
            public const string Rol = "rol", Id = "id", Login = "login";
        }
        public static class JwtClaims
        {
            public const string ApiAccess = "api_access";
        }
    }
}
