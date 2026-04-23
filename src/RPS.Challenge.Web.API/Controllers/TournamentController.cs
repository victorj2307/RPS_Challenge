using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

using Newtonsoft.Json;
using RPS.Challenge.Core.Processes;
using RPS.Challenge.Core.Validation;
using RPS.Challenge.Web.API.Models.Api;

namespace RPS.Challenge.Web.API.Controllers {
    [RoutePrefix("api/v1/tournament")]
    public class TournamentController : ApiControllerBase {
        [HttpPost]
        [Route("play")]
        public HttpResponseMessage Play(PlayTournamentRequest request) {
            if (request == null || request.Tournament == null) {
                return BuildBadRequest("validation_error", "The request body must include a tournament payload.");
            }

            string serializedTournament = JsonConvert.SerializeObject(request.Tournament);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(serializedTournament);

            string validationError;
            if (!TournamentPayloadValidator.TryValidateSingleChampionshipJson(payloadBytes, out validationError)) {
                return BuildBadRequest("validation_error", validationError);
            }

            var result = TournamentProcess.ProcessTournamentDetailed(payloadBytes);

            if (!result.IsSuccess) {
                return BuildBadRequest("validation_error", result.Message);
            }

            var response = new PlayTournamentResponse {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                WinnerName = result.WinnerName,
                SecondPlaceName = result.SecondPlaceName ?? string.Empty,
                SecondPlaceStrategy = result.SecondPlaceStrategy ?? string.Empty,
                Rounds = result.Rounds.Select(round => new ApiRoundResult {
                    RoundNumber = round.RoundNumber,
                    Matches = round.Matches.Select(match => new ApiMatchResult {
                        FirstPlayerName = match.FirstPlayerName,
                        FirstPlayerStrategy = match.FirstPlayerStrategy,
                        SecondPlayerName = match.SecondPlayerName,
                        SecondPlayerStrategy = match.SecondPlayerStrategy,
                        WinnerIsFirstPlayer = match.WinnerIsFirstPlayer,
                        WinnerName = match.WinnerName,
                        WinnerStrategy = match.WinnerStrategy,
                        LoserName = match.LoserName,
                        LoserStrategy = match.LoserStrategy,
                        IsBye = match.IsBye
                    }).ToList()
                }).ToList()
            };

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        [Route("player-count")]
        public HttpResponseMessage PlayerCount(PlayTournamentRequest request) {
            if (request == null || request.Tournament == null) {
                return BuildBadRequest("validation_error", "The request body must include a tournament payload.");
            }

            string serializedTournament = JsonConvert.SerializeObject(request.Tournament);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(serializedTournament);

            string validationError;
            if (!TournamentPayloadValidator.TryValidateSingleChampionshipJson(payloadBytes, out validationError)) {
                return BuildBadRequest("validation_error", validationError);
            }

            int playerCount;
            if (!TournamentProcess.TryCountPlayers(payloadBytes, out playerCount)) {
                return BuildBadRequest("validation_error", "Invalid tournament definition or could not count players.");
            }

            return Request.CreateResponse(HttpStatusCode.OK, new PlayerCountResponse {
                PlayerCount = playerCount
            });
        }
    }
}
