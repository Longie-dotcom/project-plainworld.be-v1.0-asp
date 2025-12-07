using Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlainWorld.MessageBroker;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        #region Attributes
        private readonly IEmailApplication emailApplication;
        #endregion

        #region Properties
        #endregion

        public EmailController(IEmailApplication emailApplication)
        {
            this.emailApplication = emailApplication;
        }

        #region Methods
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<EmailMessageDTO>> GetEmailMessages()
        {
            var emails = emailApplication.GetEmailMessages();
            return Ok(emails);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PublishEmail(
            [FromBody] EmailMessageDTO message)
        {
            await emailApplication.PublishEmail(message);
            return Ok(new { Message = "Email published successfully" });
        }

        [HttpPost("by-identity")]
        [AllowAnonymous]
        public async Task<IActionResult> PublishEmailByIdentity(
            [FromBody] UserIDMessageDTO message)
        {
            await emailApplication.PublishEmailByIdentityNumber(message);
            return Ok(new { Message = "Email published by identity successfully" });
        }
        #endregion
    }
}
