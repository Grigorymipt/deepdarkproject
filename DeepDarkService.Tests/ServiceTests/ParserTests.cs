using Xunit.Abstractions;

namespace DeepDarkService.Tests.ServiceTests;
using DeepDarkService.Parser;

public class ParserTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ParserTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void SimpleDocParse()
    {
        var mdString =
            "# My Project\n\nThis is an overview of my project.\n\n## Features\n\nHere are the key features of the project:\n\n- Feature 1\n- Feature 2\n- Feature 3\n\n### Feature 1: Details\n\nThis section provides more details about **Feature 1**.\n\n### Feature 2: Details\n\nHere’s more information about **Feature 2**.\n\n## Getting Started\n\nTo get started, follow these steps:\n\n1. Clone the repository.\n2. Install dependencies.\n3. Run the project.\n\n### Prerequisites\n\nYou will need the following installed:\n\n- .NET SDK\n- Visual Studio Code\n\n## Contact\n\nFor more information, reach out to me at [myemail@example.com](mailto:myemail@example.com).\n";
        var edges = Parser.GetVerticesFromFile(mdString).Select(a => a.Header).ToList();
        _testOutputHelper.WriteLine(edges.Aggregate((acc, x) => $"{acc}\n{x}"));
        Assert.Equal(7, edges.Count);
    }
}