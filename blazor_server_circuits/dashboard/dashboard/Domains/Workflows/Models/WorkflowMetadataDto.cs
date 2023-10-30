namespace dashboard.Domains.Workflows.Models;

public record WorkflowMetadataDto(string Subsystem, string CreatedBy, string CreatedOn, string Type, string OrderId, string ArticleSupplierDescription, string Status);
