namespace ZadanieRekrutacyjne;

public interface IFactService
{
    Task<FactResult> FetchAndSaveFactAsync();
    Task<string[]> GetSavedFactsAsync();
    void Reset();
}

public record FactResult(CatFact Fact, string SavedTo);
