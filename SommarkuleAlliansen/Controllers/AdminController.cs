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
                try
                {
                    List<EmployeGroupLocationVM> employes = operations.GetAllEmployes();
                    return View(employes);
                }
                catch (Exception)
                {
                    string message = "Det går inte att hämta anställda, vänligen försök igen.";
                    return RedirectToAction("Error","Home", new { message = message });
                }
                
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
                try
                {
                    List<caretaker> caretakers = operations.GetAllCaretakers();
                    return View(caretakers);
                }
                catch (Exception)
                {
                    string message = "Det går inte att hämta vårdnadshavare, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Caretaker(List<caretaker> caretakers)
        {
            if (ModelState.IsValid)
            {
                for (int i = 0; i < caretakers.Count; i++)
                {
                    if (caretakers[i].selectedForEmail == true)
                    {
                        try
                        {
                            var body = "Hej " + caretakers[i].caretaker_name + "! Du har fortfarande inte betalat din skuld på " + caretakers[i].debt + ":- vänligen gör detta så snart som möjligt. Mvh Sommarkulan";
                            var message = new MailMessage();
                            message.To.Add(new MailAddress(caretakers[i].caretaker_email));
                            message.From = new MailAddress("sommarkulan@outlook.com");
                            message.Subject = "Betalningspåminnelse";
                            message.Body = string.Format(body);
                            message.IsBodyHtml = true;

                            using (var smtp = new SmtpClient())
                            {
                                await smtp.SendMailAsync(message);
                            }
                        }
                        catch (Exception)
                        {
                            string message = "Det går inte att maila vårdnadshavaren, vänligen försök igen.";
                            return RedirectToAction("Error", "Home", new { message = message });
                        }
                    }
                }
            }
            return RedirectToAction("Caretaker");
        }
        public ActionResult Child()
        {
            if (Session["employe_id"] != null)
            {
                try
                {
                    List<ChildCaretakerLocationVM> children = operations.GetChildCaretakerLocation();
                    return View(children);
                }
                catch (Exception)
                {
                    string message = "Det går inte att hämta barn, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult EditEmploye(int? id)
        {
            if (Session["employe_id"] != null || id != null)
            {
                employe employe = new employe();
                List<EmployeGroupLocationVM> employe_locations = new List<EmployeGroupLocationVM>();
                try
                {
                    employe_locations = operations.GetLocationsBothWeeks();
                    employe = operations.FindEmploye(id);

                    employe_locations.Add(new EmployeGroupLocationVM{
                        name = employe.name,
                        employe_type = employe.employe_type,
                        employe_id = employe.employe_id,
                        number = employe.number,
                        adress = employe.address,
                        password = employe.password,
                        group_id = employe.group_id,
                        location_id = employe.location_id,
                        post_number = employe.post_number,
                        tax = employe.tax,
                        bank = employe.bank,
                        clearing = employe.clearing,
                        account_number = employe.account_number,
                        shirt_size = employe.shirt_size,
                        social_security = employe.social_security
                    });
                }
                catch (Exception)
                {
                    string message = "Det går inte att hitta den anställda, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
                if (employe == null)
                {
                    return HttpNotFound();
                }
                return View(employe_locations);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEmploye(int employe_id, int employe_type, string name, int number, string adress, string password, int group_id, int post_number, bool tax, string bank, int clearing, int account_number, string shirt_size, int social_security)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    long location_id = operations.GetLocationIdByGroup(group_id);
                    location_id = Convert.ToInt32(location_id);
                    operations.UpdateEmploye(employe_id, adress, location_id, employe_type, name, number, password, group_id, post_number, tax, bank, clearing, account_number, shirt_size, social_security);
                    if (employe_id == Convert.ToInt32(Session["employe_id"]))
                    {
                        Session["employe_id"] = employe_id.ToString();
                        Session["name"] = name.ToString();
                        Session["employe_type"] = employe_type.ToString();
                        Session["group_id"] = group_id.ToString();
                        Session["location_id"] = location_id.ToString();
                    }
                    return RedirectToAction("Employe");
                }
                catch (Exception e)
                {
                    string message = "Det går inte att redigera anställda, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
                
            }
            return RedirectToAction("Employe");
        }
        public ActionResult DeleteEmploye(int? id)
        {
            if (Session["employe_id"] != null || id != null)
            {
                EmployeGroupLocationVM employe = new EmployeGroupLocationVM();
                try
                {
                    employe = operations.FindDetailsEmploye(id);
                }
                catch (Exception)
                {
                    string message = "Det går inte att hitta den anställda, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
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
            try
            {
            operations.DeleteEmploye(id);
            return RedirectToAction("Employe");
            }
            catch (Exception)
            {
                string message = "Det går inte att ta bort den anställda, vänligen försök igen.";
                return RedirectToAction("Error", "Home", new { message = message });
            }
        }
        public ActionResult DetailsEmploye(int? id)
        {
            if (Session["employe_id"] != null || id != null)
            {
                EmployeGroupLocationVM employe = new EmployeGroupLocationVM();
                try
                {
                    employe = operations.FindDetailsEmploye(id);
                }
                catch (Exception e)
                {
                    string message = "Det går inte att hitta den anställda, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
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
        public ActionResult CreateEmploye(bool? exsistingName)
        {
            if (Session["employe_id"] != null)
            {
                if (exsistingName == true)
                {
                    ViewBag.Message = "Användarnamnet finns redan. Välj ett annat.";
                }
                else
                {
                    ViewBag.Message = "";
                }
                List<EmployeGroupLocationVM> locations = new List<EmployeGroupLocationVM>();
                try
                {
                    locations = operations.GetLocationsBothWeeks();
                }
                catch (Exception)
                {
                    string message = "Det går inte att lägga till en användare just nu, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
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
        public ActionResult CreateEmploye(int employe_type, string adress, string name, int number, string password, int group_id, int post_number, bool tax, string bank, int clearing, int account_number, string shirt_size, int social_security)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool exsistingName = operations.FindEmployeWithNickname(name);
                    if (exsistingName == false)
                    {
                        long location_id = operations.GetLocationIdByGroup(group_id);
                        operations.AddEmploye(name, adress, employe_type, number, password, group_id, location_id, post_number, tax, bank, clearing, account_number, shirt_size, social_security);
                        return RedirectToAction("Employe");
                    }
                    else
                    {
                        return RedirectToAction("CreateEmploye", new { exsistingName });
                    }
                }
                catch (Exception)
                {
                    string message = "Det går inte att lägga till en användare just nu, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
            }
            return View();
        }
        public ActionResult Edit(int? id)
        {
            if (Session["employe_id"] != null || id != null)
            {
                caretaker caretaker = new caretaker();
                try
                {
                     caretaker = operations.FindCaretaker(id);
                }
                catch (Exception)
                {
                    string message = "Det går inte att hitta vårdnadshavaren, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
                
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
                try
                {
                    operations.UpdateCaretaker(caretaker);
                    return RedirectToAction("Caretaker");
                }
                catch (Exception)
                {
                    string message = "Det går inte att redigera vårdnadshavaren, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
                
            }
            return View(caretaker);
        }
        public ActionResult Details(int? id, bool? justSentMessage)
        {
            if (Session["employe_id"] != null || id != null)
            {
                List<ChildCaretakerLocationVM> caretakerDetails = new List<ChildCaretakerLocationVM>();
                try
                {
                    string employe_type = Convert.ToString(Session["employe_type"]);
                    ViewBag.Item = employe_type;
                    caretakerDetails = operations.GetCaretakerDetails(id);
                }
                catch (Exception)
                {
                    string message = "Det går inte att hitta vårdnadshavaren, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
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
                try
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
                catch (Exception)
                {
                    string message = "Det går inte att maila vårdnadshavaren, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
                
            }
            return View();
        }
        public ActionResult DeleteCaretaker(int? id)
        {
            if (Session["employe_id"] != null || id != null)
            {
                caretaker caretaker = new caretaker();
                try
                {
                    caretaker = operations.FindCaretaker(id);
                }
                catch (Exception)
                {
                    string message = "Det går inte att hitta vårdnadshavaren, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
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
        public ActionResult DeleteCaretaker(int id)
        {
            try
            {
                operations.DeleteCaretaker(id);
                return RedirectToAction("Caretaker");
            }
            catch (Exception)
            {
                string message = "Det går inte att ta bort vårdnadshavaren, vänligen försök igen.";
                return RedirectToAction("Error", "Home", new { message = message });
            }
        }
        public ActionResult EditChild(int? id)
        {
            if (Session["employe_id"] != null || id != null)
            {
                ChildGroupLocationVM child = new ChildGroupLocationVM();
                List<ChildGroupLocationVM> childGroup = new List<ChildGroupLocationVM>();
                List<ChildGroupLocationVM> locations = new List<ChildGroupLocationVM>();
                try
                {
                    locations = operations.GetLocations();
                    child = operations.FindChild(id);
                    childGroup = operations.FindChildGroup(id);
                    child.group_id = childGroup[0].group_id;
                    if (childGroup.Count > 1)
                    {
                        child.group_id2 = childGroup[1].group_id2;
                    }
                    int shirtID = 0;
                    if (child.shirt_size == "80 CL")
                    {
                        shirtID = 0;
                    }
                    else if (child.shirt_size == "100 CL")
                    {
                        shirtID = 1;
                    }
                    else if (child.shirt_size == "120 CL")
                    {
                        shirtID = 2;
                    }
                    else if (child.shirt_size == "140 CL")
                    {
                        shirtID = 3;
                    }
                    else if (child.shirt_size == "Small")
                    {
                        shirtID = 4;
                    }
                    else if (child.shirt_size == "Medium")
                    {
                        shirtID = 5;
                    }
                    else if (child.shirt_size == "Large")
                    {
                        shirtID = 6;
                    }
                    locations.Add(new ChildGroupLocationVM {
                        name = child.name,
                        child_id = child.child_id,
                        comment = child.comment,
                        allergy_comment = child.allergy_comment,
                        can_swim = child.can_swim,
                        caretaker_id = child.caretaker_id,
                        birth_date = child.birth_date,
                        allow_photos = child.allow_photos,
                        vaccinated = child.vaccinated,
                        shirt_size = Convert.ToString(shirtID),
                        location_id = child.location_id,
                        social_security = child.social_security,
                        group_id = child.group_id,
                        group_id2 = child.group_id2
                    });
                }
                catch (Exception)
                {
                    string message = "Det går inte att hitta barnet, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
                if (child == null)
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
        [ValidateAntiForgeryToken]
        public ActionResult EditChild(int child_id, int caretaker_id, string name, DateTime birth_date, int social_security, string shirtSize, int group_id, string allergy_comment, string comment, bool can_swim, bool allow_photos, bool vaccinated)
        {
            if (ModelState.IsValid)
            {
                string shirt_size = "";
                if (shirtSize == "0")
                {
                    shirt_size = "80 CL";
                }
                else if (shirtSize == "1")
                {
                    shirt_size = "100 CL";
                }
                else if (shirtSize == "2")
                {
                    shirt_size = "120 CL";
                }
                else if (shirtSize == "3")
                {
                    shirt_size = "140 CL";
                }
                else if (shirtSize == "4")
                {
                    shirt_size = "Small";
                }
                else if (shirtSize == "5")
                {
                    shirt_size = "Medium";
                }
                else if (shirtSize == "6")
                {
                    shirt_size = "Large";
                }
                List<ChildGroupLocationVM> childGroup = new List<ChildGroupLocationVM>();
                List<groups> groups = new List<groups>();
                groups group = new groups();
                int debt = 300;
                try
                {
                    childGroup = operations.FindChildGroup(child_id);
                    group = operations.GetGroupInfo(group_id);
                    groups = operations.GetGroupId(group.location_id, group.birth_year);
                    group_id = groups[0].group_id;
                    int childGroup_id = childGroup[0].childGroupRelation_id;
                    operations.UpdateChildGroup(group_id, childGroup_id);
                    if (childGroup.Count > 1 && groups.Count > 1)
                    {
                        group_id = groups[1].group_id;
                        childGroup_id = childGroup[1].childGroupRelation_id;
                        operations.UpdateChildGroup(group_id, childGroup_id);
                    }
                    else if (childGroup.Count == 1 && groups.Count > 1)
                    {
                        long child_ID = child_id;
                        
                        operations.AddToGroup(groups, child_ID);
                        operations.UpdateCaretakerDebt(caretaker_id, debt);
                    }
                    else if (childGroup.Count > 1 && groups.Count == 1)
                    {
                        caretaker caretaker = new caretaker();
                        caretaker = operations.FindCaretaker(caretaker_id);
                        childGroup_id = childGroup[1].childGroupRelation_id;
                        operations.DeleteFromRelation(childGroup_id);
                        if (caretaker.debt > debt)
                        {
                            operations.DecreaseCaretakerDebt(caretaker_id, debt);
                        }
                    }
                    operations.UpdateChild(child_id, name, allergy_comment, comment, can_swim, birth_date, allow_photos, vaccinated, shirt_size, social_security, group.location_id);


                    return RedirectToAction("Child");
                }
                catch (Exception e)
                {
                    string message = "Det går inte att redigera barnet, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
                
            }
            return RedirectToAction("Child");
        }
        public ActionResult DetailsChild(int? id)
        {
            if (Session["employe_id"] != null || id != null)
            {
                ChildCaretakerLocationVM childDetails = new ChildCaretakerLocationVM();
                try
                {
                    string employe_type = Convert.ToString(Session["employe_type"]);
                    ViewBag.Item = employe_type;
                    childDetails = operations.GetChildDetails(id);
                }
                catch (Exception)
                {
                    string message = "Det går inte att hitta barnet, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
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
        public ActionResult DeleteChild(int? id)
        {
            if (Session["employe_id"] != null || id != null)
            {
                ChildCaretakerLocationVM childDetails = new ChildCaretakerLocationVM();
                try
                {
                    childDetails = operations.GetChildDetails(id);
                }
                catch (Exception)
                {
                    string message = "Det går inte att hitta barnet, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
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
        [HttpPost]
        public ActionResult DeleteChild(int id)
        {
            try
            {
                List<ChildGroupRelation> selectedGroups = operations.GetGroupInfoFromChild(id);
                int price = 0;
                if (selectedGroups.Count == 2)
                {
                    price = 1000;
                    child child = operations.FindCaretakerByChild(id);
                    operations.DecreaseCaretakerDebt(child.caretaker_id, price);
                }
                else if (selectedGroups.Count == 1)
                {
                    price = 700;
                    child child = operations.FindCaretakerByChild(id);
                    operations.DecreaseCaretakerDebt(child.caretaker_id, price);
                }
                operations.DeleteChildGroup(id);
                operations.DeleteChild(id);
                return RedirectToAction("Child");
            }
            catch (Exception e)
            {
                string message = "Det går inte att ta bort den anställda, vänligen försök igen.";
                return RedirectToAction("Error", "Home", new { message = message });
            }
        }
        public ActionResult EditInformation(int? id)
        {
            if (Session["employe_id"] != null || id != null)
            {
                information information = new information();
                try
                {
                    information = operations.FindInformation(id);
                }
                catch (Exception)
                {
                    string message = "Det går inte att hitta informationen, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
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
                try
                {
                    operations.UpdateInformation(information);
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception)
                {
                    string message = "Det går inte att uppdatera informationen, vänligen försök igen.";
                    return RedirectToAction("Error", "Home", new { message = message });
                }
            }
            return View(information);
        }
    }
}