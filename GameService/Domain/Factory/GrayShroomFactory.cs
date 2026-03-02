using Domain.Aggregate;
using Domain.Entity;
using Domain.Enum;
using Domain.ObjectValue;

namespace Domain.Factory
{
    public static class GrayShroomFactory
    {
        public static GrayShroom CreateRandom()
        {
            var position = new Position(
                Random.Shared.NextSingle() *
                    (GrayShroomConfig.SpawnMaxX - GrayShroomConfig.SpawnMinX)
                    + GrayShroomConfig.SpawnMinX,
                Random.Shared.NextSingle() *
                    (GrayShroomConfig.SpawnMaxY - GrayShroomConfig.SpawnMinY)
                    + GrayShroomConfig.SpawnMinY
            );

            return new GrayShroom(
                Guid.NewGuid(),
                new Act(
                    moveSpeed: GrayShroomConfig.MoveSpeed,
                    position: position,
                    currentDirection: Position.Zero,
                    currentAction: (int)EntityAction.IDLE,
                    collisionBox: new CollisionBox(
                    GrayShroomConfig.CollisionBoxX,
                    GrayShroomConfig.CollisionBoxY,
                    GrayShroomConfig.CollisionBoxWidth,
                    GrayShroomConfig.CollisionBoxHeight),

                    attackCooldown: GrayShroomConfig.AttackCooldown,
                    attackRange: GrayShroomConfig.AttackRange,
                    attackOffset: GrayShroomConfig.AttackOffset
                ),
                new Health(
                    max: GrayShroomConfig.MaxHealth
                )
            );
        }
    }
}