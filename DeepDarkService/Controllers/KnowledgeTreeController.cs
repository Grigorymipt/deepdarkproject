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

        // Extract headers only
        var headers = Parser.Parser.GetEdgesFromFile(fileContent);
            // .Select(edge => edge.Header).Aggregate("", (acc, edge) => $"{acc}\n{edge}")
            // .ToString();
        
        if (headers.Count < 2) return BadRequest("file is must have at least 2 headers");
        // compare first and last headers of md
        
        var firstEmb = Embedding.Get($"{headers.First().Header}\n{headers.First().Body}")
            .Select(x => (double)x);
        var lastEmb = Embedding.Get($"{headers.Last().Header}\n{headers.Last().Body}")
            .Select(x => (double)x);
        
        var dotProduct = Math.Cos(firstEmb, lastEmb);
        string modifiedContent = $"Cosine of the first and last paragraph: {dotProduct}" +
                                 $"\nFirst paragraph: {headers.First().Header}\nLast paragraph: {headers.Last().Header}";

        // Step 3: Convert the modified content to a byte array
        byte[] modifiedBytes = Encoding.UTF8.GetBytes(modifiedContent);

        // Step 4: Return the modified file to the client
        return File(modifiedBytes, "application/octet-stream", "modified_" + file.FileName);
    }   
}