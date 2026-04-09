using AutoMapper;
using RentACar.Application.DTOs.Car;
using RentACar.Application.DTOs.Responses;
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
            var cars = await _unitOfWork.Cars.GetAllAsync(c => c.Brand, c => c.CurrentLocation);
            return _mapper.Map<IReadOnlyList<CarDto>>(cars);
        }

        public async Task<CarDto?> GetCarByIdAsync(int id)
        {
            // GetById yerine FindAsync kullanıp Include veriyoruz
            var cars = await _unitOfWork.Cars.FindAsync(c => c.Id == id, c => c.Brand, c => c.CurrentLocation);
            var car = cars.FirstOrDefault();

            return _mapper.Map<CarDto>(car);
        }

        public async Task<CarDto> CreateCarAsync(CarCreateDto carCreateDto)
        {
            var car = _mapper.Map<Car>(carCreateDto);
            // Varsayılan olarak eklenen araç müsait statüde olur
            car.Status = CarStatus.Available;

            await _unitOfWork.Cars.AddAsync(car);
            await _unitOfWork.SaveChangesAsync();

            // EKLENEN KISIM: Araç veritabanına kaydedildikten sonra, Id'si oluşur. 
            // İsimlerin DTO'ya dolu gitmesi için aracı ilişkileriyle beraber DB'den tekrar çekiyoruz.
            var createdCar = (await _unitOfWork.Cars.FindAsync(c => c.Id == car.Id, c => c.Brand, c => c.CurrentLocation)).FirstOrDefault();

            return _mapper.Map<CarDto>(createdCar ?? car);
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

        /*
        💡 Mülakat İpucu: Eğer sana "Tarihlerin çakışıp çakışmadığını nasıl anlarsın?" diye sorarlarsa
           şu formülü söyle: (StartA < EndB) && (EndA > StartB). Bu, dünyadaki tüm rezervasyon sistemlerinin
           (Otel, Uçak, Araç) temel kesişim formülüdür. Uzun uzun if/else yazmaktan kurtarır.
        */
        public async Task<PaginatedResult<CarDto>> GetAvailableCarsAsync(AvailableCarSearchDto searchDto)
        {
            // 1. KURAL: Tarih doğrulama
            if (searchDto.PickUpDate >= searchDto.DropOffDate)
                throw new ArgumentException("Dönüş tarihi, alış tarihinden önce veya aynı olamaz.");

            if (searchDto.PickUpDate < DateTime.Now.Date)
                throw new ArgumentException("Geçmiş bir tarihe rezervasyon yapılamaz.");

            var overlappingRentals = await _unitOfWork.Rentals.FindAsync(r =>
                r.RentStartDate < searchDto.DropOffDate &&
                r.RentEndDate > searchDto.PickUpDate &&
                r.Status != ReservationStatus.Cancelled &&
                r.Status != ReservationStatus.Completed);

            var bookedCarIds = overlappingRentals.Select(r => r.CarId).Distinct().ToList();

            // 3. KURAL: Lokasyon ve Uygunluk Filtresi
            // DEĞİŞEN KISIM: c => c.Brand ve c => c.CurrentLocation dahil edildi
            var suitableCars = await _unitOfWork.Cars.FindAsync(c =>
                c.CurrentLocationId == searchDto.PickUpLocationId &&
                c.Status != CarStatus.Passive &&
                c.Status != CarStatus.InMaintenance &&
                !bookedCarIds.Contains(c.Id),
                c => c.Brand,
                c => c.CurrentLocation);

            // 4. KURAL: Sayfalama (Pagination)
            var totalCount = suitableCars.Count;

            var pagedCars = suitableCars
                .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToList();

            // 5. Sonucu Dönüştür ve Return et
            return new PaginatedResult<CarDto>
            {
                TotalCount = totalCount,
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize,
                Items = _mapper.Map<IReadOnlyList<CarDto>>(pagedCars)
            };
        }
    }
}