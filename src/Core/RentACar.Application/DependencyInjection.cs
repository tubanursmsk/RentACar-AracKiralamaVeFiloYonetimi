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
            // AutoMapper'ı kaydet
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Servisleri kaydet
            services.AddScoped<ICarService, CarService>();

            return services;
        }
    }
}