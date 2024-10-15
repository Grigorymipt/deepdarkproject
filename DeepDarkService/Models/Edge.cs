using System.ComponentModel.DataAnnotations;

namespace DeepDarkService.Models;

public class Edge
{
    [Key]
    public Guid Id { get; set; } = new Guid();
    
    [Required]
    public string Header { get; set; }
    
    [Required]
    public string Body { get; set; }
    
    public int LocalDepth { get; set; }
    
    // EF can use this due to reflection
    private Edge() { }

    // For other users
    public Edge(string header, string body, int localDepth)
    {
        Header = header;   
        Body = body;
        LocalDepth = localDepth;
    }
}
