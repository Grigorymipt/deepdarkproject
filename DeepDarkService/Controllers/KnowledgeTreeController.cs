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
        
        try
        {
            var modifiedContent = FileHandler.HandleWithSVG(fileContent);
           
            return File(modifiedContent, "image/svg+xml", "mind_map_from_" + file.FileName);
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }  
    [HttpPost]
    public async Task<IActionResult> DragNDropToText(IFormFile? file)
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
        
        try
        {
            var modifiedContent = FileHandler.HandleWithPlainText(fileContent);
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