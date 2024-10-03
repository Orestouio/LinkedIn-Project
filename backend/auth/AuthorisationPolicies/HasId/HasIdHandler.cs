
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BackendApp.Auth;
using Microsoft.AspNetCore.Authorization;

namespace BackendApp.auth.Filters;

public class HasIdHandler 
(IHttpContextAccessor httpContextAccessor)
: AuthorizationHandler<HasIdRequirement>
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        HasIdRequirement requirement
    )
    {
        if(context.User.HasClaim(ClaimTypes.Role, "admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        var idString = httpContextAccessor.HttpContext?
            .GetRouteData()
            .Values[requirement.IdParamName]
            ?.ToString();
            
        if( idString is null )
        {
            context.Fail();
            return Task.CompletedTask;
        }
 
        if(context.User.HasClaim(ClaimTypes.Sid, idString))
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
