using Domain.Aggregate;
using Domain.IRepository;
using Infrastructure.InfrastructureException;
using System.Collections.Concurrent;

namespace Infrastructure.Temporary
{
    public class InMemoryGameState : IInMemoryGameState
    {
        #region Attributes
        private readonly ConcurrentDictionary<Guid, Player> players = new();
        private readonly IUnitOfWork unitOfWork;
        #endregion

        #region Properties
        #endregion

        public InMemoryGameState(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #region Methods
        public async Task<Player> JoinPlayer(Guid id)
        {
            var player = await unitOfWork
                .GetRepository<IPlayerRepository>()
                .GetByIdAsync(id);

            if (player == null)
                throw new RepositoryException($"Player id: {id} is not found");

            players[player.ID] = player;

            return player;
        }

        public bool TryGetPlayer(Guid id, out Player player)
        {
            return players.TryGetValue(id, out player);
        }

        public void RemovePlayer(Guid id)
        {
            players.TryRemove(id, out _);
        }
        #endregion
    }
}
