using System;
using System.Collections.Generic;

namespace Plantagoo.Entities
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<ProjectModel> Projects { get; set; }
        public ICollection<TagModel> Tags { get; set; }
    }
}
