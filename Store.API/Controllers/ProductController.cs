using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Core.DTOs;
using Store.Core.Interfaces.Services;

namespace Lighthouse.API.Controllers
{
    
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductDTO productDTO)
        {
            var rolUserClaim = User.Claims.FirstOrDefault(c => c.Type == "rolUser");

            if (rolUserClaim == null)
            {
                return Unauthorized("El claim 'rolUser' no está presente en el token.");
            }
            else if(rolUserClaim.Value != "2")
            {
                return Unauthorized("Acceso denegado, solamente administradores pueden agregar productos");
            }
            
            var response = await _productService.AddProductAsync(productDTO);
            return StatusCode(response.StatusCode, response);
        }

        [AllowAnonymous]
        [HttpGet(Name = nameof(GetAllProduct))]
        public async Task<IActionResult> GetAllProduct(string? name)
        {
            var response = await _productService.GetAllProductsAsync(name);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var rolUserClaim = User.Claims.FirstOrDefault(c => c.Type == "rolUser");

            if (rolUserClaim == null)
            {
                return Unauthorized("El claim 'rolUser' no está presente en el token.");
            }
            else if (rolUserClaim.Value != "2")
            {
                return Unauthorized("Acceso denegado, solamente administradores pueden remover productos");
            }

            var response = await _productService.SoftDeleteProductAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("UpdateStock/{id}/{newStock}")]
        public async Task<IActionResult> UpdateStock(int id, decimal newStock)
        {
            var result = await _productService.UpdateProductStockAsync(id, newStock);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("UpdatePrice/{id}/{newPrice}")]
        public async Task<IActionResult> UpdateProductPrice(int id, decimal newPrice)
        {

            var idUseryClaim = User.Claims.FirstOrDefault(c => c.Type == "idUser");

            if (idUseryClaim == null)
            {
                return Unauthorized("El claim 'idUser' no está presente en el token.");
            }

            var rolUserClaim = User.Claims.FirstOrDefault(c => c.Type == "rolUser");

            if (rolUserClaim == null)
            {
                return Unauthorized("El claim 'rolUser' no está presente en el token.");
            }
            else if (rolUserClaim.Value != "2")
            {
                return Unauthorized("Acceso denegado, solamente administradores pueden agregar productos");
            }

            var result = await _productService.UpdateProductPriceAsync(id, newPrice, idUseryClaim.Value);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("PurchaseProduct/{id}/{quantity}")]
        public async Task<IActionResult> PurchaseProductAsync(int id, decimal quantity)
        {
            var idUseryClaim = User.Claims.FirstOrDefault(c => c.Type == "idUser");

            if (idUseryClaim == null)
            {
                return Unauthorized("El claim 'idUser' no está presente en el token.");
            }

            var result = await _productService.PurchaseProductAsync(id, idUseryClaim.Value, quantity);
            return StatusCode(result.StatusCode, result);
        }


    }
}
