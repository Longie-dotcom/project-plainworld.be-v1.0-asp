using Application.Interface.GameEventPublisher;
using AutoMapper;
using Domain.Enum;
using Domain.Interface.IInMemory;
using Infrastructure.Background.System;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Background
{
    public class WorldLoop : BackgroundService
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly IGameEventPublisher publisher;
        private readonly IInMemoryPlayerState players;
        private readonly IInMemoryGrayShroomState shrooms;
        private readonly CombatSystem combat;
        private readonly SpawnSystem spawn;
        private readonly BehaviourSystem behaviour;
        #endregion

        #region Properties
        #endregion

        public WorldLoop(
            IMapper mapper,
            IGameEventPublisher publisher,
            IInMemoryPlayerState players,
            IInMemoryGrayShroomState shrooms)
        {
            this.mapper = mapper;
            this.publisher = publisher;
            this.players = players;
            this.shrooms = shrooms;
            combat = new CombatSystem();
            spawn = new SpawnSystem();
            behaviour = new BehaviourSystem();
        }

        #region Methods
        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var start = DateTime.UtcNow;

                await Tick(SystemConfig.Tick);

                var elapsed = (DateTime.UtcNow - start).TotalSeconds;
                var delay = Math.Max(0, SystemConfig.Tick - elapsed);

                await Task.Delay(TimeSpan.FromSeconds(delay), ct);
            }
        }

        private async Task Tick(float deltaTime)
        {
            // Spawn
            var spawned = spawn.TrySpawn(mapper, shrooms);
            if (spawned != null)
                await publisher.SpawnAsync(spawned);

            // Behaviour
            foreach (var act in behaviour.Tick(mapper, shrooms, deltaTime))
                await publisher.ActAsync(act);

            // Combat
            var result = combat.Resolve(deltaTime, mapper, players, shrooms);

            foreach (var act in result.Acted)
                await publisher.ActAsync(act);

            foreach (var id in result.Despawned)
                await publisher.DespawnAsync(id);

            // Tick
            foreach (var player in players.GetAll())
                player.TickCombat(deltaTime);

            foreach (var shroom in shrooms.GetAll())
                shroom.TickCombat(deltaTime);
        }
        #endregion
    }
}
