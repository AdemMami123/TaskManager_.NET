namespace Task_Manager.ViewModels
{
    public class DeveloperManagementViewModel
    {
        public int DeveloperId { get; set; }
        public string DeveloperName { get; set; }
        public int ProjectCount { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public double ProgressRate { get; set; }
    }
}
