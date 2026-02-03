using KDS.Primitives.FluentResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult HandleResultError(Error error)
        {
            return StatusCode(int.Parse(error.Code), error.Message);
        }
    }
}
