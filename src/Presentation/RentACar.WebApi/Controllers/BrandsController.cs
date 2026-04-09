using Microsoft.AspNetCore.Mvc;
using RentACar.Application.DTOs.Responses;
using RentACar.Domain.Entities;
using RentACar.Domain.Interfaces;

namespace RentACar.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var brands = await _unitOfWork.Brands.GetAllAsync();
            return Ok(ApiResponse<object>.SuccessResult(brands));
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] string brandName)
        {
            var brand = new Brand { Name = brandName };
            await _unitOfWork.Brands.AddAsync(brand);
            await _unitOfWork.SaveChangesAsync();
            return Ok(ApiResponse<object>.SuccessResult(brand, "Marka eklendi."));
        }
    }
}