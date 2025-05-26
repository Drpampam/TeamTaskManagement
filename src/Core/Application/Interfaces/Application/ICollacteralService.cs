using Application.DTOs;
using Application.Responses;

namespace Application.Interfaces.Application
{
    public interface ICollacteralService<T> where T : class
    {
        Task<BaseResponse<CollateralDTO>> AddCollateral(CollateralDTO collateral);
    }
}
