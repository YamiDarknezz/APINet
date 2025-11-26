namespace APINet.DTOs
{
    /// <summary>
    /// DTO para crear un nuevo libro
    /// </summary>
    public class CreateLibroDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string? Genero { get; set; }
    }
}
