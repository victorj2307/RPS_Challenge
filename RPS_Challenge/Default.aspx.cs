using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using RPS_Challenge.Mappers;
using RPS_Challenge.Services.Entities;
using RPS_Challenge.Services.Processes;
using RPS_Challenge.ViewModels;

namespace RPS_Challenge {
    public partial class _Default : Page {
        private const string ResultErrorPrefix = "Error:";
        private const string ResultWinnerPrefix = "Winner:";

        protected void Page_Load(object sender, EventArgs e) {
            if (this.Form != null) {
                this.Form.Attributes["onsubmit"] = "return markFormSubmitting();";
            }

            if (!this.IsPostBack) {
                this.GetScoreboard();
            }
        }

        protected void btnPlay_Click(object sender, EventArgs e) {
            string winnerName = string.Empty;

            if (!Page.IsValid) {
                this.SetResultMessage("Error: Please select a valid .txt or .json tournament file.");
                this.ClearRoundResults();
                return;
            }

            if (this.btnTournamentFileUpload.HasFile) {
                TournamentResult tournamentResult = TournamentProcess.ProcessTournamentDetailed(this.btnTournamentFileUpload.FileBytes);
                winnerName = tournamentResult.WinnerName;
                if (tournamentResult.IsSuccess) {
                    this.SetResultMessage("Tournament completed. Champion is shown at the end of the final round.");
                }
                else {
                    this.SetResultMessage(tournamentResult.Message);
                }

                // Insertar el ganador en la base de datos de puntajes
                this.InsertWinnerScore(winnerName);

                this.GetScoreboard();
                this.BindRoundResults(tournamentResult);
            }
            else {
                this.SetResultMessage("Error: Please select a tournament file.");
                this.ClearRoundResults();
            }
        }

        protected void btnReset_Click(object sender, EventArgs e) {
            this.DeleteScoreboard();
            this.GetScoreboard();
        }

        #region InitializeScoreGrid
        /// <summary>
        /// Inicializa el grid que muestra los puntajes.
        /// </summary>
        private void InitializeScoreGrid() {
            this.scoreGrid.DataSource = new List<ScoreRowViewModel>();
            this.scoreGrid.DataBind();
        }
        #endregion InitializeScoreGrid

        #region DeleteScoreboard
        /// <summary>
        /// Elimina todos los puntajes de la base de datos.
        /// </summary>
        private void DeleteScoreboard() {
            using (var scoresEntities = new ScoresEntities()) {
                var deleteAllScores = from s in scoresEntities.Scores
                                      select s;
                foreach (var score in deleteAllScores) {
                    scoresEntities.Scores.DeleteObject(score);
                }

                scoresEntities.SaveChanges();
            }
        }
        #endregion DeleteScoreboard

        #region GetScoreboard
        /// <summary>
        /// Consulta y muestra los puntajes almacenados en la base de datos.
        /// </summary>
        private void GetScoreboard() {
            using (var scoresEntities = new ScoresEntities()) {
                var query = (from s in scoresEntities.Scores
                             orderby s.Points descending, s.PlayerName ascending
                             select s).Take(10).ToList();
                var rankedScores = ScoreboardViewModelMapper.MapToRankedRows(query);

                this.scoreGrid.DataSource = rankedScores;
                this.scoreGrid.DataBind();
            }
        }
        #endregion GetScoreboard

        #region InsertWinnerScore
        /// <summary>
        /// Inserta o actualiza el puntaje del jugador ganador en la base de datos.
        /// </summary>
        /// <param name="winnnerName"></param>
        private void InsertWinnerScore(string winnerName) {
            if (!string.IsNullOrEmpty(winnerName)) {
                using (var scoresEntities = new ScoresEntities()) {
                    var playerScore = (from s in scoresEntities.Scores
                                       where s.PlayerName == winnerName
                                       select s).SingleOrDefault();

                    if (playerScore == null) {
                        playerScore = new Score();
                        playerScore.PlayerName = winnerName;
                        playerScore.Points = 3;
                        scoresEntities.Scores.AddObject(playerScore);
                    }
                    else {
                        playerScore.Points = playerScore.Points + 3;
                    }
                    scoresEntities.SaveChanges();

                }
            }
        }
        #endregion InsertWinnerScore

        private void BindRoundResults(TournamentResult tournamentResult) {
            if (tournamentResult == null || !tournamentResult.IsSuccess || tournamentResult.Rounds == null || tournamentResult.Rounds.Count == 0) {
                this.ClearRoundResults();
                return;
            }

            var roundResults = TournamentResultViewModelMapper.MapRounds(tournamentResult);

            this.roundResultsSection.Visible = true;
            this.pnlRoundResults.Visible = true;
            this.roundResultsRepeater.DataSource = roundResults;
            this.roundResultsRepeater.DataBind();

            MatchResultViewModel finalMatch = roundResults.SelectMany(round => round.Matches).LastOrDefault();
            if (finalMatch != null) {
                this.pnlChampionSummary.Visible = true;
                this.lblChampionName.Text = finalMatch.WinnerName;
                this.lblChampionStrategy.Text = finalMatch.WinnerStrategy;
                this.lblChampionStrategyIcon.Text = this.GetStrategyIcon(finalMatch.WinnerStrategy);
            }
        }

        private void ClearRoundResults() {
            this.roundResultsSection.Visible = false;
            this.pnlRoundResults.Visible = false;
            this.roundResultsRepeater.DataSource = null;
            this.roundResultsRepeater.DataBind();
            this.pnlChampionSummary.Visible = false;
            this.lblChampionName.Text = string.Empty;
            this.lblChampionStrategy.Text = string.Empty;
            this.lblChampionStrategyIcon.Text = string.Empty;
        }

        protected void roundResultsRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e) {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) {
                return;
            }

            RoundResultViewModel round = e.Item.DataItem as RoundResultViewModel;
            Repeater matchRepeater = e.Item.FindControl("matchResultsRepeater") as Repeater;
            if (round == null || matchRepeater == null) {
                return;
            }

            matchRepeater.DataSource = round.Matches;
            matchRepeater.DataBind();
        }

        protected void scoreGrid_RowDataBound(object sender, GridViewRowEventArgs e) {
            if (e.Row.RowType != DataControlRowType.DataRow) {
                return;
            }

            ScoreRowViewModel rowData = e.Row.DataItem as ScoreRowViewModel;
            if (rowData == null) {
                return;
            }

            if (rowData.Rank == 1) {
                e.Row.CssClass = "score-row score-row-first";
            }
            else if (rowData.Rank == 2) {
                e.Row.CssClass = "score-row score-row-second";
            }
            else if (rowData.Rank == 3) {
                e.Row.CssClass = "score-row score-row-third";
            }
            else {
                e.Row.CssClass = "score-row";
            }
        }

        private void SetResultMessage(string message) {
            this.pnlResult.Visible = true;
            this.lblResult.Text = message;

            if (!string.IsNullOrEmpty(message) && message.StartsWith(ResultWinnerPrefix, StringComparison.OrdinalIgnoreCase)) {
                this.lblResult.CssClass = "result-message result-info";
                return;
            }

            if (!string.IsNullOrEmpty(message) && message.StartsWith(ResultErrorPrefix, StringComparison.OrdinalIgnoreCase)) {
                this.lblResult.CssClass = "result-message result-error";
                return;
            }

            this.lblResult.CssClass = "result-message result-info";
        }

        protected string GetStrategyIcon(object strategyValue) {
            string strategy = Convert.ToString(strategyValue);
            if (string.IsNullOrWhiteSpace(strategy)) {
                return "🎮";
            }

            switch (strategy.Trim().ToLowerInvariant()) {
                case "rock":
                case "r":
                    return "🪨";
                case "paper":
                case "p":
                    return "📄";
                case "scissors":
                case "s":
                    return "✂️";
                default:
                    return "🎮";
            }
        }
    }
}