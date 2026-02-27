using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;
using MediatR;
using AWM.Service.Application.Common.Behaviors;
using AWM.Service.Domain.Wf.Services;
using AWM.Service.Application.Features.Workflow.Services;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Application.Common.Services;

namespace AWM.Service.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(applicationAssembly);

        services.AddScoped<IStateMachine, WorkflowService>();
        services.AddScoped<IPeriodValidationService, PeriodValidationService>();

        return services;
    }
}
