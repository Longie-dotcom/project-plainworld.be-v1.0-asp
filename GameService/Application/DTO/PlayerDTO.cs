using Application.Common;

namespace Application.DTO
{
    // Request DTO
    public class PlayerMoveDTO
    {
        public PlayerMovement Movement { get; set; } = new PlayerMovement();
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
        public PlayerMovement Movement { get; set; } = new PlayerMovement();
        public PlayerAppearance Appearance { get; set; } = new PlayerAppearance();
    }

    public class PlayerEntityDTO
    {
        public Guid ID { get; set; } = Guid.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public PlayerMovement Movement { get; set; } = new PlayerMovement();
        public PlayerAppearance Appearance { get; set; } = new PlayerAppearance();
    }

    public class PlayerMovementDTO
    {
        public Guid ID { get; set; } = Guid.Empty;
        public PlayerMovement Movement { get; set; } = new PlayerMovement();
    }

    public class PlayerEntityMovementDTO
    {
        public Guid ID { get; set; } = Guid.Empty;
        public PlayerMovement Movement { get; set; } = new PlayerMovement();
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
