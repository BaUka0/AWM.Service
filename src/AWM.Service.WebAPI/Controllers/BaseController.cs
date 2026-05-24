using AWM.Service.Domain.Common;
using KDS.Primitives.FluentResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers
{
    /// <summary>
    /// Base controller providing common error handling for API controllers.
    /// </summary>
    [ApiConventionType(typeof(DefaultApiConventions))]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult HandleResultError(Error error)
        {
            var statusCode = ErrorCodes.StatusMap.TryGetValue(error.Code, out var mapped)
                ? mapped
                : StatusCodes.Status500InternalServerError;

            return Problem(
                detail: error.Message,
                instance: HttpContext.Request.Path,
                statusCode: statusCode,
                title: error.Code);
        }
    }
}

