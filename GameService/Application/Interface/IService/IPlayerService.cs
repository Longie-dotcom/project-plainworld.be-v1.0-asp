using Application.DTO;
using PlainWorld.MessageBroker;

namespace Application.Interface.IService
{
    public interface IPlayerService
    {
        #region Services (SignalR)
        Task<(
            PlayerDTO client, 
            PlayerEntityDTO entity, 
            IEnumerable<PlayerEntityDTO> online, 
            string? oldConnectionId
        )> Join(
            Guid playerId,
            string connectionId);

        Guid? Logout(
            Guid playerId, 
            string connectionId);

        (PlayerMovementDTO client, PlayerEntityMovementDTO entity) Move(
            Guid playerId,
            PlayerMoveDTO dto);

        (PlayerAppearanceDTO client, PlayerEntityAppearanceDTO entity) CreateAppearance(
            Guid playerId,
            PlayerCreateAppearanceDTO dto);
        #endregion

        #region Communication (RabbitMQ)
        void UserSyncCreating(UserCreateDTO dto);

        Task UserSyncUpdating(UserUpdateDTO dto);
        #endregion
    }
}
