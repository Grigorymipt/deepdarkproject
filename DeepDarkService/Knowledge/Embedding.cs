using System.Globalization;
using System.Numerics;
using DeepDarkService.Models;
using Microsoft.ML;
using Microsoft.ML.Transforms.Onnx;
using TorchSharp;

namespace DeepDarkService.Knowledge;

public static class Embedding
{
    public static float[] Get(string inputStr) 
    {
        // Tokenization
        string vocabFilePath = Environment
            .GetEnvironmentVariable("VOCAB_PATH") ?? throw // Specify the path
            new InvalidOperationException();
        
        var tokenizer = new Tokenizer(vocabFilePath);

        var (inputIds, attentionMask) = tokenizer.Tokenize(inputStr, maxLength: 10); // Set a max length as needed
	    
	    // 1. Create MLContext
        MLContext mlContext = new MLContext();

        // 2. Define the path to the ONNX model
        string modelPath = Environment
            .GetEnvironmentVariable("BERT_ONNX_PATH") ?? throw // Specify the path
            new InvalidOperationException();
        
        // 3. Prepare your input data
        var inputData = new[]
        {
            new InputData
            {
                InputIds = inputIds, // Tokenized input example
                AttentionMask = attentionMask // Attention mask example
            }
        };

        // 4. Load input data into IDataView
        var dataView = mlContext.Data.LoadFromEnumerable(inputData);

        // 5. Create the OnnxScoringEstimator
        var onnxEstimator = mlContext.Transforms.ApplyOnnxModel(
            modelFile: modelPath,
            inputColumnNames: new[] { "input_ids", "attention_mask" },
            outputColumnNames: new[] { "avg_embeddings" } // Adjust based on your model output
        );

        // 6. Fit the estimator to create a transformer
        var transformer = onnxEstimator.Fit(dataView);

        // 7. Transform the input data using the transformer to get predictions
        var transformedData = transformer.Transform(dataView);

        // 8. Create an enumerable for the output
        var predictions = mlContext.Data.CreateEnumerable<OutputData>(transformedData, reuseRowObject: false);

        // 9. Print the predictions

        var outputDatas = predictions as OutputData[] ?? predictions.ToArray();
        
        float[] avgEmbeddingsArray = new float[outputDatas.FirstOrDefault().LastHiddenState.Length];
	    outputDatas.FirstOrDefault().LastHiddenState.CopyTo(avgEmbeddingsArray);
	    return avgEmbeddingsArray;
    }
    public static List<T> Get<T>(string inputStr) where T: INumber<T> 
        => Get(inputStr).Select(x => T.Parse(
            s: x.ToString(CultureInfo.InvariantCulture),
            style: NumberStyles.Any, 
            provider: CultureInfo.InvariantCulture)).ToList();
}