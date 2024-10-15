using System.ComponentModel.DataAnnotations;

namespace DeepDarkService.Models;

public class Vertex
{
    [Key]
    public Guid Id { get; set; } = new Guid();
   
    [Required]
    public Guid First { get; set; }
    [Required]
    public Guid Second { get; set; }
    
    [Required]
    public string Relation { get; set; }
    
    // EF can use this due to reflection
    private Vertex() { }

    // For other users
    public Vertex(Guid first, Guid second, string relation)
    {
        First = first;   
        Second = second;
        Relation = relation;
    }
}