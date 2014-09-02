using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RPS_Challenge.Services.Processes;

namespace RPS_Challenge {
    public partial class _Default : Page {
        protected void Page_Load(object sender, EventArgs e) {
        }

        protected void btnPlay_Click(object sender, EventArgs e) {
            string winnerName = string.Empty;

            if (this.btnTournamentFileUpload.HasFile) {
                this.lblResult.Text = TournamentProcess.ProcessTournament(this.btnTournamentFileUpload.FileBytes, out winnerName);

                // Insertar el ganador en la base de datos de puntajes
                this.InsertWinnerScore(winnerName);

                this.InitializeScoreGrid();
            }
        }

        protected void btnSeeScores_Click(object sender, EventArgs e) {
            this.GetScoreboard();
        }

        protected void btnReset_Click(object sender, EventArgs e) {
            this.DeleteScoreboard();
            this.InitializeScoreGrid();
        }

        #region InitializeScoreGrid
        /// <summary>
        /// Inicializa el grid que muestra los puntajes.
        /// </summary>
        private void InitializeScoreGrid() {
            this.scoreGrid.DataSource = null;
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
                             orderby s.Points descending
                             select s).Take(10);

                this.scoreGrid.DataSource = query.ToList();
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

        #region Files Download Events
        protected void fileDown1_Click(object sender, EventArgs e) {
            Response.Redirect("ExampleFilesDownload.ashx?filename=" + this.fileDown1.Text);
        }
        protected void fileDown2_Click(object sender, EventArgs e) {
            Response.Redirect("ExampleFilesDownload.ashx?filename=" + this.fileDown2.Text);
        }
        protected void fileDown3_Click(object sender, EventArgs e) {
            Response.Redirect("ExampleFilesDownload.ashx?filename=" + this.fileDown3.Text);
        }
        protected void fileDown4_Click(object sender, EventArgs e) {
            Response.Redirect("ExampleFilesDownload.ashx?filename=" + this.fileDown4.Text);
        }
        protected void fileDown5_Click(object sender, EventArgs e) {
            Response.Redirect("ExampleFilesDownload.ashx?filename=" + this.fileDown5.Text);
        }
        #endregion Files Download Events
    }
}