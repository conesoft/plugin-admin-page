using Conesoft.Files;
using Conesoft.Hosting;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Conesoft.Plugin.AdminPage.Features.ServiceRestarter.Services;

public class ServiceRestarter(HostEnvironment environment)
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
                await Task.Delay(200);
            }
            await Task.Delay(200);
            System.IO.File.Move(tmp.Path, file.Path);
            while (tmp.Exists)
            {
                await Task.Delay(200);
            }
        } catch(Exception ex)
        {
            Log.Error("file move failed: {exception}", ex);
        }
    }
}
