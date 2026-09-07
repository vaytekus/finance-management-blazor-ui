using System.Text;
using System.Text.Json.Serialization;
using FinanceManagement.Api.Extensions;
using FinanceManagement.Api.Json;
using FinanceManagement.Api.Middleware;
using FinanceManagement.Api.Services;
using FinanceManagement.Application.Extensions;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Infrastructure.Data;
using FinanceManagement.Infrastructure.Extensions;
using FinanceManagement.Application.Options;
using FinanceManagement.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext());

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Jwt configuration section is missing.");

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

const string BlazorClientPolicy = "BlazorClient";

builder.Services.AddCors(options =>
{
    options.AddPolicy(BlazorClientPolicy, policy =>
    {
        var allowedOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>()
            ?? throw new InvalidOperationException("Cors:AllowedOrigins configuration section is missing.");

        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());
        options.JsonSerializerOptions.Converters.Add(new NullableUtcDateTimeConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Finance Management Api",
        Version = "v1",
        Description = "API for managing personal finance data (types, operations, reports)."
    });

    options.CustomSchemaIds(t => t.FullName);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token (without 'Bearer ' prefix — Swagger adds it automatically)."
    });

    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc),
            new List<string>()
        }
    });
});

builder.Services.AddHostedService<RefreshTokenCleanupService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

await app.SeedAdminAsync();

app.UseExceptionHandler();

app.UseMiddleware<CorrelationIdMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.InjectStylesheet("/SwaggerDark.css");
        options.EnablePersistAuthorization();
    });
}

app.UseCors(BlazorClientPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }
