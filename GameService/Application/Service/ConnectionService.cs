using Application.Interface.IService;
using Domain.Interface.IInMemory;

namespace Application.Service
{
    public class ConnectionService : IConnectionService
    {
        #region Attributes
        private readonly IInMemoryConnectionState inMemoryConnectionState;
        #endregion

        #region Properties
        #endregion

        public ConnectionService(IInMemoryConnectionState inMemoryConnectionState)
        {
            this.inMemoryConnectionState = inMemoryConnectionState;
        }

        #region Methods
        public string? Register(Guid userId, string connectionId)
        {
            return inMemoryConnectionState.TakeOver(userId, connectionId);
        }

        public void Unregister(Guid userId, string connectionId)
        {
            inMemoryConnectionState.Remove(userId, connectionId);
        }
        #endregion
    }
}
