using Microsoft.EntityFrameworkCore;
using RentACar.Infrastructure.Contexts;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<RentACarDbContext>(options =>
    options.UseSqlServer(connectionString));


var app = builder.Build();

app.MapOpenApi();

app.UseHttpsRedirection();


app.Run();
