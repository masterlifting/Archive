using Dapr.Client;

using dashboard.Domains.Warehouse.Inbound.Models;
using dashboard.Domains.Warehouse.Inbound.Services.Interfaces;
using dashboard.Domains.Workflows.Services.Interfaces;
using dashboard.Services.Authorization;
using dashboard.Services.Fluxor.Store.Web;

using Fluxor;

using Lungmuss.Refractory.Library.Exceptions;
using Lungmuss.Refractory.Library.Extensions;
using Lungmuss.Refractory.Library.Models.Container;
using Lungmuss.Refractory.Library.Models.Mms;
using Lungmuss.Refractory.Library.Models.Warehouse;
using Lungmuss.Refractory.Library.Models.Workflows;

using Microsoft.Extensions.Localization;

using MudBlazor;

namespace dashboard.Domains.Warehouse.Inbound.Services.Implementations;

internal sealed class DaprWarehouseInboundData : IWarehouseInboundData
{
    private readonly IStringLocalizer<App> _localizer;
    private readonly DaprClient _daprClient;
    private readonly UserAuthorization _authorization;
    private readonly IWorkflowsData _workflowsData;
    private readonly IDispatcher _dispatcher;

    public DaprWarehouseInboundData(
        IStringLocalizer<App> localizer,
        DaprClient daprClient,
        UserAuthorization authorization,
        IWorkflowsData workflowsData,
        IDispatcher dispatcher)
    {
        _localizer = localizer;
        _daprClient = daprClient;
        _authorization = authorization;
        _workflowsData = workflowsData;
        _dispatcher = dispatcher;
    }
    public async Task<InboundWorkflow[]> GetStarted(string userName, CancellationToken cToken = default)
    {
        var workflows = await _workflowsData.GetUserWorkflows(userName, cToken);

        return workflows
            .Where(x => x.Metadata.CreatedBy == userName && x.Metadata.Status == "WorkflowStarted")
            .Select(x => new InboundWorkflow(x.Id, x.Metadata.CreatedOn, x.Metadata.OrderId, x.Metadata.ArticleSupplierDescription))
            .ToArray();
    }
    public async Task<InboundWorkflowDetails?> GetDetails(string workflowId, CancellationToken cToken = default)
    {
        try
        {

            var workflowDetails = await _workflowsData.GetWorkflowDetails(workflowId, cToken);

            return new InboundWorkflowDetails(workflowDetails);
        }
        catch (Exception ex)
        {
            var userName = await _authorization.GetUserName(cToken);

            _dispatcher.Dispatch(new SnackbarShowAction(userName,new("Can't load workflow details", Severity.Error)));
            return null;
        }
    }
    public async Task<InboundWorkflowCategory[]> GetCategories(CancellationToken cToken = default)
    {

        try
        {
            var userToken = await _authorization.GetUserToken(cToken);

            var response = await _daprClient.InvokeDaprRequest<CategoriesResponseV1>(
                HttpMethod.Get,
                "cns",
                "v1/category/container",
                string.Empty,
                cToken,
                userToken);

            return response.categories
                    .Select(x => new InboundWorkflowCategory(x.CategoryId, x.CategoryName))
                    .ToArray();

        }
        catch (Exception ex)
        {
            var userName = await _authorization.GetUserName(cToken);
            _dispatcher.Dispatch(new SnackbarShowAction(userName, new("Can't load categories", Severity.Error)));
            return Array.Empty<InboundWorkflowCategory>();
        }

    }
    public async Task<InboundWorkflowArticle[]> GetArticles(CancellationToken cToken = default)
    {
        try
        {
            var userToken = await _authorization.GetUserToken(cToken);

            var response = await _daprClient.InvokeDaprRequest<ArticleSuppliersListResponseV1>(
                HttpMethod.Get,
                "mms",
                "v1/articlesuppliers",
                string.Empty,
                cToken,
                userToken);

            return response.ArticleSuppliers
                    .Where(x => !string.IsNullOrWhiteSpace(x.Description1))
                    .Select(x => new InboundWorkflowArticle(x.RowId, x.Description1, x.ArticleId, x.SupplierId)).ToArray();
        }
        catch (Exception ex)
        {
            var userName = await _authorization.GetUserName(cToken);
            _dispatcher.Dispatch(new SnackbarShowAction(userName, new("Can't load articles", Severity.Error)));
            return Array.Empty<InboundWorkflowArticle>();
        }
    }

    public Task<InboundWorkflowCreate> GetNewCreationModel(CancellationToken cToken = default)
    {
        return Task.FromResult(new InboundWorkflowCreate());
    }
    public async Task Create(InboundWorkflowCreate workflow, CancellationToken cToken = default)
    {

        var model = new CreateWarehouseInboundPostV1
        {
            OrderData = new()
            {
                ArticleSupplierRowId = workflow.ArticleSupplierRowId,
                OrderNrRowId = workflow.OrderNumber,
                ContainerTypeId = workflow.CategoryId,
                ContainersCount = workflow.ContainersCount,
                Weight = workflow.WeightDb,
                ArticleSupplierDescription = workflow.ArticleSupplierDescription,
            }
        };

        try
        {
            var userToken = await _authorization.GetUserToken(cToken);

            _ = await _daprClient.InvokeDaprRequest<CreateWarehouseInboundPostV1, WorkflowCreationResponseV1>(
                HttpMethod.Post,
                "warehouse",
                "v1/inbound",
                model,
                cToken,
                userToken);
        }
        catch (Exception _)
        {
            var userName = await _authorization.GetUserName(cToken);
            _dispatcher.Dispatch(new SnackbarShowAction(userName, new(_localizer["New Inbound workflow was not created."], Severity.Error)));
        }
    }

    public string? TryValidateWeight(string value, out double result)
    {
        result = 0;

        try
        {
            result = StringExtensions.TryConvertWeight(value);
        }
        catch (ArgumentNullException exception)
        {
            return exception.Message;
        }
        catch (BusinessException exception)
        {
            return exception.Message;
        }
        catch (RuntimeException exception)
        {
            return exception.Message;
        }

        return null;
    }
}
