using DeepDarkService.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DeepDarkService.Controllers;

public class KnowledgeGraphController : Controller
{
    public IActionResult Index(string filename)
    {
        var filePath = Environment.GetEnvironmentVariable("Maps") 
                       ?? throw new InvalidOperationException();
        
        string lines = System.IO.File.ReadAllText(filePath + filename);
        
        GraphData graphData = JsonConvert.DeserializeObject<GraphData>(lines);
        
        var mimeType = "application/json";
        // var jsonData = System.IO.File.ReadAllText(filePath + filename);
        
        var links = new List<Link>()
        {
            new (source: "A", target: "B", color: "#ED3833"),
            new (source: "A", target: "C", color: "#ED3833"),
            new (source: "B", target: "C", color: "#ED3833"),
            new (source: "C", target: "E", color: "#ED3833"),
            new (source: "C", target: "D", color: "#ED3833"),
            new (source: "G", target: "H", color: "#ED3833"),
            new (source: "H", target: "E", color: "#ED3833")
        };
        var nodes = new List<Node>()
        {
            new(
                id: "A",
                groups: ["lasers"],
                size: 1,
                color: "#1f77b4",
                link: "http://127.0.0.1:5001/api/test_get",
                text: "Лазеры",
                filename: "file1",
                file_text: ""
            ),
            new(
                id: "B",
                groups: ["object"],
                size: 2,
                color: "#ff7f0e",
                link: "none",
                text: "Тоже лазеры",
                filename: "file1",
                file_text: ""
            ),
            new(
                id: "C",
                groups: ["people"],
                size: 2,
                color: "#2ca02c",
                link: "none",
                text: "Тоже лазеры не",
                filename: "file1",
                file_text: ""
            ),
            new(
                id: "D",
                groups: ["people"],
                size: 4,
                color: "#d62728",
                link: "none",
                text: "Тоже не лазеры",
                filename: "file1",
                file_text: ""
            ),
            new(
                id: "E",
                groups: ["people"],
                size: 3,
                color: "#9467bd",
                link: "none",
                text: "Что это?",
                filename: "file1",
                file_text: ""
            ),
            new(
                id: "F",
                groups: ["file"],
                size: 4,
                color: "#9467bd",
                link: "none",
                text: "Одинокая точка...",
                filename: "file1",
                file_text: ""
            ),
            new(
                id: "G",
                groups: ["people"],
                size: 2,
                color: "#2ca02c",
                link: "none",
                text: "Бобруйск",
                filename: "file1",
                file_text: ""
            ),

            new(
                id: "H",
                groups: ["file"],
                size: 5,
                color: "#1f77b4",
                link: "none",
                text: "Ты бывал в бобруйске?",
                filename: "file1",
                file_text: "")

        };
        return View((graphData.Nodes, graphData.Links));
    }
}
