using Application.DTO;
using PlainWorld.MessageBroker;

namespace Application.Interface.IService
{
    public interface IPlayerService
    {
        // SignalR
        Task<(PlayerDTO client, PlayerEntityDTO entity, IEnumerable<PlayerEntityDTO> online)> Join(
            Guid playerId);

        (Guid client, Guid entity) Logout(
            Guid playerId);

        (PlayerMovementDTO client, PlayerEntityMovementDTO entity) Move(
            Guid playerId,
            PlayerMoveDTO dto);

        (PlayerAppearanceDTO client, PlayerEntityAppearanceDTO entity) CreateAppearance(
            Guid playerId,
            PlayerCreateAppearanceDTO dto);

        void UserSyncCreating(UserCreateDTO dto);
        Task UserSyncUpdating(UserUpdateDTO dto);
    }
}
