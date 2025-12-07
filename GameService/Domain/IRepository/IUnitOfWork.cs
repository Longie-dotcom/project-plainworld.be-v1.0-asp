namespace Domain.IRepository
{
    public interface IUnitOfWork
    {
        T GetRepository<T>() where T : IRepositoryBase;

        Task BeginTransactionAsync();
        Task<int> CommitAsync();
    }

    public interface IRepositoryBase
    {

    }
}