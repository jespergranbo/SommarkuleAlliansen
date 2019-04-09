using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SommarkuleAlliansen.Models
{
    public class employe
    {
        public int employe_id { get; set; }
        public int employe_type { get; set; }
        public string name { get; set; }
        public int number { get; set; }
        public string password { get; set; }
    }
}