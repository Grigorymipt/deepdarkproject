namespace DeepDarkService.Models;

public class Relation
{  
    public string Name { get; set; }

    private Relation() {
    }

    public Relation(string name)
    {
        Name = name;
    }
}