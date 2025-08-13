using Microsoft.EntityFrameworkCore;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Presentation.Infrastructure.DbCreate;

public static class DbCheck
{
    public static async Task EnsureDbCreatedAndMigratedMigratedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
    
        try
        {
            var dbContext = services.GetRequiredService<WarehouseDbContext>();
            var logger = services.GetRequiredService<ILogger<Program>>();
            
            if (!await dbContext.Database.CanConnectAsync())
            {
                logger.LogInformation("Database doesn't exist - creating...");
                await dbContext.Database.EnsureCreatedAsync();
                
                if (!dbContext.Database.GetMigrations().Any())
                {
                    logger.LogWarning("No migrations exist. Please create initial migration using CLI:");
                    logger.LogWarning("dotnet ef migrations add InitialMigration");
                    return;
                }
            }
            
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            var migrations = pendingMigrations as string[] ?? pendingMigrations.ToArray();
            if (migrations.Length != 0)
            {
                logger.LogInformation("Applying {MigrationsLength} pending migrations...", migrations.Length);
                await dbContext.Database.MigrateAsync();
            }
            else
            {
                logger.LogInformation("Database is up to date");
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database");
            throw;
        }
    }
}