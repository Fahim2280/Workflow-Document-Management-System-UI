namespace Workflow_Document_Management_System_UI.DTOs
{
    public class WorkflowDetailViewModel
    {
        public int WorkflowId { get; set; }
        public string WorkflowName { get; set; }
        public string WorkflowType { get; set; }
        public string Description { get; set; }
        public int CreatedByAdminId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public List<AssignedAdminViewModel> AssignedAdmins { get; set; } = new List<AssignedAdminViewModel>();
    }
}
