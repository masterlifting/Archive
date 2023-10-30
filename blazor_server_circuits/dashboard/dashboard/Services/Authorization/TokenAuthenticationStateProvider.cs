using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace dashboard.Services.Authorization;

public class TokenAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public TokenAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsPrincipal user = new(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Authentication, "false")
        }));

        if (_httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("Authorization", out var token) == true)
        {
            token = token.ToString().Replace("Bearer ", string.Empty);

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            var preferred_username =
                (jwtToken.Claims.FirstOrDefault(x => x.Type == "preferred_username")?.Value)
                ?? throw new SecurityTokenEncryptionFailedException("preferred_username is null");

            var claims = new List<Claim>(jwtToken.Claims)
            {
                new Claim(ClaimTypes.Name, preferred_username),
                new Claim("token", token!)
            };

            var identity = new ClaimsIdentity(claims, "openid-connect");

            user = new ClaimsPrincipal(identity);

            //TODO: Only for development. We should to create an another solution for production.
            if (TestTokenHelper.UsersTokens.ContainsKey(preferred_username))
                TestTokenHelper.UsersTokens[preferred_username] = token!;
            else
                TestTokenHelper.UsersTokens.TryAdd(preferred_username, token!);
        }

        return Task.FromResult(new AuthenticationState(user));
    }
}
