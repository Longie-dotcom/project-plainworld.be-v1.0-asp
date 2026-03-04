using Application.ApplicationException;
using Application.Common;
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
        private readonly IInMemoryChatState inMemoryChatState;
        private readonly IInMemoryPlayerState inMemoryPlayerState;
        private readonly IInMemoryGrayShroomState inMemoryGrayShroomState;
        private readonly IInMemoryWorldObjectState inMemoryWorldObjectState;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        #endregion

        #region Properties
        #endregion

        public PlayerService(
            IInMemoryConnectionState inMemoryConnectionState,
            IInMemoryChatState inMemoryChatState,
            IInMemoryPlayerState inMemoryPlayerState,
            IInMemoryGrayShroomState inMemoryGrayShroomState,
            IInMemoryWorldObjectState inMemoryWorldObjectState,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.inMemoryConnectionState = inMemoryConnectionState;
            this.inMemoryChatState = inMemoryChatState;
            this.inMemoryPlayerState = inMemoryPlayerState;
            this.inMemoryGrayShroomState = inMemoryGrayShroomState;
            this.inMemoryWorldObjectState = inMemoryWorldObjectState;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        #region Methods
        public async Task<(
            PlayerDTO client,
            PlayerEntityDTO entity,
            IEnumerable<PlayerEntityDTO> onlinePlayers,
            IEnumerable<GrayShroomEntityDTO> onlineGrayShrooms,
            IEnumerable<WorldObjectDTO> onlineWorldObjects,
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

            // Reload world entities
            var onlineGrayShrooms = inMemoryGrayShroomState.GetAll();

            // Reload world objects
            var onlineWorldObjects = inMemoryWorldObjectState.GetAll();

            // Return resources for caller and other clients
            return (
                mapper.Map<PlayerDTO>(player),
                mapper.Map<PlayerEntityDTO>(player),
                mapper.Map<IEnumerable<PlayerEntityDTO>>(onlinePlayers),
                mapper.Map<IEnumerable<GrayShroomEntityDTO>>(onlineGrayShrooms),
                mapper.Map<IEnumerable<WorldObjectDTO>>(onlineWorldObjects),
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

        public (PlayerActDTO client, PlayerEntityActDTO entity) Act(
            Guid playerId,
            PlayerActsDTO dto)
        {
            if (inMemoryPlayerState.TryGet(playerId, out var player))
            {
                // Replace old action
                player.CreateAction(
                    new Position(dto.Direction.X, dto.Direction.Y),
                    (EntityAction)dto.Action,
                    dto.DeltaTime);

                // Return resources for caller and other clients
                var client = mapper.Map<PlayerActDTO>(player);
                var entity = mapper.Map<PlayerEntityActDTO>(player);
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

        public ChatDTO SendChat(
            Guid playerId,
            ChatSendDTO dto)
        {
            // Recheck connection & player existence
            if (!inMemoryPlayerState.TryGet(playerId, out var player))
                throw new PlayerNotFound(
                    $"Player id: {playerId} not found in memory");

            // Apply domain
            var chat = new Chat(
                Guid.NewGuid(),
                playerId,
                player.FullName,
                ChatType.Message,
                dto.Content);

            // Fire & forget: store only for short-lived memory
            inMemoryChatState.Add(chat);

            // Return resources for caller and other clients
            var receive = mapper.Map<ChatDTO>(chat);
            return receive;
        }

        public WorldObjectDTO PlaceWorldObject(Guid playerId, PlayerPlaceWorldObjectDTO dto)
        {
            // Recheck connection & player existence
            if (!inMemoryPlayerState.TryGet(playerId, out var player))
                throw new PlayerNotFound($"Player id: {playerId} not found in memory");

            // Convert PositionDTO to Position
            var position = new Position(dto.Position.X, dto.Position.Y);

            // Calculate collision box (example: size 1x1)
            // You can also look up the item size from item catalog if available
            var collision = new CollisionBox(position.X, position.Y, 1f, 1f);

            // ✅ Check if blocked
            if (CollisionMap.IsBlocked(collision))
                throw new InvalidOperationException("Cannot place object: collision detected");

            // Create world object
            var worldObject = new WorldObject(
                Guid.NewGuid(),
                dto.ItemID,
                position,
                collision
            );

            // FIX THE FRONT END!! (SPLIT THE OBJECT TO ANOTHER SERVICE)
            //// Add the collision to the map for blocking
            //CollisionMap.AddCollision(collision);

            // Add to memory repo
            inMemoryWorldObjectState.Add(worldObject);

            // Return DTO for client/server sync
            return mapper.Map<WorldObjectDTO>(worldObject);
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
