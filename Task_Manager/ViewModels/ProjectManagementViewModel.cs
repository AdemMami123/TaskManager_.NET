namespace Task_Manager.ViewModels
{
    public class ProjectManagementViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ManagerName { get; set; }
        public int TaskCount { get; set; }
        // Here ProgressRate is calculated as the average of the tasks’ Progress values.
        public double ProgressRate { get; set; }
    }
}
