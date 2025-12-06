using Application.DTO;
using Application.Interface.IService;
using FSA.LaboratoryManagement.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PrivilegesController : ControllerBase
    {
        #region Attributes
        private readonly IPrivilegeService privilegeService;
        #endregion

        #region Properties
        #endregion

        public PrivilegesController(IPrivilegeService privilegeService)
        {
            this.privilegeService = privilegeService;
        }

        #region Methods
        [AuthorizePrivilege("ViewPrivilege")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrivilegeDTO>>> GetAllPrivileges()
        {
            var Privileges = await privilegeService.GetPrivilegesAsync();
            return Ok(Privileges);
        }

        [AuthorizePrivilege("ViewPrivilege")]
        [HttpGet("{privilegeId:guid}")]
        public async Task<ActionResult<PrivilegeDTO>> GetPrivilegeById(
            Guid privilegeId)
        {
            var Privilege = await privilegeService.GetPrivilegeByIdAsync(privilegeId);
            return Ok(Privilege);
        }

        [AuthorizePrivilege("CreatePrivilege")]
        [HttpPost]
        public async Task<ActionResult> CreatePrivilege(
            [FromBody] PrivilegeCreateDTO dto)
        {
            var Privilege = await privilegeService.CreatePrivilegeAsync(dto);
            return Ok("Privileges created successfully");
        }

        [AuthorizePrivilege("UpdatePrivilege")]
        [HttpPut("{privilegeId:guid}")]
        public async Task<ActionResult> UpdatePrivilege(
            Guid privilegeId, [FromBody] PrivilegeUpdateDTO dto)
        {
            var updated = await privilegeService.UpdatePrivilegeAsync(privilegeId, dto);
            return Ok("Privileges updated successfully");
        }

        [AuthorizePrivilege("DeletePrivilege")]
        [HttpDelete("{privilegeId:guid}")]
        public async Task<IActionResult> DeletePrivilege(
            Guid privilegeId, [FromBody] UserDeleteDTO dto)
        {
            await privilegeService.DeletePrivilegeAsync(privilegeId, dto);
            return Ok("Privilege deleted successfully");
        }
        #endregion
    }
}
