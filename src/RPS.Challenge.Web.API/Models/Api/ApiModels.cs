using System.Collections.Generic;

namespace RPS.Challenge.Web.API.Models.Api {
    public class PlayTournamentRequest {
        public object Tournament { get; set; }
    }

    public class PlayerCountResponse {
        public int PlayerCount { get; set; }
    }

    public class PlayTournamentResponse {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string WinnerName { get; set; }
        public string SecondPlaceName { get; set; }
        public string SecondPlaceStrategy { get; set; }
        public List<ApiRoundResult> Rounds { get; set; }
    }

    public class ApiRoundResult {
        public int RoundNumber { get; set; }
        public List<ApiMatchResult> Matches { get; set; }
    }

    public class ApiMatchResult {
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

    public class ChampionshipScoreRequest {
        public string WinnerName { get; set; }
        public string SecondPlaceName { get; set; }
    }

    public class ScoreboardResponse {
        public List<ScoreRowResponse> Scores { get; set; }
    }

    public class ScoreRowResponse {
        public int Rank { get; set; }
        public string PlayerName { get; set; }
        public int Points { get; set; }
    }

    public class ApiErrorResponse {
        public ApiError Error { get; set; }
    }

    public class ApiError {
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
