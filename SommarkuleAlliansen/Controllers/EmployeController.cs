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
        EmployeDatabaseOperations operations = new EmployeDatabaseOperations();
        public ActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(employe account)
        {
            employe employe = new employe();
            try
            {
                employe = operations.FindEmployeLogIn(account);
            }
            catch (Exception)
            {
                string message = "Det går inte att hämta användarna, vänligen försök igen.";
                return RedirectToAction("Error", "Home", new { message = message });
            }
            if (employe.employe_id != 0)
            {
                Session["employe_id"] = employe.employe_id.ToString();
                Session["name"] = employe.name.ToString();
                Session["employe_type"] = employe.employe_type.ToString();
                Session["group_id"] = employe.group_id.ToString();
                Session["location_id"] = employe.group_id.ToString();
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Användarnamnet eller lösenordet är fel");
            return View();
        }
        public ActionResult LogOut()
        {
            Session["employe_id"] = null;
            Session["name"] = null;
            Session["employe_type"] = null;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult EmployePage()
        {
            if (Session["employe_id"] != null)
            {
                List<GroupLocationVM> groups = new List<GroupLocationVM>();
                int id = Convert.ToInt32(Session["location_id"]);
                try
                {
                    groups = operations.FindAllGroups(id);
                    return View(groups);
                }
                catch (Exception)
                {
                    string message = "Det går inte att hitta grupperna, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
                
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult GroupDetails(int? id)
        {
            if (Session["employe_id"] != null || id != null)
            {
                List<ChildGroupVM> children = new List<ChildGroupVM>();
                try
                {
                    children = operations.GetGroupDetails(id);
                    return View(children);
                }
                catch (Exception)
                {
                    string message = "Det går inte att hitta grupperna, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
                
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult GroupDetails(List<ChildGroupVM> children)
        {
            try
            {
                operations.UpdatePresent(children);
                return RedirectToAction("GroupDetails", new { id = children[0].group_id });
            }
            catch (Exception)
            {
                return RedirectToAction("EmployePage");
            }
            
        }
    }
}