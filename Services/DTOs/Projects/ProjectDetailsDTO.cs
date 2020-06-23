using Plantagoo.DTOs.Tags;
using Plantagoo.DTOs.Tasks;
using System;
using System.Collections.Generic;

namespace Plantagoo.DTOs.Projects
{
    public class ProjectDetailsDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<TaskDetailsDTO> Tasks { get; set; }
        public List<TagDetailsDTO> Tags { get; set; }
    }
}
