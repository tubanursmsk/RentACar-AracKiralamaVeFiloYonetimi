using AutoMapper;
using RentACar.Application.DTOs.Customer;
using RentACar.Application.DTOs.Responses;
using RentACar.Application.Interfaces;
using RentACar.Domain.Entities;
using RentACar.Domain.Interfaces;

namespace RentACar.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // CustomerService.cs içindeki GetAllCustomersAsync metodunu şu şekilde güncelle:
        public async Task<PaginatedResult<CustomerDto>> GetAllCustomersAsync(int pageNumber = 1, int pageSize = 10)
        {
            var (items, totalCount) = await _unitOfWork.Customers.GetAllPagedAsync(pageNumber, pageSize);

            return new PaginatedResult<CustomerDto>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = _mapper.Map<IReadOnlyList<CustomerDto>>(items)
            };
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> CreateCustomerAsync(CustomerCreateDto customerCreateDto)
        {
            var customer = _mapper.Map<Customer>(customerCreateDto);

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto?> UpdateCustomerAsync(int id, CustomerUpdateDto customerUpdateDto)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
                return null;

            _mapper.Map(customerUpdateDto, customer);
            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
                return false;

            _unitOfWork.Customers.Delete(customer);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}