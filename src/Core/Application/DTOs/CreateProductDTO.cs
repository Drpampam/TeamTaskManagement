using Domain.Entities.Payment;

namespace Application.DTOs
{
    public class CreateProductDTO
    {
        public string Vendor { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ModelNumber { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;

        public void ConvertToDTO(Product product)
        {
            if (product == null)
            {
                new Product();
            }

            //Id = product.Id;
            ProductName = product.ProductName;
            ProductType = product.ProductType;
            Brand = product.Brand;
            ModelNumber = product.ModelNumber;
            Amount = product.Amount;
            InvoiceNumber = product.InvoiceNumber;
            Vendor = product.Vendor;
            SerialNumber = product.SerialNumber;         
        }

        public void ConvertFromDTO(Product product)
        {
            if (product == null)
            {
                new Product();
            }

            product.ProductName = ProductName;
            product.ProductType = ProductType;
            product.Brand = Brand;
            product.ModelNumber = ModelNumber;
            product.Amount = Amount;
            product.InvoiceNumber = InvoiceNumber;
            product.Vendor = Vendor;
            product.SerialNumber = SerialNumber;
        }
    }
}