using System.Web.Mvc;

using RPS_Challenge.Services;

namespace RPS_Challenge.Controllers {
    public class ExamplesController : Controller {
        public ActionResult Index() {
            string directoryPath = Server.MapPath("~/tournamentExamples");
            var files = TournamentExamplesService.GetExampleFiles(directoryPath);
            return View(files);
        }
    }
}
