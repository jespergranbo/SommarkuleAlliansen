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
    public class AdminDatabaseOperations
    {
        string constr = ConfigurationManager.ConnectionStrings["smconnection"].ConnectionString;
        public List<EmployeLocationVM> GetAllEmployes()
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
            return employes;
        }
        public List<caretaker> GetAllCaretakers()
        {
            List<caretaker> caretakers = new List<caretaker>();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT child.caretaker_id, caretaker.caretaker_name, caretaker.caretaker_number, caretaker.caretaker_email, caretaker.address, caretaker.debt, COUNT(*) " +
                    "FROM child INNER JOIN caretaker ON child.caretaker_id = caretaker.caretaker_id GROUP BY caretaker_id";
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
                                debt = Convert.ToDouble(sdr["debt"]),
                                count = Convert.ToInt32(sdr["count(*)"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return caretakers;
        }
        public List<ChildCaretakerLocationVM> GetChildCaretakerLocation()
        {
            List<ChildCaretakerLocationVM> children = new List<ChildCaretakerLocationVM>();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT child.child_id, child.name, child.birth_date, child.shirt_size, caretaker.caretaker_name, caretaker.caretaker_id, location.location_name, location.weeks FROM child " +
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
                                caretaker_id = Convert.ToInt32(sdr["caretaker_id"]),
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
            return children;
        }
        public employe FindEmploye(int? employe_id)
        {
            employe employe = new employe();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT * FROM employe WHERE employe_id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@id", employe_id);
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
            return employe;
        }
        public bool FindEmployeWithNickname(string name)
        {
            employe employe = new employe();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT * FROM employe WHERE name = @name";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@name", name);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            return true;
                        }
                    }
                    con.Close();
                }
            }
            return false;
        }
        public void UpdateEmploye(employe employe)
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
        }
        public void DeleteEmploye(int id)
        {
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
        }
        public long GetLocationIdByGroup (long group_id)
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
                        while (sdr.Read())
                        {
                            location_id = Convert.ToInt32(sdr["location_id"]);
                        }
                    }
                    con.Close();
                }
                return location_id;
            }
        }
        public List<EmployeGroupLocationVM> GetLocations()
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
            return locations;
        }
        public void AddEmploye (string name, int employe_type, int number, string password, int group_id, long location_id)
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "INSERT INTO employe (employe_id, name, employe_type, number, password, group_id, location_id) VALUES (NULL, @name, @employe_type, @number, @password, @group_id, @location_id)";
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
        }
        public caretaker FindCaretaker(int? id)
        {
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
            return caretaker;
        }
        public void UpdateCaretaker(caretaker caretaker)
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
        }
        public List<ChildCaretakerLocationVM> GetCaretakerDetails(int? id)
        {
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
            return caretakerDetails;
        }
        public ChildGroupRelationVM FindChild(int? id)
        {
            ChildGroupRelationVM child = new ChildGroupRelationVM();
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
            return child;
        }
        public ChildGroupRelationVM FindChildGroup(int? id)
        {
            ChildGroupRelationVM childGroup = new ChildGroupRelationVM();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT * FROM childgrouprelation WHERE child_id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@id", id);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        int counter = 0;
                        while (sdr.Read())
                        {
                            if (counter == 0)
                            {
                                childGroup.group_id = Convert.ToInt32(sdr["group_id"]);
                            }
                            else if (counter == 1)
                            {
                                childGroup.group_id2 = Convert.ToInt32(sdr["group_id"]);
                            }
                            counter++;
                        }
                    }
                    con.Close();
                }
            }
            return childGroup;
        }
        public void UpdateChild(child child)
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
        }
        public ChildCaretakerLocationVM GetChildDetails(int? id)
        {
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
            return childDetails;
        }
        public information FindInformation(int? id)
        {
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
            return information;
        }
        public void UpdateInformation(information information)
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "UPDATE information SET information_Title = @information_Title, information_Text = @information_Text WHERE information_id = @information_id";
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
        }
    }
}