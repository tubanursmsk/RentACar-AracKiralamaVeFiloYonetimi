using RentACar.Application.DTOs.Car;
using RentACar.Application.DTOs.Responses;

namespace RentACar.Application.Interfaces
{
    public interface ICarService
    {
        Task<IReadOnlyList<CarDto>> GetAllCarsAsync();
        Task<CarDto?> GetCarByIdAsync(int id);
        Task<CarDto> CreateCarAsync(CarCreateDto carCreateDto);
        Task UpdateCarAsync(CarUpdateDto carUpdateDto);
        Task DeleteCarAsync(int id);
        
        // Müsait araçları filtreleme
        Task<PaginatedResult<CarDto>> GetAvailableCarsAsync(AvailableCarSearchDto searchDto);
    
    }
}