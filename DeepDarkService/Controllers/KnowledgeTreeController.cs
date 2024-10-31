using System.Drawing;
using System.Linq.Expressions;
using System.Text;
using DeepDarkService.Models;
using DeepDarkService.Picture;
using Microsoft.AspNetCore.Mvc;
using DeepDarkService.Utils;
using Hangfire;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Newtonsoft.Json;
using OxyPlot;
using TorchSharp.Modules;
using Embedding = DeepDarkService.Knowledge.Embedding;
using Math = DeepDarkService.Utils.Math;
using Model = DeepDarkService.Knowledge.Model;

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
        if (file != null && file.FileName.EndsWith(".zip"))
        {
            var files = await ZIP.ExtractMdFilesFromZip(file);
        }
        
        if (file == null || file.Length == 0)
        {   
            if(file == null) return BadRequest("file is null");
            if(file.Length < 1) return BadRequest("file is empty");
            // return BadRequest("Bad extention");
        }
        
    
        // Read file content
        string fileContent;
        if (file.FileName.EndsWith(".md"))
        {
            using var reader = new StreamReader(file.OpenReadStream());
            fileContent = await reader.ReadToEndAsync();
        }
        else if (file.FileName.EndsWith(".zip"))
        {
            fileContent = (await ZIP.ExtractMdFilesFromZip(file)).Aggregate("", (current, s) => current + s);
        }
        else
        {
            return BadRequest("Bad extention");
        }
        
        try
        {
            var makeMap = Task.Run(() =>
            {
                var pathVar = Environment.GetEnvironmentVariable("Maps") ?? throw new InvalidOperationException();
                Console.WriteLine("mind map start");
                
                // Change to commented to save as svg
                // Func<List<Vertex>, List<Edge>,byte[]> saveTypeFunc = (lv, le) => SVG.RenderMindMapToByteArray(lv,le);
                Func<List<Vertex>, List<Edge>,byte[]> saveTypeFunc = (lv, le) =>
                {
                    var graphData = new GraphData()
                    {
                        Links = le.Select(LinkFactory.Create).ToList(),
                        Nodes = lv.Select(NodeFactory.Create).ToList()
                    };
                    string jsonString = JsonConvert.SerializeObject(graphData);
                    // Convert the JSON string to a byte array
                    return Encoding.UTF8.GetBytes(jsonString);
                };
                var imageBytes = FileHandler.HandleWithSVG<int>(fileContent, saveTypeFunc);
                Console.WriteLine("mind map processed");
                System.IO.File.WriteAllBytesAsync(
                    pathVar
                    + file.FileName + ".json",
                    imageBytes
                ).Wait();
                Console.WriteLine("mind map saved in local storage");
            });
            return Accepted(0); //makeMap.id); 
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }  
}