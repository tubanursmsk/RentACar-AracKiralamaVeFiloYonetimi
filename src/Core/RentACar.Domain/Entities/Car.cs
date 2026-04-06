namespace RentACar.Domain.Entities
{
    public class Car : BaseEntity
    {
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Plate { get; set; } = string.Empty;
        public decimal DailyPrice { get; set; }
        
        // Aracın şu anki durumu: Müsait, Kirada, Bakımda vb.
        public CarStatus Status { get; set; } = CarStatus.Available;
        
        public string? ImageUrl { get; set; }
    }

    public enum CarStatus
    {
        Available = 1, // Müsait
        Rented = 2, // Kirada
        InMaintenance = 3, // Bakımda
        Passive = 4 // Pasif (örneğin, satışta veya kullanımdan kaldırılmış)
    }
}