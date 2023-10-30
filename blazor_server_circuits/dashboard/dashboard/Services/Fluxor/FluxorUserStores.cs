namespace dashboard.Services.Fluxor;

/// <summary>
/// This store is required to manage Fluxor's independent states of one type.
/// </summary>
/// <typeparam name="TState">
/// The type of the state.
/// </typeparam>
/// <typeparam name="TModel">
/// The type of the data model
/// </typeparam>
/// <param name="Data">
/// The data associated with the state.
/// </param>
public abstract record FluxorUserSingleStore<TState, TModel>(Dictionary<string, TState> Data) where TState : FluxorUserSingleState<TModel>
{
    public FluxorUserSingleStore() : this(new Dictionary<string, TState>()) { }
}

/// <summary>
/// This store is required to manage Fluxor's independent states of one type.
/// </summary>
/// <typeparam name="TState">
/// The type of the state.
/// </typeparam>
/// <typeparam name="TModel">
/// The type of the data model
/// </typeparam>
/// <param name="Data">
/// The data associated with the state.
/// </param>
public abstract record FluxorUserMultiStore<TState, TModel>(Dictionary<string, TState> Data) where TState : FluxorUserMultiState<TModel>
{
    public FluxorUserMultiStore() : this(new Dictionary<string, TState>()) { }
}
