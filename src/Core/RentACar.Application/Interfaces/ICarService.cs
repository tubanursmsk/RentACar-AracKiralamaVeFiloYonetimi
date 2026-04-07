using RentACar.Application.DTOs.Car;

namespace RentACar.Application.Interfaces
{
    public interface ICarService
    {
        Task<IReadOnlyList<CarDto>> GetAllCarsAsync();
        Task<CarDto?> GetCarByIdAsync(int id);
        Task<CarDto> CreateCarAsync(CarCreateDto carCreateDto);
        Task UpdateCarAsync(CarUpdateDto carUpdateDto);
        Task DeleteCarAsync(int id);
    }
}