namespace RentACar.Application.DTOs.Car
{
    public class CarCreateDto
    {
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Plate { get; set; } = string.Empty;
        public decimal DailyPrice { get; set; }
        public string? ImageUrl { get; set; }
    }
}