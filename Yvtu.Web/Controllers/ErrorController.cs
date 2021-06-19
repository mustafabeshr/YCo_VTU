using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Yvtu.Web.Controllers
{
    [Authorize]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }

        [Route("Error")]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            ViewBag.ExceptionPath = exceptionDetails.Path;
            ViewBag.ExceptionMessage = exceptionDetails.Error.Message;
            ViewBag.ExceptionStackTrace = exceptionDetails.Error.StackTrace;
            logger.LogError(exceptionDetails.Path + "\n" + exceptionDetails.Error.Message + "\n" + exceptionDetails.Error.StackTrace);
            return View("Error");
        }
    }
}
