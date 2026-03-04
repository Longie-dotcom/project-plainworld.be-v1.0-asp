namespace Domain.Enum
{
    public static class SystemConfig
    {
        public const float Tick = 1f / 20f;
    }

    public static class PlayerConfig
    {
        public const float PlayerDefaultMoveSpeed = 10f;
        public const float PlayerDefaultPositionX = 0f;
        public const float PlayerDefaultPositionY = 0f;
        public const float PlayerDefaultDirectionX = 0f;
        public const float PlayerDefaultDirectionY = 0f;
        public const EntityAction PlayerDefaultAction = EntityAction.IDLE;

        public const float CollisionBoxX = PlayerDefaultPositionX - 0.5f;
        public const float CollisionBoxY = PlayerDefaultPositionY - 0.5f;
        public const float CollisionBoxWidth = 1f;
        public const float CollisionBoxHeight = 1f;

        public const int AttackDamage = 10;
        public const float KnockbackStrength = 16f;
        public const float AttackRange = 1.2f;
        public const float AttackOffset = 0.8f;

        public const float AttackStunDuration = SystemConfig.Tick * 2;
        public const float AttackCooldown = SystemConfig.Tick * 2;
        public const float AttackWindow = SystemConfig.Tick;

        //// Health
        public const int MaxHealth = 80;
    }

    public static class GrayShroomConfig
    {
        public const float MoveSpeed = 5f;
        public const float SpawnMinX = 0f;
        public const float SpawnMaxX = 10f;
        public const float SpawnMinY = 0f;
        public const float SpawnMaxY = 10f;

        public const float CollisionBoxX = 0;
        public const float CollisionBoxY = 0;
        public const float CollisionBoxWidth = 1f;
        public const float CollisionBoxHeight = 1f;

        public const float AttackCooldown = 0.35f;
        public const float AttackRange = 1.2f;
        public const float AttackOffset = 0.8f;

        ////// Behaviour
        public const float WanderCooldownMin = 0.15f;
        public const float WanderCooldownMax = 0.35f;

        ////// Spawn
        public const int MaxSpawnAttempts = 20;
        public const int MaxAlive = 10;

        ////// Health
        public const int MaxHealth = 80;
    }

    public static class ItemConfig
    {
        public const string Mushroom = "ITEM_MUSHROOM";
        public const string Stone = "ITEM_STONE";
        public const string Wood = "ITEM_WOOD";
        public const string Chair = "ITEM_CHAIR";
        public const string Table = "ITEM_TABLE";
        public const string Furnace = "ITEM_FURNACE";
        public const string Picture = "ITEM_PICTURE";
    }
}
