using Microsoft.AspNetCore.Mvc;

namespace DeepDarkService.Controllers;

public class SavedMapsController : Controller
{
    private readonly string _pathToMaps 
        = Environment.GetEnvironmentVariable("Maps") 
          ?? throw new InvalidOperationException();
    // GET
    public IActionResult Index()
    {
        FileInfo[] files = new DirectoryInfo(path: _pathToMaps).GetFiles("*.json"); 
        
        var texts = files.Select(fileInfo =>
        {
            using var reader = fileInfo.OpenText();
            string fileContent = reader.ReadToEnd();
            return fileContent;
        }).ToList();
        
        
        return View(files.Select(
            f => Url.Action("Index", "KnowledgeGraph", new { filename = f.Name })
            ).ToList());
    }
}