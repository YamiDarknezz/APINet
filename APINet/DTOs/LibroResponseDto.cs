namespace APINet.DTOs
{
    /// <summary>
    /// DTO para respuestas de libro
    /// </summary>
    public class LibroResponseDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string? Genero { get; set; }
    }
}
