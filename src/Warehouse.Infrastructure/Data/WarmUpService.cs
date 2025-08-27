using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Warehouse.Infrastructure.Data;

public class WarmupService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    public WarmupService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
        // Force connection and query compilation by running a simple query
        await context.Resources.Take(1).FirstOrDefaultAsync(cancellationToken);
        // Pre-warm other critical caches here
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}