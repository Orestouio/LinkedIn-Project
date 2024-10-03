

using BackendApp.Model.Enums;

namespace BackendApp.Auth;

public class TokenResponse
(string token, long id, string role)
{
    public string Token { get; set; } = token;
    public long Id { get; set; } = id;
    public string Role { get; set; } = role;
}