
using Application.DTOs;
using Application.Features.Constants;
using Application.Interfaces;
using Application.Interfaces.Application;
using Application.Responses;
using Domain.Entities.EmploymentAnalysis;
using Domain.Entities.FamilyAndFriends;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class EmploymentService : IEmploymentService<EmploymentInfo>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EmploymentService> _logger;
        private readonly IAsyncRepository<EmploymentInfo> _employmentRepository;

        public EmploymentService(IUnitOfWork unitOfWork, ILogger<EmploymentService> logger, IAsyncRepository<EmploymentInfo> asyncRepository)
        {
            _employmentRepository = asyncRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BaseResponse<AddEmploymentDetailsDto>> AddEmploymentDetails(AddEmploymentDetailsDto request)
        {
            try
            {
                var emp = new EmploymentInfo
                {
                    CreatedBy = "SYSTEM",
                    CreatedAt = DateTime.Now,
                };

                /*  Score
                    1. Employer type: If on Preferred company list = 60
                    1. If not on preferred company list = 35
                    2. If Employment letter, if other details confirmed ok by backend = 40
                    1. If other details not confirmed ok from the backend = 0
                    5. Send processed data to LEMM: Application ID, Score
                    1. Only Send to LEMM once Approved or Reject.
                    2. Expose APIs for approval and rejection               
                */

                request.ConvertFromDTO(emp);
                var newNok = _employmentRepository.AddAsync(emp);
                await _unitOfWork.CommitChangesAsync();
                request.ConvertToDTO(newNok.Result);

                return new BaseResponse<AddEmploymentDetailsDto>("Next of Kin Added Successfully", request, ResponseCodes.CREATED);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while adding Next of kin. {ex.Message}");
                return new BaseResponse<AddEmploymentDetailsDto>("Error, Please contact support");
            }
        }
    }
}
