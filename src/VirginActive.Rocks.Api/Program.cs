using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Formatting.Compact;
using System.Text.Json.Serialization;
using VirginActive.Rocks.Api.Authentication;
using VirginActive.Rocks.Api.ErrorHandling;
using VirginActive.Rocks.Api.Middleware;
using VirginActive.Rocks.Application;
using VirginActive.Rocks.Domain;
using VirginActive.Rocks.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Structured JSON logging with correlation support.
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "VirginActive.Rocks.Api")
        .WriteTo.Console(new RenderedCompactJsonFormatter());
});

// Application dependencies.
builder.Services.AddDomain();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Controllers and string enum serialization.
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Centralised RFC 7807 error handling.
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// API key authentication.
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = ApiKeyAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = ApiKeyAuthenticationDefaults.AuthenticationScheme;
    })
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
        ApiKeyAuthenticationDefaults.AuthenticationScheme,
        _ => { });

builder.Services.AddAuthorization();

// Swagger/OpenAPI documentation.
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Name = "X-Api-Key",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Enter the API key."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("ApiKey", document)] = []
    });
});

var app = builder.Build();

// Correlation should be established early so downstream logs and errors can use it.
app.UseMiddleware<CorrelationIdMiddleware>();

// Global exception handling.
app.UseExceptionHandler();

// Log request method, path, status and duration.
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("CorrelationId", httpContext.TraceIdentifier);
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
    };
});

// Swagger is exposed only during local development.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Authentication must run before authorization.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;