using Conesoft.Files;
using Conesoft.Hosting;
using Conesoft.Plugin.AdminPage.Features.ServiceWatcher.State;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace Conesoft.Plugin.AdminPage.Features.ServiceWatcher.Services;

public class ServiceWatcher(HostEnvironment environment) : BackgroundService
{
    public delegate void StateChangedEventHandler(Service[] services);

    public event StateChangedEventHandler? StateChanged;

    Service[] state = [];

    public Service[] State => state;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var storage = environment.Global.Storage / "host";
        var file = storage / Filename.From("state", "json");

        await SendState();

        await foreach (var _ in storage.Live(cancellation: stoppingToken))
        {
            Log.Information("state change detected");
            await SendState();
        }

        async Task SendState()
        {
            var host = file.Exists ? await file.ReadFromJson<State.Host>() : null;
            state = host?.Services ?? [];
            StateChanged?.Invoke(state);
        }
    }
}
