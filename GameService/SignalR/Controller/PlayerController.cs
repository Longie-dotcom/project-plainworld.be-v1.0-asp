using Application.Interface.IService;
using Microsoft.AspNetCore.Mvc;

namespace SignalR.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService playerService;

        public PlayerController(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
    }
}
