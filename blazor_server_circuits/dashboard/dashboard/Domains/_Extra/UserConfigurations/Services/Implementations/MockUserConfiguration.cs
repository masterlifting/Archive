using System.Globalization;

using dashboard.Domains._Extra.UserConfigurations.Models;
using dashboard.Domains._Extra.UserConfigurations.Services.Interfaces;

namespace dashboard.Domains._Extra.UserConfigurations.Services.Implementations;

public class MockUserConfiguration : IUserConfiguration
{
    public Task<UserConfiguration> Get(string userName, CancellationToken cToken = default)
    {
        return Task.FromResult(new UserConfiguration()
        {
            UserInfo = new UserInfo()
            {
                UserName = userName,
            },
            Culture = CultureInfo.CurrentCulture
        });
    }

    public Task Set(string userName, UserConfiguration configuration, CancellationToken cToken = default)
    {
        throw new NotImplementedException();
    }
}
