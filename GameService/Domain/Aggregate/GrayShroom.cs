using Domain.Entity;
using Domain.Enum;
using Domain.Interface.IComponent;
using Domain.ObjectValue;

namespace Domain.Aggregate
{
    public class GrayShroom : AggregateBase, ICombatEntity
    {
        #region Attributes
        private static readonly Random rng = new();
        #endregion

        #region Properties
        public float WanderTimer { get; private set; }

        public Act Act { get; private set; }
        public Health Health { get; private set; }
        #endregion

        public GrayShroom(
            Guid id,
            Act act,
            Health health) : base(id)
        {
            Act = act;
            Health = health;
            WanderTimer = 0f;
        }

        #region Methods
        public void TickBehaviour(float deltaTime)
        {
            if (Health.IsDead || Act.Combat.IsStunned)
                return;

            WanderTimer -= deltaTime;

            if (WanderTimer > 0)
                return;

            var dir = new Position(
                (float)(rng.NextDouble() * 2 - 1),
                (float)(rng.NextDouble() * 2 - 1))
                .Normalized();

            CreateAction(dir, EntityAction.RUN, deltaTime);

            // Reset wander time
            WanderTimer =
                GrayShroomConfig.WanderCooldownMin +
                (float)rng.NextDouble() *
                (GrayShroomConfig.WanderCooldownMax - GrayShroomConfig.WanderCooldownMin);
        }

        #region Action
        public void CreateAction(
            Position direction,
            EntityAction action,
            float deltaTime)
        {
            Act.ApplyInput(
                direction,
                action,
                deltaTime);
        }

        public void TickCombat(float deltaTime)
        {
            Act.TickCombat(deltaTime);
        }
        #endregion

        #region Health
        public void ReceiveDamage(
            int damage,
            Position knockbackForce,
            float stunDuration,
            float deltaTime)
        {
            Health.TakeDamage(damage);

            if (!Health.IsDead)
            {
                Act.UpdateAction((int)EntityAction.DAMAGED);
            }

            Act.ApplyHitStunned(stunDuration);
            Act.ApplyKnockbacked(knockbackForce, deltaTime);
        }
        #endregion
        #endregion
    }
}
