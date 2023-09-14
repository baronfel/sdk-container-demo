using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using sdk_container_demo.Models;
using System.Reflection;

namespace sdk_container_demo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var version = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyInformationalVersionAttribute>().First().InformationalVersion;
        return View(new { version });
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
}
