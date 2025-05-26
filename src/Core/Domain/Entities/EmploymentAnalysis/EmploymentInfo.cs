using ConfigurationService.Domain.Common;

namespace Domain.Entities.EmploymentAnalysis
{
    public class EmploymentInfo : BaseEntity
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
        public string? ApplicationScore { get; set; }

    }
}