using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Specialty;
using ClinAgenda.src.Application.DTOs.Status;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
    public class DoctorListReturnDTO
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required List<SpecialtyDTO> Specialty { get; set; }
        public required StatusDTO Status { get; set; }
    }
}