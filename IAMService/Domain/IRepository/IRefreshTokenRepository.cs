namespace Domain.IRepository
{
    public interface IRefreshTokenRepository : IRepositoryBase
    {
        Task<string> GetByTokenAsync(string token);

        Task<string> AddTokenAsync(Guid id);
        Task DeleteTokenAsync(Guid id);
    }
}
