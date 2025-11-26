using APINet.DTOs;
using APINet.Models;
using AutoMapper;

namespace APINet.Mappings
{
    /// <summary>
    /// Perfil de mapeo entre entidades Libro y DTOs
    /// </summary>
    public class LibroMappingProfile : Profile
    {
        public LibroMappingProfile()
        {
            // Mapeo de CreateLibroDto a Libro
            CreateMap<CreateLibroDto, Libro>();

            // Mapeo de UpdateLibroDto a Libro
            CreateMap<UpdateLibroDto, Libro>();

            // Mapeo de Libro a LibroResponseDto
            CreateMap<Libro, LibroResponseDto>();
        }
    }
}
