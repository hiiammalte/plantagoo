using Plantagoo.Entities.ManyToMany;
using System;
using System.Collections.Generic;

namespace Plantagoo.Entities
{
    public class ProjectModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime LastUpdated { get; set; }

        public UserModel Owner { get; set; }
        public ICollection<TaskModel> Tasks { get; set; }
        public IList<MMProjectTag> ProjectTags { get; set; }
    }
}
