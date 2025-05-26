using Application.DTOs;
using Application.Features.Constants;
using Application.Interfaces;
using Application.Interfaces.Application;
using Application.Responses;
using Domain.Entities.Collaterals;
using Microsoft.Extensions.Logging;
using Document = Domain.Entities.Collaterals.Document;

namespace Application.Services
{
    public class CollacteralService : ICollacteralService<Collateral>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CollacteralService> _logger;
        private readonly IAsyncRepository<Collateral> _collacteralRepository;
        private readonly IAsyncRepository<Document> _docRepository;

        public CollacteralService(IUnitOfWork unitOfWork, ILogger<CollacteralService> logger,
            IAsyncRepository<Collateral> colRepository, IAsyncRepository<Document> docRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _collacteralRepository = colRepository;
            _docRepository = docRepository;
        }

        public async Task<BaseResponse<CollateralDTO>> AddCollateral(CollateralDTO collateral)
        {
            try
            {
                var docValidation = CalculateScoreAsync(collateral);
                
                var c1 = new Collateral()
                {
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "SYSTEM",
                };
                c1.Score = docValidation;
                var doc = new Document
                {
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "SYSTEM",
                    DocumentName = collateral.OtherDocument.DocumentName,
                    ImageUrl = collateral.OtherDocument.ImageUrl,
                    CollacteralId = c1.Id
                };
                c1.DocumentId = doc.Id;

                collateral.ConvertFromDTO(c1);

                var newCollacteral = _collacteralRepository.AddAsync(c1);
                await _docRepository.AddAsync(doc);
                await _unitOfWork.CommitChangesAsync();

                collateral.ConvertToDTO(newCollacteral.Result);
                return new BaseResponse<CollateralDTO>($"Success, Collacteral score {docValidation}", collateral, ResponseCodes.CREATED);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while Adding Collacteral.");
                return new BaseResponse<CollateralDTO>("An error occurred while Adding Collacteral", ex.Message);
            }
        }

        private int CalculateScoreAsync(CollateralDTO collateralDto)
        {
            int score = 0;
            int filledInputsCount = 0;

            // Check if CollateralName is filled
            if (!string.IsNullOrEmpty(collateralDto.CollateralName))
            {
                filledInputsCount++;
            }

            // Check if SerialNumber is filled
            if (!string.IsNullOrEmpty(collateralDto.SerialNumber))
            {
                filledInputsCount++;
            }

            // Check if Description is filled
            if (!string.IsNullOrEmpty(collateralDto.Description))
            {
                filledInputsCount++;
            }

            // Check if Image is filled
            if (!string.IsNullOrEmpty(collateralDto.ImageUrl))
            {
                filledInputsCount++;
            }

            // Check if InvoiceNumber is filled
            if (!string.IsNullOrEmpty(collateralDto.InvoiceNumber))
            {
                filledInputsCount++;
            }

            // Check if OtherDocument is filled
            if (collateralDto.OtherDocument != null)
            {
                filledInputsCount++;
            }

            // Calculate score based on filled inputs count
            if (filledInputsCount >= 6)
            {
                score = 100;
            }
            else if (filledInputsCount < 6 && filledInputsCount != 0)
            {
                score = 45;
            }
            else
            {
                score = 0;
            }

            return score;
        }
    }
}