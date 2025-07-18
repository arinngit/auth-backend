namespace AuthenticationMicroservice.DataAccess.Helpers;

public static class SqlLoader
{
    public static string Load(string relativePath)
    {
        String baseUrl = Path.Combine(AppContext.BaseDirectory, "SqlQueries");
        String fullPath = Path.Combine(baseUrl, relativePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found: {fullPath}");
        }

        return File.ReadAllText(fullPath);
    }
}
