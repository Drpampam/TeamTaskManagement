using ConfigurationService.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Collaterals
{
    public class Document : BaseEntity
    {
        [Required(ErrorMessage = "DocumentName is required")]
        public string DocumentName { get; set; } = null!;

        [Required(ErrorMessage = "Image is required")]
        public string ImageUrl { get; set; } = null!;
        public string? CollacteralId { get; set; }
    }
}