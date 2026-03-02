using Application.DTO;
using AutoMapper;
using Domain.Interface.IInMemory;

namespace Infrastructure.Background.System
{
    public class BehaviourSystem
    {
        public IEnumerable<GrayShroomEntityActDTO> Tick(
            IMapper mapper,
            IInMemoryGrayShroomState shrooms,
            float deltaTime)
        {
            foreach (var shroom in shrooms.GetAll())
            {
                shroom.TickBehaviour(deltaTime);
                yield return mapper.Map<GrayShroomEntityActDTO>(shroom);
            }
        }
    }
}