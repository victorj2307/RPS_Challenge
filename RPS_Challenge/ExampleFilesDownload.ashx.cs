using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace RPS_Challenge {
    /// <summary>
    /// Summary description for ExampleFilesDownload
    /// </summary>
    public class ExampleFilesDownload : IHttpHandler {

        public void ProcessRequest(HttpContext context) {

            System.Web.HttpRequest request = System.Web.HttpContext.Current.Request;
            string fileName = request.QueryString["fileName"];

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