using AWM.Service.Domain.Thesis.Service;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Auth.Repositories;
using AWM.Service.Infrastructure.FileStorage;
using AWM.Service.Infrastructure.Persistence;
using AWM.Service.Infrastructure.Persistence.Interceptors;
using AWM.Service.Infrastructure.Persistence.Repositories.Common;
using AWM.Service.Infrastructure.Persistence.Repositories.Core;
using AWM.Service.Infrastructure.Persistence.Repositories.Defense;
using AWM.Service.Infrastructure.Persistence.Repositories.Dictionary;
using AWM.Service.Infrastructure.Persistence.Repositories.Auth;
using AWM.Service.Infrastructure.Persistence.Repositories.Thesis;
using AWM.Service.Infrastructure.Persistence.Repositories.University;
using AWM.Service.Infrastructure.Persistence.Repositories.Workflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AWM.Service.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var auditableInterceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
            var domainEventsInterceptor = sp.GetRequiredService<DispatchDomainEventsInterceptor>();
            var enableEfCommandLogging = bool.TryParse(
                configuration["Observability:EnableEfCommandLogging"],
                out var parsedEnableEfCommandLogging) && parsedEnableEfCommandLogging;
            var environmentName = configuration["ASPNETCORE_ENVIRONMENT"] ?? configuration["DOTNET_ENVIRONMENT"];
            var isDevelopmentOrStaging =
                string.Equals(environmentName, "Development", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(environmentName, "Staging", StringComparison.OrdinalIgnoreCase);

            options.UseSqlServer(connectionString, sqlOptions =>
                   {
                       sqlOptions.EnableRetryOnFailure(
                           maxRetryCount: 3,
                           maxRetryDelay: TimeSpan.FromSeconds(10),
                           errorNumbersToAdd: null);
                   })
                   .AddInterceptors(auditableInterceptor, domainEventsInterceptor);

            if (enableEfCommandLogging && isDevelopmentOrStaging)
            {
                var efCommandLogger = sp.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("AWM.Service.Infrastructure.EFCore.Commands");

                options.EnableDetailedErrors();
                options.LogTo(
                    message => efCommandLogger.LogInformation("{EfCommand}", message.TrimEnd()),
                    new[] { DbLoggerCategory.Database.Command.Name },
                    LogLevel.Information);
            }
        });

        // Register University Read-Only DbContext
        services.AddDbContext<UniversityDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
                   {
                       sqlOptions.EnableRetryOnFailure(
                           maxRetryCount: 3,
                           maxRetryDelay: TimeSpan.FromSeconds(10),
                           errorNumbersToAdd: null);
                   });
        });

        // Register Database Initialiser
        services.AddScoped<ApplicationDbContextInitialiser>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Common Repositories (Critical)
        services.AddScoped<ISemesterTypeRepository, SemesterTypeRepository>();
        services.AddScoped<ISemesterRepository, SemesterRepository>();
        services.AddScoped<IWorkflowStageRepository, WorkflowStageRepository>();
        services.AddScoped<IStageRepository, StageRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();

        // Register Workflow Repository (Critical)
        services.AddScoped<IWorkflowRepository, WorkflowRepository>();

        // Register Core Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        // Register Defense Repositories
        services.AddScoped<ICommissionRepository, CommissionRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<IPreDefenseAttemptRepository, PreDefenseAttemptRepository>();
        services.AddScoped<IEvaluationCriteriaRepository, EvaluationCriteriaRepository>();
        services.AddScoped<IProtocolRepository, ProtocolRepository>();

        // Register Thesis Repositories
        services.AddScoped<IDirectionRepository, DirectionRepository>();
        services.AddScoped<ITopicRepository, TopicRepository>();
        services.AddScoped<ITopicApplicationRepository, TopicApplicationRepository>();
        services.AddScoped<IStudentWorkRepository, StudentWorkRepository>();
        services.AddScoped<ISpecialityCheckTypeRepository, SpecialityCheckTypeRepository>();
        services.AddScoped<IReviewerRepository, ReviewerRepository>();
        services.AddScoped<IExpertRepository, ExpertRepository>();
        services.AddScoped<ISupervisorReviewRepository, SupervisorReviewRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();

        // Register Dictionary/Lookup Repositories
        services.AddScoped<ISpecialityRepository, SpecialityRepository>();
        services.AddScoped<ISpecialityLevelRepository, SpecialityLevelRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IOrganizationLookupRepository, OrganizationLookupRepository>();

        // Register Auth Repositories
        services.AddScoped<IRoleAccessRepository, RoleAccessRepository>();
        services.AddScoped<IRoleOperationRepository, RoleOperationRepository>();
        services.AddScoped<IRoleActionTypeRepository, RoleActionTypeRepository>();
        services.AddScoped<IRoleOperationActionRepository, RoleOperationActionRepository>();
        services.AddScoped<IUserAccessRepository, UserAccessRepository>();
        services.AddScoped<IUserAccessHistoryRepository, UserAccessHistoryRepository>();
        services.AddScoped<ILocalAccountRepository, LocalAccountRepository>();

        // Register University Read-Only Repositories
        services.AddScoped<IUserReadOnlyRepository, UserReadOnlyRepository>();
        services.AddScoped<IStudentReadOnlyRepository, StudentReadOnlyRepository>();
        services.AddScoped<IEmployeeReadOnlyRepository, EmployeeReadOnlyRepository>();
        services.AddScoped<IOrgUnitReadOnlyRepository, OrgUnitReadOnlyRepository>();
        services.AddScoped<ISemesterReadOnlyRepository, SemesterReadOnlyRepository>();
        services.AddScoped<ISpecialityReadOnlyRepository, SpecialityReadOnlyRepository>();

        // Register File Storage Service
        // Switch to S3FileStorageService for production (add AWSSDK.S3 NuGet + configure "FileStorage:S3" section)
        services.AddScoped<IAttachmentService, LocalFileStorageService>();

        return services;
    }
}


