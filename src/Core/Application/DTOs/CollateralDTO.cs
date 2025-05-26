using Domain.Entities.Collaterals;

namespace Application.DTOs
{
    public class CollateralDTO
    {
        //[Required(ErrorMessage = "CollateralName is required")]
        public string CollateralName { get; set; } = null!;

        // [Required(ErrorMessage = "SerialNumber is required")]
        public string SerialNumber { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        //[Required(ErrorMessage = "InvoiceNumber is required")]
        public string InvoiceNumber { get; set; } = null!;

        //[Required(ErrorMessage = "OtherDocument is required")]
        public DocumentDTO OtherDocument { get; set; } = new DocumentDTO();

        public void ConvertToDTO(Collateral collateral)
        {
            if (collateral == null)
            {
                new Collateral();
            }

            CollateralName = collateral.CollateralName;
            SerialNumber = collateral.SerialNumber;
            Description = collateral.Description;
            ImageUrl = collateral.Image;
            InvoiceNumber = collateral.InvoiceNumber;
        }
        public void ConvertFromDTO(Collateral collateral)
        {
            if (collateral == null)
            {
                new Collateral();
            }

            collateral.CollateralName = CollateralName;
            collateral.SerialNumber = SerialNumber;
            collateral.Description = Description;
            collateral.Image = ImageUrl;
            collateral.InvoiceNumber = InvoiceNumber;
        }
    }
}