using Application.Common;
using Application.DTO;

namespace Application.Interface.IService
{
    public interface IPlayerService
    {
        Task<PlayerDTO> Join(Guid ID, string name);
        Task<PositionDTO> Move(Guid ID, float x, float y);
    }
}
