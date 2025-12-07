namespace Domain.ObjectValue
{
    public class Position
    {
        #region Attributes
        #endregion

        #region Properties
        public float X { get; private set; }
        public float Y { get; private set; }
        #endregion

        public Position(
            float x,
            float y) 
        {
            X = x;
            Y = y;
        }

        #region Methods
        public void UpdatePosition(float x, float y)
        {
            X = x;
            Y = y;
        }
        #endregion
    }
}
