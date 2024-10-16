using Microsoft.ML.Data;

namespace DeepDarkService.Knowledge;

public class InputData
{
    [ColumnName("input_ids")]
    public long[] InputIds { get; set; }

    [ColumnName("attention_mask")]
    public long[] AttentionMask { get; set; }
}