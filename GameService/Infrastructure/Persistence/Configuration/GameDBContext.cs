using Domain.Aggregate;
using Infrastructure.Persistence.Enum;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Configuration
{
    public class GameDBContext
    {
        #region Attributes
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        #endregion

        #region Properties
        public IMongoClient Client => _client;
        public IMongoDatabase Database => _database;

        public IMongoCollection<Player> AuditLogs
            => GetCollection<Player>(CollectionName.PLAYERS);
        #endregion

        public GameDBContext(string connectionString, string databaseName)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(databaseName);
        }

        #region Methods
        public IMongoCollection<T> GetCollection<T>(string collectionName) where T : class
        {
            return _database.GetCollection<T>(collectionName);
        }
        #endregion
    }
}
