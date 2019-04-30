using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SommarkuleAlliansen.Models
{
    public class ChildGroupRelation
    {
        public int childGroupRelation_id { get; set; }
        public int child_id { get; set; }
        public int group_id { get; set; }
    }
}