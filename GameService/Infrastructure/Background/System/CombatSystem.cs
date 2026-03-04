using Application.Common;
using Application.DTO;
using AutoMapper;
using Domain.Enum;
using Domain.Interface.IInMemory;

namespace Infrastructure.Background.System
{
    public class CombatResult
    {
        public List<GrayShroomEntityActDTO> Acted { get; } = new();
        public List<Guid> Despawned { get; } = new();
        public List<(Guid playerId, Item item)> Drops { get; } = new();
    }

    public static class DropHelper
    {
        private static readonly Random rng = new();

        // Example: returns a random quantity between min and max (inclusive)
        public static int RandomQuantity(int min = 1, int max = 4)
        {
            return rng.Next(min, max + 1);
        }

        // Example: choose a random item from a list of possible drops
        public static string RandomItemId()
        {
            string[] items = new[]
            {
            ItemConfig.Mushroom,
            ItemConfig.Stone,
            ItemConfig.Wood,
            ItemConfig.Chair,
            ItemConfig.Table,
            ItemConfig.Picture,
            ItemConfig.Furnace
        };

            int index = rng.Next(items.Length);
            return items[index];
        }
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

                        // Collect drop for this player
                        var dropItem = new Item
                        {
                            Id = DropHelper.RandomItemId(),
                            Quantity = DropHelper.RandomQuantity()
                        };

                        result.Drops.Add((player.ID, dropItem));
                    }
                }
            }

            return result;
        }
    }
}
