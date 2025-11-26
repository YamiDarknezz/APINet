using APINet.Models;
using Microsoft.AspNetCore.Mvc;

namespace APINet.Controllers
{
    /// <summary>
    /// Controller de bienvenida y documentación de la API
    /// </summary>
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// Endpoint de bienvenida con información de la API
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            var info = new
            {
                nombre = "API de Gestión de Libros",
                version = "1.0.0",
                descripcion = "API RESTful con CI/CD a Azure - Proyecto Académico de Calidad de Software",
                estudiante = "Erick",
                tecnologias = new[]
                {
                    ".NET 8.0",
                    "Entity Framework Core",
                    "Azure SQL Database",
                    "GitHub Actions",
                    "Azure App Service",
                    "Serilog",
                    "FluentValidation",
                    "AutoMapper",
                    "Health Checks",
                    "Rate Limiting",
                    "JSend Specification"
                },
                endpoints = new
                {
                    swagger = "/swagger",
                    healthCheck = "/health",
                    status = "/status",
                    libros = "/api/Libros",
                    ejemploLibro = "/api/Libros/1",
                    librosPaginados = "/api/Libros?page=1&pageSize=10"
                },
                caracteristicas = new[]
                {
                    "Clean Architecture (Repository + Service Pattern)",
                    "JSend Response Format (standardized API responses)",
                    "Global Exception Handler con JSend",
                    "DTOs con AutoMapper",
                    "Validaciones con FluentValidation",
                    "Paginación en endpoints de listado",
                    "Logging estructurado con Serilog",
                    "Health Checks para monitoreo",
                    "Rate Limiting para protección DoS (100 req/min)",
                    "Environment Variables con .env (DotNetEnv)",
                    "Tests unitarios (NUnit + Moq) - Coverage >70%",
                    "CI/CD automatizado (Build → Test → Deploy)",
                    "Deployment continuo a Azure"
                }
            };

            return Ok(JSendResponse<object>.Success(info));
        }

        /// <summary>
        /// Endpoint de verificación de estado de la API
        /// </summary>
        [HttpGet("status")]
        public IActionResult Status()
        {
            var status = new
            {
                estado = "Activa",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                entorno = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                servidor = Environment.MachineName,
                uptime = Environment.TickCount64 / 1000 // segundos desde el inicio
            };

            return Ok(JSendResponse<object>.Success(status));
        }
    }
}
