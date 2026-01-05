using Application.ApplicationException;
using Application.DTO;
using Application.Interface.IService;
using AutoMapper;
using Domain.Aggregate;
using Domain.Enum;
using Domain.Interface.IInMemory;
using Domain.Interface.IRepository;
using Domain.ObjectValue;
using PlainWorld.MessageBroker;

namespace Application.Service
{
    public class PlayerService : IPlayerService
    {
        #region Attributes
        private readonly IInMemoryConnectionState inMemoryConnectionState;
        private readonly IInMemoryPlayerState inMemoryPlayerState;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        #endregion

        #region Properties
        #endregion

        public PlayerService(
            IInMemoryConnectionState inMemoryConnectionState,
            IInMemoryPlayerState inMemoryPlayerState,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.inMemoryConnectionState = inMemoryConnectionState;
            this.inMemoryPlayerState = inMemoryPlayerState;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        #region Methods
        public async Task<(
            PlayerDTO client,
            PlayerEntityDTO entity,
            IEnumerable<PlayerEntityDTO> online,
            string? oldConnectionId
        )> Join(Guid playerId, string connectionId)
        {
            // Take over connection (newest win)
            var oldConnectionId =
                inMemoryConnectionState.TakeOver(playerId, connectionId);

            // Save current player data if there are already connection
            if (oldConnectionId != null)
                inMemoryPlayerState.Unload(playerId);

            // Reload player data to memory
            var (player, onlinePlayers) =
                await inMemoryPlayerState.Load(playerId);

            // Return resources for caller and other clients
            return (
                mapper.Map<PlayerDTO>(player),
                mapper.Map<PlayerEntityDTO>(player),
                mapper.Map<IEnumerable<PlayerEntityDTO>>(onlinePlayers),
                oldConnectionId
            );
        }

        public Guid? Logout(Guid playerId, string connectionId)
        {
            // Only owner can logout
            if (!inMemoryConnectionState.IsOwner(playerId, connectionId))
                return null;

            // Remove connection
            inMemoryConnectionState.Remove(playerId, connectionId);

            // Unload player (persist)
            inMemoryPlayerState.Unload(playerId);

            return playerId;
        }

        public (PlayerMovementDTO client, PlayerEntityMovementDTO entity) Move(
            Guid playerId, 
            PlayerMoveDTO dto)
        {
            if (inMemoryPlayerState.TryGet(playerId, out var player))
            {
                // Replace old movement
                player.CreateMovement(
                    new Position(dto.Direction.X, dto.Direction.Y),
                    (EntityAction)dto.Action,
                    dto.DeltaTime);

                // Return resources for caller and other clients
                var client = mapper.Map<PlayerMovementDTO>(player);
                var entity = mapper.Map<PlayerEntityMovementDTO>(player);
                return (client, entity);
            }

            throw new PlayerNotFound($"Player id: {playerId} not found in memory");
        }

        public (PlayerAppearanceDTO client, PlayerEntityAppearanceDTO entity) CreateAppearance(
            Guid playerId,
            PlayerCreateAppearanceDTO dto)
        {
            if (inMemoryPlayerState.TryGet(playerId, out var player))
            {
                // Replace old appearance
                player.CreateAppearance(
                    dto.Appearance.HairID,
                    dto.Appearance.GlassesID,
                    dto.Appearance.ShirtID,
                    dto.Appearance.PantID,
                    dto.Appearance.ShoeID,
                    dto.Appearance.EyesID,
                    dto.Appearance.SkinID,
                    new HSV(
                        dto.Appearance.HairColor.H, 
                        dto.Appearance.HairColor.S, 
                        dto.Appearance.HairColor.V),
                    new HSV(
                        dto.Appearance.PantColor.H, 
                        dto.Appearance.PantColor.S, 
                        dto.Appearance.PantColor.V),
                    new HSV(
                        dto.Appearance.EyeColor.H, 
                        dto.Appearance.EyeColor.S, 
                        dto.Appearance.EyeColor.V),
                    new HSV(
                        dto.Appearance.SkinColor.H, 
                        dto.Appearance.SkinColor.S, 
                        dto.Appearance.SkinColor.V)
                );

                // Return resources for caller and other clients
                var client = mapper.Map<PlayerAppearanceDTO>(player);
                var entity = mapper.Map<PlayerEntityAppearanceDTO>(player);
                return (client, entity);
            }

            throw new PlayerNotFound($"Player id: {playerId} not found in memory");
        }

        #region Other services
        public void UserSyncCreating(UserCreateDTO dto)
        {
            // Apply domain
            var player = new Player(
                dto.UserID,
                dto.Email,
                dto.FullName,
                dto.Gender,
                dto.Dob);

            // Apply persistence
            unitOfWork
                .GetRepository<IPlayerRepository>()
                .Add(player);
        }

        public async Task UserSyncUpdating(UserUpdateDTO dto)
        {
            var user = await unitOfWork
                .GetRepository<IPlayerRepository>()
                .GetByIdAsync(dto.UserID);

            if (user == null)
                throw new PlayerNotFound(
                    $"Player with ID: {dto.UserID} is not found");

            // Apply domain
            if (!string.IsNullOrEmpty(dto.Email))
                user.UpdateEmail(dto.Email);

            if (!string.IsNullOrEmpty(dto.FullName))
                user.UpdateFullName(dto.FullName);

            if (!string.IsNullOrEmpty(dto.Gender))
                user.UpdateGender(dto.Gender);

            if (dto.Dob.HasValue)
                user.UpdateDob(dto.Dob.Value);

            // Apply persistence
            unitOfWork
                .GetRepository<IPlayerRepository>()
                .Update(user.ID, user);
        }
        #endregion
        #endregion
    }
}
