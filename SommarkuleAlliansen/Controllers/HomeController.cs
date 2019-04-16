﻿using System;
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
                    if (caretaker_id == 0)
                    {
                        query = "INSERT INTO caretaker (caretaker_id, caretaker_name, caretaker_number, caretaker_email, address, alternative_name, alternative_number) VALUES (NULL, @caretaker_name, @caretaker_number, @caretaker_email, address, @alternative_name, @alternative_number);";
                        using (MySqlCommand cmd = new MySqlCommand(query))
                        {
                            cmd.Connection = con;
                            cmd.Parameters.AddWithValue("@caretaker_name", caretakerName);
                            cmd.Parameters.AddWithValue("@caretaker_number", caretakerNumber);
                            cmd.Parameters.AddWithValue("@caretaker_email", caretakerEmail);
                            cmd.Parameters.AddWithValue("@address", caretakerAddress);
                            cmd.Parameters.AddWithValue("@alternative_name", altName);
                            cmd.Parameters.AddWithValue("@alternative_number", altNumber);
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
                }
                return RedirectToAction("ConfirmedOrder");
            }
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ConfirmedOrder()
        {
            return View();
        }
    }
}