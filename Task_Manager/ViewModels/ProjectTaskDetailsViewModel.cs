﻿using System;

namespace Task_Manager.ViewModels
{
    public class ProjectTaskDetailsViewModel
    {
        public string TaskName { get; set; }
        public string DeveloperName { get; set; }
        public int Progress { get; set; }
        public DateTime Deadline { get; set; }
    }
}
