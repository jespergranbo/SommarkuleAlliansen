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
        public int group_id { get; set; }
        public int location_id { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public int post_number { get; set; }
        public bool tax { get; set; }
        public string bank { get; set; }
        public int clearing { get; set; }
        public int account_number { get; set; }
        public string shirt_size { get; set; }
        public int social_security { get; set; }


    }
}