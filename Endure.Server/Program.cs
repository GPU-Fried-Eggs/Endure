using Endure.Server;
using Endure.Server.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    switch (builder.Configuration["DatabaseType"])
    {
        case "Sqlite":
            options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite") ?? throw new ApplicationException("Connection String is not set"));
            break;
        case "SqlServer":
            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer") ?? throw new ApplicationException("Connection String is not set"));
            break;
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