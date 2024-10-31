using DeepDarkService.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeepDarkService.Controllers;

public class MapToViewController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
    // GET
    // public IActionResult LoadGraph(string filename)
    // {
    //     var filePath = Environment.GetEnvironmentVariable("Resources") 
    //         ?? throw new InvalidOperationException();
    //     var mimeType = "application/json";
    //     var jsonData = System.IO.File.ReadAllText(filePath + filename);
    //     
    //     return View(links);
    // }
}