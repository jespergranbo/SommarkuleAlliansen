﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
            employe user = new employe();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT * FROM employe WHERE name = @name AND password = @password";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@name", account.name);
                    cmd.Parameters.AddWithValue("@password", account.password);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            user.employe_id = Convert.ToInt32(sdr["employe_id"]);
                            user.employe_type = Convert.ToInt32(sdr["employe_type"]);
                            user.name = sdr["name"].ToString();
                            user.number = Convert.ToInt32(sdr["number"]);
                            user.password = sdr["password"].ToString();
                            user.group_id = Convert.ToInt32(sdr["group_id"]);
                            user.location_id = Convert.ToInt32(sdr["group_id"]);
                            Session["employe_id"] = user.employe_id.ToString();
                            Session["name"] = user.name.ToString();
                            Session["employe_type"] = user.employe_type.ToString();
                            Session["group_id"] = user.group_id.ToString();
                            Session["location_id"] = user.group_id.ToString();
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    con.Close();
                    ModelState.AddModelError("", "Användarnamnet eller lösenordet är fel");
                }
            }
          return View();
        }
        public ActionResult LogOut()
        {
            Session["employe_id"] = null;
            Session["name"] = null;
            Session["employe_type"] = null;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Employe()
        {
            if (Session["employe_id"] != null)
            {
                List<EmployeLocationVM> employes = new List<EmployeLocationVM>();
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "SELECT employe.employe_id, employe.employe_type, employe.name, employe.number, employe.group_id, location.location_id, location.location_name FROM employe" +
                        " INNER JOIN location ON employe.location_id = location.location_id";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                employes.Add(new EmployeLocationVM
                                {
                                    employe_id = Convert.ToInt32(sdr["employe_id"]),
                                    employe_type = Convert.ToInt32(sdr["employe_type"]),
                                    name = Convert.ToString(sdr["name"]),
                                    number = Convert.ToInt32(sdr["number"]),
                                    location_id = Convert.ToInt32(sdr["location_id"]),
                                    group_id = Convert.ToInt32(sdr["group_id"]),
                                    location_name = Convert.ToString(sdr["location_name"])
                                });
                            }
                        }
                        con.Close();
                    }
                }
                return View(employes);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
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
                List<ChildCaretakerLocationVM> children = new List<ChildCaretakerLocationVM>();
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
                                children.Add(new ChildCaretakerLocationVM
                                {
                                    child_id = Convert.ToInt32(sdr["child_id"]),
                                    name = Convert.ToString(sdr["name"]),
                                    caretaker_name = Convert.ToString(sdr["caretaker_name"]),
                                    location_name = Convert.ToString(sdr["location_name"]),
                                    weeks = Convert.ToString(sdr["weeks"]),
                                    birth_date = Convert.ToDateTime(sdr["birth_date"]),
                                    shirt_size = Convert.ToString(sdr["shirt_size"])
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
        public ActionResult EditEmploye(int? id)
        {
            if (Session["employe_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                employe employe = new employe();
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "SELECT * FROM employe WHERE employe_id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id);
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                employe.employe_id = Convert.ToInt32(sdr["employe_id"]);
                                employe.employe_type = Convert.ToInt32(sdr["employe_type"]);
                                employe.name = Convert.ToString(sdr["name"]);
                                employe.number = Convert.ToInt32(sdr["number"]);
                                employe.password = Convert.ToString(sdr["password"]);
                                employe.group_id = Convert.ToInt32(sdr["group_id"]);
                                employe.location_id = Convert.ToInt32(sdr["location_id"]);
                            }
                        }
                        con.Close();
                    }
                }
                if (employe == null)
                {
                    return HttpNotFound();
                }
                return View(employe);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEmploye([Bind(Include = "employe_id,employe_type,name,number,password,group_id,location_id")] employe employe)
        {
            if (ModelState.IsValid)
            {
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "UPDATE employe SET name = @name, employe_type = @employe_type, number = @number, password = @password, group_id = @group_id, location_id = @location_id WHERE employe_id = @id;";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", employe.employe_id);
                        cmd.Parameters.AddWithValue("@name", employe.name);
                        cmd.Parameters.AddWithValue("@employe_type", employe.employe_type);
                        cmd.Parameters.AddWithValue("@number", employe.number);
                        cmd.Parameters.AddWithValue("@password", employe.password);
                        cmd.Parameters.AddWithValue("@group_id", employe.group_id);
                        cmd.Parameters.AddWithValue("@location_id", employe.location_id);
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                        }
                        con.Close();
                    }
                }
                return RedirectToAction("Employe");
            }
            return View(employe);
        }
        public ActionResult DeleteEmploye(int? id)
        {
            if (Session["employe_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                employe employe = new employe();
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "SELECT * FROM employe WHERE employe_id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id);
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                employe.employe_id = Convert.ToInt32(sdr["employe_id"]);
                                employe.employe_type = Convert.ToInt32(sdr["employe_type"]);
                                employe.name = Convert.ToString(sdr["name"]);
                                employe.number = Convert.ToInt32(sdr["number"]);
                                employe.password = Convert.ToString(sdr["password"]);
                                employe.group_id = Convert.ToInt32(sdr["group_id"]);
                                employe.location_id = Convert.ToInt32(sdr["location_id"]);
                            }
                        }
                        con.Close();
                    }
                }
                if (employe == null)
                {
                    return HttpNotFound();
                }
                return View(employe);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult DeleteEmploye(int id)
        {
            employe employe = new employe();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "DELETE FROM employe WHERE employe_id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@id", id);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                    }
                    con.Close();
                }
            }
            return RedirectToAction("Employe");
        }
        public ActionResult CreateEmploye()
        {
            if (Session["employe_id"] != null)
            {
                List<EmployeGroupLocationVM> locations = new List<EmployeGroupLocationVM>();
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "SELECT groups.group_id, groups.location_id, groups.birth_year, location.location_name, location.weeks FROM groups INNER JOIN location ON groups.location_id = location.location_id ORDER BY group_id ASC";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                locations.Add(new EmployeGroupLocationVM
                                {
                                group_id = Convert.ToInt32(sdr["group_id"]),
                                location_id = Convert.ToInt32(sdr["location_id"]),
                                birth_year = Convert.ToInt32(sdr["birth_year"]),
                                location_name = Convert.ToString(sdr["location_name"]),
                                weeks = Convert.ToString(sdr["weeks"])
                                });
                            }
                        }
                        con.Close();
                    }
                }
                if (locations == null)
                {
                    return HttpNotFound();
                }
                return View(locations);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult CreateEmploye(int employe_type, string name, int number, string password, int group_id)
        {
            if (ModelState.IsValid)
            {
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    long location_id = 0;
                    string query = "SELECT location_id FROM groups WHERE group_id = @group_id";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@group_id", group_id);
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while(sdr.Read())
                            {
                                location_id = Convert.ToInt32(sdr["location_id"]);
                            }
                        }
                        con.Close();
                    }
                    query = "INSERT INTO employe (employe_id, name, employe_type, number, password, group_id, location_id) VALUES (NULL, @name, @employe_type, @number, @password, @group_id, @location_id)";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@employe_type", employe_type);
                        cmd.Parameters.AddWithValue("@number", number);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@group_id", group_id);
                        cmd.Parameters.AddWithValue("@location_id", location_id);
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                        }
                        con.Close();
                    }
                }
                return RedirectToAction("Employe");
            }
            return View();
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
        public ActionResult Details(int? id, bool justSentMessage)
        {
            if (Session["employe_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                List<ChildCaretakerLocationVM> caretakerDetails = new List<ChildCaretakerLocationVM>();
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "SELECT child.name, child.birth_date, child.comment, caretaker.caretaker_name, caretaker.caretaker_number, caretaker.caretaker_email, caretaker.address, caretaker.debt, caretaker.alternative_name, caretaker.alternative_number, location.location_name, location.location_address, location.start_date, location.end_date, location.weeks " +
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
                                caretakerDetails.Add(new ChildCaretakerLocationVM
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
                                    end_date = Convert.ToDateTime(sdr["end_date"]),
                                    weeks = Convert.ToString(sdr["weeks"])
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
                if (justSentMessage == true)
                {
                    ViewData["error"] = "Betalningspåminnelsen har skickats!";
                }
                return View(caretakerDetails);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Details(string caretaker_name, string caretaker_email, int debt, int caretaker_id)
        {
            if (ModelState.IsValid)
            {
                var body = "Hej " + caretaker_name + "! Du har fortfarande inte betalat din skuld på " + debt + ":- vänligen gör detta så snart som möjligt. Mvh Sommarkulan";
                var message = new MailMessage();
                message.To.Add(new MailAddress(caretaker_email));
                message.From = new MailAddress("sommarkulan@outlook.com");
                message.Subject = "Betalningspåminnelse";
                message.Body = string.Format(body);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                    bool justSentMessage = true;
                    return RedirectToAction("Details", "Employe", new { caretaker_id = caretaker_id, justSentMessage = justSentMessage});
                }
            }
            return View();
        }
        public ActionResult EditChild(int? id)
        {
            if (Session["employe_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                child child = new child();
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "SELECT * FROM child WHERE child_id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id);
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                child.child_id = Convert.ToInt32(sdr["child_id"]);
                                child.name = Convert.ToString(sdr["name"]);
                                child.comment = Convert.ToString(sdr["comment"]);
                                child.can_swim = Convert.ToBoolean(sdr["can_swim"]);
                                child.birth_date = Convert.ToDateTime(sdr["birth_date"]);
                                child.allow_photos = Convert.ToBoolean(sdr["allow_photos"]);
                                child.vaccinated = Convert.ToBoolean(sdr["vaccinated"]);
                                child.shirt_size = Convert.ToString(sdr["shirt_size"]);
                            }
                        }
                        con.Close();
                    }
                }
                if (child == null)
                {
                    return HttpNotFound();
                }
                return View(child);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditChild([Bind(Include = "child_id,name,comment,can_swim,birth_date,allow_photos,vaccinated,shirt_size")] child child)
        {
            if (ModelState.IsValid)
            {
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "UPDATE child SET name = @name, comment = @comment, can_swim = @can_swim, birth_date = @birth_date, allow_photos = @allow_photos, vaccinated = @vaccinated, shirt_size = @shirt_size WHERE child_id = @id;";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", child.child_id);
                        cmd.Parameters.AddWithValue("@name", child.name);
                        cmd.Parameters.AddWithValue("@comment", child.comment);
                        cmd.Parameters.AddWithValue("@can_swim", child.can_swim);
                        cmd.Parameters.AddWithValue("@birth_date", child.birth_date);
                        cmd.Parameters.AddWithValue("@allow_photos", child.allow_photos);
                        cmd.Parameters.AddWithValue("@vaccinated", child.vaccinated);
                        cmd.Parameters.AddWithValue("@shirt_size", child.shirt_size);
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                        }
                        con.Close();
                    }
                }
                return RedirectToAction("Child");
            }
            return View(child);
        }
        public ActionResult EmployePage()
        {
            if (Session["employe_id"] != null)
            {
                List<GroupLocationVM> groups = new List<GroupLocationVM>();
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "SELECT childgrouprelation.group_id, groups.birth_year, location.weeks, location.location_id, count(*) FROM childgrouprelation INNER JOIN groups ON childgrouprelation.group_id = groups.group_id INNER JOIN location ON groups.location_id = location.location_id";
                    if (Convert.ToInt32(Session["location_id"]) <= 3)
                    {
                        query += " WHERE location.location_id <= 3 GROUP BY group_id";
                    }
                    else if (Convert.ToInt32(Session["location_id"]) > 3)
                    {
                        query += " WHERE location.location_id > 3 GROUP BY group_id";
                    }   
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                groups.Add(new GroupLocationVM
                                {
                                    group_id = Convert.ToInt32(sdr["group_id"]),
                                    birth_year = Convert.ToInt32(sdr["birth_year"]),
                                    weeks = Convert.ToString(sdr["weeks"]),
                                    count = Convert.ToInt32(sdr["count(*)"])
                                });
                            }
                        }
                        con.Close();
                    }
                }
                return View(groups);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult GroupDetails(int? id)
        {
            if (Session["employe_id"] != null)
            {
                List<ChildGroupVM> children = new List<ChildGroupVM>();
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "SELECT childgrouprelation.child_id, child.name, child.birth_date, child.shirt_size, child.comment, child.can_swim, child.allow_photos, child.vaccinated, childgrouprelation.group_id, groups.birth_year, child.present " +
                        "FROM childgrouprelation INNER JOIN groups ON childgrouprelation.group_id = groups.group_id INNER JOIN child ON childgrouprelation.child_id = child.child_id WHERE childgrouprelation.group_id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id);
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                children.Add(new ChildGroupVM
                                {
                                    child_id = Convert.ToInt32(sdr["child_id"]),
                                    name = Convert.ToString(sdr["name"]),
                                    comment = Convert.ToString(sdr["comment"]),
                                    can_swim = Convert.ToBoolean(sdr["can_swim"]),
                                    birth_date = Convert.ToDateTime(sdr["birth_date"]),
                                    allow_photos = Convert.ToBoolean(sdr["allow_photos"]),
                                    vaccinated = Convert.ToBoolean(sdr["vaccinated"]),
                                    shirt_size = Convert.ToString(sdr["shirt_size"]),
                                    group_id = Convert.ToInt32(sdr["group_id"]),
                                    birth_year = Convert.ToInt32(sdr["birth_year"]),
                                    present = Convert.ToBoolean(sdr["present"])
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
        public ActionResult DetailsChild(int? id)
        {
            if (Session["employe_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ChildCaretakerLocationVM childDetails = new ChildCaretakerLocationVM();
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "SELECT child.name, child.birth_date, child.shirt_size, child.comment, child.can_swim, child.allow_photos, child.vaccinated, caretaker.caretaker_name, caretaker.caretaker_number, caretaker.caretaker_email, caretaker.address, caretaker.debt, caretaker.alternative_name, caretaker.alternative_number, location.location_name, location.location_address, location.start_date, location.end_date, location.weeks " +
                        "FROM child INNER JOIN caretaker ON child.caretaker_id = caretaker.caretaker_id INNER JOIN location on child.location_id = location.location_id WHERE child.child_id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id);
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                childDetails.name = Convert.ToString(sdr["name"]);
                                childDetails.birth_date = Convert.ToDateTime(sdr["birth_date"]);
                                childDetails.shirt_size = Convert.ToString(sdr["shirt_size"]);
                                childDetails.comment = Convert.ToString(sdr["comment"]);
                                childDetails.can_swim = Convert.ToBoolean(sdr["can_swim"]);
                                childDetails.allow_photos = Convert.ToBoolean(sdr["allow_photos"]);
                                childDetails.vaccinated = Convert.ToBoolean(sdr["vaccinated"]);
                                childDetails.caretaker_name = Convert.ToString(sdr["caretaker_name"]);
                                childDetails.caretaker_number = Convert.ToInt32(sdr["caretaker_number"]);
                                childDetails.caretaker_email = Convert.ToString(sdr["caretaker_email"]);
                                childDetails.alternative_name = Convert.ToString(sdr["alternative_name"]);
                                childDetails.alternative_number = Convert.ToInt32(sdr["alternative_number"]);
                                childDetails.adress = Convert.ToString(sdr["address"]);
                                childDetails.debt = Convert.ToDouble(sdr["debt"]);
                                childDetails.location_name = Convert.ToString(sdr["location_name"]);
                                childDetails.location_adress = Convert.ToString(sdr["location_address"]);
                                childDetails.start_date = Convert.ToDateTime(sdr["start_date"]);
                                childDetails.end_date = Convert.ToDateTime(sdr["end_date"]);
                                childDetails.weeks = Convert.ToString(sdr["weeks"]);
                            }
                        }
                        con.Close();
                    }
                }
                if (childDetails == null)
                {
                    return HttpNotFound();
                }
                return View(childDetails);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult EditInformation(int? id)
        {
            id = 1;
            if (Session["employe_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                information information = new information();
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "SELECT * FROM information WHERE information_id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id);
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                information.information_id = Convert.ToInt32(sdr["information_id"]);
                                information.information_Title = Convert.ToString(sdr["information_Title"]);
                                information.information_Text = Convert.ToString(sdr["information_Text"]);

                            }
                        }
                        con.Close();
                    }
                }
                if (information == null)
                {
                    return HttpNotFound();
                }
                return View(information);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditInformation([Bind(Include = "information_id, information_Title, information_Text")] information information)
        {
            if (ModelState.IsValid)
            {
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "UPDATE information SET information_Title = @information_Title, information_Text = @information_Text";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@information_id", information.information_id);
                        cmd.Parameters.AddWithValue("@information_Title", information.information_Title);
                        cmd.Parameters.AddWithValue("@information_Text", information.information_Text);
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                        }
                        con.Close();
                    }
                }
                return RedirectToAction("Index");
            }
            return View(information);
        }
    }
}