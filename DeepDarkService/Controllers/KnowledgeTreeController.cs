using System.Text;
using Microsoft.AspNetCore.Mvc;
using DeepDarkService.Utils;

namespace DeepDarkService.Controllers;

public class KnowledgeTreeController(ILogger<HomeController> logger) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;

    public IActionResult Index()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> DragNDrop(IFormFile? file)
    {
        if (file == null || file.Length == 0 || !Validator.FileExtension(file.FileName))
        {   
            if(file == null) return BadRequest("file is null");
            if(file.Length == 0) return BadRequest("file is zero length");
            if (!Validator.FileExtension(file.FileName)) return BadRequest("Bad extention");
        }

        // Read file content
        string fileContent;
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            fileContent = await reader.ReadToEndAsync();
        }

        // Extract headers only
        var headers = Parser.Parser.GetEdgesFromFile(fileContent)
            .Select(edge => edge.Header).Aggregate("", (acc, edge) => $"{acc}\n{edge}")
            .ToString();
        
        string modifiedContent = "You headers are:\n" + headers;

        // Step 3: Convert the modified content to a byte array
        byte[] modifiedBytes = Encoding.UTF8.GetBytes(modifiedContent);

        // Step 4: Return the modified file to the client
        return File(modifiedBytes, "application/octet-stream", "modified_" + file.FileName);
    }   
}