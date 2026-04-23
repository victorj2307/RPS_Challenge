using System.Collections.Generic;

namespace RPS.Challenge.Web.ViewModels {
    public class TournamentPageViewModel {
        public string ResultMessage { get; set; }
        public string ResultMessageCssClass { get; set; }
        public List<ScoreRowViewModel> Scoreboard { get; set; }
        public List<RoundResultViewModel> RoundResults { get; set; }
        public bool ShowChampionSummary { get; set; }
        public string ChampionName { get; set; }
        public string ChampionStrategy { get; set; }
        public string ChampionStrategyIcon { get; set; }
        /// <summary>Points added to the champion on the scoreboard for this tournament, when scoring succeeded.</summary>
        public int? ChampionPointsGained { get; set; }
        public bool ShowRunnerUpSummary { get; set; }
        public string RunnerUpName { get; set; }
        public string RunnerUpStrategy { get; set; }
        public string RunnerUpStrategyIcon { get; set; }
        /// <summary>Points added to second place on the scoreboard for this tournament, when scoring succeeded.</summary>
        public int? RunnerUpPointsGained { get; set; }
    }
}

