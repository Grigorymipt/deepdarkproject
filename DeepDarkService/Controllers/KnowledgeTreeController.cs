using System.Text;
using Microsoft.AspNetCore.Mvc;
using DeepDarkService.Utils;
using TorchSharp.Modules;
using Embedding = DeepDarkService.Knowledge.Embedding;
using Math = DeepDarkService.Utils.Math;

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
            if(file.Length < 1) return BadRequest("file is empty");
            if (!Validator.FileExtension(file.FileName)) return BadRequest("Bad extention");
        }

        // Read file content
        string fileContent;
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            fileContent = await reader.ReadToEndAsync();
        }

        string modifiedContent;
        try
        {
            modifiedContent = FileHandler.Handle(fileContent);
            // Step 3: Convert the modified content to a byte array
            byte[] modifiedBytes = Encoding.UTF8.GetBytes(modifiedContent);

            // Step 4: Return the modified file to the client
            return File(modifiedBytes, "application/octet-stream", "modified_" + file.FileName);
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }   
}