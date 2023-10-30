using dashboard.Domains._Extra.AutoComplete.Models;
using dashboard.Domains._Extra.AutoComplete.Services.Interfaces;
using dashboard.Domains.Warehouse.Inbound.Models;
using dashboard.Domains.Warehouse.Inbound.Services.Interfaces;

namespace dashboard.Domains._Extra.AutoComplete.Services.Implementations;

public sealed class DaprAutoCompleteSearch : IAutoCompleteSearch
{
    private IEnumerable<IAutoComplete> _items = new List<IAutoComplete>();
    private readonly IWarehouseInboundData _warehouseInboundData;

    public DaprAutoCompleteSearch(IWarehouseInboundData warehouseInboundData)
    {
        _warehouseInboundData = warehouseInboundData;
    }

    public async Task<IEnumerable<T>> GetDefaultResults<T>(CancellationToken cToken = default) where T : IAutoComplete
    {
        var result = Enumerable.Empty<T>();

        switch (typeof(T))
        {
            case Type type when type == typeof(InboundWorkflowArticle):

                _items = await GetItems<InboundWorkflowArticle>(cToken);

                break;
        }

        return result;
    }
    public async Task<IEnumerable<T>> GetFoundResults<T>(string name, CancellationToken cToken = default) where T : IAutoComplete
    {
        _items = !_items.Any() ? (IEnumerable<IAutoComplete>)await GetDefaultResults<T>(cToken) : _items;

        return _items.Where(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).Cast<T>();
    }

    private async Task<ICollection<IAutoComplete>> GetItems<T>(CancellationToken cToken = default) where T : IAutoComplete
    {
        return typeof(T) switch
        {
            Type type when type == typeof(InboundWorkflowArticle) => await _warehouseInboundData.GetArticles(cToken),
            _ => Enumerable.Empty<IAutoComplete>().ToArray(),
        };
    }
}
