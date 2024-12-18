using System.Globalization;
using DeepDarkService.Knowledge;
using DeepDarkService.Utils;
using Xunit.Abstractions;
using Math = DeepDarkService.Utils.Math;

namespace DeepDarkService.Tests.ServiceTests;

public class EmbeddingTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public EmbeddingTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void SimpleEmbedding()
    {
        var model = Embedding.GetModel();
        var a = "A huge, frightening red car was bouncing over the rough road, heading toward a faraway city.";
        var b = "A large terrifying red car was driving over bumps towards a distant city.";
        var c = "Blue dolphin jumps on blue cold waves in the Pacific ocean.";
        
        var abc = new List<string>() { a, b, c };
        
        var eabc = abc.Select(
                z
            => Embedding.Get(z, model)
                .Select(x 
                    => (double)x
                    )
            )
            .ToList();
        var ea = eabc[0];
        var eb = eabc[0];
        var ec = eabc[0];
        
        var dp1 = Math.Cos(ea, eb);
        var dp2 = Math.Cos(ea, ec);
        _testOutputHelper.WriteLine(dp1.ToString("F2"));
        _testOutputHelper.WriteLine(dp2.ToString("F2"));
        Assert.True(dp1 > dp2);
    }

    [Fact(Skip = "russian is supported very bad")]
    public void SimpleEmbeddingRussian()
    {
        var model = Embedding.GetModel();
        var a = "Сегодня солнечный день, и я планирую прогуляться в парке. Мне нравится проводить время на свежем воздухе, наслаждаясь природой.";
        var b = "В этот ясный день я собираюсь выйти на улицу и провести время в сквере. Мне приятно быть на природе и наслаждаться окружающим миром.";
        var c = "Я готовлюсь к важному экзамену и много занимаюсь математикой. Надеюсь, что смогу успешно его сдать.";
        var abc = new List<string>() { a, b, c };
        
        var eabc = abc.Select(
                z
                    => Embedding.Get(z, model)
                        .Select(x 
                            => (double)x
                        )
            )
            .ToList();
        var ea = eabc[0];
        var eb = eabc[0];
        var ec = eabc[0];
        
        var dp1 = Math.Cos(ea, eb);
        var dp2 = Math.Cos(ea, ec);
        _testOutputHelper.WriteLine(dp1.ToString("F2"));
        _testOutputHelper.WriteLine(dp2.ToString("F2"));
        Assert.True(dp1 > dp2);
    }
}