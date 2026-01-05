using Domain.DomainException;
using Domain.Enum;
using Domain.ObjectValue;

namespace Domain.Entity
{
    public class PlayerMovement
    {
        #region Attributes
        #endregion

        #region Properties
        public float MoveSpeed { get; private set; }
        public Position Position { get; private set; }
        public Position CurrentDirection { get; private set; }
        public int CurrentAction { get; private set; }
        #endregion

        public PlayerMovement(
            float moveSpeed,
            Position position,
            Position currentDirection,
            int currentAction)
        {
            ValidateMoveSpeed(moveSpeed);
            ValidatePosition(position);
            ValidatePosition(currentDirection);

            MoveSpeed = moveSpeed;
            Position = position ?? new Position(0, 0);
            CurrentDirection = currentDirection;
            CurrentAction = currentAction;
        }

        #region Methods
        internal void ApplyInput(
            Position inputDirection,
            EntityAction inputAction,
            float deltaTime)
        {
            var dir = inputDirection.Normalized();
            var action = ValidateAction(inputAction);

            CurrentDirection = dir;
            CurrentAction = (int)action;

            if (action == EntityAction.RUN && !dir.IsZero())
            {
                Position = Position.Add(
                    dir.Multiply(MoveSpeed * deltaTime)
                );
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
        #endregion

        #region Private Helpers
        private EntityAction ValidateAction(EntityAction action)
        { 
            return action == EntityAction.RUN ? EntityAction.RUN : EntityAction.IDLE;
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
