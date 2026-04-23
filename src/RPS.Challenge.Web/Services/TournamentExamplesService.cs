using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

using RPS.Challenge.Web.ViewModels;

namespace RPS.Challenge.Web.Services {
    public static class TournamentExamplesService {
        public static List<TournamentFileViewModel> GetExampleFiles(string directoryPath) {
            if (!Directory.Exists(directoryPath)) {
                return new List<TournamentFileViewModel>();
            }

            string[] allowedExtensions = new[] { ".txt", ".json" };
            return Directory.GetFiles(directoryPath)
                            .Where(filePath => allowedExtensions.Contains(Path.GetExtension(filePath), StringComparer.OrdinalIgnoreCase))
                            .Select(Path.GetFileName)
                            .OrderBy(fileName => fileName)
                            .Select(fileName => {
                                string fullPath = Path.Combine(directoryPath, fileName);
                                TournamentFileDescriptionViewModel description = GetTournamentFileDescription(fullPath);
                                return new TournamentFileViewModel {
                                    FileName = fileName,
                                    DownloadUrl = "ExampleFilesDownload.ashx?fileName=" + HttpUtility.UrlEncode(fileName),
                                    Description = description.Message,
                                    DescriptionCssClass = description.CssClass
                                };
                            })
                            .ToList();
        }

        private static TournamentFileDescriptionViewModel GetTournamentFileDescription(string filePath) {
            byte[] fileBytes;
            try {
                fileBytes = File.ReadAllBytes(filePath);
            }
            catch {
                return new TournamentFileDescriptionViewModel {
                    Message = "Could not read tournament definition file.",
                    CssClass = "file-description file-description-invalid"
                };
            }

            int playerCount;
            string apiError;
            if (!RpsApiClient.TryGetTournamentPlayerCount(fileBytes, out playerCount, out apiError)) {
                return new TournamentFileDescriptionViewModel {
                    Message = string.IsNullOrWhiteSpace(apiError) ? "Invalid tournament definition file." : apiError,
                    CssClass = "file-description file-description-invalid"
                };
            }

            return new TournamentFileDescriptionViewModel {
                Message = string.Format("Tournament definition file with {0} players.", playerCount),
                CssClass = "file-description file-description-valid"
            };
        }
    }
}
