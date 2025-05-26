using Application.DTOs;
using Application.Responses;

namespace Application.Interfaces.Application
{
    public interface IProductService<T> where T : class
    {
        Task<BaseResponse<CreateProductDTO>> CreateProduct(CreateProductDTO request);
    }
}