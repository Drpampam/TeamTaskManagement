using Application.DTOs;
using Application.Responses;

namespace Application.Interfaces.Application
{
    public interface IFamilyAndFriendService<T> where T : class
    {
        Task<BaseResponse<ApprovalRequestDTO>> ApprovalRequest(ApprovalRequestDTO request);
        Task<BaseResponse<AddGuarantorDTO>> AddGuarantor(AddGuarantorDTO request);
        Task<BaseResponse<AddNextOfKinDTO>> AddNextOfKin(AddNextOfKinDTO request);
    }
}
