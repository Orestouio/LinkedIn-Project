
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BackendApp.auth.Filters;

public class IsAdminHandler 
(IHttpContextAccessor httpContextAccessor)
: AuthorizationHandler<IsAdminRequirement>
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        IsAdminRequirement requirement
    )
    {
        if(context.User.HasClaim(ClaimTypes.Role, "admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        context.Fail();
        return Task.CompletedTask;
        
    }
}
