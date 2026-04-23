using System.Collections.Generic;
using System.Linq;

using RPS_Challenge.Core.Entities;
using RPS_Challenge.ViewModels;

namespace RPS_Challenge.Mappers {
    public static class TournamentResultViewModelMapper {
        public static List<RoundResultViewModel> MapRounds(TournamentResult tournamentResult) {
            if (tournamentResult == null || !tournamentResult.IsSuccess || tournamentResult.Rounds == null || tournamentResult.Rounds.Count == 0) {
                return new List<RoundResultViewModel>();
            }

            List<RoundResultViewModel> roundResults = tournamentResult.Rounds.Select(round => {
                List<MatchResultViewModel> mappedMatches = round.Matches.Select(match => {
                    bool isByeMatch = match.IsBye || string.IsNullOrWhiteSpace(match.LoserName);
                    bool leftWon = match.WinnerIsFirstPlayer;

                    return new MatchResultViewModel {
                        LeftPlayerName = match.FirstPlayerName,
                        LeftPlayerStrategy = match.FirstPlayerStrategy,
                        RightPlayerName = match.SecondPlayerName,
                        RightPlayerStrategy = match.SecondPlayerStrategy,
                        LeftPlayerWon = leftWon,
                        RightPlayerWon = !leftWon && !isByeMatch,
                        WinnerName = match.WinnerName,
                        WinnerStrategy = match.WinnerStrategy,
                        LoserName = match.LoserName,
                        LoserStrategy = match.LoserStrategy,
                        IsBye = isByeMatch
                    };
                }).ToList();

                return new RoundResultViewModel {
                    RoundNumber = round.RoundNumber,
                    MatchCount = mappedMatches.Count,
                    IsFinalRound = false,
                    ChampionName = string.Empty,
                    ChampionStrategy = string.Empty,
                    Matches = mappedMatches
                };
            }).ToList();

            if (roundResults.Count > 0) {
                RoundResultViewModel finalRound = roundResults[roundResults.Count - 1];
                MatchResultViewModel finalMatch = finalRound.Matches.LastOrDefault();
                if (finalMatch != null) {
                    finalRound.IsFinalRound = true;
                    finalRound.ChampionName = finalMatch.WinnerName;
                    finalRound.ChampionStrategy = finalMatch.WinnerStrategy;
                }
            }

            return roundResults;
        }
    }
}
