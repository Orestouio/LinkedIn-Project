using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendApp.Controllers.Filters;
using BackendApp.Model;
using BackendApp.Model.Enums;
using BackendApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static BackendApp.Auth.AuthConstants.PolicyNames;


namespace BackendApp.Controller
{
    [Route("api/[Controller]")]
    [ApiController]
    [XmlConverterFilter]
    public class MessageController(IMessageService messageService, IRegularUserService userService) : ControllerBase
    {
        private readonly IMessageService messageService = messageService;  
        private readonly IRegularUserService userService = userService;  

        [Route("{id}")]
        [HttpDelete]
        [Authorize( SentMessagePolicyName )]
        [ProducesResponseType<Message[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(long id)
            => this.messageService.RemoveMessage(id) ? this.Ok() : this.NotFound();

        [HttpGet]
        [Authorize( IsAdminPolicyName )]
        [ProducesResponseType<Message[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetAll()
            => this.Ok(this.messageService.GetAllMessages());

        [Route("{id}")]
        [HttpGet]
        [Authorize( SentMessagePolicyName )]
        [ProducesResponseType<Message>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(long id)
        {
            var Message = this.messageService.GetMessageById(id);
            return Message is not null ? this.Ok(Message) : this.NotFound();
        }

        [Route("send/{senderId}/{receipientId}")]
        [HttpPost]
        [Authorize( HasIdEqualToSenderIdPolicyName )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Send(uint senderId, uint receipientId, string content)
        {
            return this.messageService.SendMessage(senderId, receipientId, content) ? this.Ok() : this.NotFound();
        }

        [Route("chat/{userAId}/{userBId}")]
        [HttpGet]
        [Authorize( IsMemberOfConversationPolicyName )]
        [ProducesResponseType<Message[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetChatHistory(uint userAId, uint userBId) 
            => this.Ok(this.messageService.GetConversationBetween(userAId, userBId));

        [Route("chat/{userAId}/{userBId}/{skip}/{take}")]
        [HttpGet]
        [Authorize( IsMemberOfConversationPolicyName )]
        [ProducesResponseType<Message[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetChatHistory(uint userAId, uint userBId, int skip, int take) 
        {    
            var userA = this.userService.GetUserById(userAId);
            var userB = this.userService.GetUserById(userBId);
            if(userA is null || userB is null) return this.NotFound("One of the users was not found.");
            return this.Ok(this.messageService.GetRangeOfConversationBetween(userA, userB, skip, take));
        }

        [Route("chat/members/{userId}")]
        [HttpGet]
        [Authorize( HasIdEqualToUserIdParamPolicyName )]
        [ProducesResponseType<RegularUser[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUsers(long userId)
        {
            var user = this.userService.GetUserById(userId);
            if(user is null) return this.NotFound("User not found.");
            return this.Ok(this.messageService.GetMembersOfChatsWith(user));
        }
    }
}