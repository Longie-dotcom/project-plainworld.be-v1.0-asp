using Domain.Enum;
using Domain.ObjectValue;
using System.Reflection.Emit;

namespace Domain.State
{
    public class CombatState
    {
        #region Attributes
        private float cooldownRemaining;
        private float attackTimer = 0f;
        private float attackWindow = PlayerConfig.AttackWindow;

        internal Action? OnHitStunFinished;
        #endregion

        #region Properties
        public float AttackCooldown { get; private set; }
        public float AttackRange { get; private set; }
        public float AttackOffset { get; private set; }
        
        public bool IsAttacking
        {
            get; private set;
        }

        public bool IsStunned
        {
            get { return HitStunRemaining > 0f; }
        }

        public float HitStunRemaining { get; private set; }
        #endregion

        public CombatState(
            float attackCooldown,
            float attackRange,
            float attackOffset)
        {
            AttackCooldown = attackCooldown;
            AttackRange = attackRange;
            AttackOffset = attackOffset;
        }

        #region Methods
        internal void Tick(float deltaTime)
        {
            if (cooldownRemaining > 0)
                cooldownRemaining -= deltaTime;

            if (HitStunRemaining > 0)
            {
                HitStunRemaining -= deltaTime;
                if (HitStunRemaining <= 0f)
                {
                    OnHitStunFinished?.Invoke();
                }
            }

            if (IsAttacking)
            {
                attackTimer -= deltaTime;
                if (attackTimer <= 0f)
                {
                    IsAttacking = false;
                }
            }
        }

        internal void TryStartAttack()
        {
            if (cooldownRemaining > 0)
                return;

            IsAttacking = true;
            cooldownRemaining = AttackCooldown;
            attackTimer = attackWindow;
        }

        internal void ApplyHitStunned(float duration)
        {
            HitStunRemaining = MathF.Max(HitStunRemaining, duration);
        }

        internal bool CanHit(
            Position attackerPos,
            Position direction,
            Position targetPos)
        {
            if (!IsAttacking)
                return false;

            var center = GetAttackCenter(attackerPos, direction);

            return center.DistanceTo(targetPos) <= AttackRange;
        }
        #endregion

        #region Private Helpers
        private Position GetAttackCenter(Position position, Position direction)
        {
            return position.Add(direction.Multiply(AttackOffset));
        }
        #endregion
    }
}
