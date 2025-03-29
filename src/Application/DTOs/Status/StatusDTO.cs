using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Status
{
    public class StatusDTO
    {
        public int StatusId { get; set; }
        public required string StatusName { get; set; }
    }
}