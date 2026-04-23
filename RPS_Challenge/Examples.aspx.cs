using System;

using RPS_Challenge.Services;

namespace RPS_Challenge {
    public partial class Examples : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            if (this.IsPostBack) {
                return;
            }

            this.BindTournamentExampleFiles();
        }

        private void BindTournamentExampleFiles() {
            string directoryPath = Server.MapPath("~/tournamentExamples");
            this.tournamentFilesRepeater.DataSource = TournamentExamplesService.GetExampleFiles(directoryPath);
            this.tournamentFilesRepeater.DataBind();
        }
    }
}
