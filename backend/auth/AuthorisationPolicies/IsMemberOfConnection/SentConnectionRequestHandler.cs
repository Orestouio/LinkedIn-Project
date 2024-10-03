
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BackendApp.Service;
using Microsoft.AspNetCore.Authorization;

namespace BackendApp.auth.Filters;

public class IsMemberOfConnectionHandler
(IHttpContextAccessor httpContextAccessor, IRegularUserService userService, IConnectionService connectionService)
: AuthorizationHandler<IsMemberOfConnectionRequirement>
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly IRegularUserService userService = userService;
    private readonly IConnectionService connectionService = connectionService;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        IsMemberOfConnectionRequirement requirement
    )
    {
        if(context.User.HasClaim(ClaimTypes.Role, "admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        var notificationIdString = httpContextAccessor.HttpContext?
            .GetRouteData()
            .Values[requirement.ConnectionIdParamName]
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
        var connectionRequest = this.connectionService.GetConnectionById(long.Parse(notificationIdString));

        if(connectionRequest is not null && (connectionRequest.SentBy == user || connectionRequest.SentTo == user))
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