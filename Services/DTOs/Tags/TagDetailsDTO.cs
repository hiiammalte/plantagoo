using System;

namespace Plantagoo.DTOs.Tags
{
    public class TagDetailsDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int AssociatedProjectsCount { get; set; }
    }
}
