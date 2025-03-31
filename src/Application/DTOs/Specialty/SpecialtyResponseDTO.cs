using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Specialty
{
        /// <summary>
        /// DTO de compatibilidade para resposta de especialidade simplificada
        /// </summary>
        public class SpecialtyResponseDTO
        {
            public int SpecialtyId { get; set; }
            public string SpecialtyName { get; set; } = string.Empty;
            public int ScheduleDuration { get; set; }
            public bool IsActive { get; set; } = true;
        }
}