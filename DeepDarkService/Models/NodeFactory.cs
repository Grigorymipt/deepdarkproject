namespace DeepDarkService.Models;

public class NodeFactory
{
    public static Node Create(Vertex vertex)
        => new Node(
            id: vertex.Id.ToString(), 
            groups: ["node"], 
            size: vertex.LocalDepth, 
            color: "#ffffff", 
            link: "", 
            text: vertex.Header, 
            filename: "", // TODO: add logic 
            file_text: vertex.Body
        );
}