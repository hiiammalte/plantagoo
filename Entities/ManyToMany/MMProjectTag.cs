using System;

namespace Plantagoo.Entities.ManyToMany
{
    public class MMProjectTag
    {
        public Guid ProjectId { get; set; }
        public ProjectModel Project { get; set; }

        public Guid TagId { get; set; }
        public TagModel Tag { get; set; }
    }
}
