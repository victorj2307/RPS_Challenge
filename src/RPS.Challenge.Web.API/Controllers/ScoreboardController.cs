using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using RPS.Challenge.Web.API.Models.Api;
using RPS.Challenge.Web.API.Services;

namespace RPS.Challenge.Web.API.Controllers {
    [RoutePrefix("api/v1/scoreboard")]
    public class ScoreboardController : ApiControllerBase {
        [HttpGet]
        [Route("")]
        public HttpResponseMessage GetTopScores(int top = 10) {
            if (top <= 0 || top > 100) {
                return BuildBadRequest("validation_error", "The 'top' query parameter must be between 1 and 100.");
            }

            return Request.CreateResponse(HttpStatusCode.OK, new ScoreboardResponse {
                Scores = ScoreboardService.GetTopScores(top)
            });
        }

        [HttpPost]
        [Route("championship")]
        public HttpResponseMessage PostChampionship(ChampionshipScoreRequest request) {
            if (request == null || string.IsNullOrWhiteSpace(request.WinnerName)) {
                return BuildBadRequest("validation_error", "The request body must include a non-empty winnerName.");
            }

            string winner = request.WinnerName.Trim();
            string second = string.IsNullOrWhiteSpace(request.SecondPlaceName) ? null : request.SecondPlaceName.Trim();

            if (second != null && string.Equals(winner, second, StringComparison.OrdinalIgnoreCase)) {
                return BuildBadRequest("validation_error", "secondPlaceName cannot be the same as winnerName.");
            }

            ScoreboardService.ApplyChampionshipScores(winner, second);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpDelete]
        [Route("")]
        public HttpResponseMessage ResetScoreboard() {
            ScoreboardService.ResetScoreboard();
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
