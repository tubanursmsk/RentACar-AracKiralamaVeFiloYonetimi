using Microsoft.Extensions.DependencyInjection;
using RentACar.Application.Interfaces;
using RentACar.Application.Services;
using System.Reflection;

namespace RentACar.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // AutoMapper ayarları
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Servisler
            services.AddScoped<ICarService, CarService>();
            services.AddScoped<IRentalService, RentalService>();

            return services;
        }
    }
}