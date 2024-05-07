using Microsoft.EntityFrameworkCore;
using SqliteTest1;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var srv = builder.Services;

srv.AddControllers();
srv.AddEndpointsApiExplorer();
srv.AddSwaggerGen();
srv.ConfigureDb();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

ConfigureDbHelper.FillDb();

app.Run();
