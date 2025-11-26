using APINet.Data;
using APINet.Middleware;
using APINet.Repositories;
using APINet.Service;
using DotNetEnv;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Threading.RateLimiting;

// ============================================================================
// CARGAR VARIABLES DE ENTORNO DESDE .env
// ============================================================================
// Cargar .env si existe (desarrollo local)
var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
    Console.WriteLine("✅ Archivo .env cargado correctamente");
}
else
{
    Console.WriteLine("⚠️ Archivo .env no encontrado - usando variables de entorno del sistema");
}

// ============================================================================
// CONFIGURACIÓN DE SERILOG (Logging Estructurado)
// ============================================================================
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/api-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    Log.Information("🚀 Iniciando API de Gestión de Libros...");

    var builder = WebApplication.CreateBuilder(args);

    // Configurar Serilog como logger
    builder.Host.UseSerilog();

    // ============================================================================
    // CONFIGURACIÓN DE BASE DE DATOS
    // ============================================================================
    // Construir connection string desde variables de entorno
    var azureSqlServer = Environment.GetEnvironmentVariable("AZURE_SQL_SERVER");
    var azureSqlDatabase = Environment.GetEnvironmentVariable("AZURE_SQL_DATABASE");
    var azureSqlUser = Environment.GetEnvironmentVariable("AZURE_SQL_USER");
    var azureSqlPassword = Environment.GetEnvironmentVariable("AZURE_SQL_PASSWORD");
    var azureSqlPort = Environment.GetEnvironmentVariable("AZURE_SQL_PORT") ?? "1433";

    var connectionString = $"Server=tcp:{azureSqlServer},{azureSqlPort};Initial Catalog={azureSqlDatabase};Persist Security Info=False;User ID={azureSqlUser};Password={azureSqlPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString));

    // ============================================================================
    // CONFIGURACIÓN DE REPOSITORIOS Y SERVICIOS
    // ============================================================================
    builder.Services.AddScoped<ILibroRepository, LibroRepository>();
    builder.Services.AddScoped<ILibroService, LibroService>();

    // ============================================================================
    // CONFIGURACIÓN DE AUTOMAPPER
    // ============================================================================
    builder.Services.AddAutoMapper(typeof(Program));

    // ============================================================================
    // CONFIGURACIÓN DE FLUENTVALIDATION
    // ============================================================================
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<Program>();

    // ============================================================================
    // CONFIGURACIÓN DE GLOBAL EXCEPTION HANDLER
    // ============================================================================
    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    // ============================================================================
    // CONFIGURACIÓN DE RATE LIMITING (Protección DoS)
    // ============================================================================
    builder.Services.AddRateLimiter(options =>
    {
        // Política de ventana fija: máximo 100 requests cada 1 minuto
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 100,
                    QueueLimit = 10,
                    Window = TimeSpan.FromMinutes(1)
                }));

        // Respuesta cuando se excede el límite
        options.OnRejected = async (context, token) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.HttpContext.Response.WriteAsJsonAsync(new
            {
                error = "Demasiadas solicitudes",
                mensaje = "Has excedido el límite de 100 requests por minuto. Intenta más tarde.",
                retryAfter = "60 segundos"
            }, token);
        };
    });

    // ============================================================================
    // CONFIGURACIÓN DE HEALTH CHECKS
    // ============================================================================
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<AppDbContext>(name: "database");

    // ============================================================================
    // CONFIGURACIÓN DE CORS
    // ============================================================================
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });

        // Política más restrictiva para producción
        options.AddPolicy("Production", policy =>
        {
            policy.WithOrigins("https://api-net-aebffhgchrgpf5bm.chilecentral-01.azurewebsites.net")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    });

    // ============================================================================
    // CONFIGURACIÓN DE CONTROLLERS Y SWAGGER
    // ============================================================================
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "API de Gestión de Libros",
            Version = "v1.0.0",
            Description = "API RESTful con CI/CD a Azure - Proyecto Académico de Calidad de Software",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "Erick",
                Email = "n00340097@upn.pe"
            }
        });
    });

    var app = builder.Build();

    // ============================================================================
    // MIDDLEWARE PIPELINE
    // ============================================================================

    // Exception Handler (debe ir primero)
    app.UseExceptionHandler();

    // Serilog Request Logging
    app.UseSerilogRequestLogging();

    // Rate Limiting
    app.UseRateLimiter();

    // CORS (debe ir antes de Authorization)
    app.UseCors("AllowAll");

    // Swagger (habilitado para demostración al profesor)
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Libros v1");
        options.RoutePrefix = "swagger";
    });

    // HTTPS Redirection
    app.UseHttpsRedirection();

    // Authorization
    app.UseAuthorization();

    // Health Checks
    app.MapHealthChecks("/health");

    // Controllers (Rate Limiting aplicado globalmente)
    app.MapControllers();

    Log.Information("✅ API iniciada correctamente");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ La aplicación falló al iniciar");
}
finally
{
    Log.CloseAndFlush();
}
