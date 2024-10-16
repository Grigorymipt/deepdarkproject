using System;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Annotations;
using DeepDarkService.Models;

// To make this work you should add this package
// dotnet add package OxyPlot.Core 

// Usage : described in Main function. Basically, you should pass output 
// file directory (probably relative path), list of Vertex and list of 
// Edge to RenderMindMapToSVG function. But I don't know, will it work without
// Main method and Program class. Anyway...  

namespace DeepDarkService.Picture;

public class SVG
{
    public static void RenderMindMapToSVG(string filePath, List<Vertex> vertices, List<Edge> edges){
        
        // Create a PlotModel for the mind map
        var plotModel = new PlotModel { Title = "Mind Map" };

        // plot vertices and edges on a PlotModel
        PutAllEdges(plotModel, vertices, edges);

        // Export the plot to an image file
        SavePlotToImage(plotModel, filePath, 1000, 800);

        // Console output about ABSOLUTE success
        Console.WriteLine("Mind map saved successfully");
    }

    // get file with svg image in it. File location - filePath
    public static byte[] RenderMindMapToByteArray(List<Vertex> vertices, List<Edge> edges){
        
        // Create a PlotModel for the mind map
        var plotModel = new PlotModel { Title = "Mind Map" };

        // plot vertices and edges on a PlotModel
        PutAllEdges(plotModel, vertices, edges);

        // Get file with svg image
        var file = GetPlotSVGImageFile(plotModel, 1000, 800);

        // Console output about ABSOLUTE success
        Console.WriteLine("Mind map saved successfully");

        // Then we can write bytes into svg file, if we need
        // File.WriteAllBytes("mindmap.svg", file);
        return file;
    }

    // Draw grapg on PlotModel
    private static void PutAllEdges(PlotModel plotModel, List<Vertex> verticies, List<Edge> edges){
        // Changing background color
        plotModel.Background = OxyColor.FromArgb(255, 255, 255, 255); // Using RGB values
        
        // Series of edges and vertices
        var scatterSeries = new ScatterSeries
        {
            MarkerType = MarkerType.Circle,
            MarkerSize = 20,
            MarkerFill = OxyColors.Red
        };
        var lineSeries = new LineSeries
        {
            StrokeThickness = 3,
            Color = OxyColors.GreenYellow
        };

        // Drawing randomly, just for now
        var random = new Random();

        //Structure to contain coordinates of verticies to correctly make connections
        var TupleVertex = new List<Tuple<Vertex, double, double>>();
        
        foreach (var vertex in verticies){
            var pos_x = random.NextDouble();
            var pos_y = random.NextDouble();
            TupleVertex.Add(new Tuple<Vertex, double, double>(vertex, pos_x, pos_y));
            plotModel.Annotations.Add(CreateLabel(vertex.Header, pos_x, pos_y));
            scatterSeries.Points.Add(new ScatterPoint(pos_x, pos_y, 40.0d / vertex.LocalDepth));
        }

        {
            foreach(var edge in edges){
                var FirstIdFromEdge = edge.First;
                var SecondIdFromEdge = edge.Second;
                var CorrespondingTupleFirst = TupleVertex.Find(tuple => tuple.Item1.Id == FirstIdFromEdge);
                var CorrespondingTupleSecond = TupleVertex.Find(tuple => tuple.Item1.Id == SecondIdFromEdge);
                if ((CorrespondingTupleFirst != null) && (CorrespondingTupleSecond != null)){
                    AddConnection(lineSeries,
                     CorrespondingTupleFirst.Item2, CorrespondingTupleFirst.Item3,
                     CorrespondingTupleSecond.Item2, CorrespondingTupleSecond.Item3);
                }
            }
        }

        // Scale dots...
        var sc = new ScatterSeries
        { MarkerType = MarkerType.Square, MarkerSize = 2, MarkerFill = OxyColors.White };
        sc.Points.Add(new ScatterPoint(-0.2, -0.2));
        sc.Points.Add(new ScatterPoint(1.2, 1.2));

        // Add Series into plotModel object
        plotModel.Series.Add(lineSeries);
        plotModel.Series.Add(scatterSeries);
        plotModel.Series.Add(sc);

    }

    // Method to add a line between two points
    private static void AddConnection(LineSeries series, double x1, double y1, double x2, double y2)
    {
        series.Points.Add(new DataPoint(x1, y1));
        series.Points.Add(new DataPoint(x2, y2));
    }

    // Method to create a TextAnnotation for labeling vertices...
    private static TextAnnotation CreateLabel(string text, double x, double y)
    {
        return new TextAnnotation
        {
            Text = text,
            TextPosition = new DataPoint(x, y),
            Stroke = OxyColors.Transparent,
            FontSize = 8,
            TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Center
        };
    }

    private static void SavePlotToImage(PlotModel model, string filePath, int width, int height)
    {
        // Ensure the directory exists
        var directory = Path.GetDirectoryName(filePath);
        if (directory != null)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
        // Export to SVG
        using (var stream = File.Create(filePath))
        {
            var pngExporter = new SvgExporter{ Width = width, Height = height};
            pngExporter.Export(model, stream);
        }
    }

    // get fileStream object with written svg file in it
    public static byte[] GetPlotSVGImageFile(PlotModel model, int width, int height)
    {
        // Export to SVG
        using (var stream = new MemoryStream()) // Create a memory stream
        {
            var pngExporter = new SvgExporter { Width = width, Height = height };
            pngExporter.Export(model, stream);  // Export image to the memory stream
            return stream.ToArray();            // Return the image as a byte array
        }
    }
}