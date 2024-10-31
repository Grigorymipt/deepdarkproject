/*
   Change file path in Embedder
*/
namespace SbertTokenizerEmbedder
{
    class Program
    {
        static void Main(string[] args)
        {
            var emb = new Embedder();
            var embedding_1 = emb.Get("В нашем городе много красивых парков, где можно гулять с друзьями.");
            var embedding_2 = emb.Get("В нашем городе есть много красивых парков, где можно гулять со знакомыми.");
            Console.WriteLine(emb.cosine_similarity(embedding_1, embedding_2));
        }
    }
}