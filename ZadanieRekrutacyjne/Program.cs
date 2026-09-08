using ZadanieRekrutacyjne;

var builder = WebApplication.CreateBuilder(args);

// Starting the http client
builder.Services.AddHttpClient<IFactService, FactService>(client =>
{
    client.BaseAddress = new Uri("https://catfact.ninja/");
});

var app = builder.Build();

// Deleting the txt file at the start of the session
using (var scope = app.Services.CreateScope())
{
    var factService = scope.ServiceProvider.GetRequiredService<IFactService>();
    factService.Reset();
}

// Setting up the UI from wwwroot folder
app.UseDefaultFiles();
app.UseStaticFiles();

// API endpoints
app.MapGet("/fetch-fact", async (IFactService factService) =>
{
    try
    {
        var result = await factService.FetchAndSaveFactAsync();
        return Results.Ok(new
        {
            message = "Fact fetched and saved.",
            savedTo = result.SavedTo,
            fact = result.Fact
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/facts", async (IFactService factService) =>
{
    var facts = await factService.GetSavedFactsAsync();
    return Results.Ok(facts);
});

app.MapDelete("/delete-facts", (IHostEnvironment env) =>
{
    var path = Path.Combine(env.ContentRootPath, "facts.txt");
    if (!File.Exists(path))
    {
        return Results.NotFound(new { message = "File not found." });
    }

    try
    {
        File.Delete(path);
        return Results.Ok(new { message = "File deleted.", deleted = path });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.Run();
