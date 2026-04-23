using System.Collections.Generic;

namespace RPS_Challenge.ViewModels {
    public class TournamentPageViewModel {
        public string ResultMessage { get; set; }
        public string ResultMessageCssClass { get; set; }
        public List<ScoreRowViewModel> Scoreboard { get; set; }
        public List<RoundResultViewModel> RoundResults { get; set; }
        public bool ShowChampionSummary { get; set; }
        public string ChampionName { get; set; }
        public string ChampionStrategy { get; set; }
        public string ChampionStrategyIcon { get; set; }
    }
}
