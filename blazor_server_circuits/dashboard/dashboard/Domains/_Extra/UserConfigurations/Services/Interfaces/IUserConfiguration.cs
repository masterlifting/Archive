using dashboard.Domains._Extra.UserConfigurations.Models;

namespace dashboard.Domains._Extra.UserConfigurations.Services.Interfaces;

public interface IUserConfiguration
{
    public Task<UserConfiguration> Get(string userName, CancellationToken cToken = default);
    public Task Set(string userName, UserConfiguration configuration, CancellationToken cToken = default);
}
