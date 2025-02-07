using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyContact.Models;
using MyContact.Repositories;

namespace MyContact.Controllers
{

    public class ContactSingleController : Controller
    {
        private readonly IContactInterface _contactRepo;
        public ContactSingleController(IContactInterface contactRepo)
        {
            _contactRepo = contactRepo;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                List<t_Contact> contacts = await _contactRepo.GetAllByUser(userId.Value.ToString());
                return View(contacts);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<ActionResult> Logout()
        {
            var session = HttpContext.Session;
            session.Clear();
            if (session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Contact");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public async Task<IActionResult> GetContactById(string id)
        {
            var contact = await _contactRepo.GetOne(id);
            if (contact == null)
            {
                return BadRequest(new { success = false, message = "There was no contact found" });
            }
            return Ok(contact);
        }

        [HttpPost]
        public async Task<ActionResult> Create(t_Contact contact)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    if (contact.ContactPicture != null && contact.ContactPicture.Length > 0)
                    {
                        var fileName = contact.c_Email + Path.GetExtension(contact.ContactPicture.FileName);
                        var filePath = Path.Combine("./wwwroot/contact_images", fileName);
                        Directory.CreateDirectory(Path.Combine("./wwwroot/contact_images"));
                        contact.c_Image = fileName;
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            contact.ContactPicture.CopyTo(stream);
                        }
                    }
                    var userId = HttpContext.Session.GetInt32("UserId");
                    if (userId.HasValue)
                    {
                        contact.c_userId = userId.Value;
                    }
                    var result = 0;
                    if (contact.c_userId == 0)
                    {
                        result = await _contactRepo.Add(contact);
                    }
                    else
                    {
                        result = await _contactRepo.Update(contact);
                    }
                    if (result == 0)
                    {
                        return BadRequest(new { success = false, message = "There was some error while adding the contact" });
                    }
                    else
                    {
                        return Ok(new { success = true, message = "contact Inserted Successfully!!" });
                    }
                }
                else
                {
                    return BadRequest(new { success = false, message = "there was some error while adding the contact" });
                }
            }
            var errors = ModelState.Where(e => e.Value.Errors.Count > 0)
                                    .ToDictionary(
                                        kvp => kvp.Key,
                                        kvp => kvp.Value.Errors.Select(err => err.ErrorMessage).ToArray()
                                    );
            return BadRequest(new { success = false, message = errors });

        }

        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            int status = await _contactRepo.Delete(id);
            if (status == 1)
            {
                return Ok(new { success = true, message = "contact Inserted Successfully!!!" });
            }
            else
            {
                return BadRequest(new { success = false, message = "There was some error while adding the contact" });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}