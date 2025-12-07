using Application.Helper;
using Application.Interface.IService;
using Microsoft.AspNetCore.SignalR;

namespace SignalR
{
    public class GameHub : Hub
    {
        #region Attributes
        private readonly IPlayerService playerService;
        #endregion

        #region Properties
        #endregion

        public GameHub(IPlayerService playerService)
        {
            this.playerService = playerService;
        }

        #region Methods
        public async Task PlayerJoin(Guid ID, string name)
        {
            ServiceLogger.Logging(
                Level.API, $"Player join: {ID} - {name}");

            var player = await playerService.Join(ID, name);
            await Clients.Caller.SendAsync(
                OnReceive.OnPlayerJoin, player.ID, player.Position);
        }

        public async Task PlayerMove(Guid ID, float x, float y)
        {
            ServiceLogger.Logging(
                Level.API, $"Player move: {ID} - {x}:{y}");

            var newPos = await playerService.Move(ID, x, y);
            await Clients.Group(ID.ToString()).SendAsync(
                OnReceive.OnPlayerMove, ID, newPos);
        }
        #endregion
    }
}
