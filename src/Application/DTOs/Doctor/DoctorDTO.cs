using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
    public class DoctorDTO
    {
        public int DoctorId { get; set; }
        public required string DoctorName { get; set; }
        public required int StatusId { get; set; }
    }
}