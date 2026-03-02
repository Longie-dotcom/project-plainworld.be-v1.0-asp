using Domain.DomainException;
using Domain.Entity;
using Domain.Enum;
using Domain.ObjectValue;

namespace Domain.Aggregate
{
    public class Player : AggregateBase
    {
        #region Attributes
        #endregion

        #region Properties
        public string Email { get; private set; }
        public string FullName { get; private set; }
        public string Gender { get; private set; }
        public DateTime Dob { get; private set; }

        public Act Act { get; private set; }
        public Health Health { get; private set; }
        public PlayerAppearance Appearance { get; private set; }
        #endregion

        protected Player() : base(Guid.Empty) { }

        public Player(
            Guid id,
            string email,
            string fullName,
            string gender,
            DateTime dob) : base(id) 
        { 
            ValidateEmail(email);
            ValidateFullName(fullName);
            ValidateGender(gender);
            ValidateDob(dob);

            Email = email;
            FullName = fullName;
            Gender = gender;
            Dob = dob;

            Act = new Act(
                PlayerConfig.PlayerDefaultMoveSpeed,
                new Position(PlayerConfig.PlayerDefaultPositionX, PlayerConfig.PlayerDefaultPositionY),
                new Position(PlayerConfig.PlayerDefaultDirectionX, PlayerConfig.PlayerDefaultDirectionY),
                (int)PlayerConfig.PlayerDefaultAction,
                new CollisionBox(
                    PlayerConfig.CollisionBoxX,
                    PlayerConfig.CollisionBoxY,
                    PlayerConfig.CollisionBoxWidth,
                    PlayerConfig.CollisionBoxHeight
                ),
                PlayerConfig.AttackCooldown,
                PlayerConfig.AttackRange,
                PlayerConfig.AttackOffset
            );

            Health = new Health(
                PlayerConfig.MaxHealth);
        }

        #region Methods
        public void UpdateEmail(string email)
        {
            ValidateEmail(email);
            Email = email;
        }

        public void UpdateFullName(string fullName)
        {
            ValidateFullName(fullName);
            FullName = fullName;
        }

        public void UpdateGender(string gender)
        {
            ValidateGender(gender);
            Gender = gender;
        }

        public void UpdateDob(DateTime dob)
        {
            ValidateDob(dob);
            Dob = dob;
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

        public bool CanHit(Position targetPosition)
        {
            return Act.CanHit(targetPosition);
        }
        #endregion

        #region Appearance
        public void CreateAppearance(
            string hairId,
            string glassesId,
            string shirtId,
            string pantId,
            string shoeId,
            string eyesId,
            string skinId,
            HSV hairColor,
            HSV pantColor,
            HSV eyeColor,
            HSV skinColor)
        {
            Appearance = new PlayerAppearance(
                hairId,
                glassesId,
                shirtId,
                pantId,
                shoeId,
                eyesId,
                skinId,
                hairColor,
                pantColor,
                eyeColor,
                skinColor);
        }
        #endregion
        #endregion

        #region Private Helpers
        private void ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new PlayerAggregateException(
                    "Email can not be null nor empty");
        }

        private void ValidateFullName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                throw new PlayerAggregateException(
                    "Full name can not be null nor empty");
        }

        private void ValidateGender(string gender)
        {
            if (string.IsNullOrEmpty(gender))
                throw new PlayerAggregateException(
                    "Gender can not be null nor empty");
        }

        private void ValidateDob(DateTime dob)
        {

        }
        #endregion
    }
}
