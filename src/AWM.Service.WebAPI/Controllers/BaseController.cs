using KDS.Primitives.FluentResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult HandleResultError(Error error)
        {
            var code = error.Code;
            int statusCode = StatusCodes.Status500InternalServerError;

            if (code == "400" || code.StartsWith("Validation")) statusCode = StatusCodes.Status400BadRequest;
            else if (code == "401" || code.StartsWith("Unauthorized")) statusCode = StatusCodes.Status401Unauthorized;
            else if (code == "403" || code.StartsWith("Forbidden")) statusCode = StatusCodes.Status403Forbidden;
            else if (code == "404" || code.StartsWith("NotFound")) statusCode = StatusCodes.Status404NotFound;
            else if (code == "409" || code.StartsWith("Conflict") || code.StartsWith("BusinessRule")) statusCode = StatusCodes.Status409Conflict;

            var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Status = statusCode,
                Title = error.Code,
                Detail = error.Message,
                Instance = HttpContext.Request.Path
            };
            
            problemDetails.Extensions["traceId"] = HttpContext.TraceIdentifier;
            problemDetails.Extensions["code"] = error.Code;

            return new ObjectResult(problemDetails) { StatusCode = statusCode };
        }
    }
}

