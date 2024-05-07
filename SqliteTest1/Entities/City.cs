namespace SqliteTest1.Entities;

public class City
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Population { get; set; }
    public long Cash { get; set; }
    public string? StateId { get; set; }
    public virtual State? State { get; set; }

    public const string DbSchema = "World"; // Ability to specify a database schema other than "public"
}
