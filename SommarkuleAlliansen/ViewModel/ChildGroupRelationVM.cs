using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SommarkuleAlliansen.ViewModel
{
    public class ChildGroupRelationVM
    {
        public int childGroupRelation_id { get; set; }
        public int group_id { get; set; }
        public int group_id2 { get; set; }

        public int child_id { get; set; }
        public string name { get; set; }
        public string comment { get; set; }
        public string allergy_comment { get; set; }
        public int caretaker_id { get; set; }
        public bool can_swim { get; set; }
        public DateTime birth_date { get; set; }
        public bool allow_photos { get; set; }
        public bool vaccinated { get; set; }
        public string shirt_size { get; set; }
        public bool present { get; set; }
        public DateTime registration_date { get; set; }
        public int social_security { get; set; }
    }
}