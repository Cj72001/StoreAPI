using AutoMapper;
using Microsoft.Data.SqlClient;
using Store.Core.DTOs;
using Store.Core.DTOs.Response;
using Store.Core.Entities;
using Store.Core.Interfaces;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.Infraestructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse<bool>> AddProductAsync(ProductDTO productDTO)
        {
            var response = new BaseResponse<bool>();

            try
            {
                // Creamos los parametros del SP
                SqlParameter[] parameters = new[]
                {
                    new SqlParameter("@Name", productDTO.Name),
                    new SqlParameter("@Description", productDTO.Description ?? (object)DBNull.Value),
                    new SqlParameter("@Stock", productDTO.Stock),
                    new SqlParameter("@UnitPrice", productDTO.UnitPrice)
                 };

                // Ejecutamos el SP usando ExecuteAsync
                await _unitOfWork.Products.ExecuteAsync("sp_add_product", parameters);

                await _unitOfWork.SaveChangesAsync();

                response.Data = true;
                response.Success = true;
                response.StatusCode = 201;
                response.Message = "Producto agregado correctamente";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = 500;
                response.Message = "Error al agregar producto";
                response.Errors.Add("Exception", ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse<List<ProductDTO>>> GetAllProductsAsync(string? name)
        {
            var response = new BaseResponse<List<ProductDTO>>();

            try
            {
                var products = new List<Product>();
                if (string.IsNullOrEmpty(name))
                {
                    // Llamamos al SP usando el metodo del BaseRepository
                     products = await _unitOfWork.Products.GetAllAsync("sp_get_all_products_name_asc");
                }
                else
                {
                    // SP para buscar productos por nombre
                    products = await _unitOfWork.Products.GetAllAsync("sp_get_products_by_name", new SqlParameter
                    {
                        ParameterName = "@Name",
                        Value = name
                    });
                }

                if(products == null || products.Count == 0)
                {
                    response.Data = new List<ProductDTO>();
                    response.Success = true;
                    response.StatusCode = 400;
                    response.Message = "No se encontraron productos";
                    return response;
                }

                // Mapeamos a DTO
                var productsDto = _mapper.Map<List<ProductDTO>>(products);

                response.Data = productsDto;
                response.Success = true;
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = 500;
                response.Message = "Error al obtener productos";
                response.Errors.Add("Exception", ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse<bool>> SoftDeleteProductAsync(int productId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                // Obtener el producto por ID para validar existencia

                SqlParameter[] parameters = new[]
                {
                    new SqlParameter("@ProductId", productId)
                };

                var existingProduct = await _unitOfWork.Products.GetSingleAsync("sp_get_product_by_id", parameters);

                if(existingProduct == null)
                {
                    response.Data = false;
                    response.Success = false;
                    response.StatusCode = 404;
                    response.Message = "Producto no encontrado";
                    return response;
                }

                var product = await _unitOfWork.Products.ExecuteAsync("sp_soft_delete_product", parameters);

                await _unitOfWork.SaveChangesAsync();

                response.Data = true;
                response.Success = true;
                response.StatusCode = 200;
                response.Message = "Producto eliminado correctamente";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = 500;
                response.Message = "Error al eliminar producto";
                response.Errors.Add("Exception", ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse<bool>> UpdateProductStockAsync(int productId, decimal newStock)
        {
            var response = new BaseResponse<bool>();

            try
            {
                SqlParameter[] parametersCheckExist = new[]
                {
                    new SqlParameter("@ProductId", productId)
                };

                var existingProduct = await _unitOfWork.Products.GetSingleAsync("sp_get_product_by_id", parametersCheckExist);

                if (existingProduct == null)
                {
                    response.Data = false;
                    response.Success = false;
                    response.StatusCode = 404;
                    response.Message = "Producto no encontrado";
                    return response;
                }

                // Se crea otro set de parametros para la actualizacion para no usar List o Linq en este caso
                SqlParameter[] parameters = new[]
                {
                    new SqlParameter("@ProductId", productId),
                    new SqlParameter("@NewStock", newStock)
                };

                int result = await _unitOfWork.Products.ExecuteAsync("sp_update_product_stock", parameters);

                await _unitOfWork.SaveChangesAsync();

                response.Data = true;
                response.Success = true;
                response.StatusCode = 200;
                response.Message = "Stock actualizado correctamente.";

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = 500;
                response.Message = "Error al actualizar stock";
                response.Errors.Add("Exception", ex.Message);
            }

            return response;
        }


        public async Task<BaseResponse<bool>> UpdateProductPriceAsync(int productId, decimal newPrice, string changedByUserId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                SqlParameter[] parametersCheckExist = new[]
                {
                    new SqlParameter("@ProductId", productId)
                };

                var existingProduct = await _unitOfWork.Products.GetSingleAsync("sp_get_product_by_id", parametersCheckExist);

                if (existingProduct == null)
                {
                    response.Data = false;
                    response.Success = false;
                    response.StatusCode = 404;
                    response.Message = "Producto no encontrado";
                    return response;
                }

                // Se crea otro set de parametros para la actualizacion para no usar List o Linq en este caso
                SqlParameter[] parameters = new[]
                {
                    new SqlParameter("@ProductId", productId),
                    new SqlParameter("@OldPrice", existingProduct.UnitPrice),
                    new SqlParameter("@NewPrice", newPrice),
                    new SqlParameter("@ChangedBy", int.Parse(changedByUserId))
                };

                await _unitOfWork.Products.ExecuteAsync("sp_update_product_price", parameters);

                await _unitOfWork.SaveChangesAsync();

                response.Data = true;
                response.Success = true;
                response.StatusCode = 200;
                response.Message = "Precio del producto actualizado correctamente";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = 500;
                response.Message = "Error al actualizar precio";
                response.Errors.Add("Exception", ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse<bool>> PurchaseProductAsync(int productId, string userId, decimal quantity)
        {
            var response = new BaseResponse<bool>();

            try
            {
                SqlParameter[] parametersCheckExist = new[]
                {
                    new SqlParameter("@ProductId", productId)
                };

                var existingProduct = await _unitOfWork.Products.GetSingleAsync("sp_get_product_by_id", parametersCheckExist);

                if (existingProduct == null)
                {
                    response.Data = false;
                    response.Success = false;
                    response.StatusCode = 404;
                    response.Message = "Producto no encontrado";
                    return response;
                }

                decimal newStock = existingProduct.Stock - quantity;

                // Validando si hay stock suficiente
                if (newStock < 0)
                {
                    response.Data = false;
                    response.Success = false;
                    response.StatusCode = 400;
                    response.Message = "No hay stock suficiente para realizar la compra, stock actual es:" + existingProduct.Stock;
                    return response;
                }

                decimal totalAmount = existingProduct.UnitPrice * quantity;

                // Se crea otro set de parametros para la actualizacion para no usar List o Linq en este caso
                SqlParameter[] parameters = new[]
                {
                    new SqlParameter("@ProductId", productId),
                    new SqlParameter("@Quantity", quantity),
                    new SqlParameter("@UserId", int.Parse(userId)),
                    new SqlParameter("@total_amount", totalAmount),
                    new SqlParameter("@NewStock", newStock)
                };

                await _unitOfWork.Products.ExecuteAsync("sp_purchase_product", parameters);

                response.Data = true;
                response.Success = true;
                response.StatusCode = 200;
                response.Message = "Compra realizada correctamente";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = 500;
                response.Message = "Error al realizar compra";
                response.Errors.Add("Exception", ex.Message);
            }

            return response;
        }

        /*public async Task<BaseResponse<IEnumerable<Product>>> SearchProductsByNameAsync(string name)
        {
            var response = new BaseResponse<IEnumerable<Product>>();

            try
            {
                var products = await _unitOfWork.Products.GetAllAsyncPredicate(
                    p => !p.IsDeleted && p.Name.Contains(name)
                );

                response.Data = products;
                response.Success = true;
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = 500;
                response.Message = "Error al buscar productos";
                response.Errors.Add("Exception", ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse<Product>> GetProductByIdAsync(int id)
        {
            var response = new BaseResponse<Product>();

            try
            {
                var product = await _unitOfWork.Products.GetById(id);

                if (product == null)
                {
                    response.Success = false;
                    response.StatusCode = 404;
                    response.Message = "Producto no encontrado";
                }
                else
                {
                    response.Data = product;
                    response.Success = true;
                    response.StatusCode = 200;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = 500;
                response.Message = "Error al obtener producto";
                response.Errors.Add("Exception", ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse<bool>> UpdateProductPriceAsync(int productId, decimal newPrice, int changedByUserId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var product = await _unitOfWork.Products.GetById(productId);
                if (product == null) throw new Exception("Producto no encontrado");

                var oldPrice = product.UnitPrice;
                product.UnitPrice = newPrice;

                _unitOfWork.Products.Update(product);

                // Registrar log de cambio de precio
                await _unitOfWork.ProductPriceLogs.Add(new ProductPriceLog
                {
                    ProductId = productId,
                    OldPrice = oldPrice,
                    NewPrice = newPrice,
                    ChangedBy = changedByUserId,
                    ChangedOnUtc = DateTime.UtcNow
                });

                await _unitOfWork.SaveChangesAsync();

                response.Data = true;
                response.Success = true;
                response.StatusCode = 200;
                response.Message = "Precio actualizado correctamente";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = 500;
                response.Message = "Error al actualizar precio";
                response.Errors.Add("Exception", ex.Message);
            }

            return response;
        }

        public async Task<BaseResponse<bool>> SoftDeleteProductAsync(int productId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var product = await _unitOfWork.Products.GetById(productId);
                if (product == null) throw new Exception("Producto no encontrado");

                product.IsDeleted = true;
                product.DeletedOnUtc = DateTime.UtcNow;

                _unitOfWork.Products.Update(product);
                await _unitOfWork.SaveChangesAsync();

                response.Data = true;
                response.Success = true;
                response.StatusCode = 200;
                response.Message = "Producto eliminado correctamente";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = 500;
                response.Message = "Error al eliminar producto";
                response.Errors.Add("Exception", ex.Message);
            }

            return response;
        }

        */


    }
}
