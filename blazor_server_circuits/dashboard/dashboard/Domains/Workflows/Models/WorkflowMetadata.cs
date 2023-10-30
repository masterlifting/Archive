namespace dashboard.Domains.Workflows.Models;
public record WorkflowMetadata(string Subsystem, string CreatedBy, DateTime CreatedOn, string Type, string OrderId, string ArticleSupplierDescription, string Status);
