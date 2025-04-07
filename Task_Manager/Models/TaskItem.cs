using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Manager.Models
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        //isCompelete or not 
        // Indicates if the task is completed.
        public bool IsCompleted { get; set; }

        // Foreign Key: Task is assigned to one Developer.
        public int DeveloperId { get; set; }

        [ForeignKey("DeveloperId")]
        public virtual Developer? Developer { get; set; }

        // Foreign Key: Task belongs to one Project.
        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project? Project { get; set; }

        public int Progress { get; set; }

        public DateTime Deadline { get; set; }

        // Optional: If you need a separate advancement field.
        public float Advancement { get; set; }
    }
}
