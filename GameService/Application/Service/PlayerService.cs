using Application.ApplicationException;
using Application.DTO;
using Application.Interface.IService;
using AutoMapper;
using Domain.Aggregate;
using Domain.IRepository;
using Domain.ObjectValue;
using PlainWorld.MessageBroker;

namespace Application.Service
{
    public class PlayerService : IPlayerService
    {
        #region Attributes
        private readonly IInMemoryGameState inMemoryGameState;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        #endregion

        #region Properties
        #endregion

        public PlayerService(
            IInMemoryGameState inMemoryGameState,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.inMemoryGameState = inMemoryGameState;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        #region Methods
        public async Task<(PlayerDTO client, PlayerEntityDTO entity, IEnumerable<PlayerEntityDTO> online)> Join(
            Guid playerId)
        {
            // Load player resources
            var result = await inMemoryGameState.JoinPlayer(playerId);

            // Return resources for caller and other clients
            var client = mapper.Map<PlayerDTO>(result.player);
            var entity = mapper.Map<PlayerEntityDTO>(result.player);
            var online = mapper.Map<IEnumerable<PlayerEntityDTO>>(result.online);
            return (client, entity, online);
        }

        public (Guid client, Guid entity) Logout(
            Guid playerId)
        {
            // Save player resources data and remove from memory
            inMemoryGameState.LogoutPlayer(playerId);

            return (playerId, playerId);
        }

        public (PlayerMovementDTO client, PlayerEntityMovementDTO entity) Move(
            Guid playerId, 
            PlayerMoveDTO dto)
        {
            if (inMemoryGameState.TryGetPlayer(playerId, out var player))
            {
                player.CreateMovement(
                    dto.Movement.MoveSpeed,
                    new Position(
                        dto.Movement.Position.X, 
                        dto.Movement.Position.Y),
                    new Position(
                        dto.Movement.CurrentDirection.X, 
                        dto.Movement.CurrentDirection.Y),
                    dto.Movement.CurrentAction);

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
            if (inMemoryGameState.TryGetPlayer(playerId, out var player))
            {
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
