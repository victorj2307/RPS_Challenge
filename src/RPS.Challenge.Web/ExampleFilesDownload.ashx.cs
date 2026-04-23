using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


namespace RPS.Challenge.Web {
    /// <summary>
    /// Summary description for ExampleFilesDownload
    /// </summary>
    public class ExampleFilesDownload : IHttpHandler {
        public void ProcessRequest(HttpContext context) {
            System.Web.HttpRequest request = System.Web.HttpContext.Current.Request;
            string requestedFileName = request.QueryString["fileName"];
            string fileName = Path.GetFileName(requestedFileName ?? string.Empty);
            string extension = Path.GetExtension(fileName);
            bool isAllowedExtension = extension.Equals(".txt", StringComparison.OrdinalIgnoreCase) ||
                                      extension.Equals(".json", StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(fileName) || !isAllowedExtension) {
                context.Response.StatusCode = 400;
                context.Response.Write("Invalid file name.");
                return;
            }

            string physicalFilePath = context.Server.MapPath("~/tournamentExamples/" + fileName);
            if (!File.Exists(physicalFilePath)) {
                context.Response.StatusCode = 404;
                context.Response.Write("File not found.");
                return;
            }

            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ContentType = "text/plain";
            response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ";");
            response.TransmitFile("/tournamentExamples/" + fileName);
            response.Flush();
            response.End();
        }

        public bool IsReusable {
            get {
                return false;
            }
        }
    }
}
