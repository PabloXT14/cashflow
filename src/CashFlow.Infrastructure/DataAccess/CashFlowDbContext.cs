using CashFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.DataAccess;

internal class CashFlowDbContext : DbContext
{
    public CashFlowDbContext(DbContextOptions options) : base(options) { }

    // OBS: O nome da propriedade DbSet deve ser o mesmo nome da tabela no banco de dados (não precisa ser case-sensitive), caso contrário, será necessário configurar o mapeamento no OnModelCreating.
    public DbSet<Expense> Expenses { get; set; }


}