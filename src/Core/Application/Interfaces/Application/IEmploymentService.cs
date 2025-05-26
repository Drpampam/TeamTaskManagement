
using Application.DTOs;
using Application.Responses;

namespace Application.Interfaces.Application
{
    public interface IEmploymentService<T> where T : class
    {
        Task<BaseResponse<AddEmploymentDetailsDto>> AddEmploymentDetails(AddEmploymentDetailsDto request);
    }
}
