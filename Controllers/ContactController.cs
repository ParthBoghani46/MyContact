using MyContact.Models;
using MyContact.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MyContact
{
    public class ContactController : Controller
    {
        private readonly IContactInterface _contact;

        public ContactController(IContactInterface contact)
        {
            _contact = contact;
        }

        // GET: ContactController
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> List()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                List<ContactListViewModel> contacts = await _contact.GetAllByUser(HttpContext.Session.GetInt32("UserId").ToString());
                return View(contacts);
            }
            else
            {
                return RedirectToAction("Login", "User");
                // List<t_Contact> contacts = await _contact.GetAll();
                // return View(contacts);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Create(string id = "")
        {
            t_Contact contacts = new t_Contact();
            if (id != "")
            {
                contacts = await _contact.GetOne(id);
            }
            return View(contacts);
        }

        [HttpPost]
        public async Task<IActionResult> Create(t_Contact contact)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    if (contact.ContactPicture != null && contact.ContactPicture.Length > 0)
                    {

                        var fileName = contact.c_Email + Path.GetExtension(contact.ContactPicture.FileName);
                        var filePath = Path.Combine("./wwwroot/contact_images/", fileName);
                        Directory.CreateDirectory(Path.Combine("./wwwroot/contact_images"));
                        contact.c_Image = fileName;
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            contact.ContactPicture.CopyTo(stream);
                        }
                    }
                    // Console.WriteLine("contact.c_fname: " + contact.c_contactName);

                    contact.c_userId = (int)HttpContext.Session.GetInt32("UserId");
                    var result = 0;
                    if (contact.c_contactId == 0)
                    {
                        result = await _contact.Add(contact);
                    }
                    else
                    {
                        result = await _contact.Update(contact);
                    }

                    if (result == 0)
                    {
                        TempData["Message"] = "There is Some Error While Add or Update Contact";
                        return RedirectToAction("List", "Contact");
                    }
                    else
                    {
                        TempData["Message"] = "Contact Added/Updated Successfully";
                        return RedirectToAction("List", "Contact");
                    }
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }



        // public IActionResult Create()
        // {
        //     return View();
        // }
        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            int status = await _contact.Delete(id);
            if (status == 1)
            {
                ViewData["Message"] = "Contact Added Successfully";
                return RedirectToAction("List");
            }
            else
            {
                ViewData["Message"] = "There is some error while delete contact";
                return RedirectToAction("List");
            }
        }
    }
}