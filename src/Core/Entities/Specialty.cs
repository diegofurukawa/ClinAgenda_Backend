using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinAgenda
{
    public class Specialty
    {
        public int SpecialtyId { get; set; }
        public required string SpecialtyName { get; set; }
        public int NScheduleDuration { get; set; }
        public DateTime DCreated { get; set; }
        public DateTime? DLastUpdated { get; set; }
        public bool LActive { get; set; } = true;
    }
}