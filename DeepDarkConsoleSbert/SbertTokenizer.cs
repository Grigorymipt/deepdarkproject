using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using SbertTokenizerEmbedder;

public class SbertTokenizer
    {
        [JsonPropertyName("vocab")]
        private Dictionary<string, long> _tokenizer { get; set; }

        public SbertTokenizer(string filePath)
        {
            string jsonVocabString = File.ReadAllText(filePath);
            _tokenizer = JsonSerializer.Deserialize<Dictionary<string, long>>(jsonVocabString);
        }
        public List<List<long>> Tokenize(string str, long batch_size)
        {
            str = Regex.Replace(str, @"[^\w\s]", "").ToLower();
            var words = str.Split(' ');
            var semi_answer = new List<long>();
            for (int i = 0; i < words.Length; i++)
            {
                var tokens = TokenizeOne(words[i]);
                semi_answer.AddRange(tokens);
            }
            var answer = get_batches(semi_answer, batch_size);
            return answer;
        }
        private List<long> TokenizeOne(string word)
        {
            List<long> tokens = new List<long>();

            int start = 0;

            // Continue until the whole word is tokenized
            while (start < word.Length)
            {
                int end = word.Length;
                long subword = _tokenizer["[UNK]"];
                // Try to find the longest possible subword in the vocabulary
                while (start < end)
                {
                    string candidate = word.Substring(start, end - start);

                    // If we're not at the start, we prefix the subword with '##' to indicate a subword
                    if (start > 0)
                    {
                        candidate = "##" + candidate;
                    }

                    // Check if the candidate subword is in the vocabulary
                    if (_tokenizer.ContainsKey(candidate))
                    {
                        subword = _tokenizer[candidate];
                        break;
                    }

                    // Shorten the candidate subword by one character from the end
                    end--;
                }

                // If a valid subword is found, add it to the tokens list
                if (subword != _tokenizer["[UNK]"])
                {
                    tokens.Add(subword);
                    start = end; // Move the start index to the end of the found subword
                }
                else
                {
                    // If no subword is found, add the unknown token and break the loop
                    tokens.Add(_tokenizer["[UNK]"]);
                    break;
                }
            }
            return tokens;
        }
        private List<List<long>> get_batches(List<long> str, long batch_size)
        {
            var answer = new List<List<long>>();
            var current = new List<long>();
            current.Add(_tokenizer["[CLS]"]);
            foreach (var i in str)
            {
                current.Add(i);
                if (current.Count >= batch_size)
                {
                    answer.Add(current);
                    current = new List<long>();
                    current.Add(_tokenizer["[CLS]"]);
                }
            }
            if (current.Count != 0)
            {
                while (current.Count < batch_size)
                {
                    current.Add(_tokenizer["[PAD]"]);
                }
                answer.Add(current);
            }
            return answer;
        }
    }