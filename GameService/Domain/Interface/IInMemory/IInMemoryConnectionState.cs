namespace Domain.Interface.IInMemory
{
    public interface IInMemoryConnectionState
    {
        /// <summary>
        /// Registers a connection and takes ownership.
        /// Returns old connectionId if existed.
        /// </summary>
        string? TakeOver(Guid userId, string connectionId);

        /// <summary>
        /// Removes connection only if owned by caller.
        /// </summary>
        bool Remove(Guid userId, string connectionId);

        /// <summary>
        /// Checks ownership.
        /// </summary>
        bool IsOwner(Guid userId, string connectionId);
        string? GetConnection(Guid userId);
    }
}
