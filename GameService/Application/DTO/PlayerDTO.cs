using Application.Common;

namespace Application.DTO
{
    // Request DTO
    public class PlayerActsDTO
    {
        public PositionDTO Direction { get; set; } = new PositionDTO();
        public int Action { get; set; }
        public float DeltaTime { get; set; }
    }

    public class PlayerCreateAppearanceDTO
    {
        public PlayerAppearance Appearance { get; set; } = new PlayerAppearance();
    }

    // Response DTO
    public class PlayerDTO
    {
        public Guid ID { get; set; } = Guid.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime Dob { get; set; }
        public Act Act { get; set; } = new Act();
        public Health Health { get; set; } = new Health();
        public PlayerAppearance Appearance { get; set; } = new PlayerAppearance();
    }

    public class PlayerEntityDTO
    {
        public Guid ID { get; set; } = Guid.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public Act Act { get; set; } = new Act();
        public Health Health { get; set; } = new Health();
        public PlayerAppearance Appearance { get; set; } = new PlayerAppearance();
    }

    public class PlayerActDTO
    {
        public Guid ID { get; set; } = Guid.Empty;
        public Act Act { get; set; } = new Act();
    }

    public class PlayerEntityActDTO
    {
        public Guid ID { get; set; } = Guid.Empty;
        public Act Act { get; set; } = new Act();
    }

    public class PlayerAppearanceDTO
    {
        public Guid ID { get; set; } = Guid.Empty;
        public PlayerAppearance Appearance { get; set; } = new PlayerAppearance();
    }


    public class PlayerEntityAppearanceDTO
    {
        public Guid ID { get; set; } = Guid.Empty;
        public PlayerAppearance Appearance { get; set; } = new PlayerAppearance();
    }
}
