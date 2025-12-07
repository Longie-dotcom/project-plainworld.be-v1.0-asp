using API.Helper;
using Application.DTO;
using Application.Interface.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlainWorld.Authorization;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        #region Attributes
        private readonly IRoleService roleService;
        #endregion

        #region Properties
        #endregion

        public RolesController(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        #region Methods
        [AuthorizePrivilege("ViewRole")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDTO>>> GetAllRoles()
        {
            var roles = await roleService.GetRoleListAsync();
            return Ok(roles);
        }

        [AuthorizePrivilege("ViewRole")]
        [HttpGet("{roleId:guid}")]
        public async Task<ActionResult<RoleDTO>> GetRoleById(
            Guid roleId)
        {
            var role = await roleService.GetRoleByIdAsync(roleId);
            return Ok(role);
        }

        [AuthorizePrivilege("CreateRole")]
        [HttpPost]
        public async Task<ActionResult> CreateRole(
            [FromBody] RoleCreateDTO dto)
        {
            var claims = CheckClaimHelper.CheckClaim(User);
            await roleService.CreateRoleAsync(
                dto,
                claims.userId);
            return Ok("Role created successfully");
        }

        [AuthorizePrivilege("UpdateRole")]
        [HttpPut("{roleId:guid}")]
        public async Task<ActionResult> UpdateRole(
            Guid roleId, [FromBody] RoleUpdateDTO dto)
        {
            var claims = CheckClaimHelper.CheckClaim(User);
            await roleService.UpdateRoleAsync(
                roleId, 
                dto,
                claims.userId);
            return Ok("Role updated successfully");
        }

        [AuthorizePrivilege("DeleteRole")]
        [HttpDelete("{roleId:guid}")]
        public async Task<IActionResult> DeleteRole(
            Guid roleId)
        {
            var claims = CheckClaimHelper.CheckClaim(User);
            await roleService.DeleteRoleAsync(
                roleId, 
                claims.userId);
            return Ok("Role deleted successfully");
        }
        #endregion
    }
}
