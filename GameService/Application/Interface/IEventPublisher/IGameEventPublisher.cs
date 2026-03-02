using Application.DTO;

namespace Application.Interface.GameEventPublisher
{
    public interface IGameEventPublisher
    {
        Task SpawnAsync(GrayShroomEntityDTO dto);

        Task ActAsync(GrayShroomEntityActDTO dto);

        Task DespawnAsync(Guid id);
    }
}
