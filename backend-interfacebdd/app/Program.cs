using app.data;
using app.repositories.classes;
using app.repositories.interfaces;
using app.services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Services
builder.Services.AddScoped<MigrationService>();
builder.Services.AddScoped<IMigrationRepository, MigrationRepository>();

// BD
var connectionString = builder.Configuration.GetConnectionString("MaDb");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
