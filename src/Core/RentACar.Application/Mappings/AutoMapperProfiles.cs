using AutoMapper;
using RentACar.Domain.Entities;
using RentACar.Application.DTOs.Car;
using RentACar.Application.DTOs.Rental;
using RentACar.Application.DTOs.Customer;


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

            // Rental map ayarları
            CreateMap<Rental, RentalDto>().ReverseMap();
            CreateMap<Rental, RentalCreateDto>().ReverseMap();

            // Customer map ayarları
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<Customer, CustomerCreateDto>().ReverseMap();
            CreateMap<Customer, CustomerUpdateDto>().ReverseMap();
            

            
        }
    }
}