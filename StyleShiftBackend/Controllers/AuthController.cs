using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StyleShiftBackend.Models;
using System.Threading.Tasks;
using StyleShiftBackend.Dto;

namespace StyleShiftBackend.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;

        public AuthController(UserManager<CustomUser> userManager, SignInManager<CustomUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new CustomUser
            {
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Пользователь успешно зарегистрирован" });
            }

            return BadRequest(result.Errors);
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("Неверный email или пароль");
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (!result.Succeeded)
            {
                return BadRequest("Неверный email или пароль");
            }
            

            var response = new
            {
                UserID = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Verification = user.Verification
            };

            return Ok(response);
        }
    }
}
