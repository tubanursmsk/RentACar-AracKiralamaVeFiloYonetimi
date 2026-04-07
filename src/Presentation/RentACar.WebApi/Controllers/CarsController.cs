using Microsoft.AspNetCore.Mvc;
using RentACar.Application.DTOs.Car;
using RentACar.Application.DTOs.Responses;
using RentACar.Application.Interfaces;

namespace RentACar.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarsController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<CarDto>>>> GetAll()
        {
            var cars = await _carService.GetAllCarsAsync();
            return Ok(ApiResponse<IReadOnlyList<CarDto>>.SuccessResult(cars));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CarDto>>> GetById(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("Belirtilen araç bulunamadı."));
            }
            return Ok(ApiResponse<CarDto>.SuccessResult(car));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CarDto>>> Create(CarCreateDto carCreateDto)
        {
            var createdCar = await _carService.CreateCarAsync(carCreateDto);
            return StatusCode(201, ApiResponse<CarDto>.SuccessResult(createdCar, "Araç başarıyla sisteme eklendi."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Update(int id, CarUpdateDto carUpdateDto)
        {
            if (id != carUpdateDto.Id)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("URL'deki ID ile gönderilen nesnenin ID'si uyuşmuyor."));
            }

            await _carService.UpdateCarAsync(carUpdateDto);
            return Ok(ApiResponse<object>.SuccessResult(null, "Araç bilgileri başarıyla güncellendi."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            await _carService.DeleteCarAsync(id);
            return Ok(ApiResponse<object>.SuccessResult(null, "Araç başarıyla silindi (Soft Delete)."));
        }
    }
}