using Fluxor;

namespace dashboard.Domains._Extra.UserConfigurations.Store;

public class UserConfigurationReducers
{
    [ReducerMethod]
    public static UserConfigurationState Set(UserConfigurationState _, UserConfigurationSetAction action)
    {
        var newState = new UserConfigurationState(action.Configuration);

        return newState;
    }

    [ReducerMethod]
    public static UserConfigurationState UpdateCulture(UserConfigurationState oldState, UserConfigurationUpdateCultureAction action)
    {
        var configuration = oldState.Configuration with
        {
            Culture = action.Culture
        };

        var newState = new UserConfigurationState(configuration);

        return newState;
    }
}
