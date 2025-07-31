using Microsoft.EntityFrameworkCore;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Presentation.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using var dbContext =
            scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();

        dbContext.Database.Migrate();
    }
}
