using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
    public class DoctorSpecialtyDTO
    {        
        public int DoctorId { get; set; }
        public List<int> SpecialtyId { get; set; }
        public bool lActive { get; set; } = true;
    }
}