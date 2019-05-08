using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using SommarkuleAlliansen.Models;
using SommarkuleAlliansen.ViewModel;

namespace SommarkuleAlliansen.Models
{
    public class HomeDatabaseOperations
    {
        string constr = ConfigurationManager.ConnectionStrings["smconnection"].ConnectionString;

        public List<location> FindAllLocations()
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
            return locations;
        }
        public long SearchForExistingCaretaker(string caretakerEmail)
        {
            string query = "SELECT * FROM caretaker WHERE caretaker_email = @email";
            long caretaker_id = 0;
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@email", caretakerEmail);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            caretaker_id = Convert.ToInt32(sdr["caretaker_id"]);
                        }
                    }
                    con.Close();
                }
            }
            return 0;
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
        public List<groups> GetGroupId(int location_id, DateTime birth)
        {
            List<groups> groups = new List<groups>();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT * FROM groups WHERE birth_year = @birth_year AND location_id = ";
                if (location_id == 3)
                {
                    query += "1 OR birth_year = @birth_year AND location_id = 2";
                }
                else if (location_id == 6)
                {
                    query += "4 OR birth_year = @birth_year AND location_id = 5";
                }
                else
                {
                    query += "@location_id";
                }
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@birth_year", birth.Year.ToString());
                    cmd.Parameters.AddWithValue("@location_id", location_id);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            groups.Add(new groups
                            {
                                group_id = Convert.ToInt32(sdr["group_id"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return groups;
        }
        public void UpdateCaretakerDebt(long caretaker_id, int price)
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
        public long AddCaretaker(string caretakerName, int caretakerNumber, string caretakerEmail, string caretakerAddress, string altName, int altNumber, int price)
        {
            long caretaker_id = 0;
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "INSERT INTO caretaker (caretaker_id, caretaker_name, caretaker_number, caretaker_email, address, alternative_name, alternative_number, debt) VALUES (NULL, @caretaker_name, @caretaker_number, @caretaker_email, @address, @alternative_name, @alternative_number, @debt);";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@caretaker_name", caretakerName);
                    cmd.Parameters.AddWithValue("@caretaker_number", caretakerNumber);
                    cmd.Parameters.AddWithValue("@caretaker_email", caretakerEmail);
                    cmd.Parameters.AddWithValue("@address", caretakerAddress);
                    cmd.Parameters.AddWithValue("@alternative_name", altName);
                    cmd.Parameters.AddWithValue("@alternative_number", altNumber);
                    cmd.Parameters.AddWithValue("@debt", price);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        caretaker_id = cmd.LastInsertedId;
                    }
                    con.Close();
                }
            }
            return caretaker_id;
        }
        public long AddChild(string child_name, string comment, long caretaker_id, bool CanSwim, DateTime birth, bool allowPhoto, bool isVaccinated, string shirtSize, int location_id, DateTime registration_date)
        {
            long child_id = 0;
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "INSERT INTO child (child_id, name, comment, caretaker_id, can_swim, birth_date, allow_photos, vaccinated, shirt_size, location_id, present, registration_date) VALUES (NULL, @name, @comment, @caretaker_id, @can_swim, @birth_date, @allow_photos, @vaccinated, @shirt_size, @location_id, @present, @registration_date);";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    bool present = false;
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
                    cmd.Parameters.AddWithValue("@present", present);
                    cmd.Parameters.AddWithValue("@registration_date", registration_date);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        child_id = cmd.LastInsertedId;
                    }
                    con.Close();
                }
            }
            return child_id;
        }
        public void AddToGroup(List<groups> groups, long child_id)
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "INSERT INTO childgrouprelation (child_group_relation_id, child_id, group_id) VALUES (null, @child_id, @group_id1)";
                if (groups.Count > 1)
                {
                    query += ", (null, @child_id, @group_id2)";
                }
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@child_id", child_id);
                    cmd.Parameters.AddWithValue("@group_id1", groups[0].group_id);
                    if (groups.Count > 1)
                    {
                        cmd.Parameters.AddWithValue("@group_id2", groups[1].group_id);
                    }
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                    }
                    con.Close();
                }
            }
        }
        public List<information> GetInformation()
        {
            List<information> informations = new List<information>();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT * FROM information";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            informations.Add(new information
                            {
                                information_id = Convert.ToInt32(sdr["information_id"]),
                                information_Title = Convert.ToString(sdr["information_Title"]),
                                information_Text = Convert.ToString(sdr["information_Text"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return informations;
        }
    }
}