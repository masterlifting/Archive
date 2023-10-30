using System.ComponentModel.DataAnnotations;

using dashboard.Domains._Extra.AutoComplete.Models;

namespace dashboard.Domains.Warehouse.Inbound.Models;

public sealed class InboundWorkflowCreate
{
    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    [Required]
    public string OrderNumber { get; set; } = string.Empty;
    
    [Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
    public int ContainersCount { get; set; } = 1;
    
    [Required]
    public string? Weight { get; set; }

    public long ArticleSupplierRowId { get; set; }
    public InboundWorkflowArticle? Article { get; set; }
    public double WeightDb { get; set; }
    public string? ArticleSupplierDescription { get; set; }
}

public sealed record InboundWorkflowArticle(long RowId, string Name, string ArticleId, string SupplierId) : IAutoComplete;

public sealed record InboundWorkflowCategory(int Id, string Name);
