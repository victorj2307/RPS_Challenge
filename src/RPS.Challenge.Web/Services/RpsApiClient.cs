using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using RPS.Challenge.Core.Entities;
using RPS.Challenge.Core.Validation;
using RPS.Challenge.Web.ViewModels;

namespace RPS.Challenge.Web.Services {
    /// <summary>
    /// Server-side HTTP client for the standalone RPS REST API.
    /// </summary>
    public static class RpsApiClient {
        private static readonly HttpClient HttpClient = CreateHttpClient();

        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        private static HttpClient CreateHttpClient() {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(120);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
            return client;
        }

        private static string GetBaseUrl() {
            string baseUrl = ConfigurationManager.AppSettings["RpsApiBaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl)) {
                return string.Empty;
            }
            return baseUrl.Trim().TrimEnd('/');
        }

        public static List<ScoreRowViewModel> GetTopScores(int top) {
            string baseUrl = GetBaseUrl();
            if (string.IsNullOrEmpty(baseUrl)) {
                return new List<ScoreRowViewModel>();
            }

            try {
                string url = string.Format("{0}/api/v1/scoreboard?top={1}", baseUrl, top);
                HttpResponseMessage response = HttpClient.GetAsync(url).GetAwaiter().GetResult();
                string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode) {
                    return new List<ScoreRowViewModel>();
                }

                ScoreboardApiDto dto = JsonConvert.DeserializeObject<ScoreboardApiDto>(body, JsonSettings);
                if (dto == null || dto.Scores == null) {
                    return new List<ScoreRowViewModel>();
                }

                return dto.Scores.Select(s => new ScoreRowViewModel {
                    Rank = s.Rank,
                    PlayerName = s.PlayerName,
                    Points = s.Points
                }).ToList();
            }
            catch {
                return new List<ScoreRowViewModel>();
            }
        }

        public static bool TryPlayTournament(byte[] fileBytes, out TournamentResult tournamentResult, out string errorMessage) {
            tournamentResult = null;
            errorMessage = string.Empty;

            string baseUrl = GetBaseUrl();
            if (string.IsNullOrEmpty(baseUrl)) {
                errorMessage = "Error: RpsApiBaseUrl is not configured in Web.config.";
                return false;
            }

            try {
                string normalizedJson;
                string validationError;
                if (!TournamentPayloadValidator.TryValidateSingleChampionshipJson(fileBytes, out normalizedJson, out validationError)) {
                    errorMessage = validationError;
                    return false;
                }

                JToken tournamentToken;
                try {
                    tournamentToken = JToken.Parse(normalizedJson);
                }
                catch (JsonReaderException) {
                    errorMessage = "Error: File content is not valid JSON.";
                    return false;
                }

                var payload = new JObject { ["tournament"] = tournamentToken };
                string json = payload.ToString(Formatting.None);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                string url = string.Format("{0}/api/v1/tournament/play", baseUrl);
                HttpResponseMessage response = HttpClient.PostAsync(url, content).GetAwaiter().GetResult();
                string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (response.StatusCode == HttpStatusCode.BadRequest) {
                    errorMessage = TryReadApiErrorMessage(body) ?? "Error: Tournament request was rejected.";
                    return false;
                }

                if (!response.IsSuccessStatusCode) {
                    errorMessage = string.Format("Error: API returned {0}.", (int)response.StatusCode);
                    return false;
                }

                PlayTournamentApiDto dto = JsonConvert.DeserializeObject<PlayTournamentApiDto>(body, JsonSettings);
                if (dto == null) {
                    errorMessage = "Error: Invalid response from tournament API.";
                    return false;
                }

                tournamentResult = MapToTournamentResult(dto);
                return true;
            }
            catch (TaskCanceledException) {
                errorMessage = "Error: The tournament API request timed out.";
                return false;
            }
            catch (HttpRequestException ex) {
                errorMessage = string.Format("Error: Could not reach the tournament API ({0}). Ensure the API project is running and RpsApiBaseUrl is correct.", ex.Message);
                return false;
            }
            catch (Exception ex) {
                errorMessage = string.Format("Error: {0}", ex.Message);
                return false;
            }
        }

        public static bool TryGetTournamentPlayerCount(byte[] fileBytes, out int playerCount, out string errorMessage) {
            playerCount = 0;
            errorMessage = string.Empty;

            string baseUrl = GetBaseUrl();
            if (string.IsNullOrEmpty(baseUrl)) {
                errorMessage = "Error: RpsApiBaseUrl is not configured in Web.config.";
                return false;
            }

            try {
                string normalizedJson;
                string validationError;
                if (!TournamentPayloadValidator.TryValidateSingleChampionshipJson(fileBytes, out normalizedJson, out validationError)) {
                    errorMessage = validationError;
                    return false;
                }

                JToken tournamentToken;
                try {
                    tournamentToken = JToken.Parse(normalizedJson);
                }
                catch (JsonReaderException) {
                    errorMessage = "Error: File content is not valid JSON.";
                    return false;
                }

                var payload = new JObject { ["tournament"] = tournamentToken };
                string json = payload.ToString(Formatting.None);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                string url = string.Format("{0}/api/v1/tournament/player-count", baseUrl);
                HttpResponseMessage response = HttpClient.PostAsync(url, content).GetAwaiter().GetResult();
                string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (response.StatusCode == HttpStatusCode.BadRequest) {
                    errorMessage = TryReadApiErrorMessage(body) ?? "Error: Invalid tournament definition.";
                    return false;
                }

                if (!response.IsSuccessStatusCode) {
                    errorMessage = string.Format("Error: API returned {0}.", (int)response.StatusCode);
                    return false;
                }

                PlayerCountApiDto dto = JsonConvert.DeserializeObject<PlayerCountApiDto>(body, JsonSettings);
                if (dto == null || dto.PlayerCount <= 0) {
                    errorMessage = "Error: Invalid response from tournament API.";
                    return false;
                }

                playerCount = dto.PlayerCount;
                return true;
            }
            catch (Exception ex) {
                errorMessage = string.Format("Error: {0}", ex.Message);
                return false;
            }
        }

        public static bool TryApplyChampionshipScores(string winnerName, string secondPlaceNameOrNull, out string errorMessage) {
            errorMessage = string.Empty;
            string baseUrl = GetBaseUrl();
            if (string.IsNullOrEmpty(baseUrl)) {
                errorMessage = "Error: RpsApiBaseUrl is not configured in Web.config.";
                return false;
            }

            try {
                var bodyDto = new ChampionshipScoreApiDto {
                    WinnerName = winnerName,
                    SecondPlaceName = string.IsNullOrWhiteSpace(secondPlaceNameOrNull) ? null : secondPlaceNameOrNull.Trim()
                };
                string json = JsonConvert.SerializeObject(bodyDto, JsonSettings);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                string url = string.Format("{0}/api/v1/scoreboard/championship", baseUrl);
                HttpResponseMessage response = HttpClient.PostAsync(url, content).GetAwaiter().GetResult();
                string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (response.StatusCode == HttpStatusCode.BadRequest) {
                    errorMessage = TryReadApiErrorMessage(body) ?? "Error: Could not update scoreboard.";
                    return false;
                }

                if (!response.IsSuccessStatusCode) {
                    errorMessage = string.Format("Error: API returned {0}.", (int)response.StatusCode);
                    return false;
                }

                return true;
            }
            catch (Exception ex) {
                errorMessage = string.Format("Error: {0}", ex.Message);
                return false;
            }
        }

        public static bool TryResetScoreboard(out string errorMessage) {
            errorMessage = string.Empty;
            string baseUrl = GetBaseUrl();
            if (string.IsNullOrEmpty(baseUrl)) {
                errorMessage = "Error: RpsApiBaseUrl is not configured in Web.config.";
                return false;
            }

            try {
                string url = string.Format("{0}/api/v1/scoreboard", baseUrl);
                HttpResponseMessage response = HttpClient.DeleteAsync(url).GetAwaiter().GetResult();
                string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode) {
                    errorMessage = TryReadApiErrorMessage(body) ?? string.Format("Error: API returned {0}.", (int)response.StatusCode);
                    return false;
                }

                return true;
            }
            catch (Exception ex) {
                errorMessage = string.Format("Error: {0}", ex.Message);
                return false;
            }
        }

        private static string TryReadApiErrorMessage(string body) {
            if (string.IsNullOrWhiteSpace(body)) {
                return null;
            }

            try {
                ApiErrorEnvelope envelope = JsonConvert.DeserializeObject<ApiErrorEnvelope>(body, JsonSettings);
                if (envelope != null && envelope.Error != null && !string.IsNullOrEmpty(envelope.Error.Message)) {
                    return envelope.Error.Message;
                }
            }
            catch {
                // ignore
            }

            return null;
        }

        private static TournamentResult MapToTournamentResult(PlayTournamentApiDto dto) {
            var result = new TournamentResult {
                IsSuccess = dto.IsSuccess,
                Message = dto.Message ?? string.Empty,
                WinnerName = dto.WinnerName ?? string.Empty,
                SecondPlaceName = dto.SecondPlaceName ?? string.Empty,
                SecondPlaceStrategy = dto.SecondPlaceStrategy ?? string.Empty,
                Rounds = new List<RoundResult>()
            };

            if (dto.Rounds == null) {
                return result;
            }

            foreach (RoundApiDto round in dto.Rounds) {
                var roundResult = new RoundResult {
                    RoundNumber = round.RoundNumber,
                    Matches = new List<MatchResult>()
                };

                if (round.Matches != null) {
                    foreach (MatchApiDto m in round.Matches) {
                        roundResult.Matches.Add(new MatchResult {
                            FirstPlayerName = m.FirstPlayerName,
                            FirstPlayerStrategy = m.FirstPlayerStrategy,
                            SecondPlayerName = m.SecondPlayerName,
                            SecondPlayerStrategy = m.SecondPlayerStrategy,
                            WinnerIsFirstPlayer = m.WinnerIsFirstPlayer,
                            WinnerName = m.WinnerName,
                            WinnerStrategy = m.WinnerStrategy,
                            LoserName = m.LoserName,
                            LoserStrategy = m.LoserStrategy,
                            IsBye = m.IsBye
                        });
                    }
                }

                result.Rounds.Add(roundResult);
            }

            return result;
        }

        private sealed class PlayTournamentApiDto {
            public bool IsSuccess { get; set; }
            public string Message { get; set; }
            public string WinnerName { get; set; }
            public string SecondPlaceName { get; set; }
            public string SecondPlaceStrategy { get; set; }
            public List<RoundApiDto> Rounds { get; set; }
        }

        private sealed class ChampionshipScoreApiDto {
            public string WinnerName { get; set; }
            public string SecondPlaceName { get; set; }
        }

        private sealed class PlayerCountApiDto {
            public int PlayerCount { get; set; }
        }

        private sealed class RoundApiDto {
            public int RoundNumber { get; set; }
            public List<MatchApiDto> Matches { get; set; }
        }

        private sealed class MatchApiDto {
            public string FirstPlayerName { get; set; }
            public string FirstPlayerStrategy { get; set; }
            public string SecondPlayerName { get; set; }
            public string SecondPlayerStrategy { get; set; }
            public bool WinnerIsFirstPlayer { get; set; }
            public string WinnerName { get; set; }
            public string WinnerStrategy { get; set; }
            public string LoserName { get; set; }
            public string LoserStrategy { get; set; }
            public bool IsBye { get; set; }
        }

        private sealed class ScoreboardApiDto {
            public List<ScoreRowApiDto> Scores { get; set; }
        }

        private sealed class ScoreRowApiDto {
            public int Rank { get; set; }
            public string PlayerName { get; set; }
            public int Points { get; set; }
        }

        private sealed class ApiErrorEnvelope {
            public ApiErrorPart Error { get; set; }
        }

        private sealed class ApiErrorPart {
            public string Code { get; set; }
            public string Message { get; set; }
        }
    }
}
