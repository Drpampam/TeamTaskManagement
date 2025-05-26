using Domain.Entities.EmploymentAnalysis;

namespace Application.DTOs
{
    public class AddEmploymentDetailsDto
    {
        public string? EmployerName { get; set; }
        public string? EmployerType { get; set; }
        public string? EmploymentType { get; set; }
        public string? StaffId { get; set; }
        public string? EmploymentDate { get; set; }
        public string? EmployerPhoneNo { get; set; }
        public string? EmployerEmail { get; set; }
        public string? EmployerAddress { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public string? EmploymentLetter { get; set; }
        public string? CompanyIndustry { get; set; }
        public string? CompanyId { get; set; }
        public string? CompanyName { get; set; }

        public void ConvertToDTO(EmploymentInfo emReq)
        {
            if (emReq == null)
            {
                new EmploymentInfo();
            }                

            EmployerName = emReq.EmployerName;
            EmployerType = emReq.EmployerType;
            EmploymentType = emReq.EmploymentType;
            StaffId = emReq.StaffId;
            EmploymentDate = emReq.EmploymentDate;
            EmployerPhoneNo = emReq.EmployerPhoneNo;
            EmployerEmail = emReq.EmployerEmail;
            EmployerAddress = emReq.EmployerAddress;
            MonthlyIncome = emReq.MonthlyIncome;
            EmploymentLetter = emReq.EmploymentLetter;
            CompanyIndustry = emReq.CompanyIndustry;
            CompanyId = emReq.CompanyId;
            CompanyName = emReq.CompanyName;
        }

        public void ConvertFromDTO(EmploymentInfo emReq)
        {
            if (emReq == null)
            {
                new EmploymentInfo();
            }

            emReq.EmployerName = EmployerName;
            emReq.EmployerType = EmployerType;
            emReq.EmploymentType = EmploymentType;
            emReq.EmploymentDate = EmploymentDate;
            emReq.EmployerPhoneNo = EmployerPhoneNo;
            emReq.EmployerEmail = EmployerEmail;
            emReq.EmployerAddress = EmployerAddress;
            emReq.MonthlyIncome = MonthlyIncome;
            emReq.EmploymentLetter = EmploymentLetter;
            emReq.CompanyIndustry = CompanyIndustry;
            emReq.CompanyId = CompanyId;
            emReq.CompanyName = CompanyName;
        }
    }
}