namespace APINet.Models
{
    /// <summary>
    /// Modelo para respuestas paginadas
    /// </summary>
    public class PagedResult<T>
    {
        /// <summary>
        /// Número de página actual (base 1)
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Tamaño de página (número de elementos por página)
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total de elementos en la base de datos
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Total de páginas calculado
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        /// <summary>
        /// Indica si hay una página anterior
        /// </summary>
        public bool HasPreviousPage => Page > 1;

        /// <summary>
        /// Indica si hay una página siguiente
        /// </summary>
        public bool HasNextPage => Page < TotalPages;

        /// <summary>
        /// Datos de la página actual
        /// </summary>
        public IEnumerable<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Crea un resultado paginado
        /// </summary>
        public static PagedResult<T> Create(IEnumerable<T> items, int page, int pageSize, int totalCount)
        {
            return new PagedResult<T>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
