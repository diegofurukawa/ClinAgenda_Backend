using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Specialty;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
    // DTO para resposta com detalhes completos, incluindo especialidades
    public class DoctorDetailDTO : DoctorWithStatusDTO
    {
        public List<SpecialtyResponseDTO> Specialties { get; set; } = new();
        public DateTime dCreated { get; set; }
        public DateTime? dLastUpdated { get; set; }
    }

}