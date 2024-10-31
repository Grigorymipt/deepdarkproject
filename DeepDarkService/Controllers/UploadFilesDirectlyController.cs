using System.Text;
using DeepDarkService.Models;
using DeepDarkService.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DeepDarkService.Controllers;

public class UploadFilesDirectlyController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> DragNDrop(IFormFile? file)
    {
        if (file != null && file.FileName.EndsWith(".zip"))
        {
            var files = await ZIP.ExtractMdFilesFromZip(file);
        }

        if (file == null || file.Length == 0 || !file.FileName.EndsWith(".json"))
        {
            if (file == null) return BadRequest("file is null");
            if (file.Length < 1) return BadRequest("file is empty");
            // return BadRequest("Bad extention");
        }

        if (file == null || file.Length == 0)
        {
            return BadRequest("File not uploaded.");
        }

        // Define the path to save the file
        var uploadsFolderPath = Environment.GetEnvironmentVariable("Maps") 
                                ?? throw new Exception("Maps folder environment variable is not set.");

        // Ensure the directory exists
        if (!Directory.Exists(uploadsFolderPath))
        {
            Directory.CreateDirectory(uploadsFolderPath);
        }

        // Define the full file path with its original filename
        var filePath = Path.Combine(uploadsFolderPath, file.FileName);

        // Save the file locally
        await using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
        return Ok(new { FilePath = filePath, Message = "File uploaded successfully." });
    }
}