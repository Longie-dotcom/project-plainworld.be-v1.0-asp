using Domain.DomainException;
using Domain.Enum;
using Domain.ObjectValue;
using Domain.State;

namespace Domain.Entity
{
    public class Act
    {
        #region Attributes
        #endregion

        #region Properties
        public float MoveSpeed { get; private set; }
        public Position Position { get; private set; }
        public Position CurrentDirection { get; private set; }
        public int CurrentAction { get; private set; }
        public CollisionBox CollisionBox { get; private set; }

        public CombatState Combat { get; private set; }
        #endregion

        public Act(
            float moveSpeed,
            Position position,
            Position currentDirection,
            int currentAction,
            CollisionBox collisionBox,

            float attackCooldown,
            float attackRange,
            float attackOffset)
        {
            ValidateMoveSpeed(moveSpeed);
            ValidatePosition(position);
            ValidatePosition(currentDirection);

            MoveSpeed = moveSpeed;
            Position = position ?? new Position(0, 0);
            CurrentDirection = currentDirection;
            CurrentAction = currentAction;
            CollisionBox = collisionBox;

            Combat = new CombatState(
                attackCooldown,
                attackRange,
                attackOffset);

            Combat.OnHitStunFinished = () =>
            {
                if (CurrentAction == (int)EntityAction.DAMAGED)
                    CurrentAction = (int)EntityAction.IDLE;
            };
        }

        #region Methods
        internal void ApplyInput(Position inputDirection, EntityAction inputAction, float deltaTime)
        {
            var dir = inputDirection.Normalized();
            var action = ValidateAction(inputAction);

            CurrentAction = (int)action;

            if (action == EntityAction.ATTACK)
            {
                if (!dir.IsZero())
                    CurrentDirection = dir;
                Combat.TryStartAttack();
                return;
            }

            if (action == EntityAction.IDLE || dir.IsZero())
                return;

            CurrentDirection = dir;

            var next = Position.Add(dir.Multiply(MoveSpeed * deltaTime));
            var nextBox = CollisionBox.MovedTo(next);

            if (!CollisionMap.IsBlocked(nextBox))
            {
                Position = next;
                CollisionBox = nextBox;
            }

            // slide X
            var tryX = new Position(next.X, Position.Y);
            var tryXBox = CollisionBox.MovedTo(tryX);
            if (!CollisionMap.IsBlocked(tryXBox))
            {
                Position = tryX;
                CollisionBox = tryXBox;
            }

            // slide Y
            var tryY = new Position(Position.X, next.Y);
            var tryYBox = CollisionBox.MovedTo(tryY);
            if (!CollisionMap.IsBlocked(tryYBox))
            {
                Position = tryY;
                CollisionBox = tryYBox;
            }
        }

        internal void UpdateMoveSpeed(float moveSpeed)
        {
            ValidateMoveSpeed(moveSpeed);
            MoveSpeed = moveSpeed;
        }

        internal void UpdatePosition(Position position)
        {
            ValidatePosition(position);
            Position = position;
        }

        internal void UpdateDirection(Position direction)
        {
            ValidatePosition(direction);
            CurrentDirection = direction;
        }

        internal void UpdateAction(int action)
        {
            CurrentAction = action;
        }

        internal void TickCombat(float deltaTime)
        {
            Combat.Tick(deltaTime);
        }

        internal bool CanHit(Position targetPosition)
        {
            return Combat.CanHit(
                Position,
                CurrentDirection,
                targetPosition);
        }

        internal void ApplyHitStunned(float duration)
        {
            Combat.ApplyHitStunned(duration);
        }

        internal void ApplyKnockbacked(
            Position force,
            float deltaTime)
        {
            var next = Position.Add(force.Multiply(deltaTime));

            if (!CollisionMap.IsBlocked(CollisionBox))
            {
                Position = next;
            }

            CurrentAction = (int)EntityAction.DAMAGED;
        }
        #endregion

        #region Private Helpers
        private EntityAction ValidateAction(EntityAction action)
        {
            return action switch
            {
                EntityAction.RUN => EntityAction.RUN,
                EntityAction.ATTACK => EntityAction.ATTACK,
                _ => EntityAction.IDLE
            };
        }

        private static void ValidateMoveSpeed(float speed)
        {
            if (speed < 0)
                throw new PlayerAggregateException("Move speed cannot be negative");
        }

        private static void ValidatePosition(Position position)
        {
            if (position == null)
                throw new PlayerAggregateException("Position nor current direction cannot be null");
        }
        #endregion
    }
}
