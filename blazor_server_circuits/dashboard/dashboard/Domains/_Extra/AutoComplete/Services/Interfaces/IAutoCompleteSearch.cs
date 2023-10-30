using dashboard.Domains._Extra.AutoComplete.Models;

namespace dashboard.Domains._Extra.AutoComplete.Services.Interfaces;

public interface IAutoCompleteSearch
{
    Task<IEnumerable<T>> GetDefaultResults<T>(CancellationToken cToken = default) where T : IAutoComplete;
    Task<IEnumerable<T>> GetFoundResults<T>(string name, CancellationToken cToken = default) where T : IAutoComplete;
}
