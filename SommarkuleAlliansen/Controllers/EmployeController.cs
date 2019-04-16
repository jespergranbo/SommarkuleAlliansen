using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using SommarkuleAlliansen.Models;
using SommarkuleAlliansen.ViewModel;

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
            if (Session["employe_id"] != null)
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
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Child()
        {
            if (Session["employe_id"] != null)
            {
                List<child> children = new List<child>();
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "SELECT child.child_id, child.name, child.birth_date, child.shirt_size, caretaker.caretaker_name, location.location_name, location.weeks FROM child " +
                        "INNER JOIN caretaker ON child.caretaker_id = caretaker.caretaker_id " +
                        "INNER JOIN location ON child.location_id = location.location_id";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                children.Add(new child
                                {
                                    child_id = Convert.ToInt32(sdr["child_id"]),
                                    name = Convert.ToString(sdr["name"]),
                                    caretaker_name = Convert.ToString(sdr["caretaker_name"]),
                                    location_name = Convert.ToString(sdr["location_name"]),
                                    weeks = Convert.ToString(sdr["weeks"]),
                                    birth_date = Convert.ToDateTime(sdr["birth_date"]),
                                    shirt_size = Convert.ToInt32(sdr["shirt_size"])
                                });
                            }
                        }
                        con.Close();
                    }
                }
                return View(children);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Edit(int? id)
        {
            if (Session["employe_id"] != null)
            {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            caretaker caretaker = new caretaker();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT * FROM caretaker WHERE caretaker_id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@id", id);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            caretaker.caretaker_id = Convert.ToInt32(sdr["caretaker_id"]);
                            caretaker.caretaker_name = Convert.ToString(sdr["caretaker_name"]);
                            caretaker.caretaker_number = Convert.ToInt32(sdr["caretaker_number"]);
                            caretaker.caretaker_email = Convert.ToString(sdr["caretaker_email"]);
                            caretaker.adress = Convert.ToString(sdr["address"]);
                            caretaker.alternative_name = Convert.ToString(sdr["alternative_name"]);
                            caretaker.alternative_number = Convert.ToInt32(sdr["alternative_number"]);
                            caretaker.debt = Convert.ToDouble(sdr["debt"]);
                        }
                    }
                    con.Close();
                }
            }
                if (caretaker == null)
                {
                    return HttpNotFound();
                }
            return View(caretaker);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "caretaker_id,caretaker_name,caretaker_number,caretaker_email,adress,alternative_name,alternative_number,debt")] caretaker caretaker)
        {
            if (ModelState.IsValid)
            {
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "UPDATE caretaker SET caretaker_name = @caretaker_name, caretaker_number = @caretaker_number, caretaker_email = @caretaker_email, address = @address, alternative_name = @alternative_name, alternative_number = @alternative_number, debt = @debt WHERE caretaker_id = @id;";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", caretaker.caretaker_id);
                        cmd.Parameters.AddWithValue("@caretaker_name", caretaker.caretaker_name);
                        cmd.Parameters.AddWithValue("@caretaker_number", caretaker.caretaker_number);
                        cmd.Parameters.AddWithValue("@caretaker_email", caretaker.caretaker_email);
                        cmd.Parameters.AddWithValue("@address", caretaker.adress);
                        cmd.Parameters.AddWithValue("@alternative_name", caretaker.alternative_name);
                        cmd.Parameters.AddWithValue("@alternative_number", caretaker.alternative_number);
                        cmd.Parameters.AddWithValue("@debt", caretaker.debt);
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                        }
                        con.Close();
                    }
                }
                return RedirectToAction("Caretaker");
            }
            return View(caretaker);
        }
        public ActionResult Details(int? id)
        {
            if (Session["employe_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                List<ChildCaretakerVM> caretakerDetails = new List<ChildCaretakerVM>();
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "SELECT child.name, child.birth_date, child.comment, caretaker.caretaker_name, caretaker.caretaker_number, caretaker.caretaker_email, caretaker.address, caretaker.debt, caretaker.alternative_name, caretaker.alternative_number, location.location_name, location.location_address, location.start_date, location.end_date " +
                        "FROM child INNER JOIN caretaker ON child.caretaker_id = caretaker.caretaker_id INNER JOIN location on child.location_id = location.location_id WHERE caretaker.caretaker_id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id);
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                caretakerDetails.Add(new ChildCaretakerVM
                                {
                                    name = Convert.ToString(sdr["name"]),
                                    birth_date = Convert.ToDateTime(sdr["birth_date"]),
                                    comment = Convert.ToString(sdr["comment"]),
                                    caretaker_name = Convert.ToString(sdr["caretaker_name"]),
                                    caretaker_number = Convert.ToInt32(sdr["caretaker_number"]),
                                    caretaker_email = Convert.ToString(sdr["caretaker_email"]),
                                    alternative_name = Convert.ToString(sdr["alternative_name"]),
                                    alternative_number = Convert.ToInt32(sdr["alternative_number"]),
                                    adress = Convert.ToString(sdr["address"]),
                                    debt = Convert.ToDouble(sdr["debt"]),
                                    location_name = Convert.ToString(sdr["location_name"]),
                                    location_adress = Convert.ToString(sdr["location_address"]),
                                    start_date = Convert.ToDateTime(sdr["start_date"]),
                                    end_date = Convert.ToDateTime(sdr["end_date"])
                                });
                            }
                        }
                        con.Close();
                    }
                }
                if (caretakerDetails == null)
                {
                    return HttpNotFound();
                }
                return View(caretakerDetails);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}