using APINet.Middleware;
using APINet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using System.Text.Json;

namespace Tests
{
    [TestFixture]
    public class GlobalExceptionHandlerTests
    {
        private Mock<ILogger<GlobalExceptionHandler>> _loggerMock;
        private GlobalExceptionHandler _handler;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<GlobalExceptionHandler>>();
            _handler = new GlobalExceptionHandler(_loggerMock.Object);
        }

        [Test]
        public async Task TryHandleAsync_DeberiaManejarArgumentException_ConStatus400()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            var exception = new ArgumentException("Argumento inválido");

            // Act
            var resultado = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            Assert.That(resultado, Is.True);
            Assert.That(httpContext.Response.StatusCode, Is.EqualTo(400));

            // Leer la respuesta
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(httpContext.Response.Body);
            var responseBody = await reader.ReadToEndAsync();
            var jsendResponse = JsonSerializer.Deserialize<JSendResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.That(jsendResponse, Is.Not.Null);
            Assert.That(jsendResponse.Status, Is.EqualTo("fail"));
        }

        [Test]
        public async Task TryHandleAsync_DeberiaManejarKeyNotFoundException_ConStatus404()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            var exception = new KeyNotFoundException("Recurso no encontrado");

            // Act
            var resultado = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            Assert.That(resultado, Is.True);
            Assert.That(httpContext.Response.StatusCode, Is.EqualTo(404));

            // Leer la respuesta
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(httpContext.Response.Body);
            var responseBody = await reader.ReadToEndAsync();
            var jsendResponse = JsonSerializer.Deserialize<JSendResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.That(jsendResponse, Is.Not.Null);
            Assert.That(jsendResponse.Status, Is.EqualTo("fail"));
        }

        [Test]
        public async Task TryHandleAsync_DeberiaManejarInvalidOperationException_ConStatus409()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            var exception = new InvalidOperationException("Operación inválida");

            // Act
            var resultado = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            Assert.That(resultado, Is.True);
            Assert.That(httpContext.Response.StatusCode, Is.EqualTo(409));

            // Leer la respuesta
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(httpContext.Response.Body);
            var responseBody = await reader.ReadToEndAsync();
            var jsendResponse = JsonSerializer.Deserialize<JSendResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.That(jsendResponse, Is.Not.Null);
            Assert.That(jsendResponse.Status, Is.EqualTo("fail"));
        }

        [Test]
        public async Task TryHandleAsync_DeberiaManejarExcepcionGenerica_ConStatus500()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            var exception = new Exception("Error inesperado");

            // Act
            var resultado = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            Assert.That(resultado, Is.True);
            Assert.That(httpContext.Response.StatusCode, Is.EqualTo(500));

            // Leer la respuesta
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(httpContext.Response.Body);
            var responseBody = await reader.ReadToEndAsync();
            var jsendResponse = JsonSerializer.Deserialize<JSendResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.That(jsendResponse, Is.Not.Null);
            Assert.That(jsendResponse.Status, Is.EqualTo("error"));
            Assert.That(jsendResponse.Message, Is.Not.Null);
            Assert.That(jsendResponse.Code, Is.EqualTo(500));
        }

        [Test]
        public async Task TryHandleAsync_DeberiaLoggearExcepcion()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            var exception = new Exception("Test exception");

            // Act
            await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert - Verificar que se llamó al logger
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    exception,
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.Once);
        }

        [Test]
        public async Task TryHandleAsync_DeberiaRetornarTrue_ParaCualquierExcepcion()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            var exception = new DivideByZeroException();

            // Act
            var resultado = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            Assert.That(resultado, Is.True);
        }

        [Test]
        public async Task TryHandleAsync_DeberiaEscribirRespuestaJSON()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            var exception = new ArgumentException("Test");

            // Act
            await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(httpContext.Response.Body);
            var responseBody = await reader.ReadToEndAsync();

            // Verificar que es JSON válido
            Assert.DoesNotThrow(() =>
            {
                JsonSerializer.Deserialize<JSendResponse>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            });
        }
    }
}
