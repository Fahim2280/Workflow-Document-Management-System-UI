namespace Workflow_Document_Management_System_UI.DTOs
{
    public class WorkflowResponseViewModel
    {
        public int WorkflowId { get; set; }
        public string WorkflowName { get; set; }
        public string WorkflowType { get; set; }
        public string Description { get; set; }
        public int CreatedByAdminId { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string AssignedAdmins { get; set; }
        public List<AdminSummaryViewModel> AssignedAdminsList { get; set; } = new List<AdminSummaryViewModel>();
    }
}
