namespace Application.Common
{
    public class PlayerMovement
    {
        public float MoveSpeed { get; set; }
        public PositionDTO Position { get; set; } = new PositionDTO();
        public PositionDTO CurrentDirection { get; set; } = new PositionDTO();
        public int CurrentAction { get; set; }
    }
}
