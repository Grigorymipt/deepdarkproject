namespace DeepDarkService.Models;

public static class LinkFactory
{
    public static Link Create(Edge edge)
        => new(
            source: edge.First.ToString(), 
            target: edge.Second.ToString(),
            color: "#ED3833");
}