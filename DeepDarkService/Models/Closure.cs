using System.ComponentModel.DataAnnotations;

namespace DeepDarkService.Models;

public class Closure
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
   
    [Required]
    public Guid Ancestor { get; set; }
    [Required]
    public Guid Descendant { get; set; }
    [Required]
    public Guid Depth { get; set; }
    
}