using Microsoft.AspNetCore.Mvc;

namespace DeepDarkService.Controllers;

public class KnowledgeGraphController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
