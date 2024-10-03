using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BackendApp.Model;
using BackendApp.Data;
using BackendApp.Model.Requests;
using Newtonsoft.Json;
using BackendApp.Service;
using BackendApp.Model.Enums;
using BackendApp.auth;
using Microsoft.AspNetCore.Authorization;
using static BackendApp.Auth.AuthConstants.PolicyNames;
using BackendApp.Controllers.Filters;


namespace BackendApp.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [XmlConverterFilter]
    public class UserController(
        IRegularUserService userService
    ) : ControllerBase
    {
        private readonly IRegularUserService linkedOutUserService = userService;

        [HttpPost]
        [Authorize( IsAdminPolicyName )]
        [ProducesResponseType<long>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult Create(RegularUser user){
            bool added = this.linkedOutUserService.AddUser(user);
            return added ? new JsonResult(this.Ok(user.Id)) : new JsonResult(this.Conflict());
        }

        [Route("{id}")]
        [HttpPost]
        [Authorize( Policy = HasIdEqualToIdParamPolicyName )]
        [ProducesResponseType<long>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult Update(long id, RegularUser user)
        {
            return this.linkedOutUserService.Update(id, user) switch
            {
                UpdateResult.KeyAlreadyExists => new JsonResult(this.Conflict()),    
                UpdateResult.NotFound => new JsonResult(this.NotFound()),    
                UpdateResult.Ok => new JsonResult(this.Ok()),
                _ => throw new Exception("Something went terribly wrong for you to be here.")
            };
        }
        [Route("{id}")]
        [HttpDelete]
        [Authorize( Policy = HasIdEqualToIdParamPolicyName )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(long id)
            => this.linkedOutUserService.RemoveUser(id) 
            ? new JsonResult(this.Ok("User successfully deleted.")) 
            : new JsonResult(this.NotFound("User not found."));
        

        [HttpGet]
        [Authorize]
        [ProducesResponseType<RegularUser[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAll(){
            var users = this.linkedOutUserService
                .GetAllUsers()
                .Select( a => RegularUser.MapNewWithHiddenPassword(a));
            return new JsonResult(users);
        }

        [Route("{id}")]
        [HttpGet]
        [Authorize]
        [ProducesResponseType<RegularUser>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(long id){
            var user = this.linkedOutUserService.GetUserById(id);
            return new JsonResult(
                user is not null 
                ? this.Ok(user) 
                : this.NotFound()
            );
        }

        [Route("search/{searchString}")]
        [HttpGet]
        [Authorize]
        [ProducesResponseType<RegularUser[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult SearchByUsername(string searchString)
        {
            return this.Ok(this.linkedOutUserService.SearchByUsernameFuzzy(searchString));
        }

        [Route("email/{email}")]
        [HttpGet]
        [Authorize]
        [ProducesResponseType<RegularUser[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUserByEmail(string email)
        {
            var user = this.linkedOutUserService.GetUserByEmail(email);
            if(user is null) return this.NotFound("User not found.");
            return this.Ok(user);
        }
        
        [Route("{id}/change/password")]
        [HttpPost]
        [Authorize( Policy = HasIdEqualToIdParamPolicyName )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ChangePassword(long id, PasswordChangeRequest passwordChangeRequest)
        {
            var result = this.linkedOutUserService.ChangePassword(
                id, 
                passwordChangeRequest.OldPassword, 
                passwordChangeRequest.NewPassword
            );
            return result switch{
                UpdateResult.NotFound => this.NotFound("User not found."),
                UpdateResult.Unauthorised => this.BadRequest("Old password does not match password in database"),
                UpdateResult.Ok => this.Ok("Password successfully changed."),
                _ => this.StatusCode(500)
            };
        }
    }
}