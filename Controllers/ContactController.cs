using MyContact.Models;
using MyContact.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public async Task<JsonResult> GetCities(int stateId)
        {
            var cities = await _contact.GetCity(stateId);
            return Json(cities);
        }



        [HttpGet]
        public async Task<ActionResult> Create(string id = "")
        {
            // Load Status List
            List<Status> status = await _contact.GetStatusList();
            ViewBag.StatusList = status.Select(s => new SelectListItem
            {
                Value = s.statusId.ToString(),
                Text = s.status
            }).ToList();

            // Load State List
            List<State> state = await _contact.GetState();
            ViewBag.StateList = state.Select(s => new SelectListItem
            {
                Value = s.stateId.ToString(),
                Text = s.stateName
            }).ToList();

            t_Contact contact = new t_Contact();
            List<SelectListItem> cityList = new List<SelectListItem>(); // Initialize empty list

            if (!string.IsNullOrEmpty(id))
            {
                // Fetch contact details for editing
                contact = await _contact.GetOne(id);

                List<City> cities = await _contact.GetCity(contact.c_stateid);
                cityList = cities.Select(c => new SelectListItem
                {
                    Value = c.cityId.ToString(),
                    Text = c.cityName,
                    Selected = c.cityId == contact.c_cityid  // Mark selected city
                }).ToList();

            }

            ViewBag.CityList = cityList; // Assign cities to ViewBag for dropdown

            return View(contact);
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
                        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/contact_images");
                        Directory.CreateDirectory(directoryPath); // Ensure directory exists

                        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(contact.c_Email);
                        var newFileExtension = Path.GetExtension(contact.ContactPicture.FileName);
                        var newFileName = $"{fileNameWithoutExt}{newFileExtension}";
                        var newFilePath = Path.Combine(directoryPath, newFileName);

                        Console.WriteLine($"New file will be saved at: {newFilePath}");

                        try
                        {
                            // Delete old files
                            var oldFiles = Directory.GetFiles(directoryPath, fileNameWithoutExt + ".*");
                            foreach (var oldFile in oldFiles)
                            {
                                Console.WriteLine($"Deleting old file: {oldFile}");
                                System.IO.File.Delete(oldFile);
                            }
                            await Task.Delay(100);
                            // Save the new profile image
                            await using (var stream = new FileStream(newFilePath, FileMode.Create))
                            {
                                await contact.ContactPicture.CopyToAsync(stream);
                            }

                            Console.WriteLine("File saved successfully!");
                            contact.c_Image = newFileName; // Save file name in the database
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error while saving file: {ex.Message}");
                        }
                    }
                    contact.c_userId = (int)HttpContext.Session.GetInt32("UserId");
                    var result = 0;

                    if (contact.c_contactId == 0)
                    {// Assign only for new contact
                        result = await _contact.Add(contact);
                    }
                    else
                    {
                        string contactid = Convert.ToString(contact.c_contactId);
                        result = await _contact.Update(contact);
                    }

                    if (result == 0)
                    {
                        return Json(new { success = false, message = "There is some error while adding or updating contact" });
                    }
                    else
                    {
                        TempData["Message"] = "Contact Added/Updated Successfully";
                        return Json(new { success = true, message = "Contact Added/Updated Successfully", redirectUrl = Url.Action("List", "Contact") });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "There is some error while adding or updating contact" });
                }
            }
            return Json(new { success = false, message = "There is some error while adding or updating contact" });
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