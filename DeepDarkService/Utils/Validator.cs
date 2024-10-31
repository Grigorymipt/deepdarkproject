namespace DeepDarkService.Utils;

public static class Validator
{
    private const string InputFileExtension = ".md"; 
    
    public static bool FileExtension(string fileName) => Path.GetExtension(fileName) == InputFileExtension;
    
    public static bool FileExtension(List<string> fileNames) => 
        fileNames.Aggregate(false, (current, fileName) => current || FileExtension(fileName));

}