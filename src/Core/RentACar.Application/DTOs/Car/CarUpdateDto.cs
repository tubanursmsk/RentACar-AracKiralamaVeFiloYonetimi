using RentACar.Domain.Entities;

namespace RentACar.Application.DTOs.Car
{
    public class CarUpdateDto
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Plate { get; set; } = string.Empty;
        public decimal DailyPrice { get; set; }
        public CarStatus Status { get; set; }
        public string? ImageUrl { get; set; }
    }
}