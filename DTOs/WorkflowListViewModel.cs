namespace Workflow_Document_Management_System_UI.DTOs
{
    public class WorkflowListViewModel
    {
        public List<WorkflowResponseViewModel> Workflows { get; set; } = new List<WorkflowResponseViewModel>();
        public string SearchTerm { get; set; }
        public string FilterType { get; set; }
        public bool FilterActive { get; set; } = true;
    }
}
