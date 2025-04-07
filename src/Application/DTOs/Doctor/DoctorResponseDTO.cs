using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
    public class DoctorResponseDTO
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public bool lActive { get; set; }
    }
}