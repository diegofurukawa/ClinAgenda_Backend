using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Status;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
    // DTO para resposta com detalhe de status
    public class DoctorWithStatusDTO : DoctorResponseDTO
    {
        public StatusResponseDTO Status { get; set; } = null!;
    }
}