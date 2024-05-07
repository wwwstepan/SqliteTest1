using EfConfigureHelpers;
using Microsoft.EntityFrameworkCore;
using SqliteTest1.Entities;

namespace SqliteTest1;

public class DbCtx : DbContext
{
    public DbSet<State> States { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Country> Countries { get; set; }

    public DbCtx(DbContextOptions<DbCtx> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    {
        modelBuilder.AllEntitiesToSnakeCase(this.GetType());
    }
}
