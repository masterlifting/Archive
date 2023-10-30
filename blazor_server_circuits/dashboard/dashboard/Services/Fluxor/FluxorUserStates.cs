namespace dashboard.Services.Fluxor;

/// <summary>
/// This is the state required for Fluxor's independent states of one type in the <see cref="FluxorUserSingleStore{TState, TModel}"/>.
/// </summary>
/// <typeparam name="TModel">
/// The type of the data model.
/// </typeparam>
/// <param name="Id">
/// The unique identifier of the state from the state store.
/// </param>
/// <param name="Model">
/// The data model is associated with the state.
/// </param>
public abstract record FluxorUserSingleState<TModel>(string Id, TModel? Model);

/// <summary>
/// This is the state required for Fluxor's independent states of one type in the <see cref="FluxorUserMultiStore{TState, TModel}"/>.
/// </summary>
/// <typeparam name="TModel">
/// The type of the data model.
/// </typeparam>
/// <param name="Id">
/// The unique identifier of the state from the state store.
/// </param>
/// <param name="Model">
/// The data model is associated with the state.
/// </param>
public abstract record FluxorUserMultiState<TModel>(string Id, TModel[] Models);