using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Doctor
{
    /// <summary>
    /// DTO de compatibilidade para especialidades de médicos
    /// </summary>
    public class DoctorSpecialtyDTO
    {
        public int DoctorId { get; set; }
        
        // Pode ser um int único ou uma Lista<int>, para compatibilidade
        public dynamic SpecialtyId { get; set; }
        
        public required string SpecialtyName { get; set; }
        public int nScheduleDuration { get; set; }
        public DateTime dCreated { get; set; }
        public DateTime? dLastUpdated { get; set; }
        public bool lActive { get; set; } = true;
    }
}