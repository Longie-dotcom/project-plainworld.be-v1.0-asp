using Domain.Interface.IInMemory;
using System.Collections.Concurrent;

namespace Infrastructure.Temporary
{
    public class InMemoryConnectionState : IInMemoryConnectionState
    {
        #region Attributes
        private readonly ConcurrentDictionary<Guid, string> connections = new();

        #endregion

        #region Properties
        public IReadOnlyDictionary<Guid, string> Snapshot
        {
            get { return connections.ToDictionary(x => x.Key, x => x.Value); }
        }
        #endregion

        public InMemoryConnectionState() { }

        #region Methods
        /// <summary>
        /// New connection always wins.
        /// </summary>
        public string? TakeOver(Guid userId, string connectionId)
        {
            if (connections.TryGetValue(userId, out var oldConnectionId))
            {
                connections[userId] = connectionId;
                return oldConnectionId;
            }

            connections[userId] = connectionId;
            return null;
        }

        public bool Remove(Guid userId, string connectionId)
        {
            if (connections.TryGetValue(userId, out var existing)
                && existing == connectionId)
            {
                return connections.TryRemove(userId, out _);
            }

            return false;
        }

        public bool IsOwner(Guid userId, string connectionId)
        {
            return connections.TryGetValue(userId, out var existing)
                   && existing == connectionId;
        }
        #endregion
    }
}
