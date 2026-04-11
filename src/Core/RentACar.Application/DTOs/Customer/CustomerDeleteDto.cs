using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Customer;
public class CustomerDeleteDto
{
    [Required]
        public int Id { get; set; }

        public bool IsDeleted { get; set; } = true;
}
