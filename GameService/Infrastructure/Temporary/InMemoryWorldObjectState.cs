using Domain.Aggregate;
using Domain.Interface.IInMemory;
using System.Collections.Concurrent;

namespace Infrastructure.Temporary
{
    public class InMemoryWorldObjectState : IInMemoryWorldObjectState
    {
        #region Attributes
        private readonly ConcurrentDictionary<Guid, WorldObject> worldObjects = new();
        #endregion

        #region Methods
        /// <summary>
        /// Add a new world object to memory
        /// </summary>
        public void Add(WorldObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            worldObjects[obj.ID] = obj;
        }

        /// <summary>
        /// Try to get a world object by ID
        /// </summary>
        public bool TryGet(Guid id, out WorldObject obj)
        {
            return worldObjects.TryGetValue(id, out obj);
        }

        /// <summary>
        /// Remove a world object by ID
        /// </summary>
        public bool Remove(Guid id)
        {
            return worldObjects.TryRemove(id, out _);
        }

        /// <summary>
        /// Get all world objects currently in memory
        /// </summary>
        public IEnumerable<WorldObject> GetAll()
        {
            return worldObjects.Values.ToList();
        }
        #endregion
    }
}