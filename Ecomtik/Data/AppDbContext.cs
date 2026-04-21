using Ecomtik.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecomtik.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Computation> Computations => Set<Computation>();
    public DbSet<Method> Methods => Set<Method>();
     
    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Method>().HasData(
            new Method { Id = 1, Code = "simpson",     Name = "Метод Симпсона"   },
            new Method { Id = 2, Code = "trapezoidal", Name = "Метод трапеций"   },
            new Method { Id = 3, Code = "monte_carlo", Name = "Метод Монте-Карло" }
        );
    }
}