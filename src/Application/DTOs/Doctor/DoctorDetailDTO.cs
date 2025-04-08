using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Specialty;
using ClinAgenda.src.Application.DTOs.Status;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
    // DTO para resposta com detalhes completos, incluindo especialidades
    public class DoctorDetailDTO
    {
        // Ordem das propriedades ajustada conforme solicitado
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public bool lActive { get; set; }
        public DateTime dCreated { get; set; }
        public DateTime? dLastUpdated { get; set; }
        public StatusResponseDTO Status { get; set; } = null!;
        public List<SpecialtyResponseDTO> Specialties { get; set; } = new();
    }
}