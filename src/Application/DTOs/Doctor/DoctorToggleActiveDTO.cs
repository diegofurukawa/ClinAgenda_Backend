// DoctorToggleActiveDTO.cs - For toggling active status
using System;
using System.ComponentModel.DataAnnotations;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
    public class DoctorToggleActiveDTO
    {
        [Required]
        public bool IsActive { get; set; }
    }
}
