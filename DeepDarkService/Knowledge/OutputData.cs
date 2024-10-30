using Microsoft.ML.Data;

namespace DeepDarkService.Knowledge;

public class OutputData
{
    [ColumnName("avg_embeddings")]
    public VBuffer<float> LastHiddenState { get; set; } // Or adjust based on the output of TinyBERT
}