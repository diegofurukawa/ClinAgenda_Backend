using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
    public class DoctorInsertDTO
    {
        public required string DoctorName { get; set; }
        public required List<int> Specialty { get; set; }
        public int StatusId { get; set; }
    }
}