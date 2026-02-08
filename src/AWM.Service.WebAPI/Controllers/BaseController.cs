using KDS.Primitives.FluentResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult HandleResultError(Error error)
        {
            return error.Code switch
            {
                var code when code.StartsWith("NotFound") => NotFound(new { error.Code, error.Message }),
                var code when code.StartsWith("ValidationError") => BadRequest(new { error.Code, error.Message }),
                var code when code.StartsWith("BusinessRule") || code.StartsWith("Conflict") => Conflict(new { error.Code, error.Message }),
                var code when code.StartsWith("Unauthorized") => Unauthorized(new { error.Code, error.Message }),
                var code when code.StartsWith("Forbidden") => Forbid(),
                _ => StatusCode(StatusCodes.Status500InternalServerError, new { error.Code, error.Message })
            };
        }
    }
}

