using System.Collections.Concurrent;

using Microsoft.AspNetCore.Components.Authorization;

namespace dashboard.Services.Authorization;

public static class TestTokenHelper
{
    public static ConcurrentDictionary<string, string> UsersTokens { get; } = new();
}
public sealed class UserAuthorization
{
    private readonly AuthenticationStateProvider _authentication;
    public UserAuthorization(AuthenticationStateProvider authentication) => _authentication = authentication;

    public async Task<bool> UserIsAuthorized(CancellationToken cToken = default)
    {
        var authState = await _authentication.GetAuthenticationStateAsync();

        var isAuthorized = authState.User.Identity?.Name is not null
            && authState.User.FindFirst("token")?.Value is not null
            && authState.User.Identity!.IsAuthenticated;

        return isAuthorized;
    }

    public async Task<string> GetUserName(CancellationToken cToken = default)
    {
        var authState = await _authentication.GetAuthenticationStateAsync();

        return authState.User.Identity?.Name is null
            ? throw new UnauthorizedAccessException("The user is not authorized.")
            : authState.User.Identity!.Name;
    }

    public async Task<string> GetUserToken(CancellationToken cToken = default)
    {
        var authState = await _authentication.GetAuthenticationStateAsync();

        if (authState.User.Identity?.Name is null)
        {
            throw new UnauthorizedAccessException("The user is not authorized.");
        }
        else
        {
            var token = authState.User.FindFirst("token")?.Value;

            return token ?? throw new UnauthorizedAccessException("The user is not authorized.");
        }
    }
}
