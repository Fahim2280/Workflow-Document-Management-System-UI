namespace Workflow_Document_Management_System_UI.DTOs
{
    public class AdminWorkloadViewModel
    {
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public int AssignedCount { get; set; }
        public int CompletedCount { get; set; }
        public int PendingCount { get; set; }
        public int OverdueCount { get; set; }
        public double AverageCompletionTimeHours { get; set; }
        public double WorkloadPercentage { get; set; }

        // Calculated properties
        public double CompletionRate => AssignedCount > 0 ? (double)CompletedCount / AssignedCount * 100 : 0;
        public string AverageCompletionTimeFormatted => AverageCompletionTimeHours > 0
            ? TimeSpan.FromHours(AverageCompletionTimeHours).ToString(@"dd\.hh\:mm")
            : "N/A";
    }
}
