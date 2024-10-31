namespace DeepDarkService.Models;

public class Link
{
    public string source { get; set; }
    public string target { get; set; }
    public string color { get; set; }
    
    private Link(){}
    
    public Link(
        string source,
        string target, 
        string color)
    {
        this.source = source;
        this.target = target;
        this.color = color;
    }
}