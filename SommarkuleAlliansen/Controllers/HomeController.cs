using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;
using SommarkuleAlliansen.Models;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SommarkuleAlliansen.Controllers
{
    public class HomeController : Controller
    {
        string constr = ConfigurationManager.ConnectionStrings["smconnection"].ConnectionString;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            List<location> locations = new List<location>();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT * FROM location";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            locations.Add(new location
                            {
                                location_id = Convert.ToInt32(sdr["location_id"]),
                                location_name = Convert.ToString(sdr["location_name"]),
                                location_adress = Convert.ToString(sdr["location_address"]),
                                start_date = Convert.ToDateTime(sdr["start_date"]),
                                end_date = Convert.ToDateTime(sdr["end_date"]),
                                location_email = Convert.ToString(sdr["location_email"]),
                                weeks = Convert.ToString(sdr["weeks"]),
                                price = Convert.ToInt32(sdr["price"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return View(locations);
        }
        [HttpPost]
        public ActionResult Register(int location_id, string child_name, DateTime birth, string shirtSize, bool CanSwim, bool allowPhoto, bool isVaccinated, string comment, string caretakerName, string caretakerAddress, string caretakerEmail, int caretakerNumber, string altName, int altNumber)
        {
            long caretaker_id = 0;
            int price = 0;
            DateTime start_date = DateTime.Now;
            DateTime end_date = DateTime.Now;
            string location_name = "";
            if (ModelState.IsValid)
            {
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "SELECT * FROM caretaker WHERE caretaker_email = @email";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@email", caretakerEmail);
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while(sdr.Read())
                            {
                                caretaker_id = Convert.ToInt32(sdr["caretaker_id"]);
                            }
                        }
                        con.Close();
                    }
                    location selectedLocation = GetLocationInformation(location_id);
                    if (caretaker_id != 0)
                    {
                        UpdateCaretakerDebt(caretaker_id, selectedLocation.price);
                    }
                    if (caretaker_id == 0)
                    {
                        query = "INSERT INTO caretaker (caretaker_id, caretaker_name, caretaker_number, caretaker_email, address, alternative_name, alternative_number, debt) VALUES (NULL, @caretaker_name, @caretaker_number, @caretaker_email, @address, @alternative_name, @alternative_number, @debt);";
                        using (MySqlCommand cmd = new MySqlCommand(query))
                        {
                            cmd.Connection = con;
                            cmd.Parameters.AddWithValue("@caretaker_name", caretakerName);
                            cmd.Parameters.AddWithValue("@caretaker_number", caretakerNumber);
                            cmd.Parameters.AddWithValue("@caretaker_email", caretakerEmail);
                            cmd.Parameters.AddWithValue("@address", caretakerAddress);
                            cmd.Parameters.AddWithValue("@alternative_name", altName);
                            cmd.Parameters.AddWithValue("@alternative_number", altNumber);
                            cmd.Parameters.AddWithValue("@debt", selectedLocation.price);
                            con.Open();
                            using (MySqlDataReader sdr = cmd.ExecuteReader())
                            {
                               caretaker_id = cmd.LastInsertedId;
                            }
                            con.Close();
                        }
                    }
                    query = "INSERT INTO child (child_id, name, comment, caretaker_id, can_swim, birth_date, allow_photos, vaccinated, shirt_size, location_id) VALUES (NULL, @name, @comment, @caretaker_id, @can_swim, @birth_date, @allow_photos, @vaccinated, @shirt_size, @location_id);";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@name", child_name);
                        cmd.Parameters.AddWithValue("@comment", comment);
                        cmd.Parameters.AddWithValue("@caretaker_id", caretaker_id);
                        cmd.Parameters.AddWithValue("@can_swim", CanSwim);
                        cmd.Parameters.AddWithValue("@birth_date", birth);
                        cmd.Parameters.AddWithValue("@allow_photos", allowPhoto);
                        cmd.Parameters.AddWithValue("@vaccinated", isVaccinated);
                        cmd.Parameters.AddWithValue("@shirt_size", shirtSize);
                        cmd.Parameters.AddWithValue("@location_id", location_id);
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                        }
                        con.Close();
                    }
                    return RedirectToAction("OrderConfermation", new { caretakerEmail , caretakerName, child_name, price, start_date, end_date, location_name});
                }
            }
            return View();
        }
        private void UpdateCaretakerDebt(long caretaker_id, int price)
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "UPDATE caretaker SET debt = debt + @debt WHERE caretaker_id = @caretaker_id;";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@caretaker_id", caretaker_id);
                    cmd.Parameters.AddWithValue("@debt", price);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                        }
                    }
                    con.Close();
                }
            }
        }
        public location GetLocationInformation(int location_id)
        {
            location selectedLocation = new location();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT * FROM location WHERE location_id = @location_id";
                
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@location_id", location_id);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            selectedLocation.price = Convert.ToInt32(sdr["price"]);
                            selectedLocation.start_date = Convert.ToDateTime(sdr["start_date"]);
                            selectedLocation.end_date = Convert.ToDateTime(sdr["end_date"]);
                            selectedLocation.location_name = Convert.ToString(sdr["location_name"]);
                        }
                    }
                    con.Close();
                }
            }
            return selectedLocation;
        }
        public async Task<ActionResult> OrderConfermation(string caretakerEmail, string caretakerName, string child_name, int price, DateTime start_date, DateTime end_date, string location_name)
        {
            if (ModelState.IsValid)
            {
                var body = "Tack för anmälan " + caretakerName + " ditt barn " + child_name + " är anmäld för perioden " + String.Format("{0:MM/dd}", start_date) + " - " + String.Format("{0:MM/dd}", end_date) + " i föreningen " + location_name + ". Du ska betala " + price + ":- inom 2 veckor.";
                var message = new MailMessage();
                message.To.Add(new MailAddress(caretakerEmail));  // replace with valid value 
                message.From = new MailAddress("sommarkulan@outlook.com");  // replace with valid value
                message.Subject = "Orderbekräftelse";
                message.Body = string.Format(body);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                    return RedirectToAction("ConfirmedOrder", new { caretakerName, caretakerEmail});
                }
            }
            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(Email email)
        {
            if (ModelState.IsValid)
            {
                var body = "<p>Email From: {0} ({1})</p><p>Message: {2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress("sommarkulan@outlook.com"));  // replace with valid value 
                message.From = new MailAddress("sommarkulan@outlook.com");  // replace with valid value
                message.Subject = email.Subject;
                message.Body = string.Format(body, email.FromName, email.FromEmail, email.Message);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                    return RedirectToAction("Sent");
                }
            }
            return View(email);
        }
        public ActionResult ConfirmedOrder(string caretakerName, string caretakerEmail)
        {
            if (caretakerName == null || caretakerEmail == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                caretaker caretakerOrder = new caretaker();
                caretakerOrder.caretaker_name = caretakerName;
                caretakerOrder.caretaker_email = caretakerEmail;
                return View(caretakerOrder);
            }
        }
    }
}