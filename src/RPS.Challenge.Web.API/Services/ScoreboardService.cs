using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

using RPS.Challenge.Core.Scoring;
using RPS.Challenge.Web.API.Models.Api;

namespace RPS.Challenge.Web.API.Services {
    public static class ScoreboardService {

        private static string GetConnectionString() {
            ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings["ScoreboardSqlConnection"];
            return connectionString != null ? connectionString.ConnectionString : string.Empty;
        }

        public static List<ScoreRowResponse> GetTopScores(int maxRows) {
            List<ScoreRowResponse> scores = new List<ScoreRowResponse>();
            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = connection.CreateCommand()) {
                command.CommandText = @"
                    SELECT TOP (@MaxRows) PlayerName, Points
                    FROM dbo.Score
                    ORDER BY Points DESC, PlayerName ASC";
                command.Parameters.AddWithValue("@MaxRows", maxRows);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader()) {
                    int rank = 1;
                    while (reader.Read()) {
                        scores.Add(new ScoreRowResponse {
                            Rank = rank,
                            PlayerName = reader.GetString(0),
                            Points = reader.GetInt32(1)
                        });
                        rank++;
                    }
                }
            }

            return scores;
        }

        /// <summary>
        /// Awards winner +3 and optional runner-up +1 in a single database transaction (winner first, then second).
        /// </summary>
        public static void ApplyChampionshipScores(string winnerName, string secondPlaceNameOrNull) {
            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction()) {
                    try {
                        AddPointsToPlayer(connection, transaction, winnerName, ChampionshipPoints.Winner);
                        if (!string.IsNullOrWhiteSpace(secondPlaceNameOrNull)) {
                            AddPointsToPlayer(connection, transaction, secondPlaceNameOrNull.Trim(), ChampionshipPoints.SecondPlace);
                        }

                        transaction.Commit();
                    }
                    catch {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private static void AddPointsToPlayer(SqlConnection connection, SqlTransaction transaction, string playerName, int delta) {
            using (SqlCommand command = connection.CreateCommand()) {
                command.Transaction = transaction;
                command.CommandText = @"
                    IF EXISTS (SELECT 1 FROM dbo.Score WHERE PlayerName = @PlayerName)
                        UPDATE dbo.Score SET Points = Points + @Delta WHERE PlayerName = @PlayerName;
                    ELSE
                        INSERT INTO dbo.Score (PlayerName, Points) VALUES (@PlayerName, @Delta);";
                command.Parameters.AddWithValue("@PlayerName", playerName);
                command.Parameters.AddWithValue("@Delta", delta);
                command.ExecuteNonQuery();
            }
        }

        public static void ResetScoreboard() {
            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = connection.CreateCommand()) {
                command.CommandText = "DELETE FROM dbo.Score;";
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
