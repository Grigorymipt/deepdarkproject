using Microsoft.ML.Data;

namespace DeepDarkService.Knowledge;


public class Tokenizer
{
    private Dictionary<string, int> _tokenizer;

    public Tokenizer(string vocabFilePath)
    {
        // Load vocabulary from file
        _tokenizer = LoadVocabulary(vocabFilePath);
    }

    private Dictionary<string, int> LoadVocabulary(string vocabFilePath)
    {
        var vocab = new Dictionary<string, int>();
        var lines = File.ReadAllLines(vocabFilePath);
        
        for (int i = 0; i < lines.Length; i++)
        {
            vocab[lines[i]] = i; // Map each token to its index
        }

        return vocab;
    }

    public (long[] inputIds, long[] attentionMask) Tokenize(string text, int maxLength = 512)
    {
        var tokens = new List<long> { _tokenizer["[CLS]"] }; // Start with [CLS]
        
        // Split the input text into words
        var words = text.ToLower().Split(new[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in words)
        {
            // Look for the word in the tokenizer
            if (_tokenizer.TryGetValue(word, out var tokenId))
            {
                tokens.Add(tokenId); // Add the token ID
            }
            else
            {
                tokens.Add(_tokenizer["[UNK]"]); // Add [UNK] token for unknown words
            }
        }
        
        tokens.Add(_tokenizer["[SEP]"]); // End with [SEP]

        // Create attention mask
        var attentionMask = new long[tokens.Count];
        for (int i = 0; i < tokens.Count; i++)
        {
            attentionMask[i] = 1; // 1 for real tokens
        }

        // Padding
        while (tokens.Count < maxLength)
        {
            tokens.Add(_tokenizer["[PAD]"]); // Add [PAD] token ID (typically 0)
            Array.Resize(ref attentionMask, tokens.Count); // Resize attention mask to match input length
            attentionMask[tokens.Count - 1] = 0; // Padding token in attention mask
        }

        return (tokens.ToArray(), attentionMask);
    }
}
