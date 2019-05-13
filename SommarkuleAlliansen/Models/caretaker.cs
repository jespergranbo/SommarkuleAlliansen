using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SommarkuleAlliansen.Models
{
    public class caretaker
    {
        public int caretaker_id { get; set; }
        public string caretaker_name { get; set; }
        public int caretaker_number { get; set; }
        public string caretaker_email { get; set; }
        public string adress { get; set; }
        public string alternative_name { get; set; }
        public int alternative_number { get; set; }
        public double debt { get; set; }
        public double paid { get; set; }
        public int count { get; set; }
        public bool selectedForEmail { get; set; }
    }
}