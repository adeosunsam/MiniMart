using Microsoft.AspNetCore.Mvc;
using MiniMart.Application.Interface;
using static MiniMart.Application.DTO.OrderDto;

namespace MiniMart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _saleService;

        public OrderController(IOrderService saleService)
        {
            _saleService = saleService;
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> CreateInventory(PurchaseProductDto request)
        {
            var response = await _saleService.PurchaseGoodsAsync(request);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("summary")]
        public async Task<IActionResult> FetchOrderItems(PurchaseProductDto request)
        {
            var response = await _saleService.FetchOrderItemsAsync(request);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
