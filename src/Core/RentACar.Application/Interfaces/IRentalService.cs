using RentACar.Application.DTOs.Rental;

namespace RentACar.Application.Interfaces
{
    public interface IRentalService
    {
        Task<IReadOnlyList<RentalDto>> GetAllRentalsAsync();
        Task<RentalDto?> GetRentalByIdAsync(int id);
        
        // İş kurallarını (Durum Makinesini) tetikleyecek iki ana metot
        Task<RentalDto> RentCarAsync(RentalCreateDto rentalCreateDto);
        Task<RentalDto> ReturnCarAsync(int rentalId); 
    }
}