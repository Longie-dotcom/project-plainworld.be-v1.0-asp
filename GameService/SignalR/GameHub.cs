using Application.DTO;
using Application.Helper;
using Application.Interface.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalR.Helper;
using SignalR.SignalRException;

namespace SignalR
{
    [Authorize]
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
        public long Ping()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public async Task PlayerJoin()
        {
            var identity = Identity();

            ServiceLogger.Logging(
                Level.API, 
                $"Player join: {identity.UserId} - {identity.Name}");

            var player = await playerService.Join(identity.UserId);

            // Join group with player ID
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                identity.UserId.ToString());

            // Send back to caller 
            await Clients.Caller.SendAsync(
                OnReceive.OnPlayerJoin,
                player.client);
            await Clients.Caller.SendAsync(
                OnReceive.OnPlayerEntityOnline,
                player.online);

            // Broadcast to everyone else except caller
            await Clients.Others.SendAsync(
                OnReceive.OnPlayerEntityJoin,
                player.entity);
        }

        public async Task PlayerLogout()
        {
            var identity = Identity();

            ServiceLogger.Logging(
                Level.API, 
                $"Player logout: {identity.UserId} - {identity.Name}");

            var player = playerService.Logout(identity.UserId);

            // Leave group with player ID
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                identity.UserId.ToString());

            // Send back to caller 
            await Clients.Caller.SendAsync(
                OnReceive.OnPlayerLogout,
                player.client);

            // Broadcast to everyone else except caller
            await Clients.Others.SendAsync(
                OnReceive.OnPlayerEntityLogout,
                player.entity);
        }

        public async Task PlayerMove(PlayerMoveDTO dto)
        {
            var identity = Identity();

            var postion = playerService.Move(
                identity.UserId,
                dto);

            // Send back to caller
            await Clients.Caller.SendAsync(
                OnReceive.OnPlayerMove,
                postion.client);

            // Broadcast to everyone else except caller
            await Clients.Others.SendAsync(
                OnReceive.OnPlayerEntityMove,
                postion.entity);
        }

        public async Task PlayerCreateAppearance(PlayerCreateAppearanceDTO dto)
        {
            var identity = Identity();

            ServiceLogger.Logging(
                Level.API,
                $"Player create appearance: {identity.UserId} - {identity.Name}");

            var appearance = playerService.CreateAppearance(
                identity.UserId,
                dto);

            // Send back to caller
            await Clients.Caller.SendAsync(
                OnReceive.OnPlayerCreateAppearance,
                appearance.client);

            // Broadcast to everyone else except caller
            await Clients.Others.SendAsync(
                OnReceive.OnPlayerEntityCreateAppearance,
                appearance.entity);
        }
        #endregion

        #region Private Helpers
        private PlayerIdentity Identity()
        {
            if (Context.User?.Identity?.IsAuthenticated != true)
                throw new ClaimNotFound("Unauthenticated connection");

            return JwtClaimHelper.Extract(Context.User);
        }
        #endregion
    }
}
