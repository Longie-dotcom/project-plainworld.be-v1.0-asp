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

        public PlayerMovement Movement { get; private set; }
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

        #region Movement
        public void CreateMovement(
            Position direction,
            EntityAction action,
            float deltaTime)
        {
            Movement.ApplyInput(
                direction,
                action,
                deltaTime);
        }

        public void UpdateMoveSpeed(float moveSpeed)
        {
            EnsureMovementCreated();
            Movement.UpdateMoveSpeed(moveSpeed);
        }

        public void UpdatePosition(Position position)
        {
            EnsureMovementCreated();
            Movement.UpdatePosition(position);
        }

        public void UpdateDirection(Position direction)
        {
            EnsureMovementCreated();
            Movement.UpdateDirection(direction);
        }

        public void UpdateAction(int action)
        {
            EnsureMovementCreated();
            Movement.UpdateAction(action);
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

        public void UpdateHair(string hairId, HSV color)
        {
            EnsureAppearanceCreated();
            Appearance.UpdateHair(hairId, color);
        }

        public void UpdateGlasses(string glassesId)
        {
            EnsureAppearanceCreated();
            Appearance.UpdateGlasses(glassesId);
        }

        public void UpdateShirt(string shirtId)
        {
            EnsureAppearanceCreated();
            Appearance.UpdateShirt(shirtId);
        }

        public void UpdatePant(string pantId, HSV color)
        {
            EnsureAppearanceCreated();
            Appearance.UpdatePant(pantId, color);
        }

        public void UpdateShoe(string shoeId)
        {
            EnsureAppearanceCreated();
            Appearance.UpdateShoe(shoeId);
        }

        public void UpdateEyes(string eyesId, HSV color)
        {
            EnsureAppearanceCreated();
            Appearance.UpdateEyes(eyesId, color);
        }

        public void UpdateSkin(string skinId, HSV color)
        {
            EnsureAppearanceCreated();
            Appearance.UpdateSkin(skinId, color);
        }
        #endregion
        #endregion

        #region Private Helpers
        private void EnsureMovementCreated()
        {
            if (Movement == null)
                throw new PlayerAggregateException(
                    "Player movement has not been created");
        }

        private void EnsureAppearanceCreated()
        {
            if (Appearance == null || !Appearance.IsCreated)
                throw new PlayerAggregateException(
                    "Player appearance has not been created");
        }

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
