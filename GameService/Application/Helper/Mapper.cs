using Application.Common;
using Application.DTO;
using AutoMapper;
using Domain.Aggregate;
using Domain.ObjectValue;

namespace Application.Helper
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            // Aggregate -> DTO
            CreateMap<Player, PlayerDTO>()
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position));

            // Entity -> DTO

            // ValueObject -> DTO
            CreateMap<Position, PositionDTO>();
        }
    }
}
