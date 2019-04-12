using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using SommarkuleAlliansen.Models;

namespace SommarkuleAlliansen.Controllers
{
    public class EmployeController : Controller
    {
        string constr = ConfigurationManager.ConnectionStrings["smconnection"].ConnectionString;
        public ActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(employe account)
        {
            List<employe> employes = new List<employe>();
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
                        foreach(employe emp in employes)
                        {
                            if (emp.name == account.name && emp.password == account.password)
                            {
                                Session["employe_id"] = emp.employe_id.ToString();
                                Session["name"] = emp.name.ToString();
                                Session["employe_type"] = emp.employe_type.ToString();
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Användarnamnet eller lösenordet är fel");
                            }
                        }
                    }
                    con.Close();
                }
            }
            return View();
        }
        public ActionResult Caretaker()
        {
            List<caretaker> caretakers = new List<caretaker>();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT * FROM caretaker";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            caretakers.Add(new caretaker
                            {
                                caretaker_id = Convert.ToInt32(sdr["caretaker_id"]),
                                caretaker_name = Convert.ToString(sdr["caretaker_name"]),
                                caretaker_number = Convert.ToInt32(sdr["caretaker_number"]),
                                caretaker_email = Convert.ToString(sdr["caretaker_email"]),
                                adress = Convert.ToString(sdr["address"]),
                                alternative_name = Convert.ToString(sdr["alternative_name"]),
                                alternative_number = Convert.ToInt32(sdr["alternative_number"]),
                                debt = Convert.ToDouble(sdr["debt"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return View(caretakers);
        }
    }
}