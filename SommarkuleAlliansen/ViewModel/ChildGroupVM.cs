using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SommarkuleAlliansen.ViewModel
{
    public class ChildGroupVM
    {
        public int child_id { get; set; }
        public string name { get; set; }
        public string comment { get; set; }
        public int caretaker_id { get; set; }
        public bool can_swim { get; set; }
        public DateTime birth_date { get; set; }
        public bool allow_photos { get; set; }
        public bool vaccinated { get; set; }
        public string shirt_size { get; set; }
        public int group_id { get; set; }
        public bool present { get; set; }
        
        public int location_id { get; set; }
        public string weeks { get; set; }
        public int birth_year { get; set; }
        public int count { get; set; }
    }
}