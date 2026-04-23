using System.Net;
using System.Net.Http;
using System.Web.Http;

using RPS.Challenge.Web.API.Models.Api;

namespace RPS.Challenge.Web.API.Controllers {
    public abstract class ApiControllerBase : ApiController {
        protected HttpResponseMessage BuildBadRequest(string code, string message) {
            return Request.CreateResponse(HttpStatusCode.BadRequest, new ApiErrorResponse {
                Error = new ApiError {
                    Code = code,
                    Message = message
                }
            });
        }
    }
}
