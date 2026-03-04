using Domain.ObjectValue;

namespace Domain.Enum
{
    public static class CollisionMap
    {
        private static readonly CollisionBox WorldBounds = new(-34f, -10f, 68f, 65f);
        // World bounds
        public const float MinX = -34f;
        public const float MinY = -10f;
        public const float MaxX = 34f;
        public const float MaxY = 30f;

        // Hard-coded collision boxes
        private static readonly List<CollisionBox> dynamicBoxes = new();

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

            // ===== House Walls =====
            new CollisionBox(-28.5f, 33f, 4f, 2.5f),         // HouseWall (1)
            new CollisionBox(-18.5f, 29f, 4f, 2.5f),         // HouseWall (2)
            new CollisionBox(-12.5f, 29f, 5f, 2.5f),         // HouseWall (3)
            new CollisionBox(-15.5f, 39f, 3f, 2.5f),         // HouseWall (4)
            new CollisionBox(-10.5f, 39f, 3f, 2.5f),         // HouseWall (5)
            new CollisionBox(15.5f, 37f, 4f, 2.5f),          // HouseWall (6)
            new CollisionBox(21.5f, 37f, 13f, 2.5f),         // HouseWall (7)
            new CollisionBox(-34.5f, 35.25f, 0.5f, 8f),      // HouseWall (8)
            new CollisionBox(-25f, 35.25f, 0.5f, 8f),        // HouseWall (9)
            new CollisionBox(-34f, 39.98f, 9f, 3.25f),       // HouseWall (10)
            new CollisionBox(-15.5f, 41.38f, 0.5f, 7.75f),   // HouseWall (11)
            new CollisionBox(-8f, 41.38f, 0.5f, 7.75f),      // HouseWall (12)
            new CollisionBox(-15f, 45.98f, 7f, 3.25f),       // HouseWall (13)
            new CollisionBox(-18.5f, 31.38f, 0.5f, 4.75f),   // HouseWall (14)
            new CollisionBox(-8f, 31.38f, 0.5f, 4.75f),      // HouseWall (15)
            new CollisionBox(-18f, 32.98f, 10f, 3.25f),      // HouseWall (16)
            new CollisionBox(15.48f, 39.38f, 0.5f, 7.75f),   // HouseWall (17)
            new CollisionBox(34f, 39.38f, 0.5f, 7.75f),      // HouseWall (18)
            new CollisionBox(16f, 43.98f, 18f, 3.25f),       // HouseWall (19)

            new CollisionBox(-34.5f, 33f, 4f, 2.5f),         // HouseWall (Base)
            new CollisionBox(-1.75f, 35f, 12.5f, 7f),        // Pond
        };

        /// <summary>
        /// Adds a new collision box dynamically
        /// </summary>
        public static void AddCollision(CollisionBox box)
        {
            if (box == null) return;
            dynamicBoxes.Add(box);
        }

        /// <summary>
        /// Checks if a box is blocked (includes dynamic collisions)
        /// </summary>
        public static bool IsBlocked(CollisionBox actorBox)
        {
            if (!actorBox.Intersects(WorldBounds)) return true;

            // Check static boxes
            foreach (var box in Boxes)
                if (actorBox.Intersects(box)) return true;

            // Check dynamic boxes
            foreach (var box in dynamicBoxes)
                if (actorBox.Intersects(box)) return true;

            return false;
        }
    }
}
