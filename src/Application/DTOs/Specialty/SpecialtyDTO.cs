using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Specialty
{
    public class SpecialtyDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required int ScheduleDuration { get; set; }
    }
}