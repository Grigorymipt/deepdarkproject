using System.Globalization;
using System.Numerics;
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
    public static List<Edge> GetListOfEdges2(List<Vertex> vertexList)
    {
        var emb = new Embedder();
        return GetListOfEdgesFromCustomEmb(
            vertexList, 
            inputString => emb.Get(inputString).Select(a => (double)a)
            );
    }

    private static List<Edge> GetListOfEdgesFromCustomEmb(List<Vertex> vertexList, Func<string,IEnumerable<double>> embed)
    {       
        var threshold = double.Parse(Environment.GetEnvironmentVariable("Threshold") 
                                    ?? throw new NullReferenceException("Threshold"), CultureInfo.InvariantCulture);

        var embList = vertexList.Select(x => embed(VertexToString(x))).ToList();
        List<Edge> edges = new();
        for (int i = 0; i < embList.Count; i++)
        {
            for (int j = i + 1; j < embList.Count; j++)
            {
                if (Math.Cos(embList[i], embList[j]) > threshold)
                {
                    edges.Add(new Edge(vertexList[i].Id, vertexList[j].Id, "horizontal"));
                } 
            }
        }
        return edges;
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

    // public static string HandleWithPlainText(string fileContent)
    // {
    //     var vertexList = GetListOfVertices(fileContent);
    //     foreach (var vertex in vertexList)
    //     {
    //         Console.WriteLine(VertexToString(vertex));
    //     }
    //     var edgesList = GetListOfEdges(vertexList);
    //     return EdgesToString(edgesList, vertexList);
    // }

    
    
    public static byte[] HandleWithSVG<T>(
        string fileContent,
        Func<List<Vertex>, List<Edge>, byte[]> func
        ) where T: IBinaryInteger<int>
    {
        var vertexList = GetListOfVertices(fileContent);
        foreach (var vertex in vertexList)
        {
            Console.WriteLine(VertexToString(vertex));
        }
        var edgesList = GetListOfEdges2(vertexList);
        return func(vertexList, edgesList);
    }
    
    // TODO: Fixme
    // public static byte[] HandleWithSVG(string fileContent) 
    // {
    //     var newFunc = (vertexList, edgeList) => SVG.RenderMindMapToSVG(fileContent, vertexList, edgeList);
    //     Func<int, int> a = (EE, EF) => Console.WriteLine(EF);
    //     HandleWithSVG<int>(fileContent, (list, edges) => new byte[]{} );
    // }
}