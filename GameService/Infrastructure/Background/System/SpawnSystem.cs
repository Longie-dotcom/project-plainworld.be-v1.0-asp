using Application.DTO;
using AutoMapper;
using Domain.Enum;
using Domain.Factory;
using Domain.Interface.IInMemory;

namespace Infrastructure.Background.System
{
    public class SpawnSystem
    {
        public GrayShroomEntityDTO? TrySpawn(
            IMapper mapper,
            IInMemoryGrayShroomState shrooms)
        {
            if (shrooms.GetAll().Count() >= GrayShroomConfig.MaxAlive)
                return null;

            for (int i = 0; i < GrayShroomConfig.MaxSpawnAttempts; i++)
            {
                var shroom = GrayShroomFactory.CreateRandom();

                if (CollisionMap.IsBlocked(
                        shroom.Act.CollisionBox))
                    continue;

                shrooms.Add(shroom);
                return mapper.Map<GrayShroomEntityDTO>(shroom);
            }

            // No valid spawn found this tick
            return null;
        }
    }
}
