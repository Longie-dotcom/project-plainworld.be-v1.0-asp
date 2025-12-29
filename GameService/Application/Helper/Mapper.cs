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
                .ForMember(dest => dest.Movement, opt => opt.MapFrom(src => src.Movement))
                .ForMember(dest => dest.Appearance, opt => opt.MapFrom(src => src.Appearance));

            CreateMap<Player, PlayerEntityDTO>()
                .ForMember(dest => dest.Movement, opt => opt.MapFrom(src => src.Movement))
                .ForMember(dest => dest.Appearance, opt => opt.MapFrom(src => src.Appearance));

            CreateMap<Player, PlayerMovementDTO>()
                .ForMember(dest => dest.Movement, opt => opt.MapFrom(src => src.Movement));

            CreateMap<Player, PlayerEntityMovementDTO>()
                .ForMember(dest => dest.Movement, opt => opt.MapFrom(src => src.Movement));

            CreateMap<Player, PlayerAppearanceDTO>()
                .ForMember(dest => dest.Appearance, opt => opt.MapFrom(src => src.Appearance));

            CreateMap<Player, PlayerEntityAppearanceDTO>()
                .ForMember(dest => dest.Appearance, opt => opt.MapFrom(src => src.Appearance));

            // Entity -> DTO
            CreateMap<Domain.Entity.PlayerMovement, Application.Common.PlayerMovement>()
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.CurrentDirection, opt => opt.MapFrom(src => src.CurrentDirection));

            CreateMap<Domain.Entity.PlayerAppearance, Application.Common.PlayerAppearance>()
                .ForMember(dest => dest.HairColor, opt => opt.MapFrom(src => src.HairColor))
                .ForMember(dest => dest.PantColor, opt => opt.MapFrom(src => src.PantColor))
                .ForMember(dest => dest.EyeColor, opt => opt.MapFrom(src => src.EyeColor))
                .ForMember(dest => dest.SkinColor, opt => opt.MapFrom(src => src.SkinColor));

            // ValueObject -> DTO
            CreateMap<Position, PositionDTO>();
            CreateMap<HSV, HSVDTO>();
        }
    }
}
