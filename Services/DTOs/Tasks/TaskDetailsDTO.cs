using System;

namespace Plantagoo.DTOs.Tasks
{
    public class TaskDetailsDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
