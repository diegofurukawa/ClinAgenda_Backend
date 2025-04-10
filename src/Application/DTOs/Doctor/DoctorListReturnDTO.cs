using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Specialty;
using ClinAgenda.src.Application.DTOs.Status;

// DoctorListReturnDTO.cs - Atualizado para manter campos originais mas no novo formato
namespace ClinAgenda.src.Application.DTOs.Doctor
{
    public class DoctorListReturnDTO
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        
        public required List<SpecialtyDTO> Specialty { get; set; }
        public required StatusDTO Status { get; set; }

        public bool lActive { get; set; }
    }

}