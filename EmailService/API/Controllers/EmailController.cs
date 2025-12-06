using Application.Interface;
using FSA.LaboratoryManagement.EmailMessage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        // GET: api/email
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<EmailMessageDTO>> GetEmailMessages()
        {
            var emails = emailApplication.GetEmailMessages();
            return Ok(emails);
        }

        // POST: api/email
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PublishEmail([FromBody] EmailMessageDTO message)
        {
            await emailApplication.PublishEmail(message);
            return Ok(new { Message = "Email published successfully" });
        }

        // POST: api/email/by-identity
        [HttpPost("by-identity")]
        [AllowAnonymous]
        public async Task<IActionResult> PublishEmailByIdentity([FromBody] IdentityNumberMessageDTO message)
        {
            await emailApplication.PublishEmailByIdentityNumber(message);
            return Ok(new { Message = "Email published by identity successfully" });
        }
        #endregion
    }
}
