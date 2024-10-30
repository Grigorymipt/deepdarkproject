using Microsoft.ML;
using Microsoft.ML.Transforms.Onnx;

using SbertTokenizerEmbedder;

public struct SbertModel(OnnxScoringEstimator estimator, SbertTokenizer tokenizer, MLContext mlContext)
{
    public OnnxScoringEstimator Estimator = estimator;
    public SbertTokenizer Tokenizer = tokenizer;
    public MLContext MlContext = mlContext;
}