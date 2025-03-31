using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Status
{
    /// <summary>
    /// DTO de compatibilidade para resposta de status simplificada
    /// </summary>
    public class StatusResponseDTO
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string StatusType { get; set; } = string.Empty;
        public bool lActive { get; set; } = true;
    } 
}