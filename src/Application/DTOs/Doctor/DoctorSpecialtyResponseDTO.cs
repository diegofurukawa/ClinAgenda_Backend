using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
    // DTO para especialidade do médico (resposta)
    public class DoctorSpecialtyResponseDTO
    {
        public int SpecialtyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ScheduleDuration { get; set; }
        public bool IsActive { get; set; }
    }
}