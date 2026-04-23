using System.Collections.Generic;
using System.Linq;

using RPS.Challenge.Web.Mappers;
using RPS.Challenge.Web.ViewModels;

namespace RPS.Challenge.Web.Services {
    public static class ScoreboardService {
        public static List<ScoreRowViewModel> GetTopScores(int maxRows) {
            using (var scoresEntities = new ScoresEntities()) {
                var query = (from s in scoresEntities.Scores
                             orderby s.Points descending, s.PlayerName ascending
                             select s).Take(maxRows).ToList();
                return ScoreboardViewModelMapper.MapToRankedRows(query);
            }
        }

        public static void InsertWinnerScore(string winnerName) {
            if (string.IsNullOrEmpty(winnerName)) {
                return;
            }

            using (var scoresEntities = new ScoresEntities()) {
                var playerScore = (from s in scoresEntities.Scores
                                   where s.PlayerName == winnerName
                                   select s).SingleOrDefault();

                if (playerScore == null) {
                    playerScore = new Score {
                        PlayerName = winnerName,
                        Points = 3
                    };
                    scoresEntities.Scores.AddObject(playerScore);
                }
                else {
                    playerScore.Points = playerScore.Points + 3;
                }

                scoresEntities.SaveChanges();
            }
        }

        public static void ResetScoreboard() {
            using (var scoresEntities = new ScoresEntities()) {
                var deleteAllScores = from s in scoresEntities.Scores
                                      select s;
                foreach (var score in deleteAllScores) {
                    scoresEntities.Scores.DeleteObject(score);
                }

                scoresEntities.SaveChanges();
            }
        }
    }
}

