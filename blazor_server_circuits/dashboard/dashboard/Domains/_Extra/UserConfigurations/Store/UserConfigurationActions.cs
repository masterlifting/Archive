using System.Globalization;

using dashboard.Domains._Extra.UserConfigurations.Models;

namespace dashboard.Domains._Extra.UserConfigurations.Store;

public sealed record UserConfigurationSetAction(UserConfiguration Configuration);
public sealed record UserConfigurationUpdateCultureAction(CultureInfo Culture);
