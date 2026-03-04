using Domain.ObjectValue;

namespace Domain.Aggregate
{
    public class WorldObject : AggregateBase
    {
        #region Attributes
        #endregion

        #region Properties
        public string ItemID { get; private set; } // Linked to the item
        public Position Position { get; private set; }
        public CollisionBox CollisionBox { get; private set; }
        #endregion

        public WorldObject(
            Guid id,
            string itemId,
            Position position,
            CollisionBox collision) : base(id)
        {
            ItemID = itemId;
            Position = position;
            CollisionBox = collision;
        }

        #region Methods
        public void SetPosition(Position newPosition)
        {
            Position = newPosition;
        }

        public void UpdateCollision(CollisionBox newCollision)
        {
            CollisionBox = newCollision;
        }
        #endregion
    }
}