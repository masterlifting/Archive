using System.Globalization;
using System.Text.Json;

using Dapr.Client;

using dashboard.Domains.Workflows.Models;
using dashboard.Domains.Workflows.Services.Interfaces;
using dashboard.Services.Authorization;
using dashboard.Services.Fluxor.Store.Web;

using Fluxor;

using Lungmuss.Refractory.Library.Extensions;
using Lungmuss.Refractory.Library.Models.Workflows;

using MudBlazor;

namespace dashboard.Domains.Workflows.Services.Implementations;

public sealed class DaprWorkflowsData : IWorkflowsData
{
    private readonly DaprClient _daprClient;
    private readonly UserAuthorization _authorization;
    private readonly IDispatcher _dispatcher;

    public DaprWorkflowsData(DaprClient daprClient, UserAuthorization authorization, IDispatcher dispatcher)
    {
        _daprClient = daprClient;
        _authorization = authorization;
        _dispatcher = dispatcher;
    }

    public async Task<Workflow[]> GetUserWorkflows(string userName, CancellationToken cToken = default)
    {
        try
        {
            var userToken = await _authorization.GetUserToken();

            var (_, response) = await _daprClient.InvokeDaprRequest(
                HttpMethod.Get,
                "blob",
                $"v1/statestore/workflows/{userName}",
                string.Empty,
                cToken, userToken);

            var workflows = !string.IsNullOrEmpty(response)
                ? JsonSerializer.Deserialize<List<WorkflowInfo>>(response)
                : new List<WorkflowInfo>();

            return await MapToWorkflowModels(workflows, cToken);
        }
        catch (Exception ex)
        {
            _dispatcher.Dispatch(new SnackbarShowAction(userName, new("Can't load workflows", Severity.Error)));
            return Array.Empty<Workflow>();
        }
    }
    //TODO: This way doesn't implement in the backend
    public async Task<Workflow[]> GetUserRolesWorkflows(string userName, CancellationToken cToken = default)
    {
        return Array.Empty<Workflow>();
    }
    public async Task<WorkflowMetadata> GetWorkflowMetadata(string workflowId, CancellationToken cToken = default)
    {
        try
        {
            var userToken = await _authorization.GetUserToken();

            var (_, response) = await _daprClient.InvokeDaprRequest(
                HttpMethod.Get,
                "blob",
                $"v1/statestore/workflows/{workflowId}",
                string.Empty,
                cToken,
                userToken);

            if (string.IsNullOrEmpty(response))
                throw new Exception("Can't load workflow metadata");

            var deserializedResult = JsonSerializer.Deserialize<WorkflowMetadataDto>(response, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = false,
            });

            return deserializedResult is null
                ? throw new Exception("Can't load workflow metadata")
                : new WorkflowMetadata(
                    deserializedResult.Subsystem,
                    deserializedResult.CreatedBy,
                    DateTime.Parse(deserializedResult.CreatedOn, CultureInfo.InvariantCulture),
                    deserializedResult.Type,
                    deserializedResult.OrderId,
                    deserializedResult.ArticleSupplierDescription,
                    deserializedResult.Status);
        }
        catch (Exception ex)
        {
            //_dispatcher.Dispatch(new SnackbarShowAction("Can't load workflow metadata", Severity.Warning));
            return new WorkflowMetadata("", "", DateTime.UtcNow, "", "", "", "");
        }
    }
    public async Task<WorkflowDetails> GetWorkflowDetails(string workflowId, CancellationToken cToken = default)
    {
        try
        {
            var userToken = await _authorization.GetUserToken();

            var data = await _daprClient.InvokeDaprRequest<GetWorkflowResponse>(
                HttpMethod.Get,
                workflowId.GetWorkflowAppId(),
                $"v1/workflow/{workflowId}",
                string.Empty,
                cToken,
                userToken);

            var documents = await _daprClient.InvokeDaprRequest(
                HttpMethod.Get, 
                "blob",
                $"v1/statestore/workflow_documents/{workflowId}", 
                string.Empty, 
                cToken,
            userToken);

            var workflowDocuments = !string.IsNullOrEmpty(documents.Response)
               ? JsonSerializer.Deserialize<List<WorkflowDocument>>(documents.Response)
               : new List<WorkflowDocument>();

            workflowDocuments ??= new List<WorkflowDocument>();

            return new WorkflowDetails(data, workflowDocuments);
        }
        catch (Exception ex)
        {
            var userName = await _authorization.GetUserName(cToken);
            _dispatcher.Dispatch(new SnackbarShowAction(userName, new("Can't load workflow details", Severity.Error)));
            return new WorkflowDetails(new(), new());
        }
    }
    private async Task<Workflow[]> MapToWorkflowModels(List<WorkflowInfo> data, CancellationToken cToken)
    {
        var chunks = data.Chunk(5);

        var result = new List<Workflow>();

        // by Alexander's request, we need to load Metadata by chunks
        foreach(var chunk in chunks)
        {
            var metadata = await Task.WhenAll(chunk.Select(async x => ValueTuple.Create(x.InstanceId, await GetWorkflowMetadata(x.InstanceId, cToken))));

            result.AddRange(metadata
                .Where(x => !string.IsNullOrWhiteSpace(x.Item2.Status))
                .Select(x => new Workflow(x.Item1, x.Item2))
                .ToArray());
        }

        return result.OrderByDescending(x => x.Metadata.CreatedOn).ToArray();
    }
}
