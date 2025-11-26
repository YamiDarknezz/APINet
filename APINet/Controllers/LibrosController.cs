using APINet.DTOs;
using APINet.Models;
using APINet.Service;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace APINet.Controllers
{
    /// <summary>
    /// Controller para gestión de libros siguiendo especificación JSend
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private readonly ILibroService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<LibrosController> _logger;

        public LibrosController(ILibroService service, IMapper mapper, ILogger<LibrosController> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los libros con paginación
        /// </summary>
        /// <param name="page">Número de página (por defecto: 1)</param>
        /// <param name="pageSize">Tamaño de página (por defecto: 10, máximo: 100)</param>
        [HttpGet]
        public async Task<ActionResult<JSendResponse<PagedResult<LibroResponseDto>>>> GetLibros(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            // Validación de parámetros
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            _logger.LogInformation("Obteniendo lista de libros - Página: {Page}, Tamaño: {PageSize}", page, pageSize);

            var libros = await _service.ObtenerTodos();
            var totalCount = libros.Count();

            // Aplicar paginación
            var librosPaginados = libros
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var librosDto = _mapper.Map<IEnumerable<LibroResponseDto>>(librosPaginados);

            var pagedResult = PagedResult<LibroResponseDto>.Create(
                librosDto,
                page,
                pageSize,
                totalCount
            );

            return Ok(JSendResponse<PagedResult<LibroResponseDto>>.Success(pagedResult));
        }

        /// <summary>
        /// Obtiene un libro por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<JSendResponse<LibroResponseDto>>> GetLibro(int id)
        {
            _logger.LogInformation("Obteniendo libro con ID: {Id}", id);
            var libro = await _service.ObtenerPorId(id);
            var response = _mapper.Map<LibroResponseDto>(libro);
            return Ok(JSendResponse<LibroResponseDto>.Success(response));
        }

        /// <summary>
        /// Crea un nuevo libro
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<JSendResponse<LibroResponseDto>>> PostLibro(CreateLibroDto createDto)
        {
            _logger.LogInformation("Creando nuevo libro: {Titulo}", createDto.Titulo);
            var libro = _mapper.Map<Libro>(createDto);
            var nuevoLibro = await _service.Crear(libro);
            var response = _mapper.Map<LibroResponseDto>(nuevoLibro);

            var jsendResponse = JSendResponse<LibroResponseDto>.Success(response);

            return CreatedAtAction(nameof(GetLibro), new { id = nuevoLibro.Id }, jsendResponse);
        }

        /// <summary>
        /// Actualiza un libro existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<JSendResponse>> PutLibro(int id, UpdateLibroDto updateDto)
        {
            if (id != updateDto.Id)
            {
                _logger.LogWarning("ID de ruta ({RouteId}) no coincide con ID del body ({BodyId})", id, updateDto.Id);
                return BadRequest(JSendResponse.Fail(new
                {
                    id = "El Id de la ruta no coincide con el Id del libro."
                }));
            }

            _logger.LogInformation("Actualizando libro con ID: {Id}", id);
            var libro = _mapper.Map<Libro>(updateDto);
            await _service.Actualizar(libro);

            return Ok(JSendResponse.Success(new
            {
                mensaje = $"Libro con Id {id} actualizado exitosamente."
            }));
        }

        /// <summary>
        /// Elimina un libro
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<JSendResponse>> DeleteLibro(int id)
        {
            _logger.LogInformation("Eliminando libro con ID: {Id}", id);
            var eliminado = await _service.Eliminar(id);

            if (!eliminado)
            {
                _logger.LogWarning("No se pudo eliminar el libro con ID: {Id}", id);
                return NotFound(JSendResponse.Fail(new
                {
                    mensaje = $"No se pudo eliminar el libro con Id {id}."
                }));
            }

            return Ok(JSendResponse.Success(new
            {
                mensaje = $"Libro con Id {id} eliminado exitosamente."
            }));
        }
    }
}
