using System.IO.Compression;

namespace DeepDarkService.Utils;

public static class ZIP
{
    public static async Task<List<string>> ExtractMdFilesFromZip(IFormFile zipFile)
    {
        List<string> mdFileContents = new List<string>();

        // Use a MemoryStream to read the uploaded file
        using var memoryStream = new MemoryStream();
        await zipFile.CopyToAsync(memoryStream);
        memoryStream.Position = 0; // Reset the position to the start

        // Open the ZIP file from the memory stream
        using var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
        // Iterate through each entry in the ZIP archive
        foreach (ZipArchiveEntry entry in zipArchive.Entries)
        {
            // Check if the entry is a markdown file
            if (entry.FullName.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            {
                // Read the content of the markdown file
                using var reader = new StreamReader(entry.Open());
                string content = await reader.ReadToEndAsync();
                mdFileContents.Add(content);
            }
        }

        return mdFileContents;
    }
}