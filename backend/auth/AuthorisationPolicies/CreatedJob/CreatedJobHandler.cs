
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BackendApp.Service;
using Microsoft.AspNetCore.Authorization;

namespace BackendApp.auth.Filters;

public class CreatedJobHandler
(IHttpContextAccessor httpContextAccessor, IRegularUserService userService, IJobService jobService)
: AuthorizationHandler<CreatedJobRequirement>
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly IRegularUserService userService = userService;
    private readonly IJobService JobService = jobService;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        CreatedJobRequirement requirement
    )
    {
        if(context.User.HasClaim(ClaimTypes.Role, "admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        var notificationIdString = httpContextAccessor.HttpContext?
            .GetRouteData()
            .Values[requirement.JobIdParamName]
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
        var post = this.JobService.GetJobById(long.Parse(notificationIdString));

        if(post is not null && post.PostedBy == user)
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