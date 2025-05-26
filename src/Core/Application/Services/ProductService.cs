using Application.DTOs;
using Application.Features.Constants;
using Application.Interfaces;
using Application.Interfaces.Application;
using Application.Responses;
using Domain.Entities.Payment;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ProductService : IProductService<Product>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductService> _logger;
        //private readonly IAsyncRepository<Product> _productRepository;
        private readonly IMongoDBService _productRepository;
        public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger, IMongoDBService mongoDBService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
           // _productRepository = productRepository;
           _productRepository = mongoDBService;
        }

        public async Task<BaseResponse<CreateProductDTO>> CreateProduct(CreateProductDTO request)
        {
            try
            {
                var product = new Product
                {
                    CreatedBy = "SYSTEM",
                    CreatedAt = DateTime.Now,
                };
                request.ConvertFromDTO(product);
                var createdProduct = _productRepository.CreateAsync(product);
                await _unitOfWork.CommitChangesAsync();

                request.ConvertToDTO(createdProduct.Result);

                return new BaseResponse<CreateProductDTO>("Product Created Successfully", request, ResponseCodes.CREATED);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while Creating Product.");
                return new BaseResponse<CreateProductDTO>("An error occurred while Creating Product", ex.Message);
            }
        }

    }
}
