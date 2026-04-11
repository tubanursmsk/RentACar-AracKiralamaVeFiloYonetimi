using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Customer;
public class RentalDeleteDto
{
    [Required]
        public int Id { get; set; }

        public bool IsDeleted { get; set; } = true;
}
