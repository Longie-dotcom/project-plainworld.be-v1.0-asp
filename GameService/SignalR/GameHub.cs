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

        public GameHub(
            IPlayerService playerService)
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

            var result = await playerService.Join(
                identity.UserId,
                Context.ConnectionId);

            // Kick old connection if exists
            if (!string.IsNullOrEmpty(result.oldConnectionId)
                && result.oldConnectionId != Context.ConnectionId)
            {
                ServiceLogger.Logging(
                    Level.API,
                    $"Player {identity.UserId} was forced to be logged out from another device: {result.oldConnectionId}");

                await Clients.Client(result.oldConnectionId).SendAsync(
                    OnReceive.OnPlayerForcedLogout);

                await Groups.RemoveFromGroupAsync(
                    result.oldConnectionId,
                    identity.UserId.ToString());
            }

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                identity.UserId.ToString());

            // Send back to caller
            await Clients.Caller.SendAsync(
                OnReceive.OnPlayerEntityOnline,
                result.onlinePlayers);

            await Clients.Caller.SendAsync(
                OnReceive.OnGrayShroomEntityOnline,
                result.onlineGrayShrooms);

            await Clients.Caller.SendAsync(
                OnReceive.OnPlayerJoin,
                result.client);

            // Broadcast to everyone else except caller
            await Clients.Others.SendAsync(
                OnReceive.OnPlayerEntityJoin,
                result.entity);

            ServiceLogger.Logging(
                Level.API,
                $"Player {identity.UserId}: {Context.ConnectionId} was logged in");
        }

        public async Task PlayerLogout()
        {
            var identity = Identity();

            // Remove gameplay state (safe if already logged out)
            var playerId = playerService.Logout(
                identity.UserId, 
                Context.ConnectionId);

            if (!playerId.HasValue)
                return;

            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                identity.UserId.ToString());

            // Send back to caller
            await Clients.Caller.SendAsync(
                OnReceive.OnPlayerLogout,
                playerId.Value);

            // Broadcast to everyone else except caller
            await Clients.Others.SendAsync(
                OnReceive.OnPlayerEntityLogout,
                playerId.Value);

            ServiceLogger.Logging(
                Level.API,
                $"Player {identity.UserId} was logged out");
        }

        public async Task PlayerAct(PlayerActsDTO dto)
        {
            var identity = Identity();

            var postion = playerService.Act(
                identity.UserId,
                dto);

            // Send back to caller
            await Clients.Caller.SendAsync(
                OnReceive.OnPlayerAct,
                postion.client);

            // Broadcast to everyone else except caller
            await Clients.Others.SendAsync(
                OnReceive.OnPlayerEntityAct,
                postion.entity);
        }

        public async Task PlayerCreateAppearance(PlayerCreateAppearanceDTO dto)
        {
            var identity = Identity();

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

            ServiceLogger.Logging(
                Level.API,
                $"Player {identity.UserId} appearance was changged");
        }

        public async Task PlayerChat(ChatSendDTO dto)
        {
            var identity = Identity();

            var receive = playerService.SendChat(
                identity.UserId,
                dto);

            // Send back to caller
            await Clients.Caller.SendAsync(
                OnReceive.OnPlayerChat,
                receive);

            // Broadcast to everyone else except caller
            await Clients.Others.SendAsync(
                OnReceive.OnPlayerEntityChat,
                receive);

            ServiceLogger.Logging(
                Level.API,
                $"Player ID:{identity.UserId} - Player Name:{receive.UserName} chatted: {receive.Content}");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User?.Identity?.IsAuthenticated == true)
            {
                var identity = Identity();

                var playerId = playerService.Logout(
                    identity.UserId, 
                    Context.ConnectionId);

                if (playerId.HasValue)
                {
                    await Clients.Others.SendAsync(
                        OnReceive.OnPlayerEntityLogout, 
                        playerId.Value);
                }
            }

            await base.OnDisconnectedAsync(exception);
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
