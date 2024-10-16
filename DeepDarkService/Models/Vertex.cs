using System.ComponentModel.DataAnnotations;

namespace DeepDarkService.Models;

public class Vertex
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public string Header { get; set; }
    
    [Required]
    public string Body { get; set; }
    
    public int LocalDepth { get; set; }
    
    // EF can use this due to reflection
    private Vertex() { }

    // For other users
    public Vertex(string header, string body, int localDepth)
    {
        Header = header;   
        Body = body;
        LocalDepth = localDepth;
    }
}
