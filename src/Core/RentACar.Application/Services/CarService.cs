using AutoMapper;
using RentACar.Application.DTOs.Car;
using RentACar.Application.Interfaces;
using RentACar.Domain.Entities;
using RentACar.Domain.Interfaces;

namespace RentACar.Application.Services
{
    public class CarService : ICarService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CarService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<CarDto>> GetAllCarsAsync()
        {
            var cars = await _unitOfWork.Cars.GetAllAsync();
            return _mapper.Map<IReadOnlyList<CarDto>>(cars);
        }

        public async Task<CarDto?> GetCarByIdAsync(int id)
        {
            var car = await _unitOfWork.Cars.GetByIdAsync(id);
            return _mapper.Map<CarDto>(car);
        }

        public async Task<CarDto> CreateCarAsync(CarCreateDto carCreateDto)
        {
            var car = _mapper.Map<Car>(carCreateDto);
            // Varsayılan olarak eklenen araç müsait statüde olur
            car.Status = CarStatus.Available; 

            await _unitOfWork.Cars.AddAsync(car);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CarDto>(car);
        }

        public async Task UpdateCarAsync(CarUpdateDto carUpdateDto)
        {
            var car = await _unitOfWork.Cars.GetByIdAsync(carUpdateDto.Id);
            if (car != null)
            {
                _mapper.Map(carUpdateDto, car); // DTO'dan gelen verileri mevcut entity üzerine yazar
                _unitOfWork.Cars.Update(car);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteCarAsync(int id)
        {
            var car = await _unitOfWork.Cars.GetByIdAsync(id);
            if (car != null)
            {
                _unitOfWork.Cars.Delete(car); // Generic Repository'deki Soft Delete çalışacak
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}