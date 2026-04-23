using System.Collections.Generic;

namespace RPS_Challenge.Core.Entities {
    public class TournamentResult {
        public string Message { get; set; }
        public string WinnerName { get; set; }
        public List<RoundResult> Rounds { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class RoundResult {
        public int RoundNumber { get; set; }
        public List<MatchResult> Matches { get; set; }
    }

    public class MatchResult {
        public string FirstPlayerName { get; set; }
        public string FirstPlayerStrategy { get; set; }
        public string SecondPlayerName { get; set; }
        public string SecondPlayerStrategy { get; set; }
        public bool WinnerIsFirstPlayer { get; set; }
        public string WinnerName { get; set; }
        public string WinnerStrategy { get; set; }
        public string LoserName { get; set; }
        public string LoserStrategy { get; set; }
        public bool IsBye { get; set; }
    }
}
