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
        public async Task<(Player player, IEnumerable<Player> online)> JoinPlayer(Guid id)
        {
            var player = await unitOfWork
                .GetRepository<IPlayerRepository>()
                .GetByIdAsync(id);

            if (player == null)
                throw new RepositoryException(
                    $"Player id: {id} is not found");

            players[player.ID] = player;

            // Snapshot online players EXCLUDING self
            var onlinePlayers = players.Values
                .Where(p => p.ID != player.ID)
                .ToList(); // snapshot to avoid concurrent mutation issues

            return (player, onlinePlayers);
        }

        public void LogoutPlayer(Guid id)
        {
            Player player;
            players.TryGetValue(id, out player);

            if (player == null)
                throw new RepositoryException(
                    $"Player id: {id} is not found");

            unitOfWork
                .GetRepository<IPlayerRepository>()
                .Update(id, player);

            players.TryRemove(id, out _);
        }

        public bool TryGetPlayer(Guid id, out Player player)
        {
            return players.TryGetValue(id, out player);
        }

        public IReadOnlyCollection<Player> GetOnMemoryPlayers()
        {
            return players.Values.ToList();
        }
        #endregion
    }
}
