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
        public List<EmployeGroupLocationVM> GetAllEmployes()
        {
            List<EmployeGroupLocationVM> employes = new List<EmployeGroupLocationVM>();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT employe.employe_id, employe.employe_type, employe.name, employe.number, location.location_id, location.location_name, groups.birth_year FROM employe " +
                    "INNER JOIN location ON employe.location_id = location.location_id " +
                    "INNER JOIN groups ON employe.group_id = groups.group_id";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            employes.Add(new EmployeGroupLocationVM
                            {
                                employe_id = Convert.ToInt32(sdr["employe_id"]),
                                employe_type = Convert.ToInt32(sdr["employe_type"]),
                                name = Convert.ToString(sdr["name"]),
                                number = Convert.ToInt32(sdr["number"]),
                                location_id = Convert.ToInt32(sdr["location_id"]),
                                birth_year = Convert.ToInt32(sdr["birth_year"]),
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
                string query = "SELECT caretaker.caretaker_name, caretaker.caretaker_number, caretaker.caretaker_email, caretaker.address, caretaker.debt, caretaker.caretaker_id, caretaker.ocr_number, IFNULL(COUNT(child.caretaker_id), 0) AS amountOfChilds FROM caretaker " +
                    "LEFT JOIN child ON caretaker.caretaker_id = child.caretaker_id GROUP BY caretaker.caretaker_id ORDER BY amountOfChilds DESC";
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
                                ocr_number = Convert.ToInt32(sdr["ocr_number"]),
                                count = Convert.ToInt32(sdr["amountOfChilds"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return caretakers;
        }
        public int GetCaretakerOCR(long caretaker_id)
        {
            caretaker caretaker = new caretaker();
            int ocr_number = 0;
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT ocr_number FROM caretaker WHERE caretaker_id = @caretaker_id";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@caretaker_id", caretaker_id);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            ocr_number = Convert.ToInt32(sdr["ocr_number"]);
                        }
                    }
                    con.Close();
                }
            }
            return ocr_number;
        }
        public List<ChildCaretakerLocationVM> GetChildCaretakerLocation()
        {
            List<ChildCaretakerLocationVM> children = new List<ChildCaretakerLocationVM>();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT child.child_id, child.name, child.birth_date, child.shirt_size, child.allergy_comment, child.social_security, caretaker.caretaker_name, caretaker.caretaker_id, location.location_name, location.weeks FROM child " +
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
                                allergy_comment = Convert.ToString(sdr["allergy_comment"]),
                                social_security = Convert.ToInt32(sdr["social_security"]),
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
                            employe.post_number = Convert.ToInt32(sdr["post_number"]);
                            employe.tax = Convert.ToBoolean(sdr["tax"]);
                            employe.bank = Convert.ToString(sdr["bank"]);
                            employe.clearing = Convert.ToInt32(sdr["clearing"]);
                            employe.account_number = Convert.ToInt32(sdr["account_number"]);
                            employe.shirt_size = Convert.ToString(sdr["shirt_size"]);
                            employe.social_security = Convert.ToInt32(sdr["social_security"]);
                            employe.address = Convert.ToString(sdr["adress"]);
                            employe.email = Convert.ToString(sdr["email"]);
                        }
                    }
                    con.Close();
                }
            }
            return employe;
        }
        public EmployeGroupLocationVM FindDetailsEmploye(int? employe_id)
        {
            EmployeGroupLocationVM employe = new EmployeGroupLocationVM();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT employe.employe_id, employe.email, employe.employe_type, employe.name, employe.number, employe.password, employe.post_number, employe.tax, employe.bank, employe.clearing, employe.account_number, employe.shirt_size, employe.social_security, employe.adress, location.location_name, groups.birth_year FROM employe " +
                    "INNER JOIN location ON employe.location_id = location.location_id INNER JOIN groups ON employe.group_id = groups.group_id WHERE employe_id = @id";
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
                            employe.email = Convert.ToString(sdr["email"]);
                            employe.number = Convert.ToInt32(sdr["number"]);
                            employe.password = Convert.ToString(sdr["password"]);
                            employe.post_number = Convert.ToInt32(sdr["post_number"]);
                            employe.tax = Convert.ToBoolean(sdr["tax"]);
                            employe.bank = Convert.ToString(sdr["bank"]);
                            employe.clearing = Convert.ToInt32(sdr["clearing"]);
                            employe.account_number = Convert.ToInt32(sdr["account_number"]);
                            employe.shirt_size = Convert.ToString(sdr["shirt_size"]);
                            employe.social_security = Convert.ToInt32(sdr["social_security"]);
                            employe.adress = Convert.ToString(sdr["adress"]);
                            employe.location_name = Convert.ToString(sdr["location_name"]);
                            employe.birth_year = Convert.ToInt32(sdr["birth_year"]);
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
        public void UpdateEmploye(int employe_id, string adress, long location_id, int employe_type, string name, int number, string email, string password, int group_id, int post_number, bool tax, string bank, int clearing, int account_number, string shirt_size, int social_security)
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "UPDATE employe SET name = @name, email = @email, adress = @adress, employe_type = @employe_type, number = @number, password = @password, group_id = @group_id, location_id = @location_id, post_number = @post_number, tax = @tax, bank = @bank, clearing = @clearing, account_number = @account_number, shirt_size = @shirt_size, social_security = @social_security WHERE employe_id = @id;";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@id", employe_id);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@employe_type", employe_type);
                    cmd.Parameters.AddWithValue("@number", number);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@group_id", group_id);
                    cmd.Parameters.AddWithValue("@adress", adress);
                    cmd.Parameters.AddWithValue("@location_id", location_id);
                    cmd.Parameters.AddWithValue("@post_number", post_number);
                    cmd.Parameters.AddWithValue("@tax", tax);
                    cmd.Parameters.AddWithValue("@bank", bank);
                    cmd.Parameters.AddWithValue("@clearing", clearing);
                    cmd.Parameters.AddWithValue("@account_number", account_number);
                    cmd.Parameters.AddWithValue("@shirt_size", shirt_size);
                    cmd.Parameters.AddWithValue("@social_security", social_security);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                    }
                    con.Close();
                }
            }
        }
        public void UpdateEmployeGroup(int group_id)
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "UPDATE employe SET group_id = @group_id";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@group_id", group_id);
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
        public void DeleteChild(int id)
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "DELETE FROM child WHERE child_id = @id";
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
        public void DeleteChildGroup(int id)
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "DELETE FROM childgrouprelation WHERE child_id = @id";
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
        public void DeleteCaretaker(int id)
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "DELETE FROM caretaker WHERE caretaker_id = @id";
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
        public List<EmployeGroupLocationVM> GetLocationsBothWeeks()
        {
            List<EmployeGroupLocationVM> locations = new List<EmployeGroupLocationVM>();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT groups.group_id, groups.location_id, groups.birth_year, location.location_name, location.weeks FROM groups INNER JOIN location ON groups.location_id = location.location_id WHERE weeks = 'Båda veckorna' ORDER BY group_id ASC";
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
        public List<ChildGroupLocationVM> GetLocations()
        {
            List<ChildGroupLocationVM> locations = new List<ChildGroupLocationVM>();
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
                            locations.Add(new ChildGroupLocationVM
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
        public void AddEmploye (string name, string adress, string email, int employe_type, int number, string password, int group_id, long location_id, int post_number, bool tax, string bank, int clearing, int account_number, string shirt_size, int social_security)
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "INSERT INTO employe (employe_id, name, adress, email, employe_type, number, password, group_id, location_id, post_number, tax, bank, clearing, account_number, shirt_size, social_security) VALUES (NULL, @name, @adress, @email, @employe_type, @number, @password, @group_id, @location_id, @post_number, @tax, @bank, @clearing, @account_number, @shirt_size, @social_security)";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@adress", adress);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@employe_type", employe_type);
                    cmd.Parameters.AddWithValue("@number", number);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@group_id", group_id);
                    cmd.Parameters.AddWithValue("@location_id", location_id);
                    cmd.Parameters.AddWithValue("@post_number", post_number);
                    cmd.Parameters.AddWithValue("@tax", tax);
                    cmd.Parameters.AddWithValue("@bank", bank);
                    cmd.Parameters.AddWithValue("@clearing", clearing);
                    cmd.Parameters.AddWithValue("@account_number", account_number);
                    cmd.Parameters.AddWithValue("@shirt_size", shirt_size);
                    cmd.Parameters.AddWithValue("@social_security", social_security);
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
        public caretaker FindCaretakerIDThroughOcr(int? ocr_number)
        {
            caretaker caretaker = new caretaker();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT * FROM caretaker WHERE ocr_number = @ocr_number";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@ocr_number", ocr_number);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            caretaker.caretaker_id = Convert.ToInt32(sdr["caretaker_id"]);
                        }
                    }
                    con.Close();
                }
            }
            return caretaker;
        }
        public child FindCaretakerByChild(int id)
        {
            child child = new child();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT caretaker_id FROM child WHERE child_id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@id", id);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            child.caretaker_id = Convert.ToInt32(sdr["caretaker_id"]);
                        }
                    }
                    con.Close();
                }
            }
            return child;
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
        
        public void UpdateCaretakerDebt(int caretaker_id, int debt)
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "UPDATE caretaker SET debt = debt + @debt WHERE caretaker_id = @id;";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@id", caretaker_id);
                    cmd.Parameters.AddWithValue("@debt", debt);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                    }
                    con.Close();
                }
            }
        }
        public void DecreaseCaretakerDebt(int caretaker_id, int debt)
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "UPDATE caretaker SET debt = debt - @debt WHERE caretaker_id = @id;";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@id", caretaker_id);
                    cmd.Parameters.AddWithValue("@debt", debt);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                    }
                    con.Close();
                }
            }
        }
        public void ResetChildGroupRelation()
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "DELETE FROM childgrouprelation";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                    }
                    con.Close();
                }
            }
        }
        public void ResetChildren()
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "DELETE FROM child";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                    }
                    con.Close();
                }
            }
        }
        public void ResetCaretakers()
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "DELETE FROM caretaker";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
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
                string query = "SELECT child.name, child.child_id, child.birth_date, child.comment, child.allergy_comment, child.social_security, caretaker.caretaker_name, caretaker.ocr_number, caretaker.caretaker_id, caretaker.caretaker_number, caretaker.caretaker_email, caretaker.address, caretaker.debt, caretaker.alternative_name, caretaker.alternative_number, location.location_name, location.location_address, location.start_date, location.end_date, location.weeks " +
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
                                child_id = Convert.ToInt32(sdr["child_id"]),
                                birth_date = Convert.ToDateTime(sdr["birth_date"]),
                                comment = Convert.ToString(sdr["comment"]),
                                allergy_comment = Convert.ToString(sdr["allergy_comment"]),
                                social_security = Convert.ToInt32(sdr["social_security"]),
                                caretaker_name = Convert.ToString(sdr["caretaker_name"]),
                                caretaker_id = Convert.ToInt32(sdr["caretaker_id"]),
                                caretaker_number = Convert.ToInt32(sdr["caretaker_number"]),
                                caretaker_email = Convert.ToString(sdr["caretaker_email"]),
                                alternative_name = Convert.ToString(sdr["alternative_name"]),
                                alternative_number = Convert.ToInt32(sdr["alternative_number"]),
                                ocr_number = Convert.ToInt32(sdr["ocr_number"]),
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
        public ChildGroupLocationVM FindChild(int? id)
        {
            ChildGroupLocationVM child = new ChildGroupLocationVM();
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
                            child.caretaker_id = Convert.ToInt32(sdr["caretaker_id"]);
                            child.name = Convert.ToString(sdr["name"]);
                            child.comment = Convert.ToString(sdr["comment"]);
                            child.allergy_comment = Convert.ToString(sdr["allergy_comment"]);
                            child.can_swim = Convert.ToBoolean(sdr["can_swim"]);
                            child.birth_date = Convert.ToDateTime(sdr["birth_date"]);
                            child.allow_photos = Convert.ToBoolean(sdr["allow_photos"]);
                            child.vaccinated = Convert.ToBoolean(sdr["vaccinated"]);
                            child.shirt_size = Convert.ToString(sdr["shirt_size"]);
                            child.social_security = Convert.ToInt32(sdr["social_security"]);
                            child.location_id = Convert.ToInt32(sdr["location_id"]);
                        }
                    }
                    con.Close();
                }
            }
            return child;
        }
        public List<ChildGroupLocationVM> FindChildGroup(int? id)
        {
            List<ChildGroupLocationVM> childGroup = new List<ChildGroupLocationVM>();
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
                                childGroup.Add(new ChildGroupLocationVM
                                {
                                    childGroupRelation_id = Convert.ToInt32(sdr["child_group_relation_id"]),
                                    group_id = Convert.ToInt32(sdr["group_id"])
                                });
                            }
                            else if (counter == 1)
                            {
                                childGroup.Add(new ChildGroupLocationVM
                                {
                                    childGroupRelation_id = Convert.ToInt32(sdr["child_group_relation_id"]),
                                    group_id2 = Convert.ToInt32(sdr["group_id"])
                                });
                            }
                            counter++;
                        }
                    }
                    con.Close();
                }
            }
            return childGroup;
        }
        public void AddToGroup(List<groups> groups, long child_id)
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "INSERT INTO childgrouprelation (child_group_relation_id, child_id, group_id) VALUES (null, @child_id, @group_id)";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@child_id", child_id);
                    cmd.Parameters.AddWithValue("@group_id", groups[1].group_id);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                    }
                    con.Close();
                }
            }
        }
        public void AddNewGroups(int newGroupYear)
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                for (int i = 1; i < 7; i++)
                {
                    string query = "INSERT INTO groups (group_id, location_id, birth_year) VALUES (null, @location_id, @newGroupYear)";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@location_id", i);
                        cmd.Parameters.AddWithValue("@newGroupYear", newGroupYear);
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                        }
                        con.Close();
                    }
                }
            }
        }
        public void DeleteFromRelation(int childGroup_id)
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "DELETE FROM childgrouprelation WHERE child_group_relation_id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@id", childGroup_id);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                    }
                    con.Close();
                }
            }
        }
        public void DeleteFromGroups(int birth_year)
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "DELETE FROM groups WHERE birth_year = @birth_year";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@birth_year", birth_year);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                    }
                    con.Close();
                }
            }
        }
        public groups GetGroupInfo(int group_id)
        {
            groups groups = new groups();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT * FROM groups WHERE group_id = @group_id";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@group_id", group_id);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            groups.location_id = Convert.ToInt32(sdr["location_id"]);
                            groups.birth_year = Convert.ToInt32(sdr["birth_year"]);
                        }
                    }
                    con.Close();
                }
            }
            return groups;
        }
        public List<ChildGroupRelation> GetGroupInfoFromChild(int id)
        {
            List<ChildGroupRelation> attendedGroups = new List<ChildGroupRelation>();
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
                        while (sdr.Read())
                        {
                            attendedGroups.Add(new ChildGroupRelation
                            {
                                group_id = Convert.ToInt32(sdr["group_id"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return attendedGroups;
        }
        public List<groups> GetGroupId(int location_id, int birth)
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
                    cmd.Parameters.AddWithValue("@birth_year", birth);
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
        public void UpdateChild(int child_id, string name, string allergy_comment, string comment, bool can_swim, DateTime birth_date, bool allow_photos, bool vaccinated, string shirt_size, int social_security, int location_id)
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "UPDATE child SET name = @name, comment = @comment, can_swim = @can_swim, birth_date = @birth_date, allow_photos = @allow_photos, vaccinated = @vaccinated, shirt_size = @shirt_size, allergy_comment = @allergy_comment, social_security = @social_security, location_id = @location_id WHERE child_id = @id;";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@id", child_id);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@comment", comment);
                    cmd.Parameters.AddWithValue("@can_swim", can_swim);
                    cmd.Parameters.AddWithValue("@birth_date", birth_date);
                    cmd.Parameters.AddWithValue("@allow_photos", allow_photos);
                    cmd.Parameters.AddWithValue("@vaccinated", vaccinated);
                    cmd.Parameters.AddWithValue("@shirt_size", shirt_size);
                    cmd.Parameters.AddWithValue("@allergy_comment", allergy_comment);
                    cmd.Parameters.AddWithValue("@social_security", social_security);
                    cmd.Parameters.AddWithValue("@location_id", location_id);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                    }
                    con.Close();
                }
            }
        }
        public void ResetEmployeGroups()
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "UPDATE employe SET group_id = NULL";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                    }
                    con.Close();
                }
            }
        }
        public void UpdateChildGroup(int group_id, int childGroupRelation_id)
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "UPDATE childgrouprelation SET group_id = @group_id WHERE child_group_relation_id = @id;";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@group_id", group_id);
                    cmd.Parameters.AddWithValue("@id", childGroupRelation_id);
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
                string query = "SELECT child.child_id, child.name, child.birth_date, child.shirt_size, child.comment, child.can_swim, child.allow_photos, child.vaccinated, child.allergy_comment, child.registration_date, child.social_security, caretaker.caretaker_id, caretaker.caretaker_name, caretaker.caretaker_number, caretaker.caretaker_email, caretaker.address, caretaker.debt, caretaker.alternative_name, caretaker.alternative_number, location.location_name, location.location_address, location.start_date, location.end_date, location.weeks " +
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
                            childDetails.child_id = Convert.ToInt32(sdr["child_id"]);
                            childDetails.name = Convert.ToString(sdr["name"]);
                            childDetails.birth_date = Convert.ToDateTime(sdr["birth_date"]);
                            childDetails.shirt_size = Convert.ToString(sdr["shirt_size"]);
                            childDetails.comment = Convert.ToString(sdr["comment"]);
                            childDetails.can_swim = Convert.ToBoolean(sdr["can_swim"]);
                            childDetails.allow_photos = Convert.ToBoolean(sdr["allow_photos"]);
                            childDetails.vaccinated = Convert.ToBoolean(sdr["vaccinated"]);
                            childDetails.allergy_comment = Convert.ToString(sdr["allergy_comment"]);
                            childDetails.registration_date = Convert.ToDateTime(sdr["registration_date"]);
                            childDetails.social_security = Convert.ToInt32(sdr["social_security"]);
                            childDetails.caretaker_id = Convert.ToInt32(sdr["caretaker_id"]);
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
        public groups GetLowestGroup()
        {
            groups group = new groups();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT * FROM groups ORDER BY birth_year desc";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            group.birth_year = Convert.ToInt32(sdr["birth_year"]);
                            group.location_id = Convert.ToInt32(sdr["location_id"]);
                            group.group_id = Convert.ToInt32(sdr["group_id"]);

                        }
                    }
                    con.Close();
                }
            }
            return group;
        }
        public groups GetHighestGroup()
        {
            groups group = new groups();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT * FROM groups ORDER BY birth_year asc";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            group.birth_year = Convert.ToInt32(sdr["birth_year"]);
                            group.location_id = Convert.ToInt32(sdr["location_id"]);
                            group.group_id = Convert.ToInt32(sdr["group_id"]);

                        }
                    }
                    con.Close();
                }
            }
            return group;
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