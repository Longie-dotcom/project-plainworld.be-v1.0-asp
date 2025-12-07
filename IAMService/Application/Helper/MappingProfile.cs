using Application.DTO;
using AutoMapper;
using Domain.Aggregate;
using Domain.Entity;

namespace Application.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Privilege
            // Aggregate
            CreateMap<Privilege, PrivilegeDTO>();
            #endregion

            #region Role
            // Aggregate
            CreateMap<Role, RoleDTO>();
            CreateMap<Role, RoleDetailDTO>()
                .ForMember(dest => dest.RolePrivileges,
                    opt => opt.MapFrom(src => src.RolePrivileges));

            // Entity
            CreateMap<RolePrivilege, RolePrivilegeDTO>()
                .ForMember(dest => dest.Privilege,
                    opt => opt.MapFrom(src => src.Privilege));
            #endregion

            #region User
            // Aggregate
            CreateMap<User, UserDTO>();
            CreateMap<User, UserDetailDTO>()
                .ForMember(dest => dest.UserRoles,
                    opt => opt.MapFrom(src => src.UserRoles.ToList()))
                .ForMember(dest => dest.UserPrivileges,
                    opt => opt.MapFrom(src => src.UserPrivileges.ToList()));

            // Entity
            CreateMap<UserRole, UserRoleDTO>()
                .ForMember(dest => dest.Role, 
                    opt => opt.MapFrom(src => src.Role));
            CreateMap<UserPrivilege, UserPrivilegeDTO>()
                .ForMember(dest => dest.Privilege, 
                    opt => opt.MapFrom(src => src.Privilege));
            #endregion
        }
    }
}
