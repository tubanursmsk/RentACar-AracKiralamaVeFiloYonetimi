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
        // [FromQuery] ile Swagger'da parametre girebilme alanı açılır.
        public async Task<ActionResult<ApiResponse<PaginatedResult<CarDto>>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _carService.GetAllCarsAsync(pageNumber, pageSize);
            return Ok(ApiResponse<PaginatedResult<CarDto>>.SuccessResult(result));
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
            // null yerine new { } kullanıyoruz
            return Ok(ApiResponse<object>.SuccessResult(new { }, "Araç bilgileri başarıyla güncellendi."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            await _carService.DeleteCarAsync(id);
            // null yerine new { } kullanıyoruz
            return Ok(ApiResponse<object>.SuccessResult(new { }, "Araç başarıyla silindi (Soft Delete)."));
        }

        // POST api/Cars/available
        // Neden GET değil de POST? Çünkü karmaşık arama filtrelerini URL'de (QueryString) 
        // taşımak yerine Request Body'de taşımak daha güvenli ve genişletilebilirdir.
        [HttpPost("available")]
        public async Task<ActionResult<ApiResponse<PaginatedResult<CarDto>>>> GetAvailableCars([FromBody] AvailableCarSearchDto searchDto)
        {
            var result = await _carService.GetAvailableCarsAsync(searchDto);

            if (result.TotalCount == 0)
            {
                return Ok(ApiResponse<PaginatedResult<CarDto>>.SuccessResult(result, "Belirtilen kriterlere uygun araç bulunamadı."));
            }

            return Ok(ApiResponse<PaginatedResult<CarDto>>.SuccessResult(result, $"{result.TotalCount} adet uygun araç listelendi."));
        }


    }
}