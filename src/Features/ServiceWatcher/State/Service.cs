namespace Conesoft.Plugin.AdminPage.Features.ServiceWatcher.State;

public record Service(string Name, string Category, int? Process = default, int? Port = default);
