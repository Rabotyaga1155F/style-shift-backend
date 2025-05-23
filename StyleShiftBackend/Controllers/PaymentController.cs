using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Yandex.Checkout.V3;

namespace StyleShiftBackend.Controllers
{
    [Route("payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly AsyncClient _client;
        
        public PaymentController(IConfiguration configuration)
        {
            var shopId = configuration["YooMoney:ShopId"];
            var secretKey = configuration["YooMoney:SecretKey"];

            _client = new Client(shopId, secretKey).MakeAsync();
        }

        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
        {
            if (request == null || request.Amount <= 0)
            {
                return BadRequest("Некорректные данные платежа");
            }

            var newPayment = new NewPayment
            {
                Amount = new Amount { Value = request.Amount, Currency = "RUB" },
                Confirmation = new Confirmation
                {
                    Type = ConfirmationType.Redirect,
                    ReturnUrl = request.ReturnUrl
                },
                Description = request.Description
            };

            Payment payment = await _client.CreatePaymentAsync(newPayment);

            return Ok(new
            {
                paymentId = payment.Id,
                confirmationUrl = payment.Confirmation.ConfirmationUrl
            });
        }

        [HttpPost("capture-payment")]
        public async Task<IActionResult> CapturePayment([FromBody] CaptureRequest request)
        {
            if (string.IsNullOrEmpty(request.PaymentId))
            {
                return BadRequest("Payment ID не указан");
            }

            var payment = await _client.CapturePaymentAsync(request.PaymentId);

            return Ok(payment);
        }
        
        

        public class PaymentRequest
        {
            public decimal Amount { get; set; }
            public string Description { get; set; }
            public string ReturnUrl { get; set; }
        }

        public class CaptureRequest
        {
            public string PaymentId { get; set; }
        }
    }
}