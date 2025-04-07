namespace Task_Manager.ViewModels
{
    public class DashboardViewModel
    {
        public int ProjectCount { get; set; }
        public int DeveloperCount { get; set; }
        public int ManagerCount { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OverdueTasks { get; set; }
        public decimal CompletionRate { get; set; }
        public decimal OverdueRate { get; set; }
        
        // For charts
        public List<string> ProjectNames { get; set; } = new List<string>();
        public List<decimal> ProjectCompletionRates { get; set; } = new List<decimal>();
    }
}
