using System.Collections.Generic;

namespace RPS.Challenge.Web.ViewModels {
    public class RoundResultViewModel {
        public int RoundNumber { get; set; }
        public int MatchCount { get; set; }
        public bool IsFinalRound { get; set; }
        public string ChampionName { get; set; }
        public string ChampionStrategy { get; set; }
        public List<MatchResultViewModel> Matches { get; set; }
    }

    public class MatchResultViewModel {
        public string LeftPlayerName { get; set; }
        public string LeftPlayerStrategy { get; set; }
        public string RightPlayerName { get; set; }
        public string RightPlayerStrategy { get; set; }
        public bool LeftPlayerWon { get; set; }
        public bool RightPlayerWon { get; set; }
        public string WinnerName { get; set; }
        public string WinnerStrategy { get; set; }
        public string LoserName { get; set; }
        public string LoserStrategy { get; set; }
        public bool IsBye { get; set; }
    }
}

