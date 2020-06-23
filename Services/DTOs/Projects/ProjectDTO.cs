using Plantagoo.DTOs.Tags;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plantagoo.DTOs.Projects
{
    public class ProjectDTO
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public List<Guid> ToLinkTags { get; set; }
        public List<TagDTO> ToAddTags { get; set; }
    }
}
