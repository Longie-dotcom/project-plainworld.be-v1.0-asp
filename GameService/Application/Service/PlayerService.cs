using Application.ApplicationException;
using Application.Common;
using Application.DTO;
using Application.Interface.IService;
using AutoMapper;
using Domain.IRepository;

namespace Application.Service
{
    public class PlayerService : IPlayerService
    {
        #region Attributes
        private readonly IInMemoryGameState inMemoryGameState;
        private readonly IMapper mapper;
        #endregion

        #region Properties
        #endregion

        public PlayerService(
            IInMemoryGameState inMemoryGameState,
            IMapper mapper)
        {
            this.inMemoryGameState = inMemoryGameState;
            this.mapper = mapper;
        }

        #region Methods
        public async Task<PlayerDTO> Join(Guid ID, string name)
        {
            var player = await inMemoryGameState.JoinPlayer(ID);

            return mapper.Map<PlayerDTO>(player);
        }

        public async Task<PositionDTO> Move(Guid ID, float x, float y)
        {
            if (inMemoryGameState.TryGetPlayer(ID, out var player))
            {
                player.Position.UpdatePosition(x, y);

                return mapper.Map<PositionDTO>(player.Position);
            }

            throw new PlayerNotFound($"Player id: {ID} not found in memory");
        }

        #endregion
    }
}
