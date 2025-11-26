using APINet.Controllers;
using APINet.Models;
using Microsoft.AspNetCore.Mvc;

namespace Tests
{
    [TestFixture]
    public class HomeControllerTests
    {
        private HomeController _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new HomeController();
        }

        [Test]
        public void Index_DeberiaRetornarOk_ConInformacionAPI()
        {
            // Act
            var resultado = _controller.Index();
            var okResult = resultado as OkObjectResult;

            // Assert
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            // Verificar estructura JSend
            var jsendResponse = okResult.Value as JSendResponse<object>;
            Assert.That(jsendResponse, Is.Not.Null);
            Assert.That(jsendResponse.Status, Is.EqualTo("success"));
            Assert.That(jsendResponse.Data, Is.Not.Null);
        }

        [Test]
        public void Index_DeberiaContenerInformacionCorrecta()
        {
            // Act
            var resultado = _controller.Index();
            var okResult = resultado as OkObjectResult;
            var jsendResponse = okResult?.Value as JSendResponse<object>;

            // Assert
            Assert.That(jsendResponse, Is.Not.Null);
            Assert.That(jsendResponse.Data, Is.Not.Null);

            // Verificar que contiene las propiedades esperadas
            var data = jsendResponse.Data;
            var dataType = data.GetType();

            Assert.That(dataType.GetProperty("nombre"), Is.Not.Null);
            Assert.That(dataType.GetProperty("version"), Is.Not.Null);
            Assert.That(dataType.GetProperty("descripcion"), Is.Not.Null);
            Assert.That(dataType.GetProperty("endpoints"), Is.Not.Null);
        }

        [Test]
        public void Status_DeberiaRetornarOk_ConEstadoAPI()
        {
            // Act
            var resultado = _controller.Status();
            var okResult = resultado as OkObjectResult;

            // Assert
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            // Verificar estructura JSend
            var jsendResponse = okResult.Value as JSendResponse<object>;
            Assert.That(jsendResponse, Is.Not.Null);
            Assert.That(jsendResponse.Status, Is.EqualTo("success"));
            Assert.That(jsendResponse.Data, Is.Not.Null);
        }

        [Test]
        public void Status_DeberiaContenerInformacionDeEstado()
        {
            // Act
            var resultado = _controller.Status();
            var okResult = resultado as OkObjectResult;
            var jsendResponse = okResult?.Value as JSendResponse<object>;

            // Assert
            Assert.That(jsendResponse, Is.Not.Null);
            Assert.That(jsendResponse.Data, Is.Not.Null);

            // Verificar que contiene las propiedades esperadas
            var data = jsendResponse.Data;
            var dataType = data.GetType();

            Assert.That(dataType.GetProperty("estado"), Is.Not.Null);
            Assert.That(dataType.GetProperty("timestamp"), Is.Not.Null);
            Assert.That(dataType.GetProperty("version"), Is.Not.Null);
            Assert.That(dataType.GetProperty("entorno"), Is.Not.Null);
        }

        [Test]
        public void Status_DeberiaRetornarEstadoActiva()
        {
            // Act
            var resultado = _controller.Status();
            var okResult = resultado as OkObjectResult;
            var jsendResponse = okResult?.Value as JSendResponse<object>;

            // Assert
            Assert.That(jsendResponse, Is.Not.Null);
            var data = jsendResponse.Data;
            var dataType = data.GetType();
            var estadoProperty = dataType.GetProperty("estado");
            var estadoValue = estadoProperty?.GetValue(data)?.ToString();

            Assert.That(estadoValue, Is.EqualTo("Activa"));
        }

        [Test]
        public void Status_DeberiaRetornarTimestampReciente()
        {
            // Act
            var resultado = _controller.Status();
            var okResult = resultado as OkObjectResult;
            var jsendResponse = okResult?.Value as JSendResponse<object>;

            // Assert
            Assert.That(jsendResponse, Is.Not.Null);
            var data = jsendResponse.Data;
            var dataType = data.GetType();
            var timestampProperty = dataType.GetProperty("timestamp");
            var timestampValue = timestampProperty?.GetValue(data);

            Assert.That(timestampValue, Is.InstanceOf<DateTime>());

            var timestamp = (DateTime)timestampValue!;
            var diferencia = DateTime.UtcNow - timestamp;

            // Verificar que el timestamp es reciente (menos de 1 segundo de diferencia)
            Assert.That(diferencia.TotalSeconds, Is.LessThan(1));
        }
    }
}
