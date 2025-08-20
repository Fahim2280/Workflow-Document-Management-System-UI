namespace Workflow_Document_Management_System_UI.DTOs
{
    public class AnalyticsFilterViewModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? WorkflowId { get; set; }
        public List<WorkflowSelectViewModel> AvailableWorkflows { get; set; } = new List<WorkflowSelectViewModel>();
    }
}
