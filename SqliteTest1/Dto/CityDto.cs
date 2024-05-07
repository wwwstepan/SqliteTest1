using SqliteTest1.Entities;

namespace SqliteTest1.Dto;

public class CityDto
{
    public string Name { get; set; } = string.Empty;
    public int Population { get; set; }
    public string? StateId { get; set; }
}
