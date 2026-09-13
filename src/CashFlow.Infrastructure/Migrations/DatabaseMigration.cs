using CashFlow.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Infrastructure.Migrations;

public static class DatabaseMigration
{
    public async static Task MigrateDatabase(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<CashFlowDbContext>();

        // Apply any pending migrations to the database
        await dbContext.Database.MigrateAsync();
    }
}