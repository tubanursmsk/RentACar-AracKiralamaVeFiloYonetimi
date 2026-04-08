using Microsoft.AspNetCore.Mvc;
using RentACar.Application.DTOs.Rental;
using RentACar.Application.DTOs.Responses;
using RentACar.Application.Interfaces;

namespace RentACar.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalService _rentalService;

        public RentalsController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<RentalDto>>>> GetAll()
        {
            var rentals = await _rentalService.GetAllRentalsAsync();
            return Ok(ApiResponse<IReadOnlyList<RentalDto>>.SuccessResult(rentals));
        }

        [HttpPost("rent")]
        public async Task<ActionResult<ApiResponse<RentalDto>>> RentCar(RentalCreateDto createDto)
        {
            var rental = await _rentalService.RentCarAsync(createDto);
            return Ok(ApiResponse<RentalDto>.SuccessResult(rental, "Araç başarıyla kiralandı."));
        }

        [HttpPost("return/{id}")]
        public async Task<ActionResult<ApiResponse<RentalDto>>> ReturnCar(int id)
        {
            var rental = await _rentalService.ReturnCarAsync(id);
            return Ok(ApiResponse<RentalDto>.SuccessResult(rental, "Araç başarıyla teslim alındı ve müsait duruma getirildi."));
        }
    }
}