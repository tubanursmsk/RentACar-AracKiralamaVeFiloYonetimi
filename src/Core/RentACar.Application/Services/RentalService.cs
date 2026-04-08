using AutoMapper;
using RentACar.Application.DTOs.Rental;
using RentACar.Application.Interfaces;
using RentACar.Domain.Entities;
using RentACar.Domain.Interfaces;

namespace RentACar.Application.Services
{
    public class RentalService : IRentalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RentalService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<RentalDto>> GetAllRentalsAsync()
        {
            var rentals = await _unitOfWork.Rentals.GetAllAsync();
            return _mapper.Map<IReadOnlyList<RentalDto>>(rentals);
        }

        public async Task<RentalDto?> GetRentalByIdAsync(int id)
        {
            var rental = await _unitOfWork.Rentals.GetByIdAsync(id);
            return _mapper.Map<RentalDto>(rental);
        }

        public async Task<RentalDto> RentCarAsync(RentalCreateDto rentalCreateDto)
        {
            // 1. Kural: Tarih Doğrulaması
            if (rentalCreateDto.RentStartDate >= rentalCreateDto.RentEndDate)
                throw new InvalidOperationException("Bitiş tarihi, başlangıç tarihinden sonra olmalıdır.");

            // 2. Kural: Araç var mı ve MÜSAİT (State Machine) mi?
            var car = await _unitOfWork.Cars.GetByIdAsync(rentalCreateDto.CarId);
            if (car == null)
                throw new InvalidOperationException("Araç bulunamadı.");

            if (car.Status != CarStatus.Available)
                throw new InvalidOperationException($"Bu araç şu anda kiralanamaz. Mevcut Durumu: {car.Status}");

            // 3. Fiyat Hesaplama
            int totalDays = (rentalCreateDto.RentEndDate - rentalCreateDto.RentStartDate).Days;
            if (totalDays == 0) totalDays = 1; // Aynı gün teslimse en az 1 günlük ücret alınır
            
            decimal totalAmount = totalDays * car.DailyPrice;

            // 4. Kiralama Kaydını Oluştur
            var rental = _mapper.Map<Rental>(rentalCreateDto);
            rental.TotalAmount = totalAmount;
            rental.IsPaid = false; // Ödeme altyapısı gelince burası değişecek

            // 5. DURUM MAKİNESİ GÜNCELLEMESİ: Aracın statüsünü 'Kirada' olarak değiştir
            car.Status = CarStatus.Rented;

            await _unitOfWork.Rentals.AddAsync(rental);
            _unitOfWork.Cars.Update(car); // Aracı da güncelliyoruz
            
            // Tek bir SaveChanges ile Transaction(iş) bütünlüğünü (UnitOfWork) sağlıyoruz
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RentalDto>(rental);
        }

        public async Task<RentalDto> ReturnCarAsync(int rentalId)
        {
            var rental = await _unitOfWork.Rentals.GetByIdAsync(rentalId);
            if (rental == null)
                throw new InvalidOperationException("Kiralama kaydı bulunamadı.");

            if (rental.ReturnDate != null)
                throw new InvalidOperationException("Bu araç zaten teslim edilmiş.");

            var car = await _unitOfWork.Cars.GetByIdAsync(rental.CarId);
            if (car == null)
                throw new InvalidOperationException("Araç bulunamadı.");

            // 1. Teslim Tarihini Ata
            rental.ReturnDate = DateTime.Now;

            // 2. DURUM MAKİNESİ GÜNCELLEMESİ: Aracın statüsünü tekrar 'Müsait' yap
            car.Status = CarStatus.Available;

            _unitOfWork.Rentals.Update(rental);
            _unitOfWork.Cars.Update(car);
            
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RentalDto>(rental);
        }
    }
}