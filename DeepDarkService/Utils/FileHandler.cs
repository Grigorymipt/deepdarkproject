using System.Globalization;
using DeepDarkService.Knowledge;
using DeepDarkService.Models;
using DeepDarkService.Picture;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DeepDarkService.Utils;

public static class FileHandler
{
    public static List<Vertex> GetListOfVertices(string fileContent)
        => Parser.Parser.GetVerticesFromFile(fileContent);
    // TODO: hardcode, rm Console
    public static List<Edge> GetListOfEdges(List<Vertex> vertexList)
    {
        var threshold = double.Parse(Environment.GetEnvironmentVariable("Threshold") 
                                    ?? throw new NullReferenceException("Threshold"), CultureInfo.InvariantCulture);
        return vertexList.Select(x => vertexList.Where(y
                =>
                {
                    var ea = Embedding.Get(VertexToString(x)).Select(a => (double)a);
                    var eb = Embedding.Get(VertexToString(y)).Select(a => (double)a);
                    var cosine = Math.Cos(ea, eb);
                    // Console.WriteLine(cosine);
                    // Console.WriteLine(x.Header);
                    // Console.WriteLine(y.Header);
                    return (cosine > threshold) && (x!=y);
                })
                .Select(y => new Edge(x.Id, y.Id, "horizontal")))
                .Aggregate((acc, x) => acc.Concat(x)).ToList();
    }

    // TODO: Add length control 
    private static string VertexToString(Vertex vertex)
        => vertex.Header + vertex.Header;
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

    public static string HandleWithPlainText(string fileContent)
    {
        var vertexList = GetListOfVertices(fileContent);
        foreach (var vertex in vertexList)
        {
            Console.WriteLine(VertexToString(vertex));
        }
        var edgesList = GetListOfEdges(vertexList);
        return EdgesToString(edgesList, vertexList);
    }

    public static byte[] HandleWithSVG(string fileContent)
    {
        var vertexList = GetListOfVertices(fileContent);
        foreach (var vertex in vertexList)
        {
            Console.WriteLine(VertexToString(vertex));
        }
        var edgesList = GetListOfEdges(vertexList);
        return SVG.RenderMindMapToByteArray(vertexList, edgesList);
    }
}