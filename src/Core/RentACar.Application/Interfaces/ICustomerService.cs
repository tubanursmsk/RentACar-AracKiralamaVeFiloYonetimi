using RentACar.Application.DTOs.Customer;
using RentACar.Application.DTOs.Responses;

namespace RentACar.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<PaginatedResult<CustomerDto>> GetAllCustomersAsync(int pageNumber = 1, int pageSize = 10);
        Task<CustomerDto?> GetCustomerByIdAsync(int id);
        Task<CustomerDto> CreateCustomerAsync(CustomerCreateDto customerCreateDto);
        Task<CustomerDto?> UpdateCustomerAsync(int id, CustomerUpdateDto customerUpdateDto);
        Task<bool> DeleteCustomerAsync(int id);
    }
}