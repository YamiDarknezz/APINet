using APINet.Models;
using Microsoft.AspNetCore.Diagnostics;

namespace APINet.Middleware
{
    /// <summary>
    /// Manejador global de excepciones siguiendo especificación JSend
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Excepción capturada: {Message}", exception.Message);

            JSendResponse response;
            int statusCode;

            switch (exception)
            {
                case ArgumentException argEx:
                    // 400 Bad Request - Validación de datos fallida
                    statusCode = StatusCodes.Status400BadRequest;
                    response = JSendResponse.Fail(new
                    {
                        validationError = argEx.Message
                    });
                    break;

                case KeyNotFoundException notFoundEx:
                    // 404 Not Found - Recurso no encontrado
                    statusCode = StatusCodes.Status404NotFound;
                    response = JSendResponse.Fail(new
                    {
                        error = "Recurso no encontrado",
                        detail = notFoundEx.Message
                    });
                    break;

                case InvalidOperationException invalidOpEx:
                    // 409 Conflict - Operación inválida (ej: duplicados)
                    statusCode = StatusCodes.Status409Conflict;
                    response = JSendResponse.Fail(new
                    {
                        error = "Operación inválida",
                        detail = invalidOpEx.Message
                    });
                    break;

                default:
                    // 500 Internal Server Error - Error no controlado
                    statusCode = StatusCodes.Status500InternalServerError;
                    response = JSendResponse.Error(
                        "Ocurrió un error inesperado en el servidor. Contacte al administrador.",
                        statusCode
                    );
                    break;
            }

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true;
        }
    }
}
