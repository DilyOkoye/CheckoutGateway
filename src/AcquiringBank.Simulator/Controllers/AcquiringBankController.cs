using AcquiringBank.Simulator.Models;
using Microsoft.AspNetCore.Mvc;

namespace AcquiringBank.Simulator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AcquiringBankController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public AcquiringBankController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("process-payment")]
        public IActionResult ProcessPayment(PaymentRequest request)
        {
            var response = _paymentService.ProcessPayment(request);
            return Ok(response);
        }
    }
}
