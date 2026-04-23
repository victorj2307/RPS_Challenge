using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

using RPS_Challenge.Core.Entities;

namespace RPS_Challenge.Core.Processes {
    /// <summary>
    /// Permite procesar un Torneo definido a partir de un archivo JSON.
    /// </summary>
    public static class TournamentProcess {
        private static readonly HashSet<string> ValidStrategies = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "R", "P", "S" };
        private sealed class MatchOutcome {
            public Player Winner { get; set; }
            public MatchResult Details { get; set; }
        }

        #region ProcessTournament
        /// <summary>
        /// Procesa un Torneo y retorna el ganador.
        /// </summary>
        /// <param name="fileBytes">Contenido del archivo del Torneo.</param>
        /// <param name="winnerName">Nombre del ganador del Torneo.</param>
        public static string ProcessTournament(byte[] fileBytes, out string winnerName) {
            TournamentResult result = ProcessTournamentDetailed(fileBytes);
            winnerName = result.WinnerName;
            return result.Message;
        }
        #endregion ProcessTournament

        public static TournamentResult ProcessTournamentDetailed(byte[] fileBytes) {
            TournamentResult tournamentResult = new TournamentResult {
                Message = string.Empty,
                WinnerName = string.Empty,
                Rounds = new List<RoundResult>(),
                IsSuccess = false
            };

            try {
                List<Player> tournamentPlayers = GetPlayersFromJson(fileBytes);
                if (tournamentPlayers.Count < 2) {
                    throw new Exception("Error: The tournament must contain at least two players.");
                }

                int roundNumber = 1;
                while (tournamentPlayers.Count > 1) {
                    RoundResult currentRound = new RoundResult {
                        RoundNumber = roundNumber,
                        Matches = new List<MatchResult>()
                    };

                    List<Player> nextRoundPlayers = new List<Player>();
                    for (int i = 0; i + 1 < tournamentPlayers.Count; i = i + 2) {
                        MatchOutcome match = ResolveMatch(tournamentPlayers[i], tournamentPlayers[i + 1]);
                        nextRoundPlayers.Add(match.Winner);
                        currentRound.Matches.Add(match.Details);
                    }

                    // If there is an odd player count in this round, carry the last player forward.
                    if ((tournamentPlayers.Count % 2) != 0) {
                        Player byePlayer = tournamentPlayers[tournamentPlayers.Count - 1];
                        currentRound.Matches.Add(new MatchResult {
                            FirstPlayerName = byePlayer.Name,
                            FirstPlayerStrategy = GetStrategyDisplayName(byePlayer.Strategy),
                            SecondPlayerName = string.Empty,
                            SecondPlayerStrategy = string.Empty,
                            WinnerIsFirstPlayer = true,
                            WinnerName = byePlayer.Name,
                            WinnerStrategy = GetStrategyDisplayName(byePlayer.Strategy),
                            LoserName = string.Empty,
                            LoserStrategy = string.Empty,
                            IsBye = true
                        });
                        nextRoundPlayers.Add(byePlayer);
                    }

                    tournamentResult.Rounds.Add(currentRound);
                    tournamentPlayers = nextRoundPlayers;
                    roundNumber++;
                }

                if (tournamentPlayers.Count == 1) {
                    tournamentResult.Message = string.Format("Winner: {0}, {1}", tournamentPlayers[0].Name, GetStrategyDisplayName(tournamentPlayers[0].Strategy));
                    tournamentResult.WinnerName = tournamentPlayers[0].Name;
                    tournamentResult.IsSuccess = true;
                }
            }
            catch (Exception e) {
                tournamentResult.Message = e.Message;
            }

            return tournamentResult;
        }

        private static List<Player> GetPlayersFromJson(byte[] fileBytes) {
            string jsonContent = Encoding.UTF8.GetString(fileBytes);
            object deserializedContent = null;

            try {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                deserializedContent = serializer.DeserializeObject(jsonContent);
            }
            catch (ArgumentException) {
                throw new Exception("Error: File content is not valid JSON.");
            }
            catch (InvalidOperationException) {
                throw new Exception("Error: File content is not valid JSON.");
            }

            List<Player> players = new List<Player>();
            CollectPlayers(deserializedContent, players);
            return players;
        }

        private static void CollectPlayers(object node, List<Player> players) {
            object[] nodeArray = node as object[];
            if (nodeArray == null) {
                throw new Exception("Error: Invalid tournament format.");
            }

            if (nodeArray.Length == 2 && nodeArray[0] is string && nodeArray[1] is string) {
                if (string.IsNullOrWhiteSpace((string)nodeArray[0]) || string.IsNullOrWhiteSpace((string)nodeArray[1])) {
                    throw new Exception("Error: Invalid player format. Name and strategy are required.");
                }
                players.Add(new Player((string)nodeArray[0], (string)nodeArray[1]));
                return;
            }

            foreach (object childNode in nodeArray) {
                if (!(childNode is object[])) {
                    throw new Exception("Error: Invalid tournament format. Nested items must be arrays.");
                }

                CollectPlayers(childNode, players);
            }
        }

        private static MatchOutcome ResolveMatch(Player firstPlayer, Player secondPlayer) {
            ValidateStrategy(firstPlayer.Strategy);
            ValidateStrategy(secondPlayer.Strategy);

            Player winner = firstPlayer;
            Player loser = secondPlayer;
            if (firstPlayer.Strategy == secondPlayer.Strategy) {
                winner = firstPlayer;
                loser = secondPlayer;
            }
            else if ((firstPlayer.Strategy == "R" && secondPlayer.Strategy == "S") ||
                     (firstPlayer.Strategy == "S" && secondPlayer.Strategy == "P") ||
                     (firstPlayer.Strategy == "P" && secondPlayer.Strategy == "R")) {
                winner = firstPlayer;
                loser = secondPlayer;
            }
            else {
                winner = secondPlayer;
                loser = firstPlayer;
            }

            return new MatchOutcome {
                Winner = winner,
                Details = new MatchResult {
                    FirstPlayerName = firstPlayer.Name,
                    FirstPlayerStrategy = GetStrategyDisplayName(firstPlayer.Strategy),
                    SecondPlayerName = secondPlayer.Name,
                    SecondPlayerStrategy = GetStrategyDisplayName(secondPlayer.Strategy),
                    WinnerIsFirstPlayer = ReferenceEquals(winner, firstPlayer),
                    WinnerName = winner.Name,
                    WinnerStrategy = GetStrategyDisplayName(winner.Strategy),
                    LoserName = loser.Name,
                    LoserStrategy = GetStrategyDisplayName(loser.Strategy),
                    IsBye = false
                }
            };
        }

        private static void ValidateStrategy(string strategy) {
            if (!ValidStrategies.Contains(strategy)) {
                throw new Exception("Error: The player's strategy is something other than \"R\", \"P\" or \"S\".");
            }
        }

        private static string GetStrategyDisplayName(string strategy) {
            switch (strategy) {
                case "R":
                    return "Rock";
                case "P":
                    return "Paper";
                case "S":
                    return "Scissors";
                default:
                    return strategy;
            }
        }

    }

}
