namespace AWM.Service.WebAPI.Authorization;

using System;
using AWM.Service.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

/// <summary>
/// Attribute that ensures the current user is authenticated and has a valid user ID via ICurrentUserProvider.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
public class RequireUserAttribute : Attribute, IAuthorizationFilter
{
    /// <summary>
    /// Executes the authorization filter.
    /// </summary>
    /// <param name="context">The authorization filter context.</param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Check if user is authenticated
        if (context.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Validate using ICurrentUserProvider to ensure consistent logic
        var currentUserProvider = context.HttpContext.RequestServices.GetService(typeof(ICurrentUserProvider)) as ICurrentUserProvider;

        if (currentUserProvider == null || !currentUserProvider.UserId.HasValue)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
