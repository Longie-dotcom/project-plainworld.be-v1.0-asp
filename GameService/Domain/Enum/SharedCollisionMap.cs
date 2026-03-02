using Domain.ObjectValue;

namespace Domain.Enum
{
    public static class CollisionMap
    {
        private static readonly CollisionBox WorldBounds = new(-34f, -10f, 68f, 40f);
        // World bounds
        public const float MinX = -34f;
        public const float MinY = -10f;
        public const float MaxX = 34f;
        public const float MaxY = 30f;

        // Hard-coded collision boxes
        private static readonly CollisionBox[] Boxes =
        {
            new CollisionBox(-7f, 21.45f, 3f, 1.5f),    // Ruin (-4 - -7 = 3, 22.95 - 21.45 = 1.5)
            new CollisionBox(-7f, 7.45f, 3f, 1.5f),     // Ruin (1)
            new CollisionBox(-10.25f, 17.25f, 1.5f, 1.5f), // Ruin (2)
            new CollisionBox(5f, 21.25f, 3f, 1.5f),     // Ruin (3)
            new CollisionBox(5f, 7.45f, 3f, 1.5f),      // Ruin (4)
            new CollisionBox(9.75f, 17.25f, 1.5f, 1.5f), // Ruin (5)
            new CollisionBox(9.75f, 11.25f, 1.5f, 1.5f), // Ruin (6)
            new CollisionBox(-10.25f, 11.25f, 1.5f, 1.5f), // Ruin (7)
            new CollisionBox(-2f, 13f, 5f, 2f),         // Big Tree (3 - (-2) = 5, 15 - 13 = 2)
            new CollisionBox(-36f, -5f, 2f, 2f),        // WoodFence
            new CollisionBox(-35f, -6f, 5f, 2f),        // WoodFence (1)
            new CollisionBox(-31f, -5f, 21f, 2f),       // WoodFence (2)
            new CollisionBox(-11f, -6f, 8f, 2f),        // WoodFence (3)
            new CollisionBox(-4f, -5f, 1f, 2f),         // WoodFence (4)
            new CollisionBox(4f, -5f, 1f, 2f),          // WoodFence (5)
            new CollisionBox(4f, -6f, 16f, 2f),         // WoodFence (6)
            new CollisionBox(19f, -5f, 11f, 2f),        // WoodFence (7)
            new CollisionBox(29f, -6f, 5f, 2f),         // WoodFence (8)
            new CollisionBox(33f, -5f, 3f, 2f),         // WoodFence (9)
            new CollisionBox(-3f, -15.5f, 1.5f, 11f),   // Bridge (-1.5 - -3 = 1.5, -4.5 - -15.5 = 11)
            new CollisionBox(2.5f, -15.5f, 1.5f, 11f),  // Bridge (1)
            new CollisionBox(-1.5f, -8.5f, 4f, 2f),     // Bridge (2)
            new CollisionBox(-28.93f, 2.55f, 1.5f, 1.5f), // Altar (Pillar 1)
            new CollisionBox(-30.70f, 6.15f, 1.5f, 1.5f), // Altar (Pillar 2)
            new CollisionBox(-23.87f, 6.15f, 1.5f, 1.5f), // Altar (Pillar 3)
            new CollisionBox(-25.65f, 2.35f, 1.5f, 1.5f), // Altar (Pillar 4)
            new CollisionBox(-27.22f, 2.35f, 1.5f, 1.5f), // Altar (Ruin)
            new CollisionBox(-27.22f, 2.35f, 1.5f, 1.5f), // Altar (Ruin duplicate)
            new CollisionBox(-27.30f, 5.24f, 1.5f, 1.5f), // Altar (Table)
        };

        public static bool IsBlocked(CollisionBox actorBox)
        {
            if (!actorBox.Intersects(WorldBounds)) return true;
            foreach (var box in Boxes)
                if (actorBox.Intersects(box)) return true;
            return false;
        }
    }
}
