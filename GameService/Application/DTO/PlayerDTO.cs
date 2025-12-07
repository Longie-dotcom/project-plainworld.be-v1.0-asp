using Application.Common;

namespace Application.DTO
{
    public class PlayerDTO
    {
        public Guid ID { get; set; } = Guid.Empty;
        public string Name { get; set; } = string.Empty;
        public PositionDTO Position { get; set; } = new PositionDTO();
    }
}
