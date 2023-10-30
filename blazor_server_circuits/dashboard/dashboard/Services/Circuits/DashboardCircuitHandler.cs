using dashboard.Services.Fluxor.Store.Web;

using Fluxor;

using Microsoft.AspNetCore.Components.Server.Circuits;

namespace dashboard.Services.Circuits;

public sealed class DashboardCircuitHandler : CircuitHandler
{
    private readonly DashboardCircuit _circuit;
    private readonly IDispatcher _dispatcher;

    public DashboardCircuitHandler(DashboardCircuit circuit, IDispatcher dispatcher)
    {
        _circuit = circuit;
        _dispatcher = dispatcher;
    }

    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        _circuit.Current = circuit;
        return base.OnCircuitOpenedAsync(circuit, cancellationToken);
    }

    public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        _circuit.Current = null;
        _dispatcher.Dispatch(new UserCircuitsServicesRemoveAction(circuit));
        return base.OnCircuitClosedAsync(circuit, cancellationToken);
    }
}
