using System;
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
                            Session["employe_id"] = user.employe_id.ToString();
                            Session["name"] = user.name.ToString();
                            Session["employe_type"] = user.employe_type.ToString();
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
                ChildCaretakerLocationVM child = new ChildCaretakerLocationVM();
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "SELECT child.child_id, child.name, child.comment, child.can_swim, child.birth_date, child.allow_photos, child.vaccinated, child.shirt_size, child.location_id, location.location_name, location.location_address, location.start_date, location.end_date, location.location_email, location.location_number, location.weeks, location.price " +
                        "FROM child INNER JOIN location ON child.location_id = location.location_id WHERE child.child_id=@id";
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
                                child.location_id = Convert.ToInt32(sdr["location_id"]);
                                child.location_name = Convert.ToString(sdr["location_name"]);
                                child.location_adress = Convert.ToString(sdr["location_address"]);
                                child.start_date = Convert.ToDateTime(sdr["start_date"]);
                                child.end_date = Convert.ToDateTime(sdr["end_date"]);
                                child.location_email = Convert.ToString(sdr["location_email"]);
                                child.location_number = Convert.ToInt32(sdr["location_number"]);
                                child.weeks = Convert.ToString(sdr["weeks"]);
                                child.price = Convert.ToInt32(sdr["price"]);
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
        public ActionResult EditChild([Bind(Include = "caretaker_id,caretaker_name,caretaker_number,caretaker_email,adress,alternative_name,alternative_number,debt")] caretaker caretaker)
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
        public ActionResult EmployePage()
        {
            if (Session["employe_id"] != null)
            {
                List<groups> groups = new List<groups>();
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "SELECT child.group_id, groups.birth_year, location.weeks, count(*) FROM child INNER JOIN location ON child.location_id = location.location_id INNER JOIN groups ON child.group_id = groups.group_id GROUP BY group_id";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                groups.Add(new groups
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
                int secondID = 0;
                if (id == 3 || id == 4)
                {
                    secondID = 5;
                }
                else if (id == 6 || id == 7)
                {
                    secondID = 8;
                }
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "SELECT child.child_id, child.name, child.birth_date, child.shirt_size, child.comment, child.can_swim, child.allow_photos, child.vaccinated, child.group_id, groups.birth_year, child.present FROM child INNER JOIN groups ON child.group_id = groups.group_id WHERE child.group_id = @id OR child.group_id = @secondID";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@secondID", secondID);
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
    }
}