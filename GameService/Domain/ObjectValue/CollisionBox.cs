namespace Domain.ObjectValue
{
    public class CollisionBox
    {
        #region Properties
        public float MinX { get; private set; }
        public float MinY { get; private set; }
        public float MaxX { get; private set; }
        public float MaxY { get; private set; }

        public float Width => MaxX - MinX;
        public float Height => MaxY - MinY;
        #endregion

        // Constructor: bottom-left + size
        public CollisionBox(float minX, float minY, float width, float height)
        {
            MinX = minX;
            MinY = minY;
            MaxX = minX + width;
            MaxY = minY + height;
        }

        // Constructor: Position (bottom-left) + size
        public CollisionBox(Position bottomLeft, float width, float height)
            : this(bottomLeft.X, bottomLeft.Y, width, height) { }

        #region Methods

        // Absolute move: returns a new box at this bottom-left position
        public CollisionBox MovedTo(Position center)
        {
            float halfWidth = Width / 2f;
            float halfHeight = Height / 2f;

            return new CollisionBox(
                center.X - halfWidth,   // MinX
                center.Y - halfHeight,  // MinY
                Width,
                Height
            );
        }

        // Offset move: keeps previous logic if needed
        public CollisionBox Translate(Position offset)
        {
            return new CollisionBox(MinX + offset.X, MinY + offset.Y, Width, Height);
        }

        public bool Intersects(CollisionBox other)
        {
            return MaxX > other.MinX &&
                   MinX < other.MaxX &&
                   MaxY > other.MinY &&
                   MinY < other.MaxY;
        }

        public bool Contains(Position point)
        {
            return point.X >= MinX && point.X <= MaxX &&
                   point.Y >= MinY && point.Y <= MaxY;
        }
        #endregion
    }
}