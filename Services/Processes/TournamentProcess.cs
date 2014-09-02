using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using RPS_Challenge.Services.Entities;

namespace RPS_Challenge.Services.Processes {
    /// <summary>
    /// Permite procesar un Torneo definido a partir de un archivo de texto.
    /// </summary>
    public static class TournamentProcess {

        /// <summary>
        /// Lista de jugadores del Torneo.
        /// </summary>
        private static List<Player> tournamentPlayers { get; set; }

        #region GetFileData
        /// <summary>
        /// Returns the file content in a list of string that represent the Player username and its game choice.
        /// </summary>
        /// <param name="fileBytes">Contenido del archivo del Torneo.</param>
        /// <returns>Lista de cadena de caracteres con los nombres y estrategias de los jugadores.</returns>
        private static List<string> GetFileData(byte[] fileBytes) {
            string fileContent = System.Text.Encoding.UTF8.GetString(fileBytes);
            fileContent = Regex.Replace(fileContent, @"\]|\[|\""|\s", string.Empty);

            char[] lineSeparator = new char[] { ',' };
            List<string> lines = fileContent.Split(lineSeparator, StringSplitOptions.RemoveEmptyEntries).ToList();

            return lines;
        }
        #endregion GetFileData

        #region ProcessTournament
        /// <summary>
        /// Procesa un Torneo y retorna el ganador.
        /// </summary>
        /// <param name="fileBytes">Contenido del archivo del Torneo.</param>
        /// <param name="winnerName">Nombre del ganador del Torneo.</param>
        public static string ProcessTournament(byte[] fileBytes, out string winnerName) {
            string messageResult = string.Empty;
            winnerName = string.Empty;

            try {
                int index = 0;
                string key = string.Empty;
                string value = string.Empty;

                List<string> fileLines = GetFileData(fileBytes);
                tournamentPlayers = new List<Player>();

                foreach (string line in fileLines) {
                    if ((index % 2) == 0) {
                        key = line;
                    }
                    else {
                        value = line;
                        tournamentPlayers.Add(new Player(key, value));
                    }
                    index++;
                }

                if ((tournamentPlayers.Count % 2) != 0) {
                    throw new Exception("Error: The number of players is different than 2.");
                }

                while (tournamentPlayers.Count > 1) {
                    Round currentRound = null;

                    // Determinar la cantidad de Rounds con base en la cantidad de jugadores
                    int roundQty = tournamentPlayers.Count / 2;
                    List<Round> roundList = new List<Round>();

                    // Asignar los jugadores por cada Round
                    int lastPlayerIndex = 0;
                    if (roundQty == 1) {
                        currentRound = new Round(tournamentPlayers);
                        roundList.Add(currentRound);
                    }
                    else {
                        for (int i = 0; i < roundQty; i++) {
                            currentRound = new Round(tournamentPlayers.GetRange(lastPlayerIndex, 2));
                            lastPlayerIndex = lastPlayerIndex + 2;
                            roundList.Add(currentRound);
                        }
                    }

                    // Determinar los ganadores de cada Round y eliminar los perdedores
                    Player previousPlayer = null;
                    tournamentPlayers = new List<Player>();

                    foreach (Round round in roundList) {
                        previousPlayer = null;

                        foreach (Player currentPlayer in round.Players) {

                            if (!Regex.IsMatch(currentPlayer.Strategy, "(R|P|S)", RegexOptions.IgnoreCase)) {
                                throw new Exception("Error: The player's strategy is something other than \"R\", \"P\" or \"S\".");
                            }

                            if (previousPlayer != null) {
                                if ((previousPlayer.Strategy == "P" && currentPlayer.Strategy == "S") ||
                                    (previousPlayer.Strategy == "S" && currentPlayer.Strategy == "R") ||
                                    (previousPlayer.Strategy == "R" && currentPlayer.Strategy == "P")) {

                                    round.Winner = currentPlayer;
                                }
                                else {
                                    if ((previousPlayer.Strategy == "R" && currentPlayer.Strategy == "S") ||
                                        (previousPlayer.Strategy == "P" && currentPlayer.Strategy == "R") ||
                                        (previousPlayer.Strategy == "S" && currentPlayer.Strategy == "P")) {

                                        round.Winner = previousPlayer;
                                    }
                                    else {
                                        if (previousPlayer.Strategy == currentPlayer.Strategy) {
                                            round.Winner = previousPlayer;
                                        }
                                    }
                                }
                            }
                            else {
                                previousPlayer = currentPlayer;
                            }
                        }

                        tournamentPlayers.Add(round.Winner);
                    }

                }

                // Retornar los datos del ganador del Torneo.
                if (tournamentPlayers.Count == 1) {
                    messageResult = string.Format("Winner: [\"{0}\", \"{1}\"]", tournamentPlayers[0].Name, tournamentPlayers[0].Strategy);
                    winnerName = tournamentPlayers[0].Name;
                }

            }
            catch (Exception e) {
                messageResult = e.Message;
            }

            return messageResult;

        }
        #endregion ProcessTournament

    }

}
