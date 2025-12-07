using Domain.Aggregate;

namespace Domain.IRepository
{
    public interface IInMemoryGameState
    {
        Task<Player> JoinPlayer(Guid id);
        bool TryGetPlayer(Guid id, out Player player);
        void RemovePlayer(Guid id);
    }
}
