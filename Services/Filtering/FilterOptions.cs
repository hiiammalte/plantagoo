using System;
using System.Collections.Generic;
using System.Text;

namespace Plantagoo.Filtering
{
    public class FilterOptions
    {
        public string SearchTerm { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 50;
        public string OrderBy { get; set; }
    }
}
