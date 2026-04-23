using RPS.Challenge.Core.Entities;

namespace RPS.Challenge.Core.Processes {
    /// <summary>
    /// Runner-up = loser of the last non-bye match in the final round (standard single-elimination finalist).
    /// </summary>
    public static class TournamentRunnerUpResolver {
        public static bool TryGetRunnerUp(TournamentResult result, out string name, out string strategy) {
            name = string.Empty;
            strategy = string.Empty;

            if (result == null || !result.IsSuccess || result.Rounds == null || result.Rounds.Count == 0) {
                return false;
            }

            RoundResult finalRound = result.Rounds[result.Rounds.Count - 1];
            if (finalRound.Matches == null || finalRound.Matches.Count == 0) {
                return false;
            }

            for (int i = finalRound.Matches.Count - 1; i >= 0; i--) {
                MatchResult match = finalRound.Matches[i];
                if (match.IsBye) {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(match.LoserName)) {
                    continue;
                }

                name = match.LoserName.Trim();
                strategy = string.IsNullOrEmpty(match.LoserStrategy) ? string.Empty : match.LoserStrategy.Trim();
                return true;
            }

            return false;
        }
    }
}
