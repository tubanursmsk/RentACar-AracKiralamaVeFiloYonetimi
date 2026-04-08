using Microsoft.EntityFrameworkCore;
using RentACar.Application;
using RentACar.Domain.Interfaces;
using RentACar.Infrastructure.Contexts;
using RentACar.Infrastructure.Repositories;
using RentACar.WebApi.Configurations; // Kendi yazdığımız Swagger konfigürasyonu için
using RentACar.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. Temel API Servisleri
builder.Services.AddControllers();
builder.Services.AddAuthorization();

// 2. Swagger / OpenAPI Yapılandırması (Kendi yazdığımız eklenti metodunu çağırıyoruz)
builder.Services.AddSwaggerWithJwt(); 
// Not: builder.Services.AddOpenApi(); sildik çünkü Swashbuckle altyapısını kullanıyoruz.

// 3. Veritabanı Bağlantısı
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<RentACarDbContext>(options =>
    options.UseSqlServer(connectionString));

// 4. Dependency Injection (Bağımlılık Kayıtları - DI Container)
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddApplicationServices();

var app = builder.Build();


// 1. Global Hata Yakalayıcı (En üstte olmalı ki alt katmanlardaki tüm hataları sarmalasın)
app.UseMiddleware<GlobalExceptionHandler>();

// 2. Geliştirme (Development) Ortamı Ayarları (Swagger UI)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Rent A Car API v1");
        options.RoutePrefix = string.Empty; // http://localhost:5271/ adresinden erişim için
    });
}

// 3. HTTPS Yönlendirmesi
app.UseHttpsRedirection();

// 4. Kimlik Doğrulama (Authentication) ve Yetkilendirme (Authorization)
// app.UseAuthentication(); // İleride JWT Login metodunu yazdığımızda burayı aktif edeceğiz
app.UseAuthorization();

// 5. Uç Noktaların (Endpoint) Haritalanması
app.MapControllers();

app.Run();