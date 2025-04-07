using System;
using System.Collections.Generic;

namespace Task_Manager.ViewModels
{
    public class ManagerDetailsViewModel
    {
        public string ManagerName { get; set; }
        public List<ProjectSummaryViewModel> Projects { get; set; }
    }

    public class ProjectSummaryViewModel
    {
        public string ProjectName { get; set; }
        public int TaskCount { get; set; }
        public DateTime Deadline { get; set; }
    }
}
