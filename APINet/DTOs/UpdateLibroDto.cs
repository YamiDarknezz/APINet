namespace APINet.DTOs
{
    /// <summary>
    /// DTO para actualizar un libro existente
    /// </summary>
    public class UpdateLibroDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string? Genero { get; set; }
    }
}
