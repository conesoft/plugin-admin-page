using Conesoft.Files;
using Conesoft.Hosting;
using Conesoft.Plugin.AdminPage.Features.ServiceWatcher.State;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Conesoft.Plugin.AdminPage.Features.ServiceWatcher.Services;

public class DeployedServiceWatcher(HostEnvironment environment) : BackgroundService
{
    public delegate void StateChangedEventHandler(Service[] services);

    public event StateChangedEventHandler? StateChanged;

    Service[] state = [];

    public Service[] State => state;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var storage = environment.Global.Live;

        await SendState();

        await foreach (var _ in storage.Live(allDirectories: true, cancellation: stoppingToken))
        {
            Log.Information("{service} state change detected", "DeployedServiceWatcher");
            await SendState();
        }

        async Task SendState()
        {
            var deployed = storage.Directories.SelectMany(c => c.Directories.Select(s => new Service(s.Name, s.Parent.Name))).ToArray();
            state = deployed ?? [];
            StateChanged?.Invoke(state);
        }
    }
}
