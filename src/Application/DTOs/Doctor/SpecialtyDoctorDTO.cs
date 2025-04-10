using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
    public class SpecialtyDoctorDTO
    {
        public int DoctorId { get; set; }
        public int SpecialtyId { get; set; }
        public string SpecialtyName { get; set; } = string.Empty;
        public int nScheduleDuration { get; set; }
        public bool lActive { get; set; }
    }
}