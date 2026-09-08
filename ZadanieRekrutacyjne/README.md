# FactApp

A minimal ASP.NET Core web app with one endpoint. Every time you call it, it fetches
a fact from https://catfact.ninja/fact and saves it to `facts.txt`.

- First call: `facts.txt` is created with the first fact.
- Every call after that: a new line is appended to the same file.

## Requirements
- .NET 8 SDK installed (https://dotnet.microsoft.com/download)

## Run it

```bash
dotnet run --project .\ZadanieRekrutacyjne\ZadanieRekrutacyjne.csproj
```

By default it will listen on `http://localhost:5000` (the console output on startup shows the exact URLs).

## Try it

After running application open your browser to the app's URL (e.g. `http://localhost:5000`) — you'll see
a page with a **Get Cat Fact** button and a list of facts that will be empty at the start.
Click the button to fetch a new fact; the list refreshes automatically
(newest fact on top).

You can also call the endpoints directly:

```bash
curl http://localhost:5000/fetch-fact   # fetch + save a new fact
curl http://localhost:5000/facts        # list all saved facts
curl http://localhost:5000/delete-facts # delete the .txt file with saved facts
```

`/fetch-fact` returns JSON like:

```json
{
  "message": "Fact fetched and saved.",
  "savedTo": "/full/path/to/facts.txt",
  "fact": { "fact": "Cats sleep 70% of their lives.", "length": 30 }
}
```

And `facts.txt` will grow with one line per call, e.g.:

```
[2026-09-08 14:02:11] Fact: Cats sleep 70% of their lives. (Length: 30)
[2026-09-08 14:03:47] Fact: The Amur leopard is one of the most endangered animals in the world. (Length: 68)
```

## Project structure

- `Program.cs` — wires up DI and maps the `/fetch-fact` and `/facts` endpoints.
- `FactService.cs` / `IFactService.cs` — service that calls catfact.ninja,
  saves results to `facts.txt`, and reads back the saved list. Registered as
  a **typed HttpClient** (`AddHttpClient<IFactService, FactService>`), so
  `HttpClient` and `ILogger<FactService>` are both injected by the framework
  instead of being created manually.
- `Fact.cs` — the model matching the API's JSON shape.
- `wwwroot/index.html` — the UI: a button to fetch a new fact plus a live
  list of everything saved in `facts.txt`, loaded on page open and refreshed
  after each fetch.

## Notes
- `facts.txt` is created in the app's working directory (usually the project
  folder when running with `dotnet run`).
- `File.AppendAllTextAsync` handles both "create if missing" and "append if
  it exists" automatically, so no extra logic is needed to check whether the
  file already exists.
