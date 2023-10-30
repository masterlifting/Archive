using dashboard.Domains._Extra.UserConfigurations.Models;

using Fluxor;

namespace dashboard.Domains._Extra.UserConfigurations.Store;

[FeatureState]
public sealed record UserConfigurationState
{
    public UserConfiguration Configuration { get; }
    
    public UserConfigurationState(UserConfiguration configuration) => Configuration = configuration;
    
    //Default user configuration
    private UserConfigurationState() : this(new UserConfiguration()) { }
}

