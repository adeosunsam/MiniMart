using Microsoft.AspNetCore.Mvc;
using MiniMart.Application.Interface;
using static MiniMart.Application.DTO.ProductDto;

namespace MiniMart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost()]
        public async Task<IActionResult> CreateGoods(CreateProductDto request)
        {
            var response = await _productService.CreateProductAsync(request);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet()]
        public async Task<IActionResult> GetProducts()
        {
            var response = await _productService.GetProducts();
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
