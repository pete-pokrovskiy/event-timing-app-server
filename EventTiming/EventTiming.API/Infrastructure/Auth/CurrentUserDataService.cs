using EventTiming.Logic.Services.Auth;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventTiming.API.Infrastructure.Auth
{
    public class CurrentUserDataService : ICurrentUserDataService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthenticationService _authService;

        public UserData CurrentUserData { get; }

        public CurrentUserDataService(UserManager<IdentityUser> userManager, IAuthenticationService authService)
        {
            _userManager = userManager;
            _authService = authService;


            CurrentUserData = GetCurrentUserData().Result;
        }


        private async Task<UserData> GetCurrentUserData()
        {

            if (!_authService.IsAuthenticated)
                return null;
                //throw new Exception("Пользователь не аутентифицирован!");

            if (string.IsNullOrEmpty(_authService.UserName))
                throw new Exception("Не удалось определить логин пользователя!");


           var user = await _userManager.FindByNameAsync(_authService.UserName);


            if (user == null)
                throw new Exception($"Не удалось найти в БД пользователя с логином {_authService.UserName.ToLower()}");

            return new UserData
            {
                Id = user.Id,
                UserName = user.UserName
            };
        }

    }
}
