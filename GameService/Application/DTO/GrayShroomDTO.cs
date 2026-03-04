using Application.Common;

namespace Application.DTO
{
    // Request

    // Response
    public class GrayShroomEntityDTO
    {
        public Guid ID { get; set; } = Guid.Empty;
        public Act Act { get; set; } = new Act();
        public Health Health { get; set; } = new Health();
    }

    public class GrayShroomEntityActDTO
    {
        public Guid ID { get; set; } = Guid.Empty;
        public Act Act { get; set; } = new Act();
    }
}
