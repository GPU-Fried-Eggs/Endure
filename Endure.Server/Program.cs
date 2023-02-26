using Endure.Server;
using Endure.Server.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    switch (Constants.DatabaseType)
    {
        case DatabaseType.Sqlite:
        {
            var connectionString = builder.Configuration.GetConnectionString("Sqlite");
            options.UseSqlite(connectionString ?? throw new ApplicationException("Connection String is not set"));
            break;
        }
        case DatabaseType.SqlServer:
        {
            var connectionString = builder.Configuration.GetConnectionString("SqlServer");
            options.UseSqlServer(connectionString ?? throw new ApplicationException("Connection String is not set"));
            break;
        }
    }
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();