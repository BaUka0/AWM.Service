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
                "400" => BadRequest(new { error.Code, error.Message }),
                var code when code.StartsWith("Validation") => BadRequest(new { error.Code, error.Message }),

                "401" => Unauthorized(new { error.Code, error.Message }),
                var code when code.StartsWith("Unauthorized") => Unauthorized(new { error.Code, error.Message }),

                "403" => Forbid(),
                var code when code.StartsWith("Forbidden") => Forbid(),

                "404" => NotFound(new { error.Code, error.Message }),
                var code when code.StartsWith("NotFound") => NotFound(new { error.Code, error.Message }),

                "409" => Conflict(new { error.Code, error.Message }),
                var code when code.StartsWith("Conflict") || code.StartsWith("BusinessRule") => Conflict(new { error.Code, error.Message }),

                _ => StatusCode(StatusCodes.Status500InternalServerError, new { error.Code, error.Message })
            };
        }
    }
}

