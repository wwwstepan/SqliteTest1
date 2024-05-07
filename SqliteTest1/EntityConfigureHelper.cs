using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using System.Text;

namespace EfConfigureHelpers;

public static class EntityConfigureHelper
{
    /// <summary>
    /// Call from OnModelCreating method of Db context class
    /// Casts all table and column names to lower case snake case format.
    /// The default database schema is "public", it can be changed
    /// by setting the DbSchema entity class field.
    /// Example:
    ///     public const string DbSchema = "MySchema";
    /// </summary>
    /// <param name="modelBuilder">ModelBuilder parameter from OnModelCreating method</param>
    /// <param name="dbCtxType">Type of Db context class</param>
    public static void AllEntitiesToSnakeCase(this ModelBuilder modelBuilder, Type dbCtxType)
    {
        var dbSetType = typeof(DbSet<>);
        var props = dbCtxType.GetProperties();
        foreach (var prop in props)
        {
            var setType = prop.PropertyType;
            if (setType.IsGenericType)
            {
                var entityType = setType.GetGenericTypeDefinition();
                var eType = setType.GenericTypeArguments[0];

#if EF5 || EF6
            var isDbSet = dbSetType.IsAssignableFrom(entityType) || setType.GetInterface(dbSetType.FullName) != null;
#else
                var isDbSet = dbSetType.IsAssignableFrom(entityType);
#endif
                if (isDbSet)
                {
                    var tabname = prop.Name.ToSnakeCase();
                    var schemaField = eType.GetFields().Where(x => x.Name == "DbSchema").FirstOrDefault();
                    var schema = "public";
                    if (schemaField is not null && schemaField.IsStatic)
                    {
                        var o = schemaField.GetValue(eType);
                        if (o is string s && !string.IsNullOrWhiteSpace(s))
                            schema = s.ToSnakeCase();
                    }
                    modelBuilder.StdPgEntity(eType, $"{schema}.{tabname}");
                }
            }
        }
    }

    /// <summary>
    /// Call from OnModelCreating method of Db context class
    /// Casts all column names of table to lower case snake case format.
    /// </summary>
    /// <param name="builder">EntityTypeBuilder from ModelBuilder.Entity</param>
    /// <param name="entityType">Type of entity</param>
    public static void StdPgEntity(this ModelBuilder builder, Type entityType, string schemaAndTable)
    {
        builder.Entity(entityType).SchemaAndTable(schemaAndTable).ColumnNamesSnakeCase(entityType);
    }

    /// <summary>
    /// Extracts the schema name and table name for mapping from the string "schema.table".
    /// </summary>
    /// <param name="builder">EntityTypeBuilder from ModelBuilder.Entity</param>
    /// <param name="schemaAndTable">schema name and table name in the format "schema.table"</param>
    public static EntityTypeBuilder SchemaAndTable(this EntityTypeBuilder builder, string schemaAndTable)
    {
        var n = schemaAndTable.IndexOf(".");

        var table = schemaAndTable[(n + 1)..];
        var schema = schemaAndTable[..n];

        builder.ToTable(table, schema);

        return builder;
    }

    /// <summary>
    /// Casts all class properties to "snake case" in lower case for mapping to the database
    /// Example: FirstName ==> first_name
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    /// <param name="builder">EntityTypeBuilder from ModelBuilder.Entity</param>
    public static EntityTypeBuilder ColumnNamesSnakeCase<T>(this EntityTypeBuilder<T> builder)
        where T : class
    {
        return builder.ColumnNamesSnakeCase(typeof(T));
    }

    /// <summary>
    /// Casts all class properties to "snake case" in lower case for mapping to the database
    /// Example: FirstName ==> first_name
    /// </summary>
    /// <param name="builder">EntityTypeBuilder from ModelBuilder.Entity</param>
    /// <typeparam name="entityType">Type of entity</typeparam>
    public static EntityTypeBuilder ColumnNamesSnakeCase(this EntityTypeBuilder builder, Type entityType)
    {
        var props = entityType.GetProperties();

        var names = props.Where(x =>
            (!x.PropertyType.IsAbstract && !x.PropertyType.IsClass)
            || x.PropertyType.UnderlyingSystemType == typeof(string)
        ).Select(x => x.Name).ToArray();

        builder.ColumnNamesSnakeCase(names);

        return builder;
    }

    /// <summary>
    /// Casts all class properties to "snake case" in lower case for mapping to the database
    /// Example: FirstName ==> first_name
    /// </summary>
    /// <param name="builder">EntityTypeBuilder from ModelBuilder.Entity</param>
    /// <typeparam name="columnNames">Comma-separated column names</typeparam>
    public static EntityTypeBuilder ColumnNamesSnakeCase<T>(this EntityTypeBuilder builder, string columnNames)
    {
        var names = columnNames.Split(',');
        builder.ColumnNamesSnakeCase(names);

        return builder;
    }

    /// <summary>
    /// Casts all class properties to "snake case" in lower case for mapping to the database
    /// Example: FirstName ==> first_name
    /// </summary>
    /// <param name="builder">EntityTypeBuilder from ModelBuilder.Entity</param>
    /// <typeparam name="columnNames">Array of column names</typeparam>
    public static EntityTypeBuilder ColumnNamesSnakeCase(this EntityTypeBuilder builder, params string[] columnNames)
    {
        foreach (var name in columnNames)
        {
            var nameSnake = name.ToSnakeCase();
            if (!string.IsNullOrWhiteSpace(nameSnake))
                builder.Property(name).HasColumnName(nameSnake);
        }

        return builder;
    }

    /// <summary>
    /// Converts the string to the lower case "snake case" format
    /// </summary>
    /// <param name="name">string to convert</param>
    /// <returns></returns>
    public static string ToSnakeCase(this string name)
    {
        var lw = name.ToLower();
        var sb = new StringBuilder();
        var isStart = true;
        for (var i = 0; i < name.Length; i++)
        {
            var c = name[i];
            if (c != lw[i] && !isStart)
                sb.Append('_');

            sb.Append(c);
            isStart = false;
        }
        return sb.ToString().ToLower().Trim();
    }
}