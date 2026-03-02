using Application.DTO;
using Application.Helper;
using AutoMapper;
using Domain.Enum;
using Domain.Interface.IInMemory;

namespace Infrastructure.Background.System
{
    public class CombatResult
    {
        public List<GrayShroomEntityActDTO> Acted { get; } = new();
        public List<Guid> Despawned { get; } = new();
    }

    public class CombatSystem
    {
        public CombatResult Resolve(
            float deltaTime,
            IMapper mapper,
            IInMemoryPlayerState players,
            IInMemoryGrayShroomState shrooms)
        {
            var result = new CombatResult();

            foreach (var player in players.GetAll())
            {
                if (!player.Act.Combat.IsAttacking)
                    continue;

                foreach (var shroom in shrooms.GetAll())
                {
                    if (!player.CanHit(shroom.Act.Position))
                        continue;

                    shroom.ReceiveDamage(
                        PlayerConfig.AttackDamage,
                        shroom.Act.Position.Subtract(player.Act.Position).Normalized().Multiply(PlayerConfig.KnockbackStrength),
                        PlayerConfig.AttackStunDuration,
                        deltaTime);

                    result.Acted.Add(
                        mapper.Map<GrayShroomEntityActDTO>(shroom));

                    if (shroom.Health.IsDead)
                    {
                        shrooms.Remove(shroom.ID);
                        result.Despawned.Add(shroom.ID);
                    }
                }
            }

            return result;
        }
    }
}
