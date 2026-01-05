namespace Application.Interface.IService
{
    public interface IConnectionService
    {
        /// <summary>
        /// Registers a connection and returns the old connection if user was already online
        /// </summary>
        string? Register(Guid userId, string connectionId);
        void Unregister(Guid userId, string connectionId);
    }
}
