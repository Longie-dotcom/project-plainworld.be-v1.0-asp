
using Domain.Aggregate;

namespace Domain.Interface.IInMemory
{
    public interface IInMemoryWorldObjectState
    {
        void Add(WorldObject obj);

        bool TryGet(Guid id, out WorldObject obj);

        bool Remove(Guid id);

        IEnumerable<WorldObject> GetAll();
    }
}
