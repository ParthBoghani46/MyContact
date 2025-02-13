using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyContact.Models;
using MyContact.Repositories;

namespace MyContact.Controllers
{
    public class UserController : Controller
    {

        private readonly IUserInterface _user;

        private readonly IValidator<RegisterVM> _validator;
        public UserController(IUserInterface user, IValidator<RegisterVM> validator)
        {
            _user = user;
            _validator = validator;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(vm_Login login)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid input. Please check your details." });
            }

            t_User UserData = await _user.Login(login);

            if (UserData != null && UserData.c_userId != 0)
            {
                HttpContext.Session.SetInt32("UserId", UserData.c_userId);
                HttpContext.Session.SetString("UserName", UserData.c_userName);
                HttpContext.Session.SetString("UserProfilePicture", UserData.c_Image ?? "user.png");

                return Json(new { success = true, message = "User logged in successfully!", redirectUrl = Url.Action("Index", "Home") });
            }
            else
            {
                return Json(new { success = false, message = "Invalid Username or Password" });
            }
        }



        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        public ActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }
        [HttpPost]
        public async Task<IActionResult> Register(t_User user)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid input. Please check your details." });
            }


            if (user.ProfilePicture != null && user.ProfilePicture.Length > 0)
            {
                var fileName = user.c_Email + Path.GetExtension(user.ProfilePicture.FileName);
                var filePath = Path.Combine("./wwwroot/profile_images/", fileName);
                Directory.CreateDirectory(Path.Combine("./wwwroot/profile_images"));
                user.c_Image = fileName;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    user.ProfilePicture.CopyTo(stream);
                }
            }


            // contact.c_userId = (int)HttpContext.Session.GetInt32("UserId");
            // Console.WriteLine("user.c_fname: " + user.c_userName);
            var status = await _user.Register(user);
            if (status == 1)
            {
                TempData["message"] = "User Registred";
                // return RedirectToAction("Login");
                return Json(new { success = true, message = "User Registerd in successfully!", redirectUrl = Url.Action("Login", "User") });
            }
            else if (status == 0)
            {
                return Json(new { success = false, message = "User Exist Already!!" });
            }
            else
            {
                TempData["message"] = "There was some error while Registration";
                return Json(new { success = false, message = TempData["message"] });

            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
        public IActionResult FRegister()
        {
            return View();
        }
        [HttpPost]
        public IActionResult FRegister(RegisterVM model)
        {
            ValidationResult result = _validator.Validate((model));
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(model);
            }
            TempData["SuccessMessage"] = "Registration successful!";
            return RedirectToAction("FRegister", "User");
        }
    }
}