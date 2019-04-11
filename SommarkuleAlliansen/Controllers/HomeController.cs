using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;
using SommarkuleAlliansen.Models;

namespace SommarkuleAlliansen.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<employe> employes = new List<employe>();
            string constr = ConfigurationManager.ConnectionStrings["smconnection"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT * FROM employe";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            employes.Add(new employe
                            {
                                employe_id = Convert.ToInt32(sdr["employe_id"]),
                                employe_type = Convert.ToInt32(sdr["employe_type"]),
                                name = sdr["name"].ToString(),
                                number = Convert.ToInt32(sdr["number"]),
                                password = sdr["password"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return View(employes);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}