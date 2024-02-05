using AcquiringBank.Simulator.Models;
using Microsoft.AspNetCore.Mvc;

namespace AcquiringBank.Simulator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AcquiringBankController(IPaymentService paymentService) : ControllerBase
    {
        [HttpPost("process-payment")]
        public IActionResult ProcessPayment(PaymentRequest request)
        {
            var response = paymentService.ProcessPayment(request);
            return Ok(response);
        }
    }
}
