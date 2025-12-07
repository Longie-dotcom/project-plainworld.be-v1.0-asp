using API.Helper;
using Application.DTO;
using Application.Interface.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlainWorld.Authorization;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        #region Attributes
        private readonly IUserService userService;
        #endregion

        #region Properties
        #endregion

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        #region Methods
        [AuthorizePrivilege("ViewUser")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers(
            [FromQuery] string sortBy,
            [FromQuery] QueryUserDTO dto)
        {
            var claims = CheckClaimHelper.CheckClaim(User);
            var users = await userService.GetUsersAsync(
                sortBy, 
                dto, 
                claims.userId,
                claims.role);
            return Ok(users);
        }

        [AuthorizePrivilege("ViewUser")]
        [HttpGet("{userId:guid}")]
        public async Task<ActionResult<UserDTO>> GetUserById(
            Guid userId)
        {
            var claims = CheckClaimHelper.CheckClaim(User);
            var user = await userService.GetUserByIdAsync(
                userId, 
                claims.userId,
                claims.role);
            return Ok(user);
        }

        [AuthorizePrivilege("CreateUser")]
        [HttpPost]
        public async Task<ActionResult> CreateUser(
            [FromBody] UserCreateDTO dto)
        {
            var claims = CheckClaimHelper.CheckClaim(User);
            await userService.CreateUserAsync(
                dto, 
                claims.userId,
                claims.role);
            return Ok("User created successfully");
        }

        [AuthorizePrivilege("ModifyUser")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateUser(
            Guid id, [FromBody] UserUpdateDTO dto)
        {
            var claims = CheckClaimHelper.CheckClaim(User);
            await userService.UpdateUserAsync(
                id, 
                dto, 
                claims.userId,
                claims.role);
            return Ok("User updated successfully");
        }

        [AuthorizePrivilege("DeleteUser")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUser(
            Guid id)
        {
            var claims = CheckClaimHelper.CheckClaim(User);
            await userService.DeleteUserAsync(
                id,
                claims.userId, 
                claims.userId,
                claims.role);
            return Ok("User deleted successfully");
        }

        [AuthorizePrivilege("ChangePassword")]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordDTO dto)
        {
            var claims = CheckClaimHelper.CheckClaim(User);
            await userService.ChangePasswordAsync(
                claims.userId, 
                dto);
            return Ok("Password changed successfully");
        }
        #endregion
    }
}
