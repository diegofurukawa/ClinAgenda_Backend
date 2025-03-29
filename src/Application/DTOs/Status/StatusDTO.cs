using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Status
{
    public class StatusDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}