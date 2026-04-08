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

            // 2. KURAL (MÜLAKAT KURTARAN ALGORİTMA): Tarih Kesişimi (Overlap) Kontrolü
            // Seçilen tarihlerde 'İptal Edilmemiş' ve 'Tamamlanmamış' olan tüm rezervasyonları bul.
            // Formül: (Mevcut_Başlangıç < İstenen_Bitiş) VE (Mevcut_Bitiş > İstenen_Başlangıç)
            var overlappingRentals = await _unitOfWork.Rentals.FindAsync(r => 
                r.RentStartDate < searchDto.DropOffDate && 
                r.RentEndDate > searchDto.PickUpDate &&
                r.Status != ReservationStatus.Cancelled && 
                r.Status != ReservationStatus.Completed);

            // Bu tarihlerde dolu olan (kesişen) araçların ID'lerini bir listeye alalım
            var bookedCarIds = overlappingRentals.Select(r => r.CarId).Distinct().ToList();

            // 3. KURAL: Lokasyon ve Uygunluk Filtresi
            // Aradığımız araç: Doğru şubede olmalı, durumu Pasif/Bakımda olmamalı ve bookedCarIds listesinde OLMAMALI.
            var suitableCars = await _unitOfWork.Cars.FindAsync(c => 
                c.CurrentLocationId == searchDto.PickUpLocationId &&
                c.Status != CarStatus.Passive &&
                c.Status != CarStatus.InMaintenance &&
                !bookedCarIds.Contains(c.Id)); // Dolu araçları eledik!

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