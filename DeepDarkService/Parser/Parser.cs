using DeepDarkService.Models;
using Markdig.Syntax;

namespace DeepDarkService.Parser;

public static class Parser
{
    public static async Task<List<Vertex>> GetVerticesFromFile(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        var fileContent = await reader.ReadToEndAsync();
        return GetVerticesFromFile(fileContent);
    }

    public static List<Vertex> GetVerticesFromFile(string fileContent)
    {
        var markdownDocument = Markdig.Markdown.Parse(fileContent);
        return ExtractHeadersWithBodies(markdownDocument);
    }


    static List<Vertex> ExtractHeadersWithBodies(MarkdownDocument document)
    {
        var result = new List<Vertex>();
        string currentHeader = null;
        string currentBody = "";
        int level = 0; 

        foreach (var node in document)
        {
            if (node is HeadingBlock heading)
            {
                // If we are processing a new header and there was an old one, save the previous header and its body
                if (currentHeader != null)
                {
                    result.Add(
                        new Vertex(currentHeader, string.Join(Environment.NewLine, currentBody), level)
                        );
                    currentBody = ""; // Clear the body for the new header
                    
                }
                // Get level of the heading 
                level = heading.Level;
                // Capture the header text
                currentHeader = string.Join("", heading.Inline.Select(x => x.ToString()));
            }
            else if (currentHeader != null) // Collect body content until the next header
            {
                currentBody += node.ToString();
            }
        }

        // Add the last header and body after the loop ends
        if (currentHeader != null)
        {
            result.Add(new Vertex(currentHeader, string.Join(Environment.NewLine, currentBody), level));
        }
        return result;
    }
}