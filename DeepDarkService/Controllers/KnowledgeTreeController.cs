using System.Linq.Expressions;
using System.Text;
using DeepDarkService.Picture;
using Microsoft.AspNetCore.Mvc;
using DeepDarkService.Utils;
using Hangfire;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using TorchSharp.Modules;
using Embedding = DeepDarkService.Knowledge.Embedding;
using Math = DeepDarkService.Utils.Math;

namespace DeepDarkService.Controllers;

public class KnowledgeTreeController(ILogger<HomeController> logger, IConfiguration configuration) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly IConfiguration _configuration = configuration;

    public IActionResult Index()
    {
        Console.WriteLine(_configuration["Environment"]);
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
            // var modifiedContent = FileHandler.HandleWithSVG(fileContent);
           
            // return File(modifiedContent, "image/svg+xml", "mind_map_from_" + file.FileName);

            // TODO: fixme
            // var task = ()
            //     => 1;
            // {
            //     return 
            //         // return File(
            //         //     FileHandler.HandleWithSVG<int>(fileContent),
            //         //     "image/svg+xml",
            //         //     "mind_map_from_" + file.FileName
            //         // );
            //     };
            

            // Return job ID to track status
            return File(
                FileHandler.HandleWithSVG<int>(fileContent, SVG.RenderMindMapToByteArray),
                "image/svg+xml",
                "mind_map_from_" + file.FileName
            );
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