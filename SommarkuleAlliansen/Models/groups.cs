using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SommarkuleAlliansen.Models
{
    public class groups
    {
        public int group_id { get; set; }
        public int child_id { get; set; }
        public int group_number { get; set; }
        public bool present { get; set; }
    }
}