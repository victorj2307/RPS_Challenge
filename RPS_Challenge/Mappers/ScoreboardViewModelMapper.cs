using System.Collections.Generic;
using System.Linq;

using RPS_Challenge.ViewModels;

namespace RPS_Challenge.Mappers {
    public static class ScoreboardViewModelMapper {
        public static List<ScoreRowViewModel> MapToRankedRows(IEnumerable<Score> scores) {
            return scores.Select((score, index) => new ScoreRowViewModel {
                Rank = index + 1,
                PlayerName = score.PlayerName,
                Points = score.Points
            }).ToList();
        }
    }
}
