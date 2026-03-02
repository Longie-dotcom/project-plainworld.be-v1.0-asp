using Domain.Aggregate;

namespace Domain.Interface.IInMemory
{
    public interface IInMemoryGrayShroomState
    {
        IEnumerable<GrayShroom> GetAll();
        bool TryGet(Guid id, out GrayShroom shroom);

        void Add(GrayShroom shroom);
        void Remove(Guid id);
    }
}
