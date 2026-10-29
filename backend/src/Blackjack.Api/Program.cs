using System.Text.Json.Serialization;
using Blackjack.Api.Endpoints;
using Blackjack.Api.Errors;
using Blackjack.Application;
using Blackjack.Infrastructure;

const string FrontendCorsPolicy = "Frontend";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GameExceptionHandler>();

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
    options.AddPolicy(FrontendCorsPolicy, policy =>
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors(FrontendCorsPolicy);

app.MapHealthChecks("/health");
app.MapGameEndpoints();

app.Run();

public partial class Program;