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
        HomeDatabaseOperations operations = new HomeDatabaseOperations();
        public ActionResult Index()
        {
            List<information> informations = new List<information>();
            try
            {
                informations = operations.GetInformation();
            }
            catch (Exception)
            {
                string message = "Det går inte att hämta hemsidan, vänligen försök igen.";
                return RedirectToAction("Error", "Home", new { message = message });
            }
            
            return View(informations);
        }
        public ActionResult Error(string message)
        {
            if (message == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Message = message;
            return View();
        }
        public ActionResult Register()
        {
            List<location> locations = new List<location>();
            try
            {
                locations = operations.FindAllLocations();
            }
            catch (Exception)
            {
                string message = "Det går inte att hämta platserna, vänligen försök igen.";
                return RedirectToAction("Error", "Home", new { message = message });
            }
            return View(locations);
        }
        [HttpPost]
        public ActionResult Register(int location_id, string child_name, DateTime birth, string shirtSize, bool CanSwim, bool allowPhoto, bool isVaccinated, string comment, string caretakerName, string caretakerAddress, string caretakerEmail, int caretakerNumber, string altName, int altNumber)
        {
            long caretaker_id = 0;
            long child_id = 0;
            List<groups> groups = new List<groups>();
            DateTime start_date = DateTime.Now;
            DateTime end_date = DateTime.Now;
            location selectedLocation = new location();
            if (ModelState.IsValid)
            {
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    try
                    {
                        caretaker_id = operations.SearchForExistingCaretaker(caretakerEmail);
                        selectedLocation = operations.GetLocationInformation(location_id);
                        groups = operations.GetGroupId(location_id, birth);

                        if (caretaker_id != 0)
                        {
                            operations.UpdateCaretakerDebt(caretaker_id, selectedLocation.price);
                        }
                        if (caretaker_id == 0)
                        {
                            caretaker_id = operations.AddCaretaker(caretakerName, caretakerNumber, caretakerEmail, caretakerAddress, altName, altNumber, selectedLocation.price);
                        }
                        child_id = operations.AddChild(child_name, comment, caretaker_id, CanSwim, birth, allowPhoto, isVaccinated, shirtSize, location_id);
                        operations.AddToGroup(groups, child_id);
                        return RedirectToAction("OrderConfermation", new { caretakerEmail, caretakerName, child_name, selectedLocation.price, selectedLocation.start_date, selectedLocation.end_date, selectedLocation.location_name });
                    }
                    catch (Exception)
                    {
                        string message = "Det går inte att registrera barnet, vänligen försök igen.";
                        return RedirectToAction("Error", "Home", new { message = message });
                    }

                }
            }
            return View();
        }
        
        public async Task<ActionResult> OrderConfermation(string caretakerEmail, string caretakerName, string child_name, int price, DateTime start_date, DateTime end_date, string location_name)
        {
            if (ModelState.IsValid)
            {
                var body = "Tack för anmälan " + caretakerName + ". Ditt barn " + child_name + " är anmäld för perioden " + String.Format("{0:MM/dd}", start_date) + " - " + String.Format("{0:MM/dd}", end_date) + " i föreningen " + location_name + ". Du ska betala " + price + ":- inom 2 veckor.";
                var message = new MailMessage();
                message.To.Add(new MailAddress(caretakerEmail));
                message.From = new MailAddress("sommarkulan@outlook.com");
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
                var body = "<p>Email Från: {0} ({1})</p><p>Meddelande: {2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress("sommarkulan@outlook.com"));
                message.From = new MailAddress("sommarkulan@outlook.com");
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