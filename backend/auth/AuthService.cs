
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackendApp.Data;
using BackendApp.Model;
using BackendApp.Model.Requests;
using BackendApp.Service;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using BackendApp.Auth;

namespace BackendApp.auth;

public interface IAuthenticationService
{
    public AppUser? Authenticate(TokenGenerationRequest loginRequest);
    public string GenerateToken(
        AppUser user,
        TimeSpan expiresAfter
    );

}

public class AuthenticationService 
(IRegularUserService userService, IAdminUserService adminService, IConfiguration config)
: IAuthenticationService
{
    private readonly IRegularUserService userService = userService;
    private readonly IAdminUserService adminService = adminService;
    private readonly IConfiguration configuration = config;
    public AppUser? Authenticate(TokenGenerationRequest loginRequest)
    {
        
        var adminUser = this.adminService.GetAdminByEmail(loginRequest.Email);
        if(
            adminUser is not null 
            && EncryptionUtility.HashPassword(loginRequest.Password) == adminUser.PasswordHash
        ) return adminUser;

        var user = this.userService.GetUserByEmail(loginRequest.Email);
        if(
            user is not null 
            && EncryptionUtility.HashPassword(loginRequest.Password) == user.PasswordHash
        ) return user;

        return null;
    }

    public string GenerateToken(
        AppUser user, 
        TimeSpan expiresAfter
    )
    {
        var secret =  this.configuration["Jwt:Key"] ?? throw new Exception("Token Key has not been set within config.");
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Sid, user.Id.ToString()), 
            new(ClaimTypes.Role, user is AdminUser ? "admin" : "regular"),
        };

        var token = new JwtSecurityToken(
            this.configuration["Jwt:Issuer"] ?? throw new Exception("Token Issuer has not been set within config."),
            this.configuration["Jwt:Audience"] ?? throw new Exception("Token Audience has not been set within config."),
            claims,
            expires: DateTime.Now.Add(expiresAfter),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
