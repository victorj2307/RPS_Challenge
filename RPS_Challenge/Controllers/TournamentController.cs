using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RPS_Challenge.Mappers;
using RPS_Challenge.Services;
using RPS_Challenge.Services.Entities;
using RPS_Challenge.Services.Processes;
using RPS_Challenge.ViewModels;

namespace RPS_Challenge.Controllers {
    public class TournamentController : Controller {
        public ActionResult Index() {
            TournamentPageViewModel viewModel = this.CreateBaseViewModel();

            if (TempData["ResultMessage"] != null) {
                viewModel.ResultMessage = Convert.ToString(TempData["ResultMessage"]);
                viewModel.ResultMessageCssClass = Convert.ToString(TempData["ResultMessageCssClass"]);
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Play(HttpPostedFileBase tournamentFile) {
            TournamentPageViewModel viewModel = this.CreateBaseViewModel();
            this.ResetRoundState(viewModel);

            if (tournamentFile == null || tournamentFile.ContentLength <= 0) {
                viewModel.ResultMessage = "Error: Please select a tournament file.";
                viewModel.ResultMessageCssClass = "result-message result-error";
                return View("Index", viewModel);
            }

            string extension = Path.GetExtension(tournamentFile.FileName);
            if (!string.Equals(extension, ".txt", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(extension, ".json", StringComparison.OrdinalIgnoreCase)) {
                viewModel.ResultMessage = "Error: Please select a valid .txt or .json tournament file.";
                viewModel.ResultMessageCssClass = "result-message result-error";
                return View("Index", viewModel);
            }

            byte[] fileBytes;
            using (MemoryStream stream = new MemoryStream()) {
                tournamentFile.InputStream.CopyTo(stream);
                fileBytes = stream.ToArray();
            }

            TournamentResult tournamentResult = TournamentProcess.ProcessTournamentDetailed(fileBytes);
            if (tournamentResult.IsSuccess) {
                viewModel.ResultMessage = "Tournament completed. Champion is shown below.";
                viewModel.ResultMessageCssClass = "result-message result-info";

                ScoreboardService.InsertWinnerScore(tournamentResult.WinnerName);
                viewModel.Scoreboard = ScoreboardService.GetTopScores(10);

                viewModel.RoundResults = TournamentResultViewModelMapper.MapRounds(tournamentResult);
                MatchResultViewModel finalMatch = viewModel.RoundResults.SelectMany(round => round.Matches).LastOrDefault();
                if (finalMatch != null) {
                    viewModel.ShowChampionSummary = true;
                    viewModel.ChampionName = finalMatch.WinnerName;
                    viewModel.ChampionStrategy = finalMatch.WinnerStrategy;
                    viewModel.ChampionStrategyIcon = this.GetStrategyIconClass(finalMatch.WinnerStrategy);
                }
            }
            else {
                viewModel.ResultMessage = tournamentResult.Message;
                viewModel.ResultMessageCssClass = "result-message result-error";
            }

            return View("Index", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetScoreboard() {
            ScoreboardService.ResetScoreboard();
            TempData["ResultMessage"] = "Scoreboard has been reset.";
            TempData["ResultMessageCssClass"] = "result-message result-info";
            return RedirectToAction("Index");
        }

        private TournamentPageViewModel CreateBaseViewModel() {
            return new TournamentPageViewModel {
                ResultMessage = string.Empty,
                ResultMessageCssClass = "result-message result-info",
                Scoreboard = ScoreboardService.GetTopScores(10),
                RoundResults = new List<RoundResultViewModel>(),
                ShowChampionSummary = false,
                ChampionName = string.Empty,
                ChampionStrategy = string.Empty,
                ChampionStrategyIcon = string.Empty
            };
        }

        private void ResetRoundState(TournamentPageViewModel viewModel) {
            viewModel.RoundResults = new List<RoundResultViewModel>();
            viewModel.ShowChampionSummary = false;
            viewModel.ChampionName = string.Empty;
            viewModel.ChampionStrategy = string.Empty;
            viewModel.ChampionStrategyIcon = string.Empty;
        }

        private string GetStrategyIconClass(string strategy) {
            if (string.IsNullOrWhiteSpace(strategy)) {
                return "fa-solid fa-gamepad";
            }

            switch (strategy.Trim().ToLowerInvariant()) {
                case "rock":
                case "r":
                    return "fa-solid fa-hand-back-fist";
                case "paper":
                case "p":
                    return "fa-solid fa-hand";
                case "scissors":
                case "s":
                    return "fa-solid fa-hand-scissors";
                default:
                    return "fa-solid fa-gamepad";
            }
        }
    }
}
