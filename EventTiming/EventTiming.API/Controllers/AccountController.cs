using EventTiming.API.Contract;
using EventTiming.API.Helpers;
using EventTiming.API.Infrastructure.Auth;
using EventTiming.Logic.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventTiming.API.Controllers
{
    [Authorize]
    [Route("api/v1/account")]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly IOptions<JwtIssuerOptions> _jwtOptions;
        private readonly ICurrentUserDataService _currentUserDataService;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IJwtFactory jwtFactory,
            IOptions<JwtIssuerOptions> jwtOptions,
            ICurrentUserDataService currentUserDataService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions;
            _currentUserDataService = currentUserDataService;
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<ActionResult> SignUp([FromBody]SignUpInput signUpInput)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid sign up information!");

            var result = await _userManager.CreateAsync(new IdentityUser
            {
                Email = signUpInput.Email,
                UserName = signUpInput.Login,
            }, signUpInput.Password);

            if (result.Succeeded)
            {
                return await SignIn(new SignInInput
                {
                    Login = signUpInput.Login,
                    Password = signUpInput.Password
                });

            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("error", error.Description);

            return StatusCode(500, ModelState);
        }

        [AllowAnonymous]
        [HttpPost("signin")]
        public async Task<ActionResult> SignIn([FromBody]SignInInput signInInput)
        {
            if (signInInput == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid input!");
            }

            var user = await _userManager.FindByNameAsync(signInInput.Login);

            if(user == null)
            {
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid user name or password", ModelState));
            }

            bool isValidPassword = await _userManager.CheckPasswordAsync(user, signInInput.Password);

            if(!isValidPassword)
            {
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid user name or password", ModelState));
            }

            var identity = await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(user.Id, user.UserName));

            var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, signInInput.Login, _jwtOptions.Value, new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented
            });

            return new OkObjectResult(new
            {
                token = jwt,
                user = new
                {
                    id = user.Id,
                    login = user.UserName
                }
            });
        }

        [HttpGet("auth")]
        public async Task<ActionResult> GetCurrentUserData()
        {
            return Ok(new 
            {
                id = _currentUserDataService.CurrentUserData.Id,
                login = _currentUserDataService.CurrentUserData.UserName
            });
        }     
    }
}
