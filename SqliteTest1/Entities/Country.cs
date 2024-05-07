namespace SqliteTest1.Entities;

public class Country
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public long Cash { get; set; }
    public string? PresidentName { get; set; }
}
