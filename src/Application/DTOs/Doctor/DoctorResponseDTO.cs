using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// DoctorStatusResponseDTO.cs
// DoctorResponseDTO.cs - Para manter a estrutura de lista com paginação
namespace ClinAgenda.src.Application.DTOs.Doctor
{
    public class DoctorResponseDTO
    {
        public int Total { get; set; }
        public List<DoctorListReturnDTO> Items { get; set; }
    }
}