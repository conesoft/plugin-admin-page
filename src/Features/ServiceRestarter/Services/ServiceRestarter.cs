using Conesoft.Files;
using Conesoft.Hosting;
using Conesoft.Plugin.AdminPage.Features.ServiceWatcher.State;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Conesoft.Plugin.AdminPage.Features.ServiceRestarter.Services;

public class ServiceRestarter(HostEnvironment environment, ServiceWatcher.Services.ServiceWatcher watcher)
{
    public async Task RestartService(string name, string category)
    {
        var file = environment.Global.Deployments / category / Filename.From(name, "zip");
        var tmp = environment.Root / Filename.From(name, "zip");
        try
        {
            System.IO.File.Move(file.Path, tmp.Path);
            while (file.Exists)
            {
                await Task.Yield();
            }
            await Task.Delay(10);
            System.IO.File.Move(tmp.Path, file.Path);
            while (tmp.Exists)
            {
                await Task.Yield();
            }
        } catch(Exception ex)
        {
            Log.Error("file move failed: {exception}", ex);
        }
    }

    public async Task StopService(string name, string category)
    {
        if(watcher.State.FirstOrDefault(s => s.Name == name && s.Category == category) is Service service && service.Process is int id)
        {
            var p = Process.GetProcessById(id);
            p.Kill();
            await p.WaitForExitAsync();
        }
    }
}
