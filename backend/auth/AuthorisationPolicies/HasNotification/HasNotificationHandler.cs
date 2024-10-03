
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BackendApp.Service;
using Microsoft.AspNetCore.Authorization;

namespace BackendApp.auth.Filters;

public class HasNotificationHandler
(IHttpContextAccessor httpContextAccessor, IRegularUserService userService, INotificationService notificationService)
: AuthorizationHandler<HasNotificationRequirement>
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly IRegularUserService userService = userService;
    private readonly INotificationService notificationService = notificationService;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        HasNotificationRequirement requirement
    )
    {
        if(context.User.HasClaim(ClaimTypes.Role, "admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        var notificationIdString = httpContextAccessor.HttpContext?
            .GetRouteData()
            .Values[requirement.NotificationIdParamName]
            ?.ToString();

        if( notificationIdString is null )
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var userClaims = context.User.Claims;
        var userIdClaim = userClaims.FirstOrDefault( c => c.Type == ClaimTypes.Sid);
        if(userIdClaim is null)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var user = this.userService.GetUserById(long.Parse(userIdClaim.Value));
        var notification = this.notificationService.GetNotificationById(long.Parse(notificationIdString));

        if(notification is not null && notification.ToUser == user)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        else
        {
            context.Fail();
            return Task.CompletedTask;
        }
    }
}