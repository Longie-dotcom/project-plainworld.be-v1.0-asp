namespace Application.Common
{
    public class PlayerAppearance
    {
        public bool IsCreated { get; set; }

        public string HairID { get; set; } = string.Empty;
        public string GlassesID { get; set; } = string.Empty;
        public string ShirtID { get; set; } = string.Empty;
        public string PantID { get; set; } = string.Empty;
        public string ShoeID { get; set; } = string.Empty;
        public string EyesID { get; set; } = string.Empty;
        public string SkinID { get; set; } = string.Empty;

        public HSVDTO HairColor { get; set; } = new HSVDTO();
        public HSVDTO PantColor { get; set; } = new HSVDTO();
        public HSVDTO EyeColor { get; set; } = new HSVDTO();
        public HSVDTO SkinColor { get; set; } = new HSVDTO();
    }
}
