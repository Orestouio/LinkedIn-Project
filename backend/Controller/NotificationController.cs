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
    public class NotificationController
    (INotificationService notifService) 
    : ControllerBase
    {
        private readonly INotificationService notificationService = notifService;  
        
        [Route("{id}")]
        [HttpDelete]
        [Authorize( HasNotificationPolicyName )]
        [ProducesResponseType<Notification>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(long id)
        {
            return this.notificationService.RemoveNotifications(id) ? this.Ok() : this.NotFound();
        }

        [HttpGet]
        [Authorize( IsAdminPolicyName )]
        [ProducesResponseType<Notification>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetAll()
        {
            return this.Ok(this.notificationService.GetAllNotifications());
        }

        [Route("{id}")]
        [HttpGet]
        [Authorize( HasNotificationPolicyName )]
        [ProducesResponseType<Notification>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(long id)
        {
            var notification = this.notificationService.GetNotificationById(id);
            return notification is not null ? this.Ok(notification) : this.NotFound();
        }

        [Route("my/{id}")]
        [HttpGet]
        [Authorize( Policy = HasIdEqualToIdParamPolicyName )]
        [ProducesResponseType<Notification[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUsersNotifications(long id)
        {
            return this.Ok(this.notificationService.GetNotificationsForUser(id));
        }

        [Route("read/{id}")]
        [HttpPost]
        [Authorize( HasNotificationPolicyName )]
        [ProducesResponseType<Notification[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult MarkNotificationAsRead(long id)
        {
            return this.notificationService.MarkNotificationAsRead(id) ? this.Ok() : this.NotFound();
        }
    }
}