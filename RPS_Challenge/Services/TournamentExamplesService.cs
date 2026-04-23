using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

using RPS_Challenge.ViewModels;

namespace RPS_Challenge.Services {
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
            int playerCount;
            if (!TryGetPlayerCountFromFile(filePath, out playerCount)) {
                return new TournamentFileDescriptionViewModel {
                    Message = "Invalid tournament definition file.",
                    CssClass = "file-description file-description-invalid"
                };
            }

            return new TournamentFileDescriptionViewModel {
                Message = string.Format("Tournament definition file with {0} players.", playerCount),
                CssClass = "file-description file-description-valid"
            };
        }

        private static bool TryGetPlayerCountFromFile(string filePath, out int playerCount) {
            playerCount = 0;

            try {
                string jsonContent = File.ReadAllText(filePath, Encoding.UTF8);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                object parsedNode = serializer.DeserializeObject(jsonContent);
                playerCount = CountPlayers(parsedNode);
                return playerCount > 0;
            }
            catch {
                return false;
            }
        }

        private static int CountPlayers(object node) {
            object[] nodeArray = node as object[];
            if (nodeArray == null) {
                throw new InvalidOperationException("Invalid tournament node.");
            }

            if (nodeArray.Length == 2 && nodeArray[0] is string && nodeArray[1] is string) {
                return 1;
            }

            int count = 0;
            foreach (object childNode in nodeArray) {
                count += CountPlayers(childNode);
            }

            return count;
        }
    }
}
