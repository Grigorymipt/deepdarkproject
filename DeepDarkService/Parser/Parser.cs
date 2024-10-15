using DeepDarkService.Models;
using Markdig.Syntax;

namespace DeepDarkService.Parser;

public static class Parser
{
    public static async Task<List<Edge>> GetEdgesFromFile(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        var fileContent = await reader.ReadToEndAsync();
        return GetEdgesFromFile(fileContent);
    }

    public static List<Edge> GetEdgesFromFile(string fileContent)
    {
        var markdownDocument = Markdig.Markdown.Parse(fileContent);
        return ExtractHeadersWithBodies(markdownDocument);
    }


    static List<Edge> ExtractHeadersWithBodies(MarkdownDocument document)
    {
        var result = new List<Edge>();
        string currentHeader = null;
        var currentBody = new List<string>();
        int level = 0; 

        foreach (var node in document)
        {
            if (node is HeadingBlock heading)
            {
                // If we are processing a new header and there was an old one, save the previous header and its body
                if (currentHeader != null)
                {
                    result.Add(
                        new Edge(currentHeader, string.Join(Environment.NewLine, currentBody), level)
                        );
                    currentBody.Clear(); // Clear the body for the new header
                    
                }
                // Get level of the heading 
                level = heading.Level;
                // Capture the header text
                currentHeader = string.Join("", heading.Inline.Select(x => x.ToString()));
            }
            else if (currentHeader != null) // Collect body content until the next header
            {
                currentBody.Add(node.ToString());
            }
        }

        // Add the last header and body after the loop ends
        if (currentHeader != null)
        {
            result.Add(new Edge(currentHeader, string.Join(Environment.NewLine, currentBody), level));
        }
        return result;
    }
}