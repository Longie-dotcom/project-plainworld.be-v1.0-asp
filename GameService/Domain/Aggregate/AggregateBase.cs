using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Aggregate
{
    public class AggregateBase
    {
        #region Attributes
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid ID { get; private set; } = Guid.Empty;
        #endregion

        #region Properties
        #endregion

        public AggregateBase(Guid id)
        {
            ID = id;
        }

        #region Methods
        #endregion
    }
}
