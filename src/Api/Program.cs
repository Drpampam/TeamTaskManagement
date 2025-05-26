using Application.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence.Confgurations;
using Persistence.Extensions.Persistence;
using Persistence.Extensions.Repository;
using Serilog;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles; // Handle circular references
    });

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Database connection string is missing."));
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment()); // Log sensitive data in development
});

// Register application services and repositories
try
{
    builder.Services.AddServices();
    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddRepository(builder.Configuration);
}
catch (Exception ex)
{
    Log.Error(ex, "Failed to register services or repositories.");
    throw;
}

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing in configuration."))),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Log.Error(context.Exception, "JWT authentication failed.");
            return Task.CompletedTask;
        }
    };
});

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    try
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Team Task Management API",
            Version = "v1",
            Description = "API for managing teams and tasks"
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
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
                    }
                },
                Array.Empty<string>()
            }
        });
        c.EnableAnnotations();
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Failed to configure SwaggerGen.");
        throw;
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exception != null)
        {
            Log.Error(exception, "Unhandled exception occurred. Path: {Path}", context.Request.Path);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync("An unexpected error occurred.");
        }
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Team Task Management API v1");
        c.RoutePrefix = "swagger"; // Serve Swagger UI at /swagger/index.html
        c.DisplayRequestDuration();
        c.InjectJavascript("https://code.jquery.com/jquery-3.6.0.min.js"); // Ensure jQuery for Swagger UI
    });
    app.Logger.LogInformation("Swagger enabled at /swagger/v1/swagger.json and UI at /swagger/index.html");
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Apply migrations at startup (optional, uncomment if needed)
// using (var scope = app.Services.CreateScope())
// {
//     var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
//     try
//     {
//         var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//         await db.Database.MigrateAsync();
//         logger.LogInformation("Database migrations applied successfully.");
//     }
//     catch (Exception ex)
//     {
//         logger.LogError(ex, "Failed to apply database migrations.");
//         throw;
//     }
// }

try
{
    app.Logger.LogInformation("Starting Team Task Management API on port {Port}...", builder.Configuration["Kestrel:Endpoints:Https:Url"]?.Split(':').Last());
    await app.RunAsync();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Application failed to start.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}