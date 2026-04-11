using Microsoft.AspNetCore.Mvc;
using RentACar.Application.DTOs.Customer;
using RentACar.Application.DTOs.Responses;
using RentACar.Application.Interfaces;

namespace RentACar.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _customerService.GetAllCustomersAsync(pageNumber, pageSize);
            // Standart ApiResponse formatına uygun dönüş
            return Ok(new ApiResponse<PaginatedResult<CustomerDto>> { Data = result, Success = true });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerCreateDto customerCreateDto)
        {
            var createdCustomer = await _customerService.CreateCustomerAsync(customerCreateDto);
            return Ok(new ApiResponse<CustomerDto> { Data = createdCustomer, Success = true, Message = "Müşteri başarıyla oluşturuldu." });
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerUpdateDto customerUpdateDto)
        {
            var updatedCustomer = await _customerService.UpdateCustomerAsync(id, customerUpdateDto);
            if (updatedCustomer == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Müşteri bulunamadı." });
            }
            return Ok(new ApiResponse<CustomerDto> { Data = updatedCustomer, Success = true, Message = "Müşteri başarıyla güncellendi." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            if (!result)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Müşteri bulunamadı." });
            }
            return Ok(new ApiResponse<object> { Success = true, Message = "Müşteri başarıyla silindi." });

        }
    }
}