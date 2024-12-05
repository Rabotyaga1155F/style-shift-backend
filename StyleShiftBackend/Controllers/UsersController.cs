using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StyleShiftBackend.Models;
using System.Threading.Tasks;

namespace StyleShiftBackend.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<CustomUser> _userManager;

        public UsersController(UserManager<CustomUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("get-userby-id/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { Message = "Пользователь не найден" });
            }

            var userDto = new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.Verification,
                user.LockoutEnd,
                user.LockoutEnabled,
                user.AccessFailedCount,
                user.EmailConfirmed,
                user.PhoneNumberConfirmed,
                user.TwoFactorEnabled,
                user.SecurityStamp
            };

            return Ok(userDto);
        }

        [HttpGet("get-user-by-email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound(new { Message = "Пользователь не найден" });
            }

            var userDto = new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.Verification,
            };

            return Ok(userDto);
        }
    }
}
