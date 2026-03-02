using Domain.Aggregate;
using Domain.Interface.IInMemory;
using Domain.Interface.IRepository;
using System.Collections.Concurrent;

namespace Infrastructure.Temporary
{
    public class InMemoryGrayShroomState : IInMemoryGrayShroomState
    {
        #region Attributes
        private readonly ConcurrentDictionary<Guid, GrayShroom> shrooms = new();
        private readonly IUnitOfWork unitOfWork;
        #endregion

        #region Properties
        #endregion

        public InMemoryGrayShroomState(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #region Methods
        public IEnumerable<GrayShroom> GetAll()
        {
            return shrooms.Values;
        }

        public bool TryGet(Guid id, out GrayShroom shroom)
        {
            return shrooms.TryGetValue(id, out shroom);
        }

        public void Add(GrayShroom shroom)
        {
            shrooms[shroom.ID] = shroom;
        }

        public void Remove(Guid id)
        {
            shrooms.TryRemove(id, out _);
        }
        #endregion
    }
}
