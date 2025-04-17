using Microsoft.AspNetCore.Mvc;
using MiniMart.Application.Interface;
using static MiniMart.Application.DTO.BankLinkDto;

namespace MiniMart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("generate-account")]
        public async Task<IActionResult> CreateGoods(GenerateAccountNumberRequestDto request)
        {
            var response = await _paymentService.GenerateVirtualAccount(request);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> GetProducts(CallBackDto request)
        {
            var response = await _paymentService.PaymentNotification(request);
            if (response)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("confirm-payment/{transactionReference}")]
        public async Task<IActionResult> GetProducts(string transactionReference)
        {
            var response = await _paymentService.PaymentConfirmation(transactionReference);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
