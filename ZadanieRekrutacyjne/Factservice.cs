using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace ZadanieRekrutacyjne;

public class FactService : IFactService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FactService> _logger;
    private const string FilePath = "facts.txt";

    public FactService(HttpClient httpClient, ILogger<FactService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<FactResult> FetchAndSaveFactAsync()
    {
        _logger.LogInformation("Calling catfact.ninja for a new fact.");

        var response = await _httpClient.GetStringAsync("fact");

        var catSingleFact = JsonSerializer.Deserialize<CatFact>(
            response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("Could not parse the response from catfact.ninja.");

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var entry = $"[{timestamp}] Fact: {catSingleFact.Fact} (Length: {catSingleFact.Length}){Environment.NewLine}";

        // Creates the file on first call, adds new line if file already exists
        await File.AppendAllTextAsync(FilePath, entry);

        var fullPath = Path.GetFullPath(FilePath);
        _logger.LogInformation("Saved fact to {FilePath}", fullPath);

        return new FactResult(catSingleFact, fullPath);
    }

    public async Task<string[]> GetSavedFactsAsync()
    {
        if (!File.Exists(FilePath))
        {
            return Array.Empty<string>();
        }

        var lines = await File.ReadAllLinesAsync(FilePath);
        return lines.Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
    }

    public void Reset()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
            _logger.LogInformation("Deleted existing {FilePath} for a fresh start.", FilePath);
        }
    }
}