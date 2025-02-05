using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyContact.Models;

namespace MyContact.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Helper()
    {
        MyViewModel model = new MyViewModel();
        model.FirstName = "Test";
        return View(model);
    }
    [HttpPost]
    public IActionResult Helper(MyViewModel model)
    {
        ViewData["Message"] = model.FirstName + " " + model.LastName;
        return View();
    }

}
