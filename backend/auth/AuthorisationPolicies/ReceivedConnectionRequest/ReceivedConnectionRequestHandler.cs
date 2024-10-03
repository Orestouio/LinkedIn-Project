
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BackendApp.Service;
using Microsoft.AspNetCore.Authorization;

namespace BackendApp.auth.Filters;

public class ReceivedConnectionRequestHandler
(IHttpContextAccessor httpContextAccessor, IRegularUserService userService, IConnectionService connectionService)
: AuthorizationHandler<ReceivedConnectionRequestRequirement>
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly IRegularUserService userService = userService;
    private readonly IConnectionService connectionService = connectionService;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        ReceivedConnectionRequestRequirement requirement
    )
    {
        if(context.User.HasClaim(ClaimTypes.Role, "admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        var connectionIdString = httpContextAccessor.HttpContext?
            .GetRouteData()
            .Values[requirement.ConnectionIdParamName]
            ?.ToString();

        if( connectionIdString is null )
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
        var connectionRequest = this.connectionService.GetConnectionById(long.Parse(connectionIdString));

        if(connectionRequest is not null && connectionRequest.SentTo == user)
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