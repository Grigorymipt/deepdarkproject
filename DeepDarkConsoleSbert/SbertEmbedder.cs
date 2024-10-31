using Microsoft.ML;
using Microsoft.ML.Transforms.Onnx;

using SbertTokenizerEmbedder;
public class Embedder
{
    public Embedder() => init();
    public const int batch_size = 512;   // This is valid for traced sbert model, i think
    public string vocabPath = Environment
            .GetEnvironmentVariable("SBERT_VOCAB") ?? throw // Specify the path
            new InvalidOperationException(); 

    public string modelPath = Environment
            .GetEnvironmentVariable("SBERT_MODEL") ?? throw // Specify the path
            new InvalidOperationException();

    public PredictionEngine<SbertInput, SbertOutput> predictionEngine = null;
    public OnnxTransformer model = null;
    public SbertModel sbertModel;

    public void init()
    {
        
        var mlContext = new MLContext();
        var enc = new SbertTokenizer(vocabPath);

        // Create the pipeline
        var pipeline = mlContext.Transforms.ApplyOnnxModel(
            modelFile: modelPath,
            inputColumnNames: new[] { "input_ids" },
            outputColumnNames: new[] { "output" });
        // Create an empty data view based on input data schema
        var emptyData = mlContext.Data.LoadFromEnumerable(new SbertInput[] { });
        // Fit the pipeline to the empty data view
        model = pipeline.Fit(emptyData);
        // Create a PredictionEngine for making predictions
        predictionEngine = mlContext.Model.CreatePredictionEngine<SbertInput, SbertOutput>(model);

        sbertModel = new SbertModel(pipeline, enc, mlContext);
        Console.WriteLine("Model loaded");
    }
    public float[] Get(string input_string)
    {
        var batches = sbertModel.Tokenizer.Tokenize(input_string, batch_size);
        var one_batch = batches[0];
        var answer = new List<float>();
        var answers = new List<List<float>>();

        // Use the prediction engine to predict
        var input = new long[3 * batch_size];
        foreach (var batch in batches)
        {
            Array.Copy(get_input(batch), input, batch_size * 3);
            
            var bert_input = new SbertInput { input_ids = input };
            var bert_output = predictionEngine.Predict(bert_input);
            
            var emb_size = bert_output.output.Length / batch_size;
            for (int j = 0; j < batch_size; j++)
            {
                if (input[(2 * batch_size) + j] != 0)
                {
                    var semi_answer = new float[emb_size];
                    for (int i = 0; i < emb_size; i++)
                    {
                        semi_answer[i] = bert_output.output[i + j * emb_size];                    }
                    answers.Add(semi_answer.ToList());
                }
            }
        }
        answer = mean_pulling(answers);

        return answer.ToArray();
    }
    public long[] get_input(List<long> tokens)
    {
        var answer = new long[batch_size * 3];
        for (int i = 0; i < batch_size; i++)
        {
            answer[i] = tokens[i];
            answer[i + batch_size] = 0;
            if (tokens[i] == 0) { answer[i + batch_size * 2] = 0; }
            else { answer[i + batch_size * 2] = 1; }
        }
        return answer;
    }
    public List<float> mean_pulling(List<List<float>> embeddings)
    {
        var answer = new List<float>();
        for (int i = 0; i < embeddings[0].Count; i++)
        {
            var current = 0.0f;
            for (int j = 0; j < embeddings.Count; j++)
            {
                current += embeddings[j][i];
            }
            current = current / embeddings.Count;
            answer.Add(current);
        }
        return answer;
    }
    public float dot_product(float[] a, float[] b)
    {
        var answer = 0.0f;
        for (int i = 0; i < a.Length; i++)
        {
            answer += a[i] * b[i];
        }
        return answer;
    }
    public List<float> masked(List<float> a, long[] b)
    {
        var answer = new List<float>();
        for (int i = 0; i < a.Count; i++)
        {
            answer.Add(a[i] * b[i]);
        }
        return answer;
    }
    public float cosine_similarity(float[] a, float[] b)
    {
        float answer = dot_product(a, b) / (float)Math.Sqrt(dot_product(a, a) * dot_product(b, b));
        return answer;
    }
}