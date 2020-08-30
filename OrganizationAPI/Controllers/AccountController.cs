using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OrganizationAPI.ViewModel;

namespace OrganizationAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signinManager;

        public AccountController(UserManager<IdentityUser> _userManager, SignInManager<IdentityUser> _signinManager)
        {
            userManager = _userManager ?? throw new ArgumentNullException(nameof(_userManager));
            signinManager = _signinManager ?? throw new ArgumentNullException(nameof(_signinManager));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new IdentityUser()
                    {
                        UserName = registerViewModel.UserName,
                        Email = registerViewModel.Email
                    };
                    var userResult = await userManager.CreateAsync(user, registerViewModel.Password);

                    if (userResult.Succeeded)
                    {
                        var roleResult = await userManager.AddToRoleAsync(user, "user");
                        if(roleResult.Succeeded)
                            return Ok(user);
                    }
                    else
                    {
                        foreach (var error in userResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return BadRequest(ModelState.Values);
                    }
                }
                return BadRequest(ModelState.Values);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
               return BadRequest(ModelState.Values);
            }
        }

        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn(SignInViewModel signInViewModel)
        {
            if (ModelState.IsValid)
            {
                var signinResult = await signinManager.PasswordSignInAsync(signInViewModel.UserName,
                                                                            signInViewModel.Password, false, false);
                if (signinResult.Succeeded)
                {
                    var user = await userManager.FindByNameAsync(signInViewModel.UserName);
                    var roles = await userManager.GetRolesAsync(user);

                    IdentityOptions identityOpts = new IdentityOptions();
                    var claims = new Claim[]
                    {
                        new Claim(identityOpts.ClaimsIdentity.UserIdClaimType,user.Id),
                        new Claim(identityOpts.ClaimsIdentity.UserNameClaimType,user.UserName),
                        new Claim(identityOpts.ClaimsIdentity.RoleClaimType,roles[0])
                    };

                    var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this-is-my-secret-key"));
                    var signinCredential = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256);
                    var jwt = new JwtSecurityToken(signingCredentials: signinCredential, 
                                                    claims: claims,
                                                    expires: DateTime.Now.AddMinutes(30));
                    var obj = new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(jwt),
                        UserId = user.Id,
                        UserName = user.UserName,
                        Role = roles[0]
                    };
                    return Ok(obj);
                }
                   
            }
            return BadRequest(ModelState);
        }

        [HttpPost("signOut")]
        public async Task<IActionResult> SignOut()
        {
            await signinManager.SignOutAsync();
            return NoContent();
        }
    }
}
