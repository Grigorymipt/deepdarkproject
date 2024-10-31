namespace DeepDarkService.Models;

public class Node
{
    public string id { get; set; }
    public List<string> groups { get; set; }
    public int size { get; set; }
    public string color { get; set; }
    public string link { get; set; }
    public string text { get; set; }
    public string filename { get; set; }
    public string file_text { get; set; }
    private Node(){}

    public Node(
        string id,
        List<string> groups,
        int size,
        string color,
        string link,
        string text,
        string filename,
        string file_text
    )
    {
        this.id = id;
        this.groups = groups;
        this.size = size;
        this.color = color;
        this.link = link;
        this.text = text;
        this.filename = filename;
        this.file_text = file_text;
    }
}