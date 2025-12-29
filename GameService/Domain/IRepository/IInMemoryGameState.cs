using Domain.Aggregate;

namespace Domain.IRepository
{
    public interface IInMemoryGameState
    {
        Task<(Player player, IEnumerable<Player> online)> JoinPlayer(Guid id);
        void LogoutPlayer(Guid id);
        bool TryGetPlayer(Guid id, out Player player);
        IReadOnlyCollection<Player> GetOnMemoryPlayers();
    }
}
