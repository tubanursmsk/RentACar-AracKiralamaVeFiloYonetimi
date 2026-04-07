using AutoMapper;
using RentACar.Application.DTOs.Car;
using RentACar.Domain.Entities;

namespace RentACar.Application.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Car map ayarları
            CreateMap<Car, CarDto>().ReverseMap();
            CreateMap<Car, CarCreateDto>().ReverseMap();
            CreateMap<Car, CarUpdateDto>().ReverseMap();
        }
    }
}