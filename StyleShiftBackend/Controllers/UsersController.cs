using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StyleShiftBackend.Models;
using System.Linq;
using System.Threading.Tasks;
using StyleShiftBackend.Requests;

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

        private async Task<IActionResult> GetUsersByRole(string role)
        {
            var users = (await _userManager.GetUsersInRoleAsync(role)).Select(user => new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.Verification,
                user.Balance,
                IsBanned = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow // Проверяем, истек ли бан
            }).ToList();

            return Ok(users);
        }


        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllRegularUsers()
        {
            return await GetUsersByRole("User");
        }

        [HttpGet("get-all-admins")]
        public async Task<IActionResult> GetAllAdmins()
        {
            return await GetUsersByRole("Admin");
        }

        [HttpGet("get-all-stylists")]
        public async Task<IActionResult> GetAllStylists()
        {
            return await GetUsersByRole("Stylist");
        }

        [HttpGet("get-user-by-id/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { Message = "Пользователь не найден" });
            }

            var roles = await _userManager.GetRolesAsync(user);

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
                user.SecurityStamp,
                user.Balance,
                Roles = roles
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

            var roles = await _userManager.GetRolesAsync(user);

            var userDto = new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.Verification,
                user.Balance,
                Roles = roles
            };

            return Ok(userDto);
        }

        [HttpPost("update-verification")]
        public async Task<IActionResult> UpdateUserVerification([FromBody] UpdateVerificationRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new { Message = "Email не может быть пустым" });
            }

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return NotFound(new { Message = "Пользователь не найден" });
            }

            user.Verification = request.Verification;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return StatusCode(500, new { Message = "Не удалось обновить верификацию пользователя" });
            }

            return Ok(new { Message = "Верификация успешно обновлена" });
        }
        
        
        [HttpPost("ban/{userId}")]
        public async Task<IActionResult> BanUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.LockoutEnd = DateTimeOffset.MaxValue; // Устанавливаем максимальное время блокировки
            user.LockoutEnabled = true; // Включаем блокировку
            user.Verification = false;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest("Failed to ban user.");
            }

            return Ok("User banned successfully.");
        }



        [HttpPost("unban/{userId}")]
        public async Task<IActionResult> UnbanUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("Пользователь не найден");

            // Разблокируем пользователя
            user.LockoutEnd = null; // Разблокировка без срока

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok($"Пользователь {user.UserName} разблокирован");
            }
            return BadRequest("Ошибка при разблокировке пользователя");
        }
        
        
        [HttpGet("is-banned/{userId}")]
        public async Task<IActionResult> IsUserBanned(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "Пользователь не найден" });
            }

            bool isBanned = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow;

            return Ok(new { IsBanned = isBanned });
        }


    }
}
