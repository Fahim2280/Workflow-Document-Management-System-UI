using System.ComponentModel.DataAnnotations;

namespace Workflow_Document_Management_System_UI.DTOs
{
    public class CreateWorkflowViewModel
    {
        [Required(ErrorMessage = "Workflow name is required")]
        [StringLength(100, ErrorMessage = "Workflow name cannot exceed 100 characters")]
        public string WorkflowName { get; set; }

        [Required(ErrorMessage = "Workflow type is required")]
        public string WorkflowType { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "At least one admin must be assigned")]
        public List<int> AssignedAdminIds { get; set; } = new List<int>();

        public List<AdminSelectViewModel> AvailableAdmins { get; set; } = new List<AdminSelectViewModel>();
    }
}
