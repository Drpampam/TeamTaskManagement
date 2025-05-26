using System.ComponentModel.DataAnnotations;


namespace Application.DTOs
{
    public class DocumentDTO
    {
        [Required(ErrorMessage = "DocumentName is required")]
        public string DocumentName { get; set; } = null!;

        [Required(ErrorMessage = "Image is required")]
        public string ImageUrl { get; set; } = null!;
    }
}
