using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Yvtu.api.Errors;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data;

namespace Yvtu.api.Controllers
{
    public class ErrorController : BaseApiController
    {
        private readonly IApiDbLog _apiDbLog;

        public ErrorController(IApiDbLog apiDbLog)
        {
            this._apiDbLog = apiDbLog;
        }

        [HttpPost("/api/Error")]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            _ = _apiDbLog.Create(new ApiLogFile
            {
                Data = $"{exceptionDetails.Error.GetType().Name} Path={exceptionDetails.Path} Message={exceptionDetails.Error.Message} StackTrace={exceptionDetails.Error.StackTrace}",
                Action = "Error",
                Ip = this.HttpContext.Connection.RemoteIpAddress.ToString(),
                Level = 1,
                User = this.HttpContext.User.Identity.Name
            });

            return StatusCode(500, new ApiResponse(500));
        }
    }
}
