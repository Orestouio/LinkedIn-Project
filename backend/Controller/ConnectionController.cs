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
    public class ConnectionController
    (IConnectionService connectionService, IRegularUserService userService) 
    : ControllerBase
    {
        private readonly IConnectionService connectionService = connectionService;  
        private readonly IRegularUserService userService = userService;  


        [Route("{id}")]
        [HttpDelete]
        [Authorize( IsMemberOfConnectionPolicyName )] //TODO: Add apropriate filter
        public IActionResult Delete(long id)
            => this.connectionService.RemoveConnection(id) ? this.Ok() : this.NotFound();

        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
            => this.Ok(this.connectionService.GetAllConnections());

        [Route("{id}")]
        [HttpGet]
        [Authorize]
        public IActionResult Get(long id)
        {
            var Connection = this.connectionService.GetConnectionById(id);
            return Connection is not null ? this.Ok(Connection) : this.NotFound();
        }

        [Route("send/{senderId}/{receipientId}")]
        [HttpPost]
        [Authorize( Policy = HasIdEqualToSenderIdPolicyName)]
        public IActionResult Send(uint senderId, uint receipientId)
        {
            return this.connectionService.SendConnectionRequest(senderId, receipientId) ? this.Ok() : this.NotFound();
        }
        
        [Route("accept/{id}")]
        [HttpPost]
        [Authorize( ReceivedConnectionRequestPolicyName )]
        public IActionResult Accept(uint id)
        {
            return this.connectionService.AcceptConnectionRequest(id) ? this.Ok() : this.NotFound();
        }

        [Route("decline/{id}")]
        [HttpPost]
        [Authorize( ReceivedConnectionRequestPolicyName )]
        public IActionResult Decline(uint id, RegularUser connectionReceipient)
        {
            return this.connectionService.DeclineConnectionRequest(connectionReceipient, id) ? this.Ok() : this.NotFound();
        }

        [Route("network/{id}")]
        [HttpGet]
        [Authorize]
        public IActionResult GetUsersNetwork(uint id)
        {
            if(this.userService.GetUserById(id) is not RegularUser user) return this.NotFound("User not found.");
            return this.Ok(this.connectionService.GetUsersConnectedTo(user));
        }

        [Route("sent/{userId}")]
        [HttpGet]
        [Authorize( HasIdEqualToUserIdParamPolicyName )]
        public IActionResult GetConnectionSentBy(uint userId)
        {
            if(this.userService.GetUserById(userId) is not RegularUser user) return this.NotFound("User not found.");
            return this.Ok(this.connectionService.GetConnectionRequestsSentBy(user));
        }

        [Route("received/{userId}")]
        [HttpGet]
        [Authorize( HasIdEqualToUserIdParamPolicyName )]
        public IActionResult GetConnectionReceivedBy(uint userId)
        {
            if(this.userService.GetUserById(userId) is not RegularUser user) return this.NotFound("User not found.");
            return this.Ok(this.connectionService.GetConnectionRequestsReceivedBy(user));
        }
    
    }
}