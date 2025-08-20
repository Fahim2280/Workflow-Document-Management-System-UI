namespace Workflow_Document_Management_System_UI.DTOs
{
    public class WorkflowAnalyticsViewModel
    {
        public int WorkflowId { get; set; }
        public string WorkflowName { get; set; }
        public string WorkflowType { get; set; }
        public int TotalDocuments { get; set; }
        public int CompletedDocuments { get; set; }
        public int PendingDocuments { get; set; }
        public int RejectedDocuments { get; set; }
        public double AverageCompletionTimeHours { get; set; }
        public int OverdueDocuments { get; set; }
        public List<AdminWorkloadViewModel> AdminWorkloads { get; set; } = new List<AdminWorkloadViewModel>();
        public Dictionary<string, int> DocumentsByStatus { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, double> AverageTimeByStep { get; set; } = new Dictionary<string, double>();

        // Calculated properties
        public double CompletionRate => TotalDocuments > 0 ? (double)CompletedDocuments / TotalDocuments * 100 : 0;
        public string AverageCompletionTimeFormatted => AverageCompletionTimeHours > 0
            ? TimeSpan.FromHours(AverageCompletionTimeHours).ToString(@"dd\.hh\:mm")
            : "N/A";
    }

}
