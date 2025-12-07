using Domain.ObjectValue;

namespace Domain.Aggregate
{
    public class Player : AggregateBase
    {
        #region Attributes
        #endregion

        #region Properties
        public string Name { get; private set; } = string.Empty;
        public Position Position { get; private set; }
        #endregion

        public Player(
            Guid id,
            string name,
            float x, float y) : base(id) 
        { 
            Name = name;
            Position = new Position(x, y);
        }

        #region Methods
        #endregion
    }
}
