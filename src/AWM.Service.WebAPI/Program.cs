#region Using Statements
using System.Text;
using AWM.Service.Infrastructure;
using AWM.Service.WebAPI.Common.Services;
using AWM.Service.WebAPI.Common.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Middleware;
using AWM.Service.Application;
using Mapster;
using Microsoft.AspNetCore.RateLimiting;
#endregion

/// <summary>
/// Application entry point and configuration.
/// Configures services, middleware, and the HTTP request pipeline for the AWM Service API.
/// </summary>
var builder = WebApplication.CreateBuilder(args);

#region Service Configuration
builder.Services.AddControllers();

builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);

TypeAdapterConfig<AWM.Service.Application.Features.University.DTOs.OrgUnitDto, AWM.Service.WebAPI.Common.Contracts.Responses.University.OrgUnitResponse>.NewConfig()
    .Map(dest => dest.NameRu, src => src.Name)
    .Map(dest => dest.NameKz, src => src.Name)
    .Map(dest => dest.NameEn, src => src.Name)
    .Map(dest => dest.Code, src => src.Id.ToString())
    .Map(dest => dest.ParentId, src => src.ParentId);

TypeAdapterConfig<AWM.Service.Application.Features.University.DTOs.SpecialityDto, AWM.Service.WebAPI.Common.Contracts.Responses.University.AcademicProgramResponse>.NewConfig()
    .Map(dest => dest.NameRu, src => src.Name)
    .Map(dest => dest.NameKz, src => src.Name)
    .Map(dest => dest.NameEn, src => src.Name)
    .Map(dest => dest.DepartmentId, src => src.OrgUnitId);

TypeAdapterConfig<AWM.Service.Application.Features.University.DTOs.SpecialityLevelDto, AWM.Service.WebAPI.Common.Contracts.Responses.University.SpecialityLevelResponse>.NewConfig()
    .Map(dest => dest.NameRu, src => src.Name)
    .Map(dest => dest.NameKz, src => src.Name)
    .Map(dest => dest.NameEn, src => src.Name)
    .Map(dest => dest.Name, src => src.Name);

TypeAdapterConfig<AWM.Service.Domain.Auth.Entities.RoleAccess, AWM.Service.WebAPI.Common.Contracts.Responses.Auth.RoleAccessResponse>.NewConfig()
    .Map(dest => dest.Name, src => src.NameRu)
    .Map(dest => dest.UsersCount, src => src.UserAccesses.Count);

TypeAdapterConfig<AWM.Service.Application.Features.University.DTOs.UserDto, AWM.Service.WebAPI.Common.Contracts.Responses.University.AdminUserResponse>.NewConfig()
    .Map(dest => dest.Roles, src => src.Roles)
    .Map(dest => dest.IsActive, src => src.IsActive)
    .Map(dest => dest.CreatedAt, src => src.CreatedAt);

TypeAdapterConfig<AWM.Service.WebAPI.Common.Contracts.Requests.Workflow.CreateWorkTypeRequest, AWM.Service.Application.Features.Workflow.WorkTypes.Commands.CreateWorkType.CreateWorkTypeCommand>.NewConfig()
    .Map(dest => dest.SpecialityLevelId, src => src.DegreeLevelId);

TypeAdapterConfig<AWM.Service.WebAPI.Common.Contracts.Requests.Workflow.UpdateWorkTypeRequest, AWM.Service.Application.Features.Workflow.WorkTypes.Commands.UpdateWorkType.UpdateWorkTypeCommand>.NewConfig()
    .Map(dest => dest.SpecialityLevelId, src => src.DegreeLevelId);

TypeAdapterConfig<AWM.Service.Application.Features.Workflow.WorkTypes.DTOs.WorkTypeDto, AWM.Service.WebAPI.Common.Contracts.Responses.Workflow.WorkTypeResponse>.NewConfig()
    .Map(dest => dest.DegreeLevelId, src => src.SpecialityLevelId);

TypeAdapterConfig<AWM.Service.Application.Features.Workflow.Works.DTOs.WorkAttachmentDto, AWM.Service.WebAPI.Common.Contracts.Responses.Works.WorkAttachmentResponse>.NewConfig()
    .Map(dest => dest.DownloadUrl, src => $"/api/v1/student-works/attachments/{src.Id}/download");

TypeAdapterConfig<AWM.Service.Application.Features.Workflow.Attachments.DTOs.AttachmentDto, AWM.Service.WebAPI.Common.Contracts.Responses.Attachments.AttachmentResponse>.NewConfig()
    .Map(dest => dest.DownloadUrl, src => $"/api/v1/student-works/attachments/{src.Id}/download");

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AWM.Service.Infrastructure.Persistence.ApplicationDbContext>();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("AuthLimiter", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 10;
        opt.QueueLimit = 2;
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
    });
});

builder.Services.AddApplication();

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AWM.Service API", Version = "v1" });
    c.CustomSchemaIds(type => type.FullName);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AWM.Service.Domain.Common.ICurrentUserProvider, AWM.Service.WebAPI.Common.Services.CurrentUserProvider>();
builder.Services.AddScoped<AWM.Service.Domain.Auth.Interfaces.IJwtTokenService, JwtTokenService>();

#endregion

var app = builder.Build();

#region Database Initialization

var migrateAtStartup = builder.Configuration.GetValue<bool>("DatabaseSettings:MigrateAtStartup", true);
if (migrateAtStartup)
{
    using (var scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<AWM.Service.Infrastructure.Persistence.ApplicationDbContextInitialiser>();
        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

#endregion

#region Middleware Configuration

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AWM.Service API v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();
#endregion

#region Endpoints

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
#endregion
