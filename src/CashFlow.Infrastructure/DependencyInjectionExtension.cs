using Microsoft.Extensions.DependencyInjection;

using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Infrastructure.DataAccess.Repositories;
using CashFlow.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Domain.Repositories.User;

namespace CashFlow.Infrastructure;

public static class DependencyInjectionExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddDbContext(services, configuration);
        AddRepositories(services);

        services.AddScoped<IPasswordEncripter, Security.Cryptography.BCrypt>();
    }

    public static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IExpensesReadOnlyRepository, ExpensesRepository>();
        services.AddScoped<IExpensesWriteOnlyRepository, ExpensesRepository>();
        services.AddScoped<IExpensesUpdateOnlyRepository, ExpensesRepository>();

        services.AddScoped<IUserReadOnlyRepository, UserRepository>();
        services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
    }

    public static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Connection");

        var version = new Version(8, 4);
        var serverVersion = new MySqlServerVersion(version);

        services.AddDbContext<CashFlowDbContext>(config => config.UseMySql(connectionString, serverVersion));
    }
}