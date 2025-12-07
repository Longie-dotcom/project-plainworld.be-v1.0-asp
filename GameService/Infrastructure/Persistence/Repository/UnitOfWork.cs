using Domain.IRepository;
using Infrastructure.Persistence.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Attributes
        private readonly GameDBContext context;
        private readonly IServiceProvider provider;
        private readonly Dictionary<Type, object> repositories = new();
        private IClientSessionHandle? session;
        #endregion

        #region Properties
        #endregion

        public UnitOfWork(
            GameDBContext context,
            IServiceProvider provider)
        {
            this.context = context;
            this.provider = provider;
        }

        #region Methods
        public T GetRepository<T>() where T : IRepositoryBase
        {
            var type = typeof(T);

            if (!repositories.TryGetValue(type, out var repo))
            {
                // Resolve from DI
                repo = (IRepositoryBase)provider.GetRequiredService(type);

                // Cache it
                repositories[type] = repo;
            }

            return (T)repo;
        }

        public async Task BeginTransactionAsync()
        {
            if (session == null)
            {
                session = await context.Client.StartSessionAsync();
                session.StartTransaction();
            }
        }

        public async Task<int> CommitAsync()
        {
            if (session == null)
                throw new InvalidOperationException("Transaction has not been started.");

            try
            {
                await session.CommitTransactionAsync();
                return 1;
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                session.Dispose();
                session = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (session != null)
            {
                await session.AbortTransactionAsync();
                session.Dispose();
                session = null;
            }
        }

        public IClientSessionHandle? Session => session;
        #endregion
    }
}
