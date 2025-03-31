using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
    // DTO específico para listagem, com informações resumidas
    public class DoctorListItemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public List<string> SpecialtyNames { get; set; } = new();
        public bool IsActive { get; set; }
    }
}