using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Company;

    public class CompanyDeleteDto
    {
        [Required]
        public int Id { get; set; }

        public bool IsDeleted { get; set; } = true;
    }

