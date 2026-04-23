using System.Web.Mvc;

using RPS.Challenge.Web.Services;

namespace RPS.Challenge.Web.Controllers {
    public class ExamplesController : Controller {
        public ActionResult Index() {
            string directoryPath = Server.MapPath("~/tournamentExamples");
            var files = TournamentExamplesService.GetExampleFiles(directoryPath);
            return View(files);
        }
    }
}

