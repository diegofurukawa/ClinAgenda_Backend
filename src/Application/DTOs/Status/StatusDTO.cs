using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Status
{
    public class StatusDTO
    {
        public int StatusId { get; set; }
        public required string StatusName { get; set; }
        public string StatusType { get; set; }
        public DateTime DCreated { get; set; }
        public DateTime? DLastUpdated { get; set; }
        public bool lActive { get; set; }
    }   
}