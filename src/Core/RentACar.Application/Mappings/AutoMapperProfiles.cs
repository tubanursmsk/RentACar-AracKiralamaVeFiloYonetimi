using AutoMapper;
using RentACar.Domain.Entities;
using RentACar.Application.DTOs.Car;
using RentACar.Application.DTOs.Rental;
using RentACar.Application.DTOs.Customer;
using ECommerce.Domain.Entities;
using ECommerce.Application.DTOs.User;
using ECommerce.Application.DTOs.Company;
using ECommerce.Application.DTOs.Customer;


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
            CreateMap<Rental, RentalUpdateDto>().ReverseMap();
            CreateMap<Rental, RentalDeleteDto>().ReverseMap();

            // Customer map ayarları
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<Customer, CustomerCreateDto>().ReverseMap();
            CreateMap<Customer, CustomerUpdateDto>().ReverseMap();
            CreateMap<Customer, CustomerDeleteDto>().ReverseMap();

            //User map ayarları
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserCreateDto>().ReverseMap();
            CreateMap<User, UserUpdateDto>().ReverseMap();
            CreateMap<User, UserDeleteDto>().ReverseMap();

            //Company map ayarları
            CreateMap<Company, CompanyDto>().ReverseMap();
            CreateMap<Company, CompanyCreateDto>().ReverseMap();
            CreateMap<Company, CompanyUpdateDto>().ReverseMap();
            CreateMap<Company, CompanyDeleteDto>().ReverseMap();



        }
    }
}