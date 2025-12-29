using Domain.DomainException;
using Domain.ObjectValue;

namespace Domain.Entity
{
    public class PlayerAppearance
    {
        #region Attributes
        #endregion

        #region Properties
        public bool IsCreated { get; private set; } = false;

        public string HairID { get; private set; }
        public string GlassesID { get; private set; }
        public string ShirtID { get; private set; }
        public string PantID { get; private set; }
        public string ShoeID { get; private set; }
        public string EyesID { get; private set; }
        public string SkinID { get; private set; }

        public HSV HairColor { get; private set; }
        public HSV PantColor { get; private set; }
        public HSV EyeColor { get; private set; }
        public HSV SkinColor { get; private set; }
        #endregion

        public PlayerAppearance(
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
            ValidatePartId(hairId, "HairID");
            ValidatePartId(glassesId, "GlassesID");
            ValidatePartId(shirtId, "ShirtID");
            ValidatePartId(pantId, "PantID");
            ValidatePartId(shoeId, "ShoeID");
            ValidatePartId(eyesId, "EyesID");
            ValidatePartId(skinId, "SkinID");

            HairID = hairId;
            GlassesID = glassesId;
            ShirtID = shirtId;
            PantID = pantId;
            ShoeID = shoeId;
            EyesID = eyesId;
            SkinID = skinId;

            HairColor = hairColor;
            PantColor = pantColor;
            EyeColor = eyeColor;
            SkinColor = skinColor;

            IsCreated = true;
        }

        #region Methods
        internal void UpdateHair(string hairId, HSV color)
        {
            ValidatePartId(hairId, "HairID");
            HairID = hairId;
            HairColor = color;
        }

        internal void UpdateGlasses(string glassesId)
        {
            ValidatePartId(glassesId, "GlassesID");
            GlassesID = glassesId;
        }

        internal void UpdateShirt(string shirtId)
        {
            ValidatePartId(shirtId, "ShirtID");
            ShirtID = shirtId;
        }

        internal void UpdatePant(string pantId, HSV color)
        {
            ValidatePartId(pantId, "PantID");
            PantID = pantId;
            PantColor = color;
        }

        internal void UpdateShoe(string shoeId)
        {
            ValidatePartId(shoeId, "ShoeID");
            ShoeID = shoeId;
        }

        internal void UpdateEyes(string eyesId, HSV color)
        {
            ValidatePartId(eyesId, "EyesID");
            EyesID = eyesId;
            EyeColor = color;
        }

        internal void UpdateSkin(string skinId, HSV color)
        {
            ValidatePartId(skinId, "SkinID");
            SkinID = skinId;
            SkinColor = color;
        }
        #endregion

        #region Private Helpers
        private void ValidatePartId(string id, string fieldName)
        {
            if (string.IsNullOrEmpty(id))
                throw new PlayerAggregateException(
                    $"{fieldName} cannot be null or empty");
        }
        #endregion
    }
}
