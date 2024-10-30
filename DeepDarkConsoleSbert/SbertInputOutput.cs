using Microsoft.ML.Data;

using SbertTokenizerEmbedder;

// Define a class for the input data
public class SbertInput
{
    [VectorType(3, 512)] // Adjust according to your input size
    public long[] input_ids { get; set; } = new long[3 * 512];
}
// Define a class for the output data
public class SbertOutput
{
    [VectorType(1, 1, 512, 1024)]// Adjust based on your model's output shape
    public float[] output { get; set; } = new float[512 * 1024];
}