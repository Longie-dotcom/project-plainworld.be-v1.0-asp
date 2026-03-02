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
            // -------------------- Aggregate --------------------
            // Chat
            CreateMap<Chat, ChatDTO>();

            // Spawning
            CreateMap<Player, PlayerDTO>()
                .ForMember(dest => dest.Act, opt => opt.MapFrom(src => src.Act))
                .ForMember(dest => dest.Health, opt => opt.MapFrom(src => src.Health))
                .ForMember(dest => dest.Appearance, opt => opt.MapFrom(src => src.Appearance));

            CreateMap<Player, PlayerEntityDTO>()
                .ForMember(dest => dest.Act, opt => opt.MapFrom(src => src.Act))
                .ForMember(dest => dest.Health, opt => opt.MapFrom(src => src.Health))
                .ForMember(dest => dest.Appearance, opt => opt.MapFrom(src => src.Appearance));

            CreateMap<GrayShroom, GrayShroomEntityDTO>()
                .ForMember(dest => dest.Act, opt => opt.MapFrom(src => src.Act))
                .ForMember(dest => dest.Health, opt => opt.MapFrom(src => src.Health));

            // Action
            CreateMap<Player, PlayerActDTO>()
                .ForMember(dest => dest.Act, opt => opt.MapFrom(src => src.Act));

            CreateMap<Player, PlayerEntityActDTO>()
                .ForMember(dest => dest.Act, opt => opt.MapFrom(src => src.Act));

            CreateMap<GrayShroom, GrayShroomEntityActDTO>()
                .ForMember(dest => dest.Act, opt => opt.MapFrom(src => src.Act));

            // Apperance
            CreateMap<Player, PlayerAppearanceDTO>()
                .ForMember(dest => dest.Appearance, opt => opt.MapFrom(src => src.Appearance));

            CreateMap<Player, PlayerEntityAppearanceDTO>()
                .ForMember(dest => dest.Appearance, opt => opt.MapFrom(src => src.Appearance));

            // -------------------- Entity --------------------
            CreateMap<Domain.Entity.Act, Application.Common.Act>()
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.CurrentDirection, opt => opt.MapFrom(src => src.CurrentDirection))
                .ForMember(dest => dest.CollisionBox, opt => opt.MapFrom(src => src.CollisionBox));

            CreateMap<Domain.Entity.Health, Application.Common.Health>()
                .ForMember(dest => dest.Current, opt => opt.MapFrom(src => src.Current))
                .ForMember(dest => dest.Max, opt => opt.MapFrom(src => src.Max));

            CreateMap<Domain.Entity.PlayerAppearance, Application.Common.PlayerAppearance>()
                .ForMember(dest => dest.HairColor, opt => opt.MapFrom(src => src.HairColor))
                .ForMember(dest => dest.PantColor, opt => opt.MapFrom(src => src.PantColor))
                .ForMember(dest => dest.EyeColor, opt => opt.MapFrom(src => src.EyeColor))
                .ForMember(dest => dest.SkinColor, opt => opt.MapFrom(src => src.SkinColor));

            // ValueObject -> DTO
            CreateMap<CollisionBox, CollisionBoxDTO>();
            CreateMap<HSV, HSVDTO>();
            CreateMap<Position, PositionDTO>();
        }
    }
}
