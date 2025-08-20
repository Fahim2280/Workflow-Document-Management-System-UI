using System.ComponentModel.DataAnnotations;

namespace Workflow_Document_Management_System_UI.DTOs
{
    public class CreateDocumentTypeDto
    {
        [Required(ErrorMessage = "Type name is required")]
        [StringLength(100, ErrorMessage = "Type name cannot exceed 100 characters")]
        [Display(Name = "Type Name")]
        public string TypeName { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Allowed extensions are required")]
        [Display(Name = "Allowed Extensions")]
        [RegularExpression(@"^(\.[a-zA-Z0-9]+)(,\.[a-zA-Z0-9]+)*$", ErrorMessage = "Extensions must be in format: .pdf,.docx,.xlsx (comma-separated with dots)")]
        public string AllowedExtensions { get; set; } = ".pdf,.docx,.xlsx,.jpg,.jpeg,.png";

        [Required(ErrorMessage = "Maximum file size is required")]
        [Range(1, 500, ErrorMessage = "Maximum file size must be between 1 and 500 MB")]
        [Display(Name = "Maximum File Size (MB)")]
        public int MaxFileSizeMB { get; set; } = 50;
    }
}
