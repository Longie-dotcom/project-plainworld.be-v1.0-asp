using Domain.Aggregate;

namespace Domain.IRepository
{
    public interface IPlayerRepository : 
        IGenericRepository<Player>,
        IRepositoryBase
    {
    }
}
