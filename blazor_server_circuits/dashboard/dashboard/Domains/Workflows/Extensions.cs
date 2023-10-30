using dashboard.Domains.Workflows.Services.Implementations;
using dashboard.Domains.Workflows.Services.Interfaces;

namespace dashboard.Domains.Workflows;

public static class Extensions
{
    public static IServiceCollection AddWorkflows(this IServiceCollection services)
    {
        //TODO: MOCK SERVICES
        services.AddTransient<IWorkflowsData, DaprWorkflowsData>();

        return services;
    }
}
