# RPS Challenge

Rock-Paper-Scissors tournament web app built on .NET Framework 4.8.

This repository started as a WebForms technical test and was modernized to an ASP.NET MVC 5 application with a cleaner, layered structure.

## Solution Structure

- `src/RPS.Challenge.Web`: MVC web application (controllers, views, UI assets)
- `src/RPS.Challenge.Web.API`: standalone REST API project
- `src/RPS.Challenge.Core`: Tournament domain logic (entities and processes)
- `tests/RPS.Challenge.Core.Tests`: Unit tests for Core validation and runner-up logic
- `RPS_Challenge.sln`: Solution entry point

## Technology Stack

- ASP.NET MVC 5
- ASP.NET Web API 2
- .NET Framework 4.8
- C# + LINQ
- Entity Framework 5
- SQL Server LocalDB
- Razor views
- jQuery 3.7.1 / jQuery UI 1.13.2
- Custom CSS UI

## Main Routes

- `/Tournament` -> main tournament flow
- `/Examples` -> sample tournament files
- `/About` -> project and author information

## REST API (v1)

Base path: `/api/v1`

- `POST /api/v1/tournament/play` -> process a tournament payload and return rounds, winner, and runner-up (`secondPlaceName` / `secondPlaceStrategy` when applicable). Tournament JSON must be **exactly one** root array (one bracket per file); extra JSON after the array is rejected.
- `POST /api/v1/tournament/player-count` -> return leaf player count for a tournament payload (used by the Examples page); same one-bracket JSON rules as `play`.
- `GET /api/v1/scoreboard?top=10` -> fetch ranked scoreboard entries
- `POST /api/v1/scoreboard/championship` -> apply **winner +3** and optional **runner-up +1** in a single database transaction. Body: `{ "winnerName": "...", "secondPlaceName": "..." }` (`secondPlaceName` optional). Returns **400** if `secondPlaceName` equals `winnerName` (case-insensitive).
- `DELETE /api/v1/scoreboard` -> reset all scoreboard data

**Breaking change:** `POST /api/v1/scoreboard/winner` was removed; use `championship` instead.

### Example Requests

```http
POST /api/v1/tournament/play
Content-Type: application/json

{
  "tournament": [
    [["Armando", "P"], ["Dave", "S"]],
    [["Richard", "R"], ["Michael", "S"]]
  ]
}
```

```http
POST /api/v1/scoreboard/championship
Content-Type: application/json

{
  "winnerName": "Dave",
  "secondPlaceName": "Armando"
}
```

## Local Setup

1. Open `RPS_Challenge.sln` in Visual Studio.
2. Ensure .NET Framework 4.8 developer tools are installed.
3. Restore NuGet packages:
   - Visual Studio restore, or
   - `MSBuild /t:Restore /p:RestorePackagesConfig=true`
4. Run `src/RPS.Challenge.Web` with IIS Express for the UI.
5. Run `src/RPS.Challenge.Web.API` with IIS Express for the API.
6. Set `RpsApiBaseUrl` in `src/RPS.Challenge.Web/Web.config` to match the API base URL (scheme + host + port, no trailing slash), for example `http://localhost:5410`. The MVC site calls the REST API for tournaments and the scoreboard.

## Build

From repository root:

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" .\RPS_Challenge.sln /t:Restore,Build /p:RestorePackagesConfig=true /p:Configuration=Debug
```

## Notes

- Tournament uploads accept `.txt` and `.json` files with the expected JSON bracket structure. Files must contain **one** JSON array at the root (no concatenated documents).
- The MVC UI validates uploads before calling the API; the API applies the same rules for direct clients.
- After a successful tournament, the UI calls `championship` once: **+3** for the winner and **+1** for second place when a runner-up exists. Tournament **play** and scoreboard updates are separate HTTP calls: the UI can show a completed bracket even if the scoreboard request fails. Submitting **Play** multiple times can **award points multiple times** (no idempotency key).
- The MVC UI uses the REST API for tournament play, example file previews (player count), and scoreboard read/write; the API persists scores to LocalDB (`RPSChallengeScores`). Static file downloads on the Examples page still use `ExampleFilesDownload.ashx` (no tournament logic).
- This project is intended as a modernized technical challenge/portfolio piece, not a production deployment.
