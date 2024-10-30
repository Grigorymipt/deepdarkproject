using System.IO;
using System.Text;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


string path = "";
if (args.Length > 1)
    path = args[1];
else
    path = Environment.GetEnvironmentVariable("InputDocs") ?? throw new InvalidOperationException();

DirectoryInfo d = new DirectoryInfo(path: path); //Assuming Test is your Folder

FileInfo[] files = d.GetFiles("*.txt"); //Getting Text files

Func<string[], string> promptTemplate = args => 
    args.Aggregate((a, b) => $"{a}, {b}");

Func<string, Task<string>> askLlama = async input =>
{
    string url = "http://localhost:11434/api/generate";
    string prompt = input;
    string filename = args[0];

    // Read the content of the file
    string text = File.ReadAllText(filename);

    // Prepare the request headers and data
    var client = new HttpClient();
    client.DefaultRequestHeaders.Add("Content-Type", "application/json");

    var data = new
    {
        model = "llama3.1",
        prompt = prompt + text,
        stream = false
    };

    // Convert data to JSON
    string jsonData = JObject.FromObject(data).ToString();

    // Make the POST request
    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
    HttpResponseMessage response = await client.PostAsync(url, content);

    if (response.IsSuccessStatusCode)
    {
        string responseText = await response.Content.ReadAsStringAsync();
        var jsonResponse = JObject.Parse(responseText);
        string actualResponse = jsonResponse["response"]?.ToString();
        return actualResponse;
    }
    else
    {
        throw new Exception($"Error: {response.StatusCode}, {response.Content.ReadAsStringAsync().Result}");
    }
};

var texts = files.Select(fileInfo =>
{
    using var reader = fileInfo.OpenText();
    string fileContent = reader.ReadToEnd();
    return fileContent;
}).ToList();

string prompt = ""; 
if (args.Length == 0) 
    prompt = Environment.GetEnvironmentVariable("PromptTemplate") ?? throw new InvalidOperationException();
else
    prompt = args[0];

    

List<string> ties = [""];
for (int i = 0; i < texts.Count; i++)
{
    for (int j = i + 1; j < texts.Count; j++)
    {
        ties.Add($"{prompt} {texts[i]} {texts[j]}");
    }
}

var outputs = ties.Select(t => askLlama(t));
foreach (var output in outputs) { Console.WriteLine(output); }

Console.WriteLine("Finished");