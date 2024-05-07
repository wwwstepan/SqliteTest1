namespace SqliteTest1.Entities;

public class State
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public long Cash { get; set; }
    public virtual IEnumerable<City>? Cities { get; set; }
}
