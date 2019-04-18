using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SommarkuleAlliansen.ViewModel
{
    public class ChildCaretakerLocationVM
    {
        public int caretaker_id { get; set; }
        public string caretaker_name { get; set; }
        public int caretaker_number { get; set; }
        public string caretaker_email { get; set; }
        public string adress { get; set; }
        public string alternative_name { get; set; }
        public int alternative_number { get; set; }
        public double debt { get; set; }

        public int child_id { get; set; }
        public string name { get; set; }
        public string comment { get; set; }
        public bool can_swim { get; set; }
        public DateTime birth_date { get; set; }
        public bool allow_photos { get; set; }
        public bool vaccinated { get; set; }
        public string shirt_size { get; set; }

        public int location_id { get; set; }
        public string location_name { get; set; }
        public string location_adress { get; set; }
        public string location_email { get; set; }
        public int location_number { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string weeks { get; set; }
        public int price { get; set; }
    }
}