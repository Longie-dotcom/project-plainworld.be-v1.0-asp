namespace Domain.ObjectValue
{
    public class Position
    {
        #region Attributes
        #endregion

        #region Properties
        public static readonly Position Zero = new(0f, 0f);
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
        public bool IsZero()
        {
            return MathF.Abs(X) <= float.Epsilon &&
                  MathF.Abs(Y) <= float.Epsilon;
        }

        public Position Add(Position other)
        {
            return new(X + other.X, Y + other.Y);
        }

        public Position Multiply(float scalar)
        {
            return new(X * scalar, Y * scalar);
        }

        public Position Normalized()
        {
            var mag = MathF.Sqrt(X * X + Y * Y);
            if (mag <= float.Epsilon)
                return Zero;

            return new(X / mag, Y / mag);
        }

        public float DistanceTo(Position other)
        {
            var dx = X - other.X;
            var dy = Y - other.Y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }

        public Position Subtract(Position other)
        {
            return new(X - other.X, Y - other.Y);
        }
        #endregion
    }
}
