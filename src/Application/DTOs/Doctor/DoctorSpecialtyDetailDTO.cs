using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
    /// <summary>
    /// DTO de compatibilidade para detalhe de especialidade
    /// </summary>
    public class DoctorSpecialtyDetailDTO
    {
        public int DoctorId { get; set; }
        public int SpecialtyId { get; set; }
        public DateTime dCreated { get; set; }
        public DateTime? dLastUpdated { get; set; }
        public bool lActive { get; set; }
    }
}