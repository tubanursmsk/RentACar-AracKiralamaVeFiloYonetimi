namespace RentACar.Domain.Entities
{
    public class Rental : BaseEntity // rental: kiralama işlemi, bir aracın bir kullanıcı tarafından belirli bir süre için kiralanması durumunu temsil eder.
    {
        public int CarId { get; set; }
        public Car Car { get; set; } = null!;
        
        public int UserId { get; set; } // Kullanıcı ID (Identity ile bağlanacak)
        
        public DateTime RentStartDate { get; set; }
        public DateTime RentEndDate { get; set; }
        public DateTime? ReturnDate { get; set; } // Fiili teslim tarihi
        
        public decimal TotalAmount { get; set; } 
        public bool IsPaid { get; set; } // Ödendi mi?
    }
}