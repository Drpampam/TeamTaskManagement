using ConfigurationService.Domain.Common;

namespace Domain.Entities.Collaterals
{
    public class Collateral : BaseEntity
    {
        public string CollateralName { get; set; } = null!;
        public string SerialNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public string DocumentId { get; set; } = string.Empty;
        public int? Score { get; set; }
    }
}