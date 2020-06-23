using System.ComponentModel.DataAnnotations;

namespace Plantagoo.DTOs.Tasks
{
    public class TaskDTO
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
