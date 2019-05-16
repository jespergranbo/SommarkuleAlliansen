﻿using System;
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
            catch (Exception e)
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
        public ActionResult Register(int location_id, string child_name, DateTime birth, string shirtSize, bool CanSwim, bool allowPhoto, bool isVaccinated, string comment, string caretakerName, string caretakerAddress, string caretakerEmail, int caretakerNumber, string altName, int altNumber, int social_security, string allergy_comment)
        {
            long caretaker_id = 0;
            long child_id = 0;
            List<groups> groups = new List<groups>();
            DateTime start_date = DateTime.Now;
            DateTime end_date = DateTime.Now;
            DateTime registration_date = DateTime.Now;
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
                        child_id = operations.AddChild(child_name, comment, caretaker_id, CanSwim, birth, allowPhoto, isVaccinated, shirtSize, location_id, registration_date, social_security, allergy_comment);
                        operations.AddToGroup(groups, child_id);
                        return RedirectToAction("OrderConfermation", new { caretakerEmail, caretakerName, child_name, selectedLocation.price, selectedLocation.start_date, selectedLocation.end_date, selectedLocation.location_name, selectedLocation.location_email,selectedLocation.location_number, birth, shirtSize, CanSwim, allowPhoto, isVaccinated, allergy_comment, comment, caretakerAddress, caretakerNumber, altName, altNumber, selectedLocation.weeks });
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
        public string CheckIfTrue(bool check)
        {
            string result = "";
            if (check == true)
            {
                result = "Ja";
            }
            else
            {
                result = "Nej";
            }
            return result;
        }
        public async Task<ActionResult> OrderConfermation(string caretakerEmail, string caretakerName, string child_name, int price, DateTime start_date, DateTime end_date, string location_name, string location_email, int location_number, DateTime birth,string shirtSize, bool CanSwim, bool allowPhoto, bool isVaccinated, string allergy_comment, string comment, string caretakerAddress, int caretakerNumber, string altName, int altNumber, string weeks)
        {
            if (ModelState.IsValid)
            {
                string result = "";
                
                var body = "<p>Hej " + caretakerName + "</p> <p>här är BEKRÄFTELSEN på din anmälan till Sommarkulan! Följande uppgifter om dig har registrerats.</p> <p>Observera att eftersom det inte går att besvara detta mail så måste du om du har frågor eller vill ändra något kontakta din Förening "
                    + location_name + ", ring (+46) " + location_number + " eller maila till "
                    + location_email + "</p> <b>BETALINFO :</b> <p> Du skall betala " + price + ":- för årets SOMMARKULA.</p> <p>Avgiften,"
                    + price + ":- sista betaldatum är 2019-06-14, till bankgiro 5894-0172 eller SWISH till 123 613 34 25</p><p></p>" +
                //HÄR KOMMER ALL INFORMATION SOM DEM REGISTRERAT SIG MED
                "<p>Namn: " + child_name + "</p><p>Födelsedatum: " + String.Format("{0:yyy/MM/dd}", birth) + "</p><p>Tröjstorlek: " + shirtSize + "</p><p>" + (result = CheckIfTrue(CanSwim)) + "</p><p>Tillåt att barnet är med på foton: " + (result = CheckIfTrue(allowPhoto)) + " </p><p>Vaccinerad mot stelkramp: " + (result = CheckIfTrue(isVaccinated)) + "</p><p>Allergi: " + allergy_comment + "</p><p>Övrig info: " + comment + "</p><p></p>" +
                "<p>Målsman: " + caretakerName + ", " + caretakerAddress + ", Telnr: (+46)" + caretakerNumber + "</p><p>Alt.kontakt: " + altName + ", Telnr: (+46)" + altNumber + "</p><p></p>" +
                "<p>Period: " + location_name + " " + String.Format("{0:MM/dd}", start_date) + " - " + String.Format("{0:MM/dd}", end_date) + " (" + weeks + ")</p>" +
                "<p>Plats: " + location_name + "</p><p>Samling: Måndag 17 Juni kl 9:00</p><p>Kontakt: " + location_email + ", " + location_number + "</p>" +
                "<p>Du kan redan nu betala din avgift. Vänligen ange barnets namn (" + child_name + ") samt plats ("+ location_name + ") på din betalning.</p> <p>Kom ihåg!</p><p>Kläder efter väder samt badkläder. Tisdagar, onsdagar samt fredagar 28/6 åker vi till Fjärran och  badar. " +
                "Torsdagar åker vi till DOME https://www.thedome.se/ </p><p>Ingen Sommarkulan på midsommarafton.</p><p>Materialpaket ingår till ALLA!</p<p>Tiderna som gäller är 09:00 - 15:00 varje dag.</p><p>Varje dag ingår en lättlunch som består av youghurt/kräm, smörgås eller varmkorv. " +
                "Frukt och mjölk ingår i alla måltider.</p><h3>VÄLKOMMEN önskar vi på SOMMARKULAN</h3>";

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
        public ActionResult Sent()
        {
            return View();
        }
    }
}