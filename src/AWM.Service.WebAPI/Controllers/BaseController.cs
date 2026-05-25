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
            var statusCode = ResolveStatusCode(error.Code);

            return Problem(
                detail: error.Message,
                instance: HttpContext.Request.Path,
                statusCode: statusCode,
                title: error.Code);
        }

        /// <summary>
        /// Resolves HTTP status code from error code.
        /// Uses exact match first, then convention-based suffix matching.
        /// For example, "Topics.NotFound" → tries "Topics.NotFound", then "NotFound" → 404.
        /// Falls back to 500 if no match is found.
        /// </summary>
        private static int ResolveStatusCode(string errorCode)
        {
            // 1. Exact match
            if (ErrorCodes.StatusMap.TryGetValue(errorCode, out var mapped))
                return mapped;

            // 2. Convention-based suffix match (e.g., "Topics.NotFound" → "NotFound")
            var lastDot = errorCode.LastIndexOf('.');
            if (lastDot >= 0)
            {
                var suffix = errorCode[(lastDot + 1)..];
                if (ErrorCodes.StatusMap.TryGetValue(suffix, out var suffixMapped))
                    return suffixMapped;
            }

            // 3. Fallback
            return StatusCodes.Status500InternalServerError;
        }
    }
}

