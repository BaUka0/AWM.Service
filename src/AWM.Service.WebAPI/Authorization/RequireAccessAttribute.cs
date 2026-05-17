namespace AWM.Service.WebAPI.Authorization;

using AWM.Service.Application.Features.Auth.RbacPlus.Queries.CheckUserAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

/// <summary>
/// Authorization attribute that checks user access for a specific operation and action.
/// Uses RBAC+ permission matrix via CheckUserAccessQuery.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class RequireAccessAttribute : Attribute, IAsyncAuthorizationFilter
{
    public string Operation { get; }
    public string Action { get; }

    public RequireAccessAttribute(string operation, string action)
    {
        Operation = operation ?? throw new ArgumentNullException(nameof(operation));
        Action = action ?? throw new ArgumentNullException(nameof(action));
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var userIdClaim = context.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            context.Result = new ForbidResult();
            return;
        }

        var mediator = context.HttpContext.RequestServices.GetRequiredService<IMediator>();
        var availableActions = await mediator.Send(new CheckUserAccessQuery
        {
            UserId = userId,
            OperationName = Operation
        });

        if (!availableActions.Contains(Action, StringComparer.OrdinalIgnoreCase))
        {
            context.Result = new ForbidResult();
        }
    }
}
