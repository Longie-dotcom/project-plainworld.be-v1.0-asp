using Domain.Aggregate;

namespace Domain.Interface.IInMemory
{
    public interface IInMemoryPlayerState
    {
        Task<(Player player, IEnumerable<Player> online)> Load(
            Guid playerId);

        void Unload(
            Guid playerId);

        bool TryGet(
            Guid playerId, 
            out Player player);
    }
}
