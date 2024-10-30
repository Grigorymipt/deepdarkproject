using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace DeepDarkService.Tests.ServiceTests;

public class ConfigTest
{ 
    private readonly ITestOutputHelper _testOutputHelper;

    

    [Fact]
    public void Test_Config()
    {
        // _testOutputHelper.WriteLine("Your variable: "+_configuration["Environment"].FirstOrDefault().ToString());
        Assert.True(1 == 1);
    }
}