using AutoMapper;
using Recetas.Core.Entities;
using Recetas.Infrastructure.DTOs;

namespace Recetas.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeos básicos
            CreateMap<Receta, RecetaDto>()
                .ForMember(d => d.NombreUsuario,
                    o => o.MapFrom(s => s.Usuario != null ? s.Usuario.Nombre : null))
                .ForMember(d => d.NombreCategoria,
                    o => o.MapFrom(s => s.Categoria != null ? s.Categoria.Nombre : null))
                .ReverseMap();

            CreateMap<Usuario, UsuarioDto>()
                .ForMember(d => d.RecetasCount,
                    o => o.MapFrom(s => s.Recetas.Count))
                .ReverseMap();

            CreateMap<Categoria, CategoriaDto>()
                .ForMember(d => d.RecetasCount,
                    o => o.MapFrom(s => s.Recetas.Count))
                .ReverseMap();
        }
    }
}