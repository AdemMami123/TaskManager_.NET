using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Task_Manager.Models
{
    public class Developer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Position { get; set; }

        // Navigation Property: One Developer can have many TaskItems.
        public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
