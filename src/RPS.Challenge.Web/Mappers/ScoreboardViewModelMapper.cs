using System.Collections.Generic;
using System.Linq;

using RPS.Challenge.Web.ViewModels;

namespace RPS.Challenge.Web.Mappers {
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

