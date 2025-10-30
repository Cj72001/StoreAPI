using Store.Core.DTOs;
using Store.Core.DTOs.Response;
using Store.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces.Services
{
    public interface IProductService
    {
        Task<BaseResponse<bool>> AddProductAsync(ProductDTO product);
        Task<BaseResponse<List<ProductDTO>>> GetAllProductsAsync(string? name);
        Task<BaseResponse<bool>> SoftDeleteProductAsync(int productId);
        Task<BaseResponse<bool>> UpdateProductStockAsync(int productId, decimal newStock);
        Task<BaseResponse<bool>> UpdateProductPriceAsync(int productId, decimal newPrice, string changedByUserId);

        Task<BaseResponse<bool>> PurchaseProductAsync(int productId, string userId, decimal quantity);
    }
}
