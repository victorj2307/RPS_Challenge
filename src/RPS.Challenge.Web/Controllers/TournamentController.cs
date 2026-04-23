using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RPS.Challenge.Web.Mappers;
using RPS.Challenge.Web.Services;
using RPS.Challenge.Core.Entities;
using RPS.Challenge.Core.Scoring;
using RPS.Challenge.Core.Validation;
using RPS.Challenge.Web.ViewModels;

namespace RPS.Challenge.Web.Controllers {
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

            string validationError;
            if (!TournamentPayloadValidator.TryValidateSingleChampionshipJson(fileBytes, out validationError)) {
                viewModel.ResultMessage = validationError;
                viewModel.ResultMessageCssClass = "result-message result-error";
                return View("Index", viewModel);
            }

            string apiError;
            TournamentResult tournamentResult;
            if (!RpsApiClient.TryPlayTournament(fileBytes, out tournamentResult, out apiError)) {
                viewModel.ResultMessage = apiError;
                viewModel.ResultMessageCssClass = "result-message result-error";
                return View("Index", viewModel);
            }

            if (tournamentResult.IsSuccess) {
                viewModel.ResultMessage = "Tournament completed. Results are shown below.";
                viewModel.ResultMessageCssClass = "result-message result-info";

                string scoreError;
                string secondForScore = string.IsNullOrWhiteSpace(tournamentResult.SecondPlaceName) ? null : tournamentResult.SecondPlaceName.Trim();
                bool scoresApplied = RpsApiClient.TryApplyChampionshipScores(tournamentResult.WinnerName, secondForScore, out scoreError);
                if (!scoresApplied) {
                    viewModel.ResultMessage = "Tournament completed, but the scoreboard could not be updated: " + scoreError;
                    viewModel.ResultMessageCssClass = "result-message result-error";
                }

                viewModel.Scoreboard = RpsApiClient.GetTopScores(10);

                viewModel.RoundResults = TournamentResultViewModelMapper.MapRounds(tournamentResult);
                MatchResultViewModel finalMatch = viewModel.RoundResults.SelectMany(round => round.Matches).LastOrDefault();
                if (finalMatch != null) {
                    viewModel.ShowChampionSummary = true;
                    viewModel.ChampionName = finalMatch.WinnerName;
                    viewModel.ChampionStrategy = finalMatch.WinnerStrategy;
                    viewModel.ChampionStrategyIcon = this.GetStrategyIconClass(finalMatch.WinnerStrategy);
                    if (scoresApplied) {
                        viewModel.ChampionPointsGained = ChampionshipPoints.Winner;
                    }
                }

                if (!string.IsNullOrWhiteSpace(tournamentResult.SecondPlaceName)) {
                    viewModel.ShowRunnerUpSummary = true;
                    viewModel.RunnerUpName = tournamentResult.SecondPlaceName.Trim();
                    viewModel.RunnerUpStrategy = string.IsNullOrEmpty(tournamentResult.SecondPlaceStrategy) ? string.Empty : tournamentResult.SecondPlaceStrategy.Trim();
                    viewModel.RunnerUpStrategyIcon = this.GetStrategyIconClass(viewModel.RunnerUpStrategy);
                    if (scoresApplied) {
                        viewModel.RunnerUpPointsGained = ChampionshipPoints.SecondPlace;
                    }
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
            string resetError;
            if (!RpsApiClient.TryResetScoreboard(out resetError)) {
                TempData["ResultMessage"] = resetError;
                TempData["ResultMessageCssClass"] = "result-message result-error";
            }
            else {
                TempData["ResultMessage"] = "Scoreboard has been reset.";
                TempData["ResultMessageCssClass"] = "result-message result-info";
            }

            return RedirectToAction("Index");
        }

        private TournamentPageViewModel CreateBaseViewModel() {
            return new TournamentPageViewModel {
                ResultMessage = string.Empty,
                ResultMessageCssClass = "result-message result-info",
                Scoreboard = RpsApiClient.GetTopScores(10),
                RoundResults = new List<RoundResultViewModel>(),
                ShowChampionSummary = false,
                ChampionName = string.Empty,
                ChampionStrategy = string.Empty,
                ChampionStrategyIcon = string.Empty,
                ShowRunnerUpSummary = false,
                RunnerUpName = string.Empty,
                RunnerUpStrategy = string.Empty,
                RunnerUpStrategyIcon = string.Empty,
                ChampionPointsGained = null,
                RunnerUpPointsGained = null
            };
        }

        private void ResetRoundState(TournamentPageViewModel viewModel) {
            viewModel.RoundResults = new List<RoundResultViewModel>();
            viewModel.ShowChampionSummary = false;
            viewModel.ChampionName = string.Empty;
            viewModel.ChampionStrategy = string.Empty;
            viewModel.ChampionStrategyIcon = string.Empty;
            viewModel.ShowRunnerUpSummary = false;
            viewModel.RunnerUpName = string.Empty;
            viewModel.RunnerUpStrategy = string.Empty;
            viewModel.RunnerUpStrategyIcon = string.Empty;
            viewModel.ChampionPointsGained = null;
            viewModel.RunnerUpPointsGained = null;
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

