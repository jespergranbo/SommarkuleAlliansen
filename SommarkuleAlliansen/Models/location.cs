using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SommarkuleAlliansen.Models
{
    public class location
    {
        public int location_id { get; set; }
        public string name { get; set; }
        public string adress { get; set; }
        public DateTime date { get; set; }
    }
}