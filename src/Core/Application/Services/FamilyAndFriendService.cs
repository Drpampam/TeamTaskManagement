using Application.DTOs;
using Application.Features.Constants;
using Application.Interfaces;
using Application.Interfaces.Application;
using Application.Responses;
using Domain.Entities.FamilyAndFriends;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class FamilyAndFriendService : IFamilyAndFriendService<PersonInfo>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FamilyAndFriendService> _logger;
        private readonly IAsyncRepository<PersonInfo> _fnfRepository;

        public FamilyAndFriendService(IUnitOfWork unitOfWork, ILogger<FamilyAndFriendService> logger, IAsyncRepository<PersonInfo> asyncRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _fnfRepository = asyncRepository;
        }

        public async Task<BaseResponse<ApprovalRequestDTO>> ApprovalRequest(ApprovalRequestDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.FnFType))
                {
                    return new BaseResponse<ApprovalRequestDTO>("FnFType cannot be null. Please choose type, i.e., NOK or GUARANTOR", ResponseCodes.VALIDATION_ERROR);
                }

                var application = await _fnfRepository.SingleOrDefaultAsync(x => x.Bvn == request.Bvn);

                if (application == null)
                {
                    return new BaseResponse<ApprovalRequestDTO>("Error, Could not retrieve Next of kin application", ResponseCodes.NOT_FOUND);
                }

                if (request.FnFType.ToUpper() == "NOK" || request.FnFType.ToUpper() == "GUARANTOR")
                {
                    // Verify hash for OTP
                    if (request.Otp != application.Otp || string.IsNullOrEmpty(request.Otp))
                    {
                        return new BaseResponse<ApprovalRequestDTO>("Error, Please provide a valid token", ResponseCodes.FAILURE);
                    }

                    if (string.IsNullOrEmpty(request.ApprovalStatus))
                    {
                        return new BaseResponse<ApprovalRequestDTO>("Error, Please input status, i.e., APPROVED OR REJECT", ResponseCodes.FAILURE);
                    }

                    if (request.ApprovalStatus.ToUpper() == "REJECT")
                    {
                        application.ApplicationScore = 0;
                    }
                    else if (request.ApprovalStatus.ToUpper() == "APPROVED")
                    {
                        application.ApplicationScore = 100;
                    }

                    application.ApplicationStatus = request.ApprovalStatus.ToUpper();
                    application.LastUpdatedBy = "SYSTEM";
                    application.LastUpdatedAt = DateTime.Now;

                    request.ConvertFromDTO(application);
                    var updatedApplication = await _fnfRepository.UpdateAsync(application);
                    await _unitOfWork.CommitChangesAsync();
                    request.ConvertToDTO(updatedApplication);

                    return new BaseResponse<ApprovalRequestDTO>("Success", request, ResponseCodes.UPDATED);
                }
                else
                {
                    return new BaseResponse<ApprovalRequestDTO>("Invalid FnFType. Please choose type, i.e., NOK or GUARANTOR", ResponseCodes.VALIDATION_ERROR);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred. Please see logs.");
                return new BaseResponse<ApprovalRequestDTO>("Error, Please contact support", ResponseCodes.FAILURE);
            }
        }

        public async Task<BaseResponse<AddGuarantorDTO>> AddGuarantor(AddGuarantorDTO request)
        {
            try
            {
                var guarantor = new PersonInfo
                {
                    CreatedBy = "SYSTEM",
                    CreatedAt = DateTime.Now,
                    FnFType = "Guarantor".ToUpper(),
                    ApplicationScore = 50,
                    ApplicationStatus = "Pending".ToUpper()
                };

                //Validate BVN and NIN separately, then against each other (Last Name, DOB)
                //Check if Blacklist: Is guarantor Blacklisted?
                //Sends otp to next of kin phone via sms
                //retrieve next of kin details and send verification link, compose mail from the brd

                request.ConvertFromDTO(guarantor);
                guarantor.Otp = "5846"; //delete later
                var newGuarantor = _fnfRepository.AddAsync(guarantor);
                await _unitOfWork.CommitChangesAsync();
                request.ConvertToDTO(newGuarantor.Result);

                return new BaseResponse<AddGuarantorDTO>("Guarantor Added Successfully", request, ResponseCodes.CREATED);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while adding Guarantor. {ex.Message}");
                return new BaseResponse<AddGuarantorDTO>("Error, Please contact support");
            }
        }

        public async Task<BaseResponse<AddNextOfKinDTO>> AddNextOfKin(AddNextOfKinDTO request)
        {
            try
            {
                var nok = new PersonInfo
                {
                    CreatedBy = "SYSTEM",
                    CreatedAt = DateTime.Now,
                    FnFType = "Nok".ToUpper(),
                    ApplicationScore = 50,
                    ApplicationStatus = "Pending".ToUpper()
                };

                //Validate BVN and NIN separately, then against each other (Last Name, DOB)
                //Check if Blacklist: Is guarantor Blacklisted?
                //Sends otp to next of kin phone via sms
                //retrieve next of kin details and send verification link, compose mail from the brd
                /*
                 *  Dear First name of Next of Kin,
                    Name of Applicant applied for X amount of loan for X duration and put you as Next Of Kin.
                    kindly fill in the details below, accept Terms & Conditions and accept or Reject this
                    application.                 
                 */

                request.ConvertFromDTO(nok);
                nok.Otp = "5846"; //delete later
                var newNok = _fnfRepository.AddAsync(nok);
                await _unitOfWork.CommitChangesAsync();
                request.ConvertToDTO(newNok.Result);

                return new BaseResponse<AddNextOfKinDTO>("Next of Kin Added Successfully", request, ResponseCodes.CREATED);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while adding Next of kin. {ex.Message}");
                return new BaseResponse<AddNextOfKinDTO>("Error, Please contact support");
            }
        }
    }
}