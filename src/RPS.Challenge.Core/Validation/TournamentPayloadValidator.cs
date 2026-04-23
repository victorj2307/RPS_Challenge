using System;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RPS.Challenge.Core.Validation {
    /// <summary>
    /// Ensures tournament bytes are exactly one JSON array (one bracket per file) with no trailing content,
    /// then verifies the same text parses with <see cref="System.Web.Script.Serialization.JavaScriptSerializer"/>
    /// as used by the tournament pipeline.
    /// </summary>
    public static class TournamentPayloadValidator {
        public static bool TryValidateSingleChampionshipJson(byte[] fileBytes, out string errorMessage) {
            string normalizedJson;
            return TryValidateSingleChampionshipJson(fileBytes, out normalizedJson, out errorMessage);
        }

        /// <summary>
        /// Validates file bytes. On success, <paramref name="normalizedJson"/> is the BOM-stripped, trimmed string used for deserialization.
        /// </summary>
        public static bool TryValidateSingleChampionshipJson(byte[] fileBytes, out string normalizedJson, out string errorMessage) {
            normalizedJson = null;
            errorMessage = null;

            if (fileBytes == null || fileBytes.Length == 0) {
                errorMessage = "Error: Please select a tournament file.";
                return false;
            }

            string json = Encoding.UTF8.GetString(fileBytes);
            if (json.Length > 0 && json[0] == '\uFEFF') {
                json = json.Substring(1);
            }

            json = json.Trim();
            if (string.IsNullOrEmpty(json)) {
                errorMessage = "Error: File is empty.";
                return false;
            }

            try {
                using (var stringReader = new StringReader(json))
                using (var reader = new JsonTextReader(stringReader) {
                    CloseInput = true,
                    SupportMultipleContent = true
                }) {
                    if (!reader.Read()) {
                        errorMessage = "Error: File content is not valid JSON.";
                        return false;
                    }

                    if (reader.TokenType != JsonToken.StartArray) {
                        errorMessage = "Error: Tournament file must contain a single JSON array at the root (one bracket per file).";
                        return false;
                    }

                    JToken root = JToken.ReadFrom(reader);
                    var array = root as JArray;
                    if (array == null) {
                        errorMessage = "Error: Tournament file must contain a single JSON array at the root (one bracket per file).";
                        return false;
                    }

                    if (array.Count == 0) {
                        errorMessage = "Error: The tournament bracket array is empty.";
                        return false;
                    }

                    while (reader.Read()) {
                        if (reader.TokenType == JsonToken.Comment) {
                            continue;
                        }

                        errorMessage = "Error: Tournament file must contain exactly one JSON value (one bracket per file). Remove extra data after the closing bracket.";
                        return false;
                    }
                }
            }
            catch (JsonException) {
                errorMessage = "Error: File content is not valid JSON.";
                return false;
            }

            try {
                var serializer = new JavaScriptSerializer();
                serializer.DeserializeObject(json);
            }
            catch (ArgumentException) {
                errorMessage = "Error: File content is not valid JSON.";
                return false;
            }
            catch (InvalidOperationException) {
                errorMessage = "Error: File content is not valid JSON.";
                return false;
            }

            normalizedJson = json;
            return true;
        }
    }
}
