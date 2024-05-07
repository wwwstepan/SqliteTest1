using Microsoft.EntityFrameworkCore;
using SqliteTest1.Entities;
using System.Reflection;

namespace SqliteTest1;

public static class ConfigureDbHelper
{
    private static readonly string dbPath = GetBdPath();

    public static string GetBdPath()
    {
        string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var path = Path.GetDirectoryName(assemblyLocation);
        return Path.Join(path, "SqliteTest1.db");
    }

    public static void ConfigureDb(this IServiceCollection srv)
    {
        srv.AddDbContextFactory<DbCtx>(UseSqlAction);
        srv.AddDbContext<DbCtx>(UseSqlAction);
    }

    static void UseSqlAction(DbContextOptionsBuilder op) { op.UseSqlite($"Data Source={dbPath}"); }

    public static async Task FillDb()
    {
        using var dbCtx = new DbCtx(new DbContextOptionsBuilder<DbCtx>()
            .UseSqlite($"Data Source={dbPath}")
            .Options);

        var stateCA = await dbCtx.States.FirstOrDefaultAsync(x => x.Id == "CA");
        if (stateCA is null)
        {
            stateCA = new State
            {
                Id = "CA",
                Name = "California",
                Cash = 20_000
            };
            await dbCtx.States.AddAsync(stateCA);
        }

        var stateNY = await dbCtx.States.FirstOrDefaultAsync(x => x.Id == "NY");
        if (stateNY is null)
        {
            stateNY = new State
            {
                Id = "NY",
                Name = "New York",
                Cash = 12_000
            };
            await dbCtx.States.AddAsync(stateNY);
        }

        var stateNE = await dbCtx.States.FirstOrDefaultAsync(x => x.Id == "NE");
        if (stateNE is null)
        {
            stateNE = new State
            {
                Id = "NE",
                Name = "Nebraska",
                Cash = 3_000
            };
            await dbCtx.States.AddAsync(stateNE);
        }

        var cityId = dbCtx.Cities.OrderByDescending(x => x.Id).FirstOrDefault()?.Id ?? 1;

        if (!await dbCtx.Cities.AnyAsync(x => x.Name == "Los Angeles"))
            await dbCtx.Cities.AddAsync(new City
            {
                Id = cityId++,
                Name = "Los Angeles",
                Population = 3792,
                State = stateCA
            });

        if (!await dbCtx.Cities.AnyAsync(x => x.Name == "San Francisco"))
            await dbCtx.Cities.AddAsync(new City
            {
                Id = cityId++,
                Name = "San Francisco",
                Population = 805,
                State = stateCA
            });

        if (!await dbCtx.Cities.AnyAsync(x => x.Name == "New York"))
            await dbCtx.Cities.AddAsync(new City
            {
                Id = cityId++,
                Name = "New York",
                Population = 8804,
                State = stateNY
            });

        if (!await dbCtx.Cities.AnyAsync(x => x.Name == "Buffalo"))
            await dbCtx.Cities.AddAsync(new City
            {
                Id = cityId++,
                Name = "Buffalo",
                Population = 278,
                State = stateNY
            });

        if (!await dbCtx.Cities.AnyAsync(x => x.Name == "Utica"))
            await dbCtx.Cities.AddAsync(new City
            {
                Id = cityId++,
                Name = "Utica",
                Population = 65,
                State = stateNY
            });    

        if (!await dbCtx.Cities.AnyAsync(x => x.Name == "Omaha"))
            await dbCtx.Cities.AddAsync(new City
            {
                Id = cityId++,
                Name = "Omaha",
                Population = 485,
                State = stateNE
            });

        await dbCtx.SaveChangesAsync();
    }
}
