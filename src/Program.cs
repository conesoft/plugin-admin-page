using Conesoft.Hosting;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddHostConfigurationFiles()
    .AddHostEnvironmentInfo()
    .AddLoggingService()
    ;

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();