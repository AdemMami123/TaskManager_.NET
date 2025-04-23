using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Task_Manager.Models;

namespace Task_Manager.ViewModels
{
    public class ProjectCreateViewModel
    {
        // Project properties
        [Required]
        public string Name { get; set; }

        [Required]
        public int ManagerId { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        // Tasks collection
        public List<TaskCreateModel> Tasks { get; set; } = new List<TaskCreateModel>();
    }

    public class TaskCreateModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int DeveloperId { get; set; }

        public DateTime? Deadline { get; set; }
    }
}
