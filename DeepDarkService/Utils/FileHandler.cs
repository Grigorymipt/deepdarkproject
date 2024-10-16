using DeepDarkService.Knowledge;
using DeepDarkService.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DeepDarkService.Utils;

public static class FileHandler
{
    public static List<Vertex> GetListOfVertices(string fileContent)
        => Parser.Parser.GetVerticesFromFile(fileContent);

    // TODO: rm hardcode
    private const double Threshold = 0.9;
    private const int MaxChunkSize = 50;
    
    public static List<Edge> GetListOfEdges(List<Vertex> vertexList)
        => vertexList.Select(x => vertexList.Where(y
                => Math.Cos(Embedding.Get<double>(VertexToString(x)),
                    Embedding.Get<double>(VertexToString(y))) > Threshold)
            .Select(y => new Edge(x.Id, y.Id, "horizontal")))
            .Aggregate((acc, x) => acc.Concat(x)).ToList();
    
    private static string VertexToString(Vertex vertex) 
        => vertex.Header 
           + vertex.Body.Take(
               System.Math.Min(MaxChunkSize - vertex.Header.Length, 0));
    public static string EdgesToString(List<Edge> edges, List<Vertex> vertices)
    {
        if (vertices.Count < 2) throw new Exception("file is must have at least 2 vertices");
        
        string edgesString = "Your edges:\n" 
                             + edges.Aggregate("", (current, edge) 
                                 => current + $"First: {edge.First}, Second: {edge.Second}\n");
        string verticesString = "Your vertices:\n" + vertices.Aggregate("", (current, vertex) 
            => current + $"Header: {vertex.Header}, GUID: {vertex.Id}\n");
        
        string outputString = verticesString + edgesString;
        
        return outputString;
    }

    public static string Handle(string fileContent)
    {
        var vertexList = GetListOfVertices(fileContent);
        return EdgesToString(GetListOfEdges(vertexList), vertexList);
    }
    

}