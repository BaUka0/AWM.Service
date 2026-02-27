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
using FluentValidation;
using AWM.Service.Application.Common.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Global Exception Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Add MediatR with ValidationBehavior
var applicationAssembly = typeof(AWM.Service.Application.Features.Auth.Commands.Login.LoginCommand).Assembly;
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(applicationAssembly);
    cfg.AddBehavior(typeof(MediatR.IPipelineBehavior<,>), typeof(AWM.Service.Application.Common.Behaviors.ValidationBehavior<,>));
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(applicationAssembly);

// Add API Versioning
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

// Configure JWT Settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

// Add JWT Authentication
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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AWM.Service API", Version = "v1" });

    // Add JWT Authentication support to Swagger
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

// Add Infrastructure Layer
builder.Services.AddInfrastructure(builder.Configuration);

// Add HttpContextAccessor and CurrentUserProvider for audit
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AWM.Service.Domain.Common.ICurrentUserProvider, AWM.Service.WebAPI.Common.Services.CurrentUserProvider>();

// Add Authentication Services
builder.Services.AddScoped<AWM.Service.Domain.Auth.Interfaces.IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<AWM.Service.Domain.Auth.Interfaces.IJwtTokenService, JwtTokenService>();

// Add Context-Aware RBAC Authorization
builder.Services.AddContextAwareAuthorization();
builder.Services.AddPermissionPolicies();

// Add Application Services
builder.Services.AddScoped<AWM.Service.Domain.Wf.Services.IStateMachine, AWM.Service.Application.Features.Workflow.Services.WorkflowService>();
builder.Services.AddScoped<AWM.Service.Domain.CommonDomain.Services.IPeriodValidationService, PeriodValidationService>();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Seed test data (only when tables are empty)
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AWM.Service.Infrastructure.Persistence.ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<AWM.Service.Domain.Auth.Interfaces.IPasswordHasher>();
        await AWM.Service.Infrastructure.Persistence.DbSeeder.SeedAsync(dbContext, passwordHasher);
    }

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AWM.Service API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
