using Domain.Aggregate;
using Domain.Interface.IInMemory;
using Domain.Interface.IRepository;
using Infrastructure.InfrastructureException;
using System.Collections.Concurrent;

namespace Infrastructure.Temporary
{
    public class InMemoryGameState : IInMemoryPlayerState
    {
        #region Attributes
        private readonly ConcurrentDictionary<Guid, Player> players = new();
        private readonly IUnitOfWork unitOfWork;
        #endregion

        #region Properties
        public IReadOnlyCollection<Player> Snapshot
        {
            get { return players.Values.ToList(); } 
        }
        #endregion

        public InMemoryGameState(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #region Methods
        public async Task<(Player player, IEnumerable<Player> online)> Load(Guid playerId)
        {
            var player = await unitOfWork
                .GetRepository<IPlayerRepository>()
                .GetByIdAsync(playerId);
            
            if (player == null)
                throw new RepositoryException(
                    $"Player {playerId} not found");

            players[playerId] = player;

            var online = players.Values
                .Where(p => p.ID != playerId)
                .ToList();

            return (player, online);
        }

        public void Unload(Guid playerId)
        {
            if (!players.TryRemove(playerId, out var player))
                return;

            unitOfWork
                .GetRepository<IPlayerRepository>()
                .Update(player.ID, player);
        }

        public bool TryGet(Guid playerId, out Player player)
        { 
            return players.TryGetValue(playerId, out player);
        }
        #endregion
    }
}
