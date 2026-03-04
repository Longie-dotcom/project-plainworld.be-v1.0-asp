using Application.Common;

namespace Application.DTO
{
    // Request
    public class PlayerPlaceWorldObjectDTO
    {
        public string ItemID { get; set; } = string.Empty;
        public PositionDTO Position { get; set; } = new PositionDTO();
    }

    // Response
    public class WorldObjectDTO
    {
        public Guid ID { get; set; }
        public string ItemID { get; set; } = string.Empty;
        public PositionDTO Position { get; set; } = new PositionDTO();
        public CollisionBoxDTO CollisionBox { get; set; } = new CollisionBoxDTO();
    }
}
