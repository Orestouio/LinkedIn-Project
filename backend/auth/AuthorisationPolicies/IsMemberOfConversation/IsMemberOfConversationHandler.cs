
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BackendApp.Service;
using Microsoft.AspNetCore.Authorization;

namespace BackendApp.auth.Filters;

public class IsMemberOfConversationHandler
(IHttpContextAccessor httpContextAccessor)
: AuthorizationHandler<IsMemberOfConversationRequirement>
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        IsMemberOfConversationRequirement requirement
    )
    {
        if(context.User.HasClaim(ClaimTypes.Role, "admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        var firstUserIdString = httpContextAccessor.HttpContext?
            .GetRouteData()
            .Values[requirement.FirstUserParamName]
            ?.ToString();

        var secondUserIdString = httpContextAccessor.HttpContext?
            .GetRouteData()
            .Values[requirement.SecondUserParamName]
            ?.ToString();

        if( firstUserIdString is null || secondUserIdString is null )
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
        
        if(userIdClaim.Value == firstUserIdString || userIdClaim.Value == secondUserIdString)
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