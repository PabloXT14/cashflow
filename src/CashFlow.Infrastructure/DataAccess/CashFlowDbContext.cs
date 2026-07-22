using CashFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.DataAccess;

public class CashFlowDbContext : DbContext
{
    // OBS: O nome da propriedade DbSet deve ser o mesmo nome da tabela no banco de dados (não precisa ser case-sensitive), caso contrário, será necessário configurar o mapeamento no OnModelCreating.
    public DbSet<Expense> Expenses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = "Server=localhost;Port=3306;Database=cashflow_db;User=cashflow_user;Password=@Password123;";

        var version = new Version(8, 4);
        var serverVersion = new MySqlServerVersion(version);

        optionsBuilder.UseMySql(connectionString, serverVersion);
    }
}