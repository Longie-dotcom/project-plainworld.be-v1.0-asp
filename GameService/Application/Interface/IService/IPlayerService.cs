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
            IEnumerable<PlayerEntityDTO> onlinePlayers,
            IEnumerable<GrayShroomEntityDTO> onlineGrayShrooms,
            IEnumerable<WorldObjectDTO> onlineWorldObjects,
            string? oldConnectionId
        )> Join(
            Guid playerId,
            string connectionId);

        Guid? Logout(
            Guid playerId,
            string connectionId);

        (PlayerActDTO client, PlayerEntityActDTO entity) Act(
            Guid playerId,
            PlayerActsDTO dto);

        (PlayerAppearanceDTO client, PlayerEntityAppearanceDTO entity) CreateAppearance(
            Guid playerId,
            PlayerCreateAppearanceDTO dto);

        WorldObjectDTO PlaceWorldObject(
            Guid playerId,
            PlayerPlaceWorldObjectDTO dto);

        ChatDTO SendChat(
            Guid playerId,
            ChatSendDTO dto);
        #endregion

        #region Communication (RabbitMQ)
        void UserSyncCreating(UserCreateDTO dto);

        Task UserSyncUpdating(UserUpdateDTO dto);
        #endregion
    }
}
