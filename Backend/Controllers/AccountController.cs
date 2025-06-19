using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Areas.Identity.Data;

namespace Backend.Controllers
{
    [Authorize]
    [Route("api/account")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly BackendDbContext _dbContext;
        private readonly UserManager<BackendUser> _userManager;
        private readonly SignInManager<BackendUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<BackendUser> userManager, SignInManager<BackendUser> signInManager, BackendDbContext dbContext, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        // POST: /change-username  
        [Authorize]
        [HttpPost("change-username")]
        public async Task<IActionResult> ChangeUsername([FromForm] string NewUsername)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // check if the new username is already in use
            if (await _userManager.FindByNameAsync(NewUsername) is not null && user.UserName != NewUsername)
            {
                return BadRequest(new { Errors = new[] { "Username is already taken." } });
            }

            user.UserName = NewUsername;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
            }

            return Ok(new { Message = "Username updated" });
        }

        [HttpPost("change-email")]
        public async Task<IActionResult> ChangeEmail([FromForm] string NewEmail)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // check if the new email is already in use
            if (await _userManager.FindByEmailAsync(NewEmail) is not null && user.Email != NewEmail)
            {
                return BadRequest(new { Errors = new[] { "Email is already in use." } });
            }

            user.Email = NewEmail;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
            }

            return Ok(new { Message = "Email updated" });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordDto model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
            }

            return Ok(new { Message = "Password updated" });
        }
    }
}