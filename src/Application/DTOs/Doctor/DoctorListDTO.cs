using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
    public class DoctorListDTO
    {
        public int DoctorId { get; set; }
        public required string DoctorName { get; set; }
        public required int StatusId { get; set; }
        public required string StatusName { get; set; }
        public int SpecialtyId { get; set; }
        public required string SpecialtyName { get; set; }
        public int nScheduleDuration { get; set; }
        public bool lActive { get; set; }
    }
}