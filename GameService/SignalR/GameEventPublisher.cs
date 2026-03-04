using Application.Common;
using Application.DTO;
using Application.Interface.GameEventPublisher;
using Microsoft.AspNetCore.SignalR;

namespace SignalR
{
    public class GameEventPublisher : IGameEventPublisher
    {
        #region Attributes
        private readonly IHubContext<GameHub> hub;
        #endregion

        #region Properties
        #endregion

        public GameEventPublisher(IHubContext<GameHub> hub)
        {
            this.hub = hub;
        }

        #region Methods
        public async Task SpawnAsync(GrayShroomEntityDTO dto)
        {
            await hub.Clients.All.SendAsync(
                OnReceive.OnGrayShroomEntitySpawn,
                dto);
        }

        public async Task ActAsync(GrayShroomEntityActDTO dto)
        {
            await hub.Clients.All.SendAsync(
                OnReceive.OnGrayShroomEntityAct,
                dto);
        }

        public async Task DespawnAsync(Guid id)
        {
            await hub.Clients.All.SendAsync(
                OnReceive.OnGrayShroomEntityDespawn,
                id);
        }

        public async Task PlayerPickItemAsync(string connectionId, Item item)
        {
            await hub.Clients.Client(connectionId).SendAsync(
                OnReceive.OnPlayerPickItem,
                item
            );
        }
        #endregion
    }
}