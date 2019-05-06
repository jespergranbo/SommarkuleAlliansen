using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using SommarkuleAlliansen.Models;
using SommarkuleAlliansen.ViewModel;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;

namespace SommarkuleAlliansen.Controllers
{
    public class AdminController : Controller
    {
        AdminDatabaseOperations operations = new AdminDatabaseOperations();
        string constr = ConfigurationManager.ConnectionStrings["smconnection"].ConnectionString;
        public ActionResult Employe()
        {
            if (Session["employe_id"] != null)
            {
                List<EmployeLocationVM> employes = operations.GetAllEmployes();
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
                List<caretaker> caretakers = operations.GetAllCaretakers();
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
                List<ChildCaretakerLocationVM> children = operations.GetChildCaretakerLocation();
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
                employe = operations.FindEmploye(id);
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
                operations.UpdateEmploye(employe);
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
                operations.FindEmploye(id);
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
            operations.DeleteEmploye(id);
            return RedirectToAction("Employe");
        }
        public ActionResult CreateEmploye()
        {
            if (Session["employe_id"] != null)
            {
                List<EmployeGroupLocationVM> locations = new List<EmployeGroupLocationVM>();
                locations = operations.GetLocations();
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
                long location_id = operations.GetLocationIdByGroup(group_id);
                operations.AddEmploye(name, employe_type, number, password, group_id, location_id);
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
                caretaker caretaker = operations.FindCaretaker(id);
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
                operations.UpdateCaretaker(caretaker);
                return RedirectToAction("Caretaker");
            }
            return View(caretaker);
        }
        public ActionResult Details(int? id, bool? justSentMessage)
        {
            if (Session["employe_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                List<ChildCaretakerLocationVM> caretakerDetails = new List<ChildCaretakerLocationVM>();
                caretakerDetails = operations.GetCaretakerDetails(id);
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
                    return RedirectToAction("Details", "Admin", new { caretaker_id = caretaker_id, justSentMessage = justSentMessage });
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
                child child = operations.FindChild(id);
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
                operations.UpdateChild(child);
                return RedirectToAction("Child");
            }
            return View(child);
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
                childDetails = operations.GetChildDetails(id);
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
            if (Session["employe_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                information information = new information();
                information = operations.FindInformation(id);
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
                operations.UpdateInformation(information);
                return RedirectToAction("Index", "Home");
            }
            return View(information);
        }
    }
}