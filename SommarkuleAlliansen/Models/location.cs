using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SommarkuleAlliansen.Models
{
    public class location
    {
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