using Domain.Aggregate;

namespace Domain.Interface.IRepository
{
    public interface IPlayerRepository : 
        IGenericRepository<Player>,
        IRepositoryBase
    {
    }
}
