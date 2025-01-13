using Conesoft.Hosting;
using Conesoft.Plugin.AdminPage.Components;
using Conesoft.Plugin.AdminPage.Features.ServiceRestarter.Services;
using Conesoft.Plugin.AdminPage.Features.ServiceWatcher.Services;
using Conesoft.PwaGenerator;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddHostConfigurationFiles()
    .AddHostEnvironmentInfo()
    .AddLoggingService()
    .AddUsersWithStorage()
    ;

builder.Services
    .AddCompiledHashCacheBuster()
    .AddHttpClient()
    .AddSingleton<ServiceRestarter>()
    .AddSingleton<ServiceWatcher>()
    .AddHostedService(s => s.GetRequiredService<ServiceWatcher>())
    .AddSingleton<DeployedServiceWatcher>()
    .AddHostedService(s => s.GetRequiredService<DeployedServiceWatcher>())
    .AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

app
    .UseDefaultFiles()
    .UseStaticFiles()
    .UseAntiforgery();

app.MapPwaInformationFromAppSettings();

app.MapUsersWithStorage();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();