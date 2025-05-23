using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StyleShiftBackend.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using StyleShiftBackend.Dto;

namespace StyleShiftBackend.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        public AuthController(UserManager<CustomUser> userManager, SignInManager<CustomUser> signInManager, IConfiguration configuration,IMemoryCache cache)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _cache = cache;
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
                await _userManager.AddToRoleAsync(user, "User");
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

            var roles = await _userManager.GetRolesAsync(user);

            var response = new
            {
                UserID = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Roles = roles,
                Verification = user.Verification,
                Balance = user.Balance
            };

            return Ok(response);
        }



        [HttpPost("request-verification")]
        public async Task<IActionResult> RequestVerification([FromBody] RequestVerificationDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("Пользователь с таким email не найден");
            }

            // Генерация случайного 6-значного кода
            var verificationCode = new Random().Next(100000, 999999).ToString();

            // Кэшируем код на 10 минут
            _cache.Set(model.Email, verificationCode, TimeSpan.FromMinutes(10));

            // Отправка кода на почту
            SendEmail(model.Email, "Код для верификации", $"Ваш код: <b>{verificationCode}</b>");

            return Ok(new { Message = "Код отправлен на почту" });
        }

        [HttpPost("request-verification-registration")]
        public async Task<IActionResult> RequestVerificationForRegistration([FromBody] RequestVerificationDto model)
        {
            // Проверяем, существует ли пользователь с таким email
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest(new { Message = "Пользователь с таким email уже существует" });
            }
            
            var existingUserByUsername = await _userManager.FindByNameAsync(model.Username);
            if (existingUserByUsername != null)
            {
                return BadRequest(new { Message = "Пользователь с таким именем пользователя уже существует" });
            }
            
            

            // Генерация случайного 6-значного кода
            var verificationCode = new Random().Next(100000, 999999).ToString();

            // Кэшируем код на 10 минут
            _cache.Set(model.Email, verificationCode, TimeSpan.FromMinutes(10));

            // Отправка кода на почту
            SendEmail(model.Email, "Код для подтверждения регистрации", $"Ваш код: <b>{verificationCode}</b>");

            return Ok(new { Message = "Код для регистрации отправлен на почту" });
        }



        [HttpPost("verify-code")]
        public IActionResult VerifyCode([FromBody] VerifyCodeDto model)
        {
            if (_cache.TryGetValue(model.Email, out string cachedCode) && cachedCode == model.Code)
            {
                return Ok(new { Message = "Код подтвержден" });
            }

            return BadRequest("Неверный код или истек срок действия");
        }

        // 3. Сброс пароля по коду
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordWithCodeDto model)
        {
            if (!_cache.TryGetValue(model.Email, out string cachedCode) || cachedCode != model.Code)
            {
                return BadRequest("Неверный код или истек срок действия");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("Пользователь не найден");
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);

            if (result.Succeeded)
            {
                _cache.Remove(model.Email); // Удалить код из кэша
                return Ok(new { Message = "Пароль успешно сброшен" });
            }

            return BadRequest(result.Errors);
        }

        // Вспомогательный метод для отправки почты
        private void SendEmail(string to, string subject, string body)
        {
            var smtpSettings = _configuration.GetSection("Smtp");
            using (var client = new SmtpClient(smtpSettings["Host"], int.Parse(smtpSettings["Port"])))
            {
                client.Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpSettings["FromEmail"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);
                client.Send(mailMessage);
            }
        }
        
    }
}
