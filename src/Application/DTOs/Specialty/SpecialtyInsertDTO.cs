using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda.src.Application.DTOs.Specialty
{
    public class SpecialtyInsertDTO
    {
        public required string Name { get; set; }

        public required int ScheduleDuration { get; set; }
    }
}