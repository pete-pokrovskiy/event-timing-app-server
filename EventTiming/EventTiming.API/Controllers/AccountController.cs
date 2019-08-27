using EventTiming.API.Auth;
using EventTiming.API.Helpers;
using EventTiming.API.Inputs;
using EventTiming.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EventTiming.API.Controllers
{
    [Authorize]
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly IOptions<JwtIssuerOptions> _jwtOptions;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IJwtFactory jwtFactory,
            IOptions<JwtIssuerOptions> jwtOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions;
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody]SignUpInput signUpInput)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid sign up information!");

            var result = await _userManager.CreateAsync(new IdentityUser
            {
                Email = signUpInput.Email,
                UserName = signUpInput.Login,
            }, signUpInput.Password);

            if (result.Succeeded)
                return Ok();

            foreach (var error in result.Errors)
                ModelState.AddModelError("error", error.Description);

            return StatusCode(500, ModelState);
        }


        //[HttpPost("signin")]
        //public async Task<IActionResult> SignIn([FromBody]SignInInput signInInput)
        //{
        //    if (signInInput == null || !ModelState.IsValid)
        //    {
        //        return BadRequest("Invalid input!");
        //    }

        //    var result = await _signInManager.PasswordSignInAsync(signInInput.Login, signInInput.Password, false, false);

        //    if (result.Succeeded)
        //    {
        //        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super_secret_key"));
        //        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        //        var tokenOptions = new JwtSecurityToken(
        //            issuer: "http://localhost:5000",
        //            audience: "http://localhost:5000",
        //            claims: new List<Claim>(),
        //            expires: DateTime.Now.AddMinutes(2),
        //            signingCredentials: signingCredentials);

        //        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        //        return Ok(new { Token = tokenString });
        //    }

        //    return Unauthorized();

        //}

        [AllowAnonymous]
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody]SignInInput signInInput)
        {
            if (signInInput == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid input!");
            }


            var identity = await GetClaimsIdentity(signInInput.Login, signInInput.Password);
            if (identity == null)
            {
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid user name or password", ModelState));
            }

            var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, signInInput.Login, _jwtOptions.Value, new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented
            });

            return new OkObjectResult(jwt);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return await Task.FromResult<ClaimsIdentity>(null);

            // get the user to verifty
            var userToVerify = await _userManager.FindByNameAsync(userName);

            if (userToVerify == null) return await Task.FromResult<ClaimsIdentity>(null);

            // check the credentials
            if (await _userManager.CheckPasswordAsync(userToVerify, password))
            {
                return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(userName, userToVerify.Id));
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }

    }
}
