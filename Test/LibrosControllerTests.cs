using APINet.Controllers;
using APINet.DTOs;
using APINet.Models;
using APINet.Service;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace Tests
{
    [TestFixture]
    public class LibrosControllerTests
    {
        private Mock<ILibroService> _serviceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<LibrosController>> _loggerMock;
        private LibrosController _controller;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<ILibroService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<LibrosController>>();
            _controller = new LibrosController(_serviceMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetLibros_DeberiaRetornarOkConListaPaginada_EnFormatoJSend()
        {
            // Arrange
            var libros = new List<Libro>
            {
                new Libro { Id = 1, Titulo = "Libro 1", Autor = "Autor 1", Anio = 2000, Genero = "Drama" },
                new Libro { Id = 2, Titulo = "Libro 2", Autor = "Autor 2", Anio = 2010, Genero = "Ficción" }
            };

            var librosDto = new List<LibroResponseDto>
            {
                new LibroResponseDto { Id = 1, Titulo = "Libro 1", Autor = "Autor 1", Anio = 2000, Genero = "Drama" },
                new LibroResponseDto { Id = 2, Titulo = "Libro 2", Autor = "Autor 2", Anio = 2010, Genero = "Ficción" }
            };

            _serviceMock.Setup(s => s.ObtenerTodos()).ReturnsAsync(libros);
            _mapperMock.Setup(m => m.Map<IEnumerable<LibroResponseDto>>(It.IsAny<IEnumerable<Libro>>()))
                .Returns(librosDto);

            // Act
            var resultado = await _controller.GetLibros(page: 1, pageSize: 10);
            var okResult = resultado.Result as OkObjectResult;

            // Assert
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            // Verificar estructura JSend
            var jsendResponse = okResult.Value as JSendResponse<PagedResult<LibroResponseDto>>;
            Assert.That(jsendResponse, Is.Not.Null);
            Assert.That(jsendResponse.Status, Is.EqualTo("success"));
            Assert.That(jsendResponse.Data, Is.Not.Null);
            Assert.That(jsendResponse.Data.TotalCount, Is.EqualTo(2));
        }

        [Test]
        public void GetLibro_DeberiaLanzarExcepcion_SiNoExiste()
        {
            // Arrange
            _serviceMock.Setup(s => s.ObtenerPorId(It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException("No se encontró un libro con Id 99."));

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _controller.GetLibro(99));
        }

        [Test]
        public async Task PostLibro_DeberiaRetornarCreated_ConFormatoJSend()
        {
            // Arrange
            var createDto = new CreateLibroDto { Titulo = "Nuevo Libro", Autor = "Autor X", Anio = 2021, Genero = "Ficción" };
            var libro = new Libro { Id = 1, Titulo = "Nuevo Libro", Autor = "Autor X", Anio = 2021, Genero = "Ficción" };
            var responseDto = new LibroResponseDto { Id = 1, Titulo = "Nuevo Libro", Autor = "Autor X", Anio = 2021, Genero = "Ficción" };

            _mapperMock.Setup(m => m.Map<Libro>(createDto)).Returns(libro);
            _serviceMock.Setup(s => s.Crear(It.IsAny<Libro>())).ReturnsAsync(libro);
            _mapperMock.Setup(m => m.Map<LibroResponseDto>(libro)).Returns(responseDto);

            // Act
            var resultado = await _controller.PostLibro(createDto);
            var createdResult = resultado.Result as CreatedAtActionResult;

            // Assert
            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult.StatusCode, Is.EqualTo(201));

            // Verificar estructura JSend
            var jsendResponse = createdResult.Value as JSendResponse<LibroResponseDto>;
            Assert.That(jsendResponse, Is.Not.Null);
            Assert.That(jsendResponse.Status, Is.EqualTo("success"));
            Assert.That(jsendResponse.Data?.Titulo, Is.EqualTo("Nuevo Libro"));
        }

        [Test]
        public async Task PutLibro_DeberiaRetornarOk_ConFormatoJSend()
        {
            // Arrange
            var updateDto = new UpdateLibroDto { Id = 1, Titulo = "Actualizado", Autor = "Autor Y", Anio = 2005, Genero = "Drama" };
            var libro = new Libro { Id = 1, Titulo = "Actualizado", Autor = "Autor Y", Anio = 2005, Genero = "Drama" };

            _mapperMock.Setup(m => m.Map<Libro>(updateDto)).Returns(libro);
            _serviceMock.Setup(s => s.Actualizar(It.IsAny<Libro>())).ReturnsAsync(true);

            // Act
            var resultado = await _controller.PutLibro(1, updateDto);
            var okResult = resultado.Result as OkObjectResult;

            // Assert
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            // Verificar estructura JSend
            var jsendResponse = okResult.Value as JSendResponse;
            Assert.That(jsendResponse, Is.Not.Null);
            Assert.That(jsendResponse.Status, Is.EqualTo("success"));
        }

        [Test]
        public async Task PutLibro_DeberiaRetornarBadRequest_ConFormatoJSend_SiIdNoCoincide()
        {
            // Arrange
            var updateDto = new UpdateLibroDto { Id = 2, Titulo = "Error", Autor = "Autor Z", Anio = 2010, Genero = "Terror" };

            // Act
            var resultado = await _controller.PutLibro(1, updateDto);
            var badRequestResult = resultado.Result as BadRequestObjectResult;

            // Assert
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));

            // Verificar estructura JSend (fail)
            var jsendResponse = badRequestResult.Value as JSendResponse;
            Assert.That(jsendResponse, Is.Not.Null);
            Assert.That(jsendResponse.Status, Is.EqualTo("fail"));
        }

        [Test]
        public async Task DeleteLibro_DeberiaRetornarNotFound_ConFormatoJSend_SiNoSeElimina()
        {
            // Arrange
            _serviceMock.Setup(s => s.Eliminar(99)).ReturnsAsync(false);

            // Act
            var resultado = await _controller.DeleteLibro(99);
            var notFoundResult = resultado.Result as NotFoundObjectResult;

            // Assert
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));

            // Verificar estructura JSend (fail)
            var jsendResponse = notFoundResult.Value as JSendResponse;
            Assert.That(jsendResponse, Is.Not.Null);
            Assert.That(jsendResponse.Status, Is.EqualTo("fail"));
        }

        [Test]
        public async Task DeleteLibro_DeberiaRetornarOk_ConFormatoJSend_SiSeEliminaExitosamente()
        {
            // Arrange
            _serviceMock.Setup(s => s.Eliminar(1)).ReturnsAsync(true);

            // Act
            var resultado = await _controller.DeleteLibro(1);
            var okResult = resultado.Result as OkObjectResult;

            // Assert
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            // Verificar estructura JSend (success)
            var jsendResponse = okResult.Value as JSendResponse;
            Assert.That(jsendResponse, Is.Not.Null);
            Assert.That(jsendResponse.Status, Is.EqualTo("success"));
        }
    }
}
