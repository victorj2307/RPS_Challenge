# RPS Challenge

Rock-Paper-Scissors tournament web app built on .NET Framework 4.8.

This project started as a WebForms technical challenge and has been incrementally migrated to ASP.NET MVC 5 while preserving the original service/domain logic.

## Current Architecture

- UI: ASP.NET MVC 5 (`Tournament`, `Examples`, `About`)
- Legacy UI: WebForms pages still present for compatibility/redirect cutover
- Business logic: `Services` project (`TournamentProcess`, entities)
- Data: Entity Framework 5 with LocalDB (`Scores`)

## Main Routes

- `/Tournament` -> main tournament flow
- `/Examples` -> tournament example files
- `/About` -> project and author information

## Local Setup

1. Open `RPS_Challenge.sln` in Visual Studio.
2. Ensure .NET Framework 4.8 developer tools are installed.
3. Restore NuGet packages (if Visual Studio does not auto-restore).
4. Run the `RPS_Challenge` web project with IIS Express.

## Build

From repository root:

```powershell
"C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" .\RPS_Challenge.sln /t:Build /p:Configuration=Debug
```

## Notes

- Tournament uploads accept `.txt` and `.json` files with the expected JSON bracket structure.
- Scoreboard data uses LocalDB and is not intended for production deployment.
- Repository ignore rules exclude local build outputs and local database files.
