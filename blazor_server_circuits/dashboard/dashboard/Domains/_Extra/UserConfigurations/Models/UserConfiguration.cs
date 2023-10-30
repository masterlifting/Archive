using System.Globalization;

namespace dashboard.Domains._Extra.UserConfigurations.Models;

public sealed record UserConfiguration
{
    public UserInfo UserInfo { get; init; } = new();
    public CultureInfo Culture { get; init; } = CultureInfo.CurrentCulture;
}
