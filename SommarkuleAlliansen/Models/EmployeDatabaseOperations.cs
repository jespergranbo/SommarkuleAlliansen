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
    public class EmployeDatabaseOperations
    {
        string constr = ConfigurationManager.ConnectionStrings["smconnection"].ConnectionString;

        public employe FindEmployeLogIn(employe employe)
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT * FROM employe WHERE name = @name AND password = @password";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@name", employe.name);
                    cmd.Parameters.AddWithValue("@password", employe.password);
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            employe.employe_id = Convert.ToInt32(sdr["employe_id"]);
                            employe.employe_type = Convert.ToInt32(sdr["employe_type"]);
                            employe.name = sdr["name"].ToString();
                            employe.number = Convert.ToInt32(sdr["number"]);
                            employe.password = sdr["password"].ToString();
                            employe.group_id = Convert.ToInt32(sdr["group_id"]);
                            employe.location_id = Convert.ToInt32(sdr["group_id"]);
                        }
                    }
                    con.Close();
                    
                }
            }
            return employe;
        }
        public List<GroupLocationVM> FindAllGroups(int id, int employe_type)
        {
            List<GroupLocationVM> groups = new List<GroupLocationVM>();
            
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "SELECT childgrouprelation.group_id, groups.birth_year, location.weeks, location.location_id, count(*) FROM childgrouprelation INNER JOIN groups ON childgrouprelation.group_id = groups.group_id INNER JOIN location ON groups.location_id = location.location_id";
                if (id <= 3 && employe_type != 1 )
                {
                    query += " WHERE location.location_id <= 3 GROUP BY group_id";
                }
                else if (id > 3 && employe_type != 1)
                {
                    query += " WHERE location.location_id > 3 GROUP BY group_id";
                }
                else
                {
                    query += " GROUP BY group_id";
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
            return groups;
        }
        public List<ChildGroupVM> GetGroupDetails(int? id)
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
            return children;
        }
        public void UpdatePresent(List<ChildGroupVM> children)
        {
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                for (int i = 0; i < children.Count; i++)
                {
                    string query = "UPDATE child SET present = @present WHERE child_id = @child_id;";
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@present", children[i].present);
                        cmd.Parameters.AddWithValue("@child_id", children[i].child_id);
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
}