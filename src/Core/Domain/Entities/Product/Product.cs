using ConfigurationService.Domain.Common;

namespace Domain.Entities.Payment
{
    public class Product : BaseEntity
    {
        public string Vendor { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ModelNumber { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
    }
}