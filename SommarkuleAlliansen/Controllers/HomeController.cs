using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace SommarkuleAlliansen.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string mainconn = ConfigurationManager.ConnectionStrings["smconnection"].ConnectionString;
            MySqlConnection sqlconn = new MySqlConnection(mainconn);
            MySqlCommand comm = new MySqlCommand("select * from employe");
            {
                MySqlDataAdapter da = new MySqlDataAdapter();
                comm.Connection = sqlconn;
                da.SelectCommand = comm;
                DataTable dt = new DataTable();
                {
                    da.Fill(dt);
                    return View(dt);
                }
            }
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