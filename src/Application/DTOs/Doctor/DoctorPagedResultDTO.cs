using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
    // DTO para resultado paginado
    public class DoctorPagedResultDTO
    {
        public int Total { get; set; }
        public List<DoctorListItemDTO> Items { get; set; } = new();
    }
}