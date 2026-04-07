using Microsoft.EntityFrameworkCore;
using RentACar.Application;
using RentACar.Infrastructure.Contexts;
using RentACar.Domain.Interfaces;
using RentACar.Infrastructure.Repositories;
using RentACar.WebApi.Middleware;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<RentACarDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repository ve UnitOfWork kayıtları
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();  
builder.Services.AddApplicationServices();  

var app = builder.Build();


app.UseMiddleware<GlobalExceptionHandler>(); // Hata yakalayıcıyı devreye aldık

app.UseAuthorization();
app.MapControllers();
app.MapOpenApi();

app.UseHttpsRedirection();

app.Run();
