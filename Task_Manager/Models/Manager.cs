using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Task_Manager.Models
{
    public class Manager
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // Navigation Property: One Manager can manage many Projects.
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
