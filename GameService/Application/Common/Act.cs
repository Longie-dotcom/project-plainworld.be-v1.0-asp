namespace Application.Common
{
    public class Act
    {
        public float MoveSpeed { get; set; }
        public PositionDTO Position { get; set; } = new PositionDTO();
        public PositionDTO CurrentDirection { get; set; } = new PositionDTO();
        public int CurrentAction { get; set; }
        public CollisionBoxDTO CollisionBox { get; set; } = new CollisionBoxDTO();
    }
}
