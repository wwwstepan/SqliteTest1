using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SqliteTest1.Dto;
using SqliteTest1.Entities;

namespace SqliteTest1.Controllers;

[ApiController]
[Route("api/main")]
public class MainController(
        DbCtx dbCtx, // Не будет работать в Singleton
        ILogger<MainController> logger,
        IDbContextFactory<DbCtx> dbCtxFactory,
        IServiceScopeFactory serviceScopeFactory
    ) : ControllerBase
{
    // curl -X GET https://localhost:7290/api/main/get-states
    [HttpGet("get-states")]
    public async Task<IEnumerable<State>> GetEquipment()
    {
        using var db = dbCtxFactory.CreateDbContext();
        return await db.States.OrderBy(x => x.Name).ToListAsync();
    }

    // curl -X GET https://localhost:7290/api/main/get-states2
    [HttpGet("get-states2")]
    public async Task<IEnumerable<State>> GetEquipment2()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DbCtx>();
        return await db.States.OrderByDescending(x => x.Name).ToListAsync();
    }

    // curl -X GET https://localhost:7290/api/main/get-cities
    [HttpGet("get-cities")]
    public async Task<IEnumerable<CityDto>> GetFuelRoutes()
    {
        return await dbCtx.Cities.Select(x => new CityDto
        {
            Name = x.Name,
            Population = x.Population,
            StateId = x.State.Id
        }).ToListAsync();
    }
}
