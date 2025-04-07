using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Manager.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime Deadline { get; set; }

        // Foreign Key: Project is managed by one Manager.
        public int ManagerId { get; set; }

        [ForeignKey("ManagerId")]
        public virtual Manager? Manager { get; set; }

        // Navigation Property: One Project can have many TaskItems.
        public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
