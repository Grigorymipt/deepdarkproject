namespace DeepDarkService.Utils;

public static class JobResultStore
{
    private static readonly Dictionary<string, object> JobResults = new Dictionary<string, object>();

    public static void StoreResult(string jobId, object result)
    {
        JobResults[jobId] = result;
    }

    public static object GetResult(string jobId)
    {
        JobResults.TryGetValue(jobId, out var result);
        return result;
    }
}