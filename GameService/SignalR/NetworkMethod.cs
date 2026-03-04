namespace SignalR
{
    public static class OnReceive
    {
        // --- Player Service ---
        public const string OnPlayerJoin = "OnPlayerJoin";
        public const string OnPlayerLogout = "OnPlayerLogout";
        public const string OnPlayerAct = "OnPlayerAct";
        public const string OnPlayerCreateAppearance = "OnPlayerCreateAppearance";
        public const string OnPlayerForcedLogout = "OnPlayerForcedLogout";
        public const string OnPlayerPickItem = "OnPlayerPickItem";
        public const string OnPlayerPlaceWorldObject = "OnPlayerPlaceWorldObject";
        public const string OnPlayerChat = "OnPlayerChat";

        // --- Entity Service ---
        public const string OnPlayerEntityJoin = "OnPlayerEntityJoin";
        public const string OnPlayerEntityLogout = "OnPlayerEntityLogout";
        public const string OnPlayerEntityAct = "OnPlayerEntityAct";
        public const string OnPlayerEntityCreateAppearance = "OnPlayerEntityCreateAppearance";
        public const string OnPlayerEntityOnline = "OnPlayerEntityOnline";

        public const string OnWorldObjectPlaced = "OnWorldObjectPlaced";

        public const string OnGrayShroomEntitySpawn = "OnGrayShroomEntitySpawn";
        public const string OnGrayShroomEntityAct = "OnGrayShroomEntityAct";
        public const string OnGrayShroomEntityDespawn = "OnGrayShroomEntityDespawn";
        public const string OnGrayShroomEntityOnline = "OnGrayShroomEntityOnline";

        // --- Console Service ---
        public const string OnPlayerEntityChat = "OnPlayerEntityChat";
    }
}
