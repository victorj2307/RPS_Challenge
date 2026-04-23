# RPS Challenge

Rock-Paper-Scissors tournament web app built on .NET Framework 4.8.

This repository started as a WebForms technical test and was modernized to an ASP.NET MVC 5 application with a cleaner, layered structure.

## Solution Structure

- `src/RPS.Challenge.Web`: MVC web application (controllers, views, UI assets)
- `src/RPS.Challenge.Core`: Tournament domain logic (entities and processes)
- `RPS_Challenge.sln`: Solution entry point

## Technology Stack

- ASP.NET MVC 5
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

## Local Setup

1. Open `RPS_Challenge.sln` in Visual Studio.
2. Ensure .NET Framework 4.8 developer tools are installed.
3. Restore NuGet packages:
   - Visual Studio restore, or
   - `MSBuild /t:Restore /p:RestorePackagesConfig=true`
4. Run `src/RPS.Challenge.Web` with IIS Express.

## Build

From repository root:

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" .\RPS_Challenge.sln /t:Restore,Build /p:RestorePackagesConfig=true /p:Configuration=Debug
```

## Notes

- Tournament uploads accept `.txt` and `.json` files with the expected JSON bracket structure.
- Scoreboard data is stored in LocalDB using the `RPSChallengeScores` catalog.
- This project is intended as a modernized technical challenge/portfolio piece, not a production deployment.

