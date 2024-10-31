using Microsoft.ML;
using Microsoft.ML.Transforms.Onnx;

namespace DeepDarkService.Knowledge;

public class Model(OnnxScoringEstimator estimator, Tokenizer tokenizer, MLContext mlContext)
{
    public OnnxScoringEstimator Estimator = estimator;
    public Tokenizer Tokenizer = tokenizer;
    public MLContext MlContext = mlContext;
}
    