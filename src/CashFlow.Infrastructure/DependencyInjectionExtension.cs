using Microsoft.Extensions.DependencyInjection;

using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Infrastructure.DataAccess.Repositories;
using CashFlow.Infrastructure.DataAccess;

namespace CashFlow.Infrastructure;

public static class DependencyInjectionExtension
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        AddDbContext(services);
        AddRepositories(services);
    }

    public static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IExpensesRepository, ExpensesRepository>();
    }

    public static void AddDbContext(IServiceCollection services)
    {
        services.AddDbContext<CashFlowDbContext>();
    }
}